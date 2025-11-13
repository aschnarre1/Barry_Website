namespace BarryJBriggs.Models
{
    public class WorkItem
    {
        public int Id { get; set; }
        public int SectionId { get; set; }
        public Section Section { get; set; } = null!;
        public string Caption { get; set; } = "";
        public string YoutubeId { get; set; } = ""; 
        public string ImageUrl { get; set; } = "";  
        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    }

}
