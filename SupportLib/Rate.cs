namespace SupportLib
{
    public class Rate:IComparable<Rate>
    {
        public int Id1 { get; set; } = 0;
        public int Id2 { get; set; } = 0;
        public int Persent { get; set; } = 0;
        public string Name1 { get; set; } =string.Empty;
        public string Name2 { get; set; } = string.Empty;
        public Rate(int id, int id2, int persent)
        {
            Id1 = id;
            Id2 = id2;
            Persent = persent;
        }
        public override string ToString()
        {
            return $"{Id1}; {Id2} ;{Persent}; {Name1};{Name2};";
        }
        public int CompareTo(Rate? other)
        {
            if(other == null) return 1;
            if(other.Id1 != Id1) return Id1.CompareTo(other.Id1);
            return Persent.CompareTo(other.Persent);
        }
    }
}
