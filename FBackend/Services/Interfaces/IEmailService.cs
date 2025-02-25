namespace ApiCitaOdon.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendWelcomeEmailAsync(string to, string name);
        Task SendPasswordResetEmailAsync(string to, string token);
        Task SendAppointmentConfirmationAsync(string to, string name, DateTime appointmentDate, string treatmentType);
        Task SendAppointmentCancellationAsync(string to, string name, DateTime appointmentDate);
    }
}
