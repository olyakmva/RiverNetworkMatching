﻿using System.Runtime.Serialization.Formatters.Binary;

namespace SupportLib
{
    // Данные слоя карты. В списке хранятся точки на карте  
    // слой одной геометрии 
    [Serializable]
    public class MapData
    {
        public Dictionary<int, List<MapPoint>> MapObjDictionary { get; set; }
        public Dictionary<int,string> MapObjNameDictionary { get; set; }
        public string FileName { get; set; } = string.Empty;
        public int Count => GetAllVertices().Count;
        public string ColorName { get;  set; }
        public GeometryType Geometry {  get; set; }

        public MapData()
        {
            MapObjDictionary = new Dictionary<int, List<MapPoint>>();
            MapObjNameDictionary = new Dictionary<int, string>();
            ColorName = Colors.GetNext();
        }

        public MapData(GeometryType type) :this()
        {
            Geometry = type;
        }

        public List<MapPoint> GetAllVertices()
        {
            var resultList = new List<MapPoint>();
            foreach (var objPair in MapObjDictionary)
            {
                resultList.AddRange(objPair.Value);
            }
            return resultList;
        }
        
        public MapData Clone()
        {
            MapData clone;
            var bf = new BinaryFormatter();
            using (Stream fs = new FileStream("temp.bin", FileMode.Create, FileAccess.Write, FileShare.None))
            {
                bf.Serialize(fs, this);
            }
            using (Stream fs = new FileStream("temp.bin", FileMode.Open, FileAccess.Read, FileShare.None))
            {
                clone = (MapData)bf.Deserialize(fs);
            }
            return clone;
        }
        
        public void ClearWeights()
        {
            foreach (var chain in MapObjDictionary)
            {
                foreach (var vertex in chain.Value)
                {
                    vertex.Weight = 1;
                }
            }
        }
        public List<MapObjItem> GetMapObjItems()
        {
            var items = new List<MapObjItem>();
            foreach(var obj in MapObjDictionary)
            {
                var item = new MapObjItem()
                {
                    Id = obj.Key,
                    Points = obj.Value                    
                };
                items.Add(item);
            }
            return items;
        }
        public MapData MultiplyOffsetMapData(double offset = 0, double mul_offset = 1)
        {
            var result = new MapData();
            result.FileName = FileName + "_Clone";
            result.ColorName = ColorName;
            result.Geometry = Geometry;

            foreach (var obj in MapObjDictionary)
            {
                var tmp = new List<MapPoint>();

                foreach (var point in obj.Value)
                {
                    tmp.Add(new MapPoint(point.X, point.Y, point.Id, point.Weight));
                }

                foreach (var t in tmp)
                {
                    t.X = t.X * mul_offset + offset;
                    t.Y = t.Y * mul_offset + offset;
                }
                result.MapObjDictionary.Add(obj.Key, tmp);
            }
            return result;
        }
    }
}
