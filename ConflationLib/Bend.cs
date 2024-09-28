using AlgorithmsLibrary;
using SupportLib;


namespace ConflationLib
{
    public class Bend
    {
        /// <summary>
        /// список вершин в последовательности, в которой они идут в изгибе
        /// </summary>
        public List<MapPoint> PointsList { set; get; }

        #region Constructors

        public Bend()
        {
            PointsList = new List<MapPoint>();
        }

        public Bend(List<MapPoint> vert)
            : this()
        {
            PointsList.AddRange(vert);
        }

        #endregion

        /// <summary>
        /// вычисление площади изгиба
        /// </summary>
        /// <returns>площадь изгиба</returns>
        public double Area()
        {
            if (PointsList.Count < 3)
                return 0;

            double result = 0;
            for (var i = 0; i < PointsList.Count - 1; i++)
            {
                result += (PointsList[i].X + PointsList[i + 1].X) * (PointsList[i].Y - PointsList[i + 1].Y);
            }

            result += (PointsList[PointsList.Count - 1].X + PointsList[0].X) * (PointsList[PointsList.Count - 1].Y - PointsList[0].Y);

            return Math.Abs(result / 2);
        }

        /// <summary>
        /// вычисление периметра изгиба
        /// </summary>
        /// <returns>периметр изгиба, включая основание</returns>
        public double Perimeter()
        {
            if (PointsList.Count < 3)
                return 0;

            return Length() + BaseLineLength();
        }
        /// <summary>
        /// вычисление длины линии, образующей изгиб
        /// </summary>
        /// <returns>длина</returns>
        public double Length()
        {
            if (PointsList.Count < 3)
                return 0;

            double result = 0;

            for (var i = 0; i < PointsList.Count - 1; i++)
            {
                result += Math.Sqrt((PointsList[i + 1].X - PointsList[i].X) * (PointsList[i + 1].X - PointsList[i].X) + (PointsList[i + 1].Y - PointsList[i].Y) * (PointsList[i + 1].Y - PointsList[i].Y));
            }
            return result;
        }
        public VectorLib.Vector? GetBaseVector()
        {
            if (PointsList.Count < 3)
                return null;
            return new VectorLib.Vector(PointsList[0], PointsList[PointsList.Count - 1]);
        }
        /// <summary>
        /// подсчёт индекса компактности
        /// </summary>
        /// <returns>индекс компактности</returns>
        public double CompactIndex()
        {
            if (PointsList.Count < 3)
                return 0;

            return 4 * Math.PI * Area() / Math.Pow(Perimeter(), 2);
        }

        public double BaseLineLength()
        {
            if (PointsList.Count < 2)
                return 0;
            return Math.Sqrt((PointsList[PointsList.Count - 1].X - PointsList[0].X) * (PointsList[PointsList.Count - 1].X - PointsList[0].X) + (PointsList[PointsList.Count - 1].Y - PointsList[0].Y) * (PointsList[PointsList.Count - 1].Y - PointsList[0].Y));
        }

        public double[] BendMiddlePoint()
        {
            if (PointsList.Count == 0)
                return new double[2];

            return new[] { (PointsList[0].X + PointsList[PointsList.Count - 1].X) / 2, (PointsList[0].Y + PointsList[PointsList.Count - 1].Y) / 2 };
        }

        /// <summary>
        /// поиск пика изгиба
        /// </summary>
        /// <returns>индекс пика изгиба</returns>
        public int PeakIndex()
        {
            if (PointsList.Count == 0)
                return 0;

            MapPoint begin = PointsList[0];
            MapPoint end = PointsList[PointsList.Count - 1];
            var peakIndex = 0;
            double maxSum = 0;
            for (var i = 1; i < PointsList.Count - 1; i++)
            {
                var tempSum = Math.Sqrt(Math.Pow((PointsList[i].X - begin.X), 2) + Math.Pow((PointsList[i].Y - begin.Y), 2)) +
                                 Math.Sqrt(Math.Pow((PointsList[i].X - end.X), 2) + Math.Pow((PointsList[i].Y - end.Y), 2));

                if (!(tempSum > maxSum)) continue;
                maxSum = tempSum;
                peakIndex = i;
            }
            return peakIndex;
        }
        /// <summary>
        /// вычисление высоты изгиба
        /// </summary>
        /// <returns>высота</returns>
        public double GetHeight()
        {
            Line baseLine;
            try
            {
                baseLine = new Line(PointsList[0], PointsList[PointsList.Count - 1]);
                int peakIndex = PeakIndex();
                return baseLine.GetDistance(PointsList[peakIndex]);
            }
            catch(LineCoefEqualsZeroException )
            {
                return 0;
            }
        }
        /// <summary>
        /// вычисление ширины изгиба
        /// </summary>
        /// <returns> ширина</returns>
        public double GetWidth()
        {
            if (PointsList.Count < 3)
                return 0;

            var baseLine = new Line(PointsList[0], PointsList[PointsList.Count - 1]);
            //  проведем перпендикулярную прямую
            var leftLine = baseLine.GetPerpendicularLine(PointsList[0]);
            var leftPoint = PointsList[0];

            if (PointsList.Count == 3)
            {
                leftPoint = baseLine.GetPerpendicularFoundationPoint(PointsList[1]);
                leftLine = new Line(PointsList[1], leftPoint);
                if (leftLine.GetSign(PointsList[0]) != leftLine.GetSign(PointsList[PointsList.Count - 1]))
                {
                    return PointsList[0].DistanceToVertex(PointsList[2]);
                }
                return Math.Max(PointsList[0].DistanceToVertex(leftPoint), PointsList[2].DistanceToVertex(leftPoint));
            }

            if (leftLine.GetSign(PointsList[1]) * leftLine.GetSign(PointsList[PointsList.Count - 1]) < 0)
            {
                leftPoint = baseLine.GetPerpendicularFoundationPoint(PointsList[1]);
                if (baseLine.GetSign(leftPoint) != 0)
                    leftLine = new Line(PointsList[1], leftPoint);
                else leftLine = baseLine.GetPerpendicularLine(PointsList[1]);
                var i = 2;
                while (i < PointsList.Count)
                {
                    if (leftLine.GetSign(PointsList[0]) == leftLine.GetSign(PointsList[i]))
                    {
                        i++;
                    }
                    else
                    {
                        leftPoint = baseLine.GetPerpendicularFoundationPoint(PointsList[i]);
                        if (baseLine.GetSign(leftPoint) != 0)
                            leftLine = new Line(PointsList[i], leftPoint);
                        else leftLine = baseLine.GetPerpendicularLine(PointsList[i]);
                        i++;
                    }
                }
            }
            // то же самое справа
            var endIndex = PointsList.Count - 1;

            var max = leftPoint.DistanceToVertex(PointsList[endIndex]);
            int j = endIndex - 1;
            while (j > 0)
            {

                var p = baseLine.GetPerpendicularFoundationPoint(PointsList[j]);
                var d = leftPoint.DistanceToVertex(p);
                if (d > max)
                    max = d;
                j--;
            }
            return Math.Round(max);


        }

        /// <summary>
        /// вычисление квадрата евклидова расстояния между изгибами, где
        /// нормализованная площадь по оси x
        /// нормализованная высота основания по оси y
        /// нормализованный иднекс компактности по оси z
        /// </summary>
        /// <param name="otherBend">изгиб, расстояние до которого вычисляем</param>
        /// <returns>квадрат расстояния между изгибами</returns>
        public double SquareDistanceToBend(Bend otherBend)
        {
            var size1 = Area();
            var size2 = otherBend.Area();
            var base1 = BaseLineLength();
            var base2 = otherBend.BaseLineLength();
            var cmp1 = CompactIndex();
            var cmp2 = otherBend.CompactIndex();

            return Math.Pow((size2 - size1) / (size2 + size1), 2) +
                Math.Pow((cmp1 - cmp2) / (cmp1 + cmp2), 2) +
                Math.Pow((base2 - base1) / (base2 + base1), 2);
        }

        /// <summary>
        /// раздувание изгиба
        /// при помощи радиального расширения
        /// </summary>
        /// <param name="factor">множитель раздувания</param>
        public void ExaggerateRadial(double factor)
        {
            var c = BendMiddlePoint();
            for (var i = 1; i < PointsList.Count - 1; i++)
            {
                PointsList[i].X = (1 - factor) * c[0] + factor * PointsList[i].X;
                PointsList[i].Y = (1 - factor) * c[1] + factor * PointsList[i].Y;
            }
        }

        

        /// <summary>
        /// сливает два последовательных похожих изгиба
        /// </summary> 
        /// <param name="nextBend">смежный изгиб</param>
        public void CombineWith(Bend nextBend)
        {
            double factor = 1.2;
            MapPoint baseVertex = nextBend.PointsList[0];
            int peakIndex = PeakIndex();
            MapPoint thisPeak = PointsList[peakIndex];
            int nextPeakInd = nextBend.PeakIndex();
            MapPoint nextPeak = nextBend.PointsList[nextPeakInd];
            var centerX = (thisPeak.X + nextPeak.X) / 2;
            var centerY = (thisPeak.Y + nextPeak.Y) / 2;

            var x = (1 - factor) * baseVertex.X + factor * centerX;
            var y = (1 - factor) * baseVertex.Y + factor * centerY;

            baseVertex.X = x;
            baseVertex.Y = y;
            PointsList.RemoveRange(peakIndex + 1, PointsList.Count - peakIndex - 1);
            PointsList.Add(baseVertex);

            for (int i = nextPeakInd; i < nextBend.PointsList.Count; i++)
            {
                PointsList.Add(nextBend.PointsList[i]);
            }
        }

        /// <summary>
        /// сливает два последовательных похожих изгиба
        /// </summary> 
        /// <param name="nextNextBend">смежный изгиб</param>
        public void CombineWith2(Bend nextNextBend)
        {
            double factor = 1.2;
            MapPoint baseVertex = nextNextBend.PointsList[0];
            int peakIndex = PeakIndex();
            MapPoint thisPeak = PointsList[peakIndex];
            int nextPeakInd = nextNextBend.PeakIndex();
            MapPoint nextPeak = nextNextBend.PointsList[nextPeakInd];
            var centerX = (thisPeak.X + nextPeak.X) / 2;
            var centerY = (thisPeak.Y + nextPeak.Y) / 2;

            var x = (1 - factor) * baseVertex.X + factor * centerX;
            var y = (1 - factor) * baseVertex.Y + factor * centerY;

            baseVertex.X = x;
            baseVertex.Y = y;
            PointsList.RemoveRange(peakIndex + 1, PointsList.Count - peakIndex - 1);
            PointsList.Add(baseVertex);

            for (int i = nextPeakInd; i < nextNextBend.PointsList.Count; i++)
            {
                PointsList.Add(nextNextBend.PointsList[i]);
            }
        }

    }

}
