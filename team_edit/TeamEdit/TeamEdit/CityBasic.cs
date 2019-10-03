namespace TeamEdit
{
    public class City
    {
        public int CityId { get; set; }
        public string Name { get; set; }
        public int NationId { get; set; }

        public City(int id, string name, int nation)
        {
            CityId = id;
            Name = name;
            NationId = nation;
        }
    }

    
}