using DocuLink.Models;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Consultation> Consultations { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Medication> Medications { get; set; }
    public DbSet<PrescribedMedication> PrescribedMedications { get; set; }
    public DbSet<SideEffect> SideEffects { get; set; }
    public DbSet<Indication> Indications { get; set; }
    public DbSet<Review> Reviews { get; set; }
}