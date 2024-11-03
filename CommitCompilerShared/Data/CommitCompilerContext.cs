using Microsoft.EntityFrameworkCore;
using CommitCompilerShared.Models;

namespace CommitCompilerShared.Data
{
    public class CommitCompilerContext : DbContext
    {
        // Constructor sin parámetros
        public CommitCompilerContext() : base()
        {
        }

        // Constructor que acepta opciones
        public CommitCompilerContext(DbContextOptions<CommitCompilerContext> options) : base(options)
        {
        }

        // DbSet para la entidad BuildConfiguration
        public DbSet<BuildConfiguration> BuildConfigurations { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Usa la ruta completa donde deseas guardar la base de datos
                optionsBuilder.UseSqlite(@"Data Source=C:\BaseDeDatos\commitcompiler.db");
            }
        }
    }
}
