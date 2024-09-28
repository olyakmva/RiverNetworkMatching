using AlgorithmsLibrary;
using ConflationLib;
using DotSpatial.Data;
using SupportLib;
using System.Data;
using System.Text;
// получаем данные
string path = "..\\..\\..\\..\\Data\\ClipUfa";
//получить данные
var mapDatas = new List<MapData>();
string shapeFileName = Path.Combine(path, "hdrlin1000.shp");
var inputShape = FeatureSet.Open(shapeFileName);
var inputMap = Converter.ToMapData(inputShape);
mapDatas.Add(inputMap);

shapeFileName = Path.Combine(path, "hdrlin500.shp");
inputShape = FeatureSet.Open(shapeFileName);
inputMap = Converter.ToMapData(inputShape);
mapDatas.Add(inputMap);
ISimplificationAlgm[] algms = new ISimplificationAlgm[]
{
    new DouglasPeuckerAlgm(),
    new SleeveFitAlgm(),
    new VisWhyattAlgmWithTolerance()
};

Func<List<double>, double>[] funcs1 = new Func<List<double>, double>[3];
funcs1[0] = new Func<List<double>, double>(GetMin);
funcs1[1] = new Func<List<double>, double>(Median);
funcs1[2] = new Func<List<double>, double>(GetAverage);
string[] names1 = new string[] { "min", "median", "ave" };


Func<List<double>, double>[] funcs2 = new Func<List<double>, double>[2];
funcs2[0] = new Func<List<double>, double>(DoubleMedian);
funcs2[1] = new Func<List<double>, double>(Median);

string[] names2 = new string[] {  "median2",  "median" };

foreach (ISimplificationAlgm algm in algms)
{
    var bendCharacterists = new BendCharacteristics();
    var dictionary = bendCharacterists.GetBendsCharacteristics(mapDatas[0]);

    algm.Options = new SimplificationAlgmParameters()
    {
        PointNumberGap = 2.0
    };

    for (int i = 0; i < funcs1.Length; i++)
    {
        var in1 = mapDatas[0].Clone();
        foreach (var pair in in1.MapObjDictionary)
        {
            var pointsList = pair.Value;
            if (dictionary.ContainsKey(pair.Key))
            {
                var bends = dictionary[pair.Key];
                var paramList = (from b in bends select b.Height).ToList();
                paramList.Sort();
                algm.Options.Tolerance = funcs1[i](paramList);
                algm.Run(pointsList);
            }
        }

        var bendCharacterists2 = new BendCharacteristics();
        var dictionary2 = bendCharacterists2.GetBendsCharacteristics(mapDatas[1]);
        for (int j = 0; j < funcs2.Length; j++)
        {
            var in2 = mapDatas[1].Clone();
            foreach (KeyValuePair<int, List<MapPoint>> pair in in2.MapObjDictionary)
            {
                var pointsList = pair.Value;
                if (dictionary2.ContainsKey(pair.Key))
                {
                    var bends = dictionary2[pair.Key];
                    var paramList = (from b in bends select b.Height).ToList();
                    paramList.Sort();
                    algm.Options.Tolerance = funcs2[j].Invoke(paramList);
                    algm.Run(pointsList);
                }
            }

            // bends
            double maxDistanceBetweenPoints = 800.0;
            var bendCharacteristics = new BendCharacteristics(in1, in2, maxDistanceBetweenPoints);
            bendCharacteristics.Run();
            bendCharacteristics.Save("rate.txt");
            var f = GetFscore();
            var s1 = string.Format("{0};{1};{2};{3};{4};{5};", in1.Count, in2.Count, i, j, Math.Round(f.Item1, 3), f.Item2);
            Save("result.txt", s1);
        }
    }
}
var f1 = GetFscoreArcGis();
var s2 = string.Format("{0};{1};", Math.Round(f1.Item1, 3), f1.Item2);
Save("resA.txt", s2);
Console.ReadLine();


double GetMin(List<double> values)
{
    if (values.Count > 0)
    {
        return values[0];
    }
    return 0;
}

double GetMax(List<double> values)
{
    if (values.Count > 0)
    {
        return values[^1];
    }
    return 0;
}

double GetAverage(List<double> values)
{
    if (values.Count > 0)
    {
        return values.Average();
    }
    return 0;
}

double FirstLast(List<double> values)
{
    if (values.Count > 0)
    {
        return (values[0] + values[^1]) / 2;
    }
    return 0;
}

double Median(List<double> values)
{
    if (values.Count > 0)
    {
        return values[values.Count / 2];
    }
    return 0;
}

double DoubleMax(List<double> values)
{
    return 2 * GetMax(values);
}

double DoubleMedian(List<double> values)
{
    return 2 * Median(values);
}
double TripleMedian(List<double> values)
{
    return 3 * Median(values);
}

(double, string) GetFscore()
{
    var realRate = GetFromFile("real.txt");
    var accList = Get("rate.txt");
    accList.Sort();

    var ids = (from t in accList select t.Id1).Distinct().ToList();
    Console.WriteLine(ids.Count);
    var ids2 = (from t in realRate select t.Id1).Distinct().ToList();
    Console.WriteLine(ids2.Count);
    
    int truePositive = 0, trueNegative = 0, falsePositive = 0, falseNegative = 0;
    int positiveLimit = 75, negativeLimit = 34, persentLimit = 20;
    var nomatch = (from p in realRate where p.Id2 == -1 select p.Id1).ToList();

    foreach (var id in nomatch)
    {
        var items = accList.FindAll(i => i.Id1 == id);
        if (items.Count == 0)
        {
            trueNegative++;
            continue;
        }
        foreach (var item in items)
        {
            if (item.Persent < negativeLimit)
            {
                trueNegative++;
            }
            else
            {
                falsePositive++;
            }
        }
        accList.RemoveAll(i => i.Id1 == id);
    }
    var match = (from p in realRate where p.Id2 != -1 select p).ToList();

    foreach (var rate in match)
    {
        var items = accList.FindAll(i => i.Id1 == rate.Id1);
        if (items.Count == 0)
        {
            falseNegative++;
            continue;
        }

        var trueItem = items.FindAll(i => i.Id2 == rate.Id2).FirstOrDefault();
        if (trueItem != null)
        {
            truePositive++;
            
        }
        accList.RemoveAll(i => i.Id1 == rate.Id1 && i.Id2 == rate.Id2);
    }

    foreach (var item in accList)
    {
        if (item.Persent < negativeLimit)
        {
            trueNegative++;
        }
        else
        {
            falsePositive++;
            
        }
    }
    string l = string.Format("{0};{1};{2};{3};", truePositive, trueNegative, falsePositive, falseNegative);
    Save("result.txt", l);
    double fscore = (2.0 * truePositive) / (1.0 * falseNegative + falsePositive + 2.0 * truePositive);
    Save("result.txt", fscore.ToString());
    Console.WriteLine(fscore);
    return (fscore, l);
}

(double, string) GetFscoreArcGis()
{
    var realRate = GetFromFile("real.txt");
    var accList = Get2("arcgisAvg.txt");
    accList.Sort();

    var ids = (from t in accList select t.Id1).Distinct().ToList();
    Console.WriteLine(ids.Count);
    var ids2 = (from t in realRate select t.Id1).Distinct().ToList();
    Console.WriteLine(ids2.Count);

    int truePositive = 0, trueNegative = 0, falsePositive = 0, falseNegative = 0;
    var nomatch = (from p in realRate where p.Id2 == -1 select p.Id1).ToList();

    foreach (var id in nomatch)
    {
        var items = accList.FindAll(i => i.Id1 == id);
        if (items.Count == 0)
        {
            trueNegative++;
            continue;
        }
        foreach (var item in items)
        {
            if (item.Id2 == -1)
            {
                trueNegative++;
            }
            else
            {
                falsePositive++;
            }
        }
        accList.RemoveAll(i => i.Id1 == id);
    }
    var match = (from p in realRate where p.Id2 != -1 select p).ToList();

    foreach (var rate in match)
    {
        
        var items = accList.FindAll(i => i.Id1 == rate.Id1);
        if (items.Count == 0)
        {
            falseNegative++;
            continue;
        }

        var trueItem = items.FindAll(i => i.Id2 == rate.Id2).FirstOrDefault();
        if (trueItem != null)
        {
            truePositive++;
         }
        accList.RemoveAll(i => i.Id1 == rate.Id1 && i.Id2 == rate.Id2);
    }

    foreach (var item in accList)
    {
        falsePositive++;
       
    }

    string l = string.Format("{0};{1};{2};{3};", truePositive, trueNegative, falsePositive, falseNegative);
    Save("result.txt", l);
    double fscore = (2.0 * truePositive) / (1.0 * falseNegative + falsePositive + 2.0 * truePositive);
    Save("result.txt", fscore.ToString());
    Console.WriteLine(fscore);
    return (fscore, l);
}


void Save(string fileName, string s)
{
    string dir = Path.Combine(Environment.CurrentDirectory, "output", fileName);
    using var sw = new StreamWriter(dir, true);
    sw.WriteLine(s);
}
List<Rate> GetFromFile(string fileName)
{
    var sr = new StreamReader(fileName);
    var realRate = new List<Rate>();
    string line;
    while ((line = sr.ReadLine()) != null)
    {
        var strs = line.Split(new char[] { ' ', '\t', ';' }, StringSplitOptions.RemoveEmptyEntries);
        int id1 = int.Parse(strs[0]);
        int id2 = int.Parse(strs[1]);
        int persent = 0;
        if (strs.Length >= 3)
        {
            persent = int.Parse(strs[2]);
        }
        realRate.Add(new Rate(id1, id2, persent));
    }
    sr.Close();
    return realRate;
}
List<Rate> Get(string fileName)
{
    List<Rate> accList = new List<Rate>();
    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    var sr = new StreamReader(fileName, Encoding.GetEncoding(1251));
    string line = sr.ReadLine();
    line = sr.ReadLine();
    while ((line = sr.ReadLine()) != null)
    {
        var strs = line.Split(new char[] { ' ', '\t', ';' }, StringSplitOptions.RemoveEmptyEntries);
        int id1 = int.Parse(strs[0]);
        int id2 = int.Parse(strs[1]);
        int p = (int)Math.Truncate(double.Parse(strs[2]));
        var rate = new Rate(id1, id2, p);
        if (strs.Length >= 4)
        {
            rate.Name1 = strs[3];
        }
        if (strs.Length >= 5)
        {
            rate.Name2 = strs[4];
        }
        accList.Add(rate);
    }
    sr.Close();
    return accList;
}

List<Rate> Get2(string fileName)
{
    List<Rate> accList = new List<Rate>();
    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    var sr = new StreamReader(fileName, Encoding.GetEncoding(1251));
    string line = sr.ReadLine();
    line = sr.ReadLine();
    while ((line = sr.ReadLine()) != null)
    {
        var strs = line.Split(new char[] { ' ', '\t', ';' }, StringSplitOptions.RemoveEmptyEntries);
        int id1 = int.Parse(strs[0]);
        int id2 = int.Parse(strs[1]);
        int p = 0;
        if (strs.Length >= 3)
        {
            p = (int)Math.Truncate(double.Parse(strs[2]));
        }

        var rate = new Rate(id1, id2, p);
        if (strs.Length >= 4)
        {
            rate.Name1 = strs[3];
        }
        if (strs.Length >= 5)
        {
            rate.Name2 = strs[4];
        }
        accList.Add(rate);
    }
    sr.Close();
    return accList;
}



