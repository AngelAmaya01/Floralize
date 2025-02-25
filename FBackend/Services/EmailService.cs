using System.Net.Mail;
using System.Net;
using ApiCitaOdon.Services.Interfaces;

namespace ApiCitaOdon.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendWelcomeEmailAsync(string to, string name)
    {
        var subject = "¡Bienvenido a nuestra Tienda Online!";
        var body = $"Hola {name},\n\nBienvenido a nuestra Floreria. Estamos encantados de tenerte con nosotros.";

        await SendEmailAsync(to, subject, body);
    }

    public async Task SendPasswordResetEmailAsync(string to, string token)
    {
        //var subject = "Recuperación de Contraseña";
        //var body = $"Para restablecer tu contraseña, utiliza el siguiente código: {token}";

        //await SendEmailAsync(to, subject, body);

        // Verifica que el token no sea nulo o vacío antes de usarlo
        if (string.IsNullOrEmpty(token))
        {
            throw new ArgumentException("El token es inválido.");
        }

        var resetPasswordUrl = $"http://localhost:5173/reset-password?token={token}"; // Usar un valor de token válido
        var subject = "Recuperación de Contraseña";
        var body = $"Para restablecer tu contraseña, haz clic en el siguiente enlace: <a href='{resetPasswordUrl}'>Restablecer contraseña</a>";

        await SendEmailAsync(to, subject, body);
    }

    //public async Task SendAppointmentConfirmationAsync(string to, string name, DateTime appointmentDate, string treatmentType)
    //{
    //    var subject = "Confirmación de Cita Dental";
    //    var body = $"Hola {name},\n\n" +
    //              $"Tu cita ha sido confirmada para el {appointmentDate:dd/MM/yyyy} a las {appointmentDate:HH:mm}.\n" +
    //              $"Tipo de tratamiento: {treatmentType}\n\n" +
    //              "Por favor, llega 10 minutos antes de tu cita.\n" +
    //              "Si necesitas cancelar o reprogramar, hazlo con al menos 24 horas de anticipación.";

    //    await SendEmailAsync(to, subject, body);
    //}

    //public async Task SendAppointmentCancellationAsync(string to, string name, DateTime appointmentDate)
    //{
    //    var subject = "Cancelación de Cita Dental";
    //    var body = $"Hola {name},\n\n" +
    //              $"Tu cita programada para el {appointmentDate:dd/MM/yyyy} a las {appointmentDate:HH:mm} " +
    //              "ha sido cancelada exitosamente.\n\n" +
    //              "Si deseas programar una nueva cita, puedes hacerlo a través de nuestra plataforma.";

    //    await SendEmailAsync(to, subject, body);
    //}

    private async Task SendEmailAsync(string to, string subject, string body)
    {
        var smtpClient = new SmtpClient(_configuration["EmailSettings:SmtpServer"])
        {
            Port = int.Parse(_configuration["EmailSettings:SmtpPort"]),
            Credentials = new NetworkCredential(
                _configuration["EmailSettings:SmtpUsername"],
                _configuration["EmailSettings:SmtpPassword"]
            ),
            EnableSsl = true,
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(
                _configuration["EmailSettings:SenderEmail"],
                _configuration["EmailSettings:SenderName"]
            ),
            Subject = subject,
            Body = body,
            IsBodyHtml = false,
        };
        mailMessage.To.Add(to);

        await smtpClient.SendMailAsync(mailMessage);
    }
}