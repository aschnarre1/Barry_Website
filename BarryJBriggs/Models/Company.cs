namespace BarryJBriggs.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string ImageUrl { get; set; } = "";
        public int SortOrder { get; set; } = 0;
    }
}
