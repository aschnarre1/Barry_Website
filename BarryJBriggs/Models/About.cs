namespace BarryJBriggs.Models;
public class AboutPage
{
    public int Id { get; set; }
    public string Slug { get; set; } = "";  
    public string Html { get; set; } = "";  
    public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;
}

