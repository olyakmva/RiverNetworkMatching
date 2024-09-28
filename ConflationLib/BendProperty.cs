using SupportLib;

namespace ConflationLib
{
    public class BendProperty
    {
        public List<MapPoint> PointsList { get; set; }
        public MapPoint PeakPoint { get; set; }
        public bool Orientation { get; set; }
        public double Area { get; set; }
        public double Height { get; set; }
        public override string ToString()
        {
            return string.Format("{0};{1};{2};{3}", PeakPoint, Orientation,Area,Height);
        }
    }
}
