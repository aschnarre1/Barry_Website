namespace BarryJBriggs.Models
{
    public class Section
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Slug { get; set; } = "";
        public int SortOrder { get; set; }
        public ICollection<WorkItem> Items { get; set; } = new List<WorkItem>();
    }

}
