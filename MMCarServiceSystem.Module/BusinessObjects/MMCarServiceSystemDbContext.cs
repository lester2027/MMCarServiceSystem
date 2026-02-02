using DevExpress.ExpressApp.Design;
using DevExpress.ExpressApp.EFCore.DesignTime;
using DevExpress.ExpressApp.EFCore.Updating;
using DevExpress.Persistent.BaseImpl.EF;
using DevExpress.Persistent.BaseImpl.EF.PermissionPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MMCarServiceSystem.Module.BusinessObjects
{
    [TypesInfoInitializer(typeof(DbContextTypesInfoInitializer<MMCarServiceSystemEFCoreDbContext>))]
    public class MMCarServiceSystemEFCoreDbContext : DbContext
    {
        public MMCarServiceSystemEFCoreDbContext(DbContextOptions<MMCarServiceSystemEFCoreDbContext> options) : base(options)
        {
        }
        //public DbSet<ModuleInfo> ModulesInfo { get; set; }
        public DbSet<ModelDifference> ModelDifferences { get; set; }
        public DbSet<ModelDifferenceAspect> ModelDifferenceAspects { get; set; }
        public DbSet<PermissionPolicyRole> Roles { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<ServiceOrder> ServiceOrders { get; set; }
        public DbSet<CarWashDetail> CarWashDetails { get; set; }
        public DbSet<CarPaintDetail> CarPaintDetails { get; set; }
        public DbSet<CarDetailingDetail> CarDetailingDetails { get; set; }
        public DbSet<CarRepairDetail> CarRepairDetails { get; set; }
        public DbSet<Part> Parts { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<MMCarServiceSystem.Module.BusinessObjects.ApplicationUser> Users { get; set; }
        public DbSet<MMCarServiceSystem.Module.BusinessObjects.ApplicationUserLoginInfo> UserLoginsInfo { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.UseDeferredDeletion(this);
            modelBuilder.UseOptimisticLock();
            modelBuilder.SetOneToManyAssociationDeleteBehavior(DeleteBehavior.SetNull, DeleteBehavior.Cascade);
            modelBuilder.HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangingAndChangedNotificationsWithOriginalValues);
            modelBuilder.UsePropertyAccessMode(PropertyAccessMode.PreferFieldDuringConstruction);
            modelBuilder.Entity<MMCarServiceSystem.Module.BusinessObjects.ApplicationUserLoginInfo>(b =>
            {
                b.HasIndex(nameof(DevExpress.ExpressApp.Security.ISecurityUserLoginInfo.LoginProviderName), nameof(DevExpress.ExpressApp.Security.ISecurityUserLoginInfo.ProviderUserKey)).IsUnique();
            });
            modelBuilder.Entity<ModelDifference>()
                .HasMany(t => t.Aspects)
                .WithOne(t => t.Owner)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
