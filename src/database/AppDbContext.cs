using Microsoft.EntityFrameworkCore;
using GerenciaAPI.Models;

namespace GerenciaAPI.Database
{
    public class AppDbContext : DbContext
    {
        public DbSet<Projeto> Projetos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Tarefa> Tarefas { get; set; }
        public DbSet<Data> Datas { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
    }
}