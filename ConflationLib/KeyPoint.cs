using SupportLib;

namespace ConflationLib
{
    [Serializable]
    public class ObjAccordance
    {
        public int Map1ObjId {  get; set; }
        public int Map2ObjId { get; set; }

        public int BendsNumber1 {  get; set; }
        public int BendsNumber2 { get; set; }
        public int KeyPointNumber { get; set; }
        public double AccordanceCoef
        {
            get
            {
                return Math.Min(100, Math.Round(((double)KeyPointNumber) / (Math.Min(BendsNumber1, BendsNumber2)) * 100,0));
            }
        }
        public override string ToString()
        {
            return string.Format("{0}; {1}; {2:f2};", Map1ObjId,Map2ObjId,AccordanceCoef);
        }
    }

    public class MapKeyPoint
    {
        public MapPoint Point1 { get; set; }
        public MapPoint Point2 { get; set; }

        public override string ToString()
        {
            return string.Format("{0};{1};", Point1, Point2);
        }
    }

}
