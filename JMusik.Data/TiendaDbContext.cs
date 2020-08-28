using JMusik.Data.EntityConfiguration;
using JMusik.Models;
using Microsoft.EntityFrameworkCore;

namespace JMusik.Data
{
    public partial class TiendaDbContext : DbContext
    {
        public TiendaDbContext()
        {
        }

        public TiendaDbContext(DbContextOptions<TiendaDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<DetalleOrden> DetallesOrden { get; set; }
        public virtual DbSet<Orden> Ordenes { get; set; }
        public virtual DbSet<Perfil> Perfiles { get; set; }
        public virtual DbSet<Producto> Productos { get; set; }
        public virtual DbSet<Usuario> Usuarios { get; set; }

        // El siguiente metodo se debe eliminar por seguridad, ya que no debe incluirse la cadena de conexión en el código fuente
        // La configuración debe ir en el Startup.cs, como servicio
        // Y la cadena de conexión debe ir en el archivo appsettings.json en la capa de la aplicación web

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    if (!optionsBuilder.IsConfigured)
        //    {
        //        optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=TiendaDb;Integrated Security=True;");
        //    }
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Mejores practicas indican que es mejor separa en archivos separados las configuraciones de las clases/entidades
            // Se crea una carpeta "Configuraciones" en la capa Data
            // Por cada entidad, se crea una clase que implementa la interfaz IEntityTypeConfiguration
            // Las configuraciones que estan en el dbcontext, se situaran en la clase de configuracion

            // Implementando la configuración
            modelBuilder.ApplyConfiguration(new DetalleOrdenConfiguration());
            modelBuilder.ApplyConfiguration(new OrdenConfiguration());
            modelBuilder.ApplyConfiguration(new PerfilConfiguration());
            modelBuilder.ApplyConfiguration(new ProductoConfiguration());
            modelBuilder.ApplyConfiguration(new UsuarioConfiguration());

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}