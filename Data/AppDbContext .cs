using DocuLink.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace DocuLink.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> ApplicationUser { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Consultation> Consultations { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Medication> Medications { get; set; }
        public DbSet<PrescribedMedication> PrescribedMedications { get; set; }
        public DbSet<SideEffect> SideEffects { get; set; }
        public DbSet<Indication> Indications { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {

           

            
            base.OnModelCreating(builder);

            // Define relationships for Appointment and User
            builder.Entity<Appointment>()
                .HasOne(a => a.User)
                .WithMany(u => u.Appointments)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany()
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.Entity<Indication>()
                .HasOne(i => i.Medication)
                .WithMany(m => m.Indications)
                .HasForeignKey(i => i.MedicationId)
                .OnDelete(DeleteBehavior.Cascade);




        }

        public async Task SeedAdminUserAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Ensure Admin role exists
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            // Seed admin user
            var adminEmail = "admin@doculink.com";
            var adminUserName = "Admin";
            var adminPassword = "Admin@123";
            var adminName = "Administrator";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new User
                {
                    UserName = adminUserName,
                    Email = adminEmail,
                    Name = adminName,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    Phone = 1234567890
                };

                var createResult = await userManager.CreateAsync(adminUser, adminPassword);
                if (createResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
                else
                {
                    foreach (var error in createResult.Errors)
                    {
                        Console.WriteLine($"Error creating admin user: {error.Description}");
                    }
                }
            }
            else
            {
                if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}