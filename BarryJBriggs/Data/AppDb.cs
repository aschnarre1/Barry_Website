using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;

namespace BarryJBriggs.Data
{
    using BarryJBriggs.Models;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.EntityFrameworkCore;

    public class AppDb : DbContext
    {
        public AppDb(DbContextOptions<AppDb> options) : base(options) { }

        public DbSet<Section> Sections { get; set; }
        public DbSet<WorkItem> WorkItems { get; set; }
        public DbSet<Company> Companies => Set<Company>();
        public DbSet<AboutPage> AboutPages => Set<AboutPage>();


        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Section>().HasIndex(x => x.Slug).IsUnique();
            builder.Entity<WorkItem>()
                .HasOne(w => w.Section)
                .WithMany(s => s.Items)
                .HasForeignKey(w => w.SectionId);
        }
    }
}
