using SupportLib;

namespace VectorLib
{
    public class Vector
    {
        public double x; 
        public double y; 

        public Vector(MapPoint a, MapPoint b)
        {
            x = b.X-a.X; 
            y = b.Y-a.Y;
        }
        public Vector(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public double Length 
        { 
            get {  return Math.Sqrt(x*x + y*y); }
        }
        public double GetAngle(Vector other)
        {
            var cosA = (x*other.x + y*other.y) /(Length* other.Length);
            return Math.Acos(cosA);
        }
        public static Vector operator +(Vector a, Vector b)
        {
            return new Vector(a.x+b.x, a.y+b.y);
        }

        public override string ToString() 
        { 
            return string.Format("{0:f2};{1:f2};{2:f2};",x,y,Length);
        }
    }
}