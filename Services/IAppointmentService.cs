using DocuLink.Models;
using Microsoft.EntityFrameworkCore;

namespace DocuLink.Services
{
    public interface IAppointmentService
    {
        Task<Appointment> CreateAppointmentAsync(Appointment appointment);
        Task<Appointment> GetAppointmentByIdAsync(int id);
        Task<IEnumerable<Appointment>> GetAllAppointmentsAsync();
        Task<bool> CancelAppointmentAsync(int appointmentId);
    }

}