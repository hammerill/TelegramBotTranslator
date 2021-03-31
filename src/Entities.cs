using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace TGBotCSharp
{
    public class TranslatorContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Lang> Langs { get; set; }
        private readonly string ConnectionString;

        public TranslatorContext(string connectionString)
        {
            ConnectionString = connectionString;
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source=\"{ConnectionString}\"");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
    public class LangsContext : DbContext
    {
        public DbSet<Lang> Langs { get; set; }
        private readonly string ConnectionString;

        public LangsContext(string connectionString)
        {
            ConnectionString = connectionString;
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source=\"{ConnectionString}\"");
        }
    }

    public class User
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        public int SrcLangId { get; set; }
        [Required]
        [ForeignKey("SrcLangId")]
        public Lang SrcLang { get; set; }

        [Required]
        public int ToLangId { get; set; }
        [Required]
        [ForeignKey("ToLangId")]
        public Lang ToLang { get; set; }
    }
    public class Lang
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string LangCode { get; set; }
        [Required]
        public string FriendlyTitle { get; set; }
    }
}
