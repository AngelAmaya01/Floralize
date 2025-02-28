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
        if (string.IsNullOrEmpty(token))
        {
            throw new ArgumentException("El token es inválido.");
        }

        // Obtén la URL del frontend desde la configuración
        var frontendUrl = _configuration["FrontendURL"];
        var resetPasswordUrl = $"{frontendUrl}/reset-password?token={token}";
        var subject = "Recuperación de Contraseña";

        // Cuerpo del correo en HTML
        var body = $@"
        <html>
        <head>
            <style>
                body {{
                    font-family: Arial, sans-serif;
                    background-color: #f4f4f4;
                    color: #333;
                    line-height: 1.6;
                }}
                .container {{
                    max-width: 600px;
                    margin: 20px auto;
                    padding: 20px;
                    background-color: #fff;
                    border-radius: 8px;
                    box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                }}
                .button {{
                    display: inline-block;
                    padding: 10px 20px;
                    margin: 20px 0;
                    font-size: 16px;
                    color: #000;
                    background-color: #9a5ea7;
                    border-radius: 5px;
                    text-decoration: none;
                }}
                .footer {{
                    margin-top: 20px;
                    font-size: 12px;
                    color: #777;
                }}
            </style>
        </head>
        <body>
            <div class='container'>
                <h2>Recuperación de Contraseña</h2>
                <p>Hemos recibido una solicitud para restablecer tu contraseña. Si no fuiste tú, puedes ignorar este mensaje.</p>
                <p>Para continuar con el proceso, haz clic en el siguiente botón:</p>
                <a href='{resetPasswordUrl}' class='button'>Restablecer Contraseña</a>
                <p>Si el botón no funciona, copia y pega el siguiente enlace en tu navegador:</p>
                <p><a href='{resetPasswordUrl}'>{resetPasswordUrl}</a></p>
                <div class='footer'>
                    <p>Este enlace es válido por 24 horas. Si expira, deberás solicitar otro.</p>
                    <p>Gracias,<br>El equipo de Soporte</p>
                </div>
            </div>
        </body>
        </html>";

        // Envía el correo electrónico como HTML
        await SendEmailAsync(to, subject, body, isBodyHtml: true);
    }

    private async Task SendEmailAsync(string to, string subject, string body, bool isBodyHtml = false)
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
            IsBodyHtml = isBodyHtml,
        };
        mailMessage.To.Add(to);

        await smtpClient.SendMailAsync(mailMessage);
    }
}