using Microsoft.EntityFrameworkCore;

namespace InventorySystemCore
{
    public class InventorySystemContext : DbContext
    {
        public readonly string DBlocation = "C:/ProgramData/InventorySystem/";
        public DbSet<Administrator> Administrators { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<WorkingObject> WorkingObjects { get; set; }
        public DbSet<DamageReport> DamageReports { get; set; }
        public DbSet<InventoryUsage> InventoryUsage { get; set; }
        protected override void OnConfiguring(
            DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=" + DBlocation + "InventorySystem.db");
            optionsBuilder.UseLazyLoadingProxies();
            base.OnConfiguring(optionsBuilder);
        }
    }
}
