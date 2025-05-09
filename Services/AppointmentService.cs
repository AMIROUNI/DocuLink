using DocuLink.Models;
using Microsoft.EntityFrameworkCore;

namespace DocuLink.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly AppDbContext _context;

        public AppointmentService(AppDbContext context)
        {
            _context = context;
        }

        // Créer un rendez-vous
        public async Task<Appointment> CreateAppointmentAsync(Appointment appointment)
        {
            // Validation ou autre logique métier
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            return appointment;
        }

        // Récupérer un rendez-vous par son ID
        public async Task<Appointment> GetAppointmentByIdAsync(int id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.User)
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(a => a.Id == id);

            return appointment;
        }

        // Récupérer tous les rendez-vous
        public async Task<IEnumerable<Appointment>> GetAllAppointmentsAsync()
        {
            return await _context.Appointments
                .Include(a => a.User)
                .Include(a => a.Doctor)
                .ToListAsync();
        }

        // Annuler un rendez-vous
        public async Task<bool> CancelAppointmentAsync(int appointmentId)
        {
            var appointment = await _context.Appointments.FindAsync(appointmentId);
            if (appointment == null) return false;

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}


