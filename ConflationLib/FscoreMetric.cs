using SupportLib;
using System.Text;


namespace ConflationLib
{
    public  class FscoreMetric
    {
        public int positiveLimit = 75, negativeLimit = 24, persentLimit = 20;
        public FscoreMetric() 
        { 
        }

        public (double, string) GetFscore(string fileName1, string fileName2)
        {
            var realRate = GetFromFile(fileName1);
            var accList = Get(fileName2);
            accList.Sort();

            var ids = (from t in accList select t.Id1).Distinct().ToList();
            Console.WriteLine(ids.Count);
            var ids2 = (from t in realRate select t.Id1).Distinct().ToList();
            Console.WriteLine(ids2.Count);
            //var diff = (from k in ids2 select k).Except(ids).ToList();
            //string diffId = string.Join(' ', diff);
            //Console.WriteLine(diffId);
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
                    if (item.Persent < negativeLimit)
                    {
                        trueNegative++;
                    }
                    else
                    {
                        falsePositive++;
                        Save("fpos.txt", item.ToString());
                    }
                }
                accList.RemoveAll(i => i.Id1 == id);
            }
            var match = (from p in realRate where p.Id2 != -1 select p).ToList();

            foreach (var rate in match)
            {
                string info = string.Format("{0}", rate.ToString());
                var items = accList.FindAll(i => i.Id1 == rate.Id1);
                if (items.Count == 0)
                {
                    falseNegative++;
                    Save("noRelation.txt", info); continue;
                }

                var trueItem = items.FindAll(i => i.Id2 == rate.Id2).FirstOrDefault();
                if (trueItem != null)
                {
                    truePositive++;
                    info += trueItem.ToString();
                    Save("true.txt", info);
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
                    Save("fpos.txt", item.ToString());
                }
            }
            string l = string.Format("{0};{1};{2};{3};", truePositive, trueNegative, falsePositive, falseNegative);
            double fscore = (2.0 * truePositive) / (1.0 * falseNegative + falsePositive + 2.0 * truePositive);
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
    }
}
