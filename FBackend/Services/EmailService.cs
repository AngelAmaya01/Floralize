using System.Net.Mail;
using System.Net;
using ApiCitaOdon.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using ApiCitaOdon.Data;
using FBackend.Models;
using System.Globalization;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace ApiCitaOdon.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _context;

    public EmailService(IConfiguration configuration, ApplicationDbContext context)
    {
        _configuration = configuration;
        _context = context;
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

    public async Task SendOrderCompletedEmailAsync(Guid pedidoId, string clienteEmail, string tipoPedido, string detalles, decimal? total = null)
    {
        var subject = $"¡Tu pedido #{pedidoId.ToString().Substring(0, 8).ToUpper()} ha sido completado!";

        var body = $@"
    <html>
    <head>
        <style>
            body {{ font-family: Arial, sans-serif; line-height: 1.6; }}
            .container {{ max-width: 600px; margin: 20px auto; padding: 20px; border: 1px solid #ddd; border-radius: 5px; }}
            .header {{ color: #28a745; font-size: 24px; text-align: center; }}
            .details {{ margin: 20px 0; padding: 15px; background-color: #f8f9fa; border-radius: 5px; }}
        </style>
    </head>
    <body>
        <div class='container'>
            <div class='header'>¡Tu pedido está listo!</div>
            
            <p>Hemos completado tu pedido {tipoPedido} y está listo para ser entregado.</p>
            
            <div class='details'>
                <p><strong>Número de pedido:</strong> #{pedidoId.ToString().Substring(0, 8).ToUpper()}</p>
                <p><strong>Detalles:</strong> {detalles}</p>
            </div>
            
            <p>Por favor contáctanos para coordinar la entrega o recolecta tu pedido.</p>
        </div>
    </body>
    </html>";

        await SendEmailAsync(clienteEmail, subject, body, isBodyHtml: true);
    }

    public async Task SendLowStockNotificationAsync(Guid productoId, int cantidadActual)
    {
        // Obtener información del producto
        var producto = await _context.Productos
            .Include(p => p.Proveedor)
            .FirstOrDefaultAsync(p => p.Id == productoId);

        if (producto == null) return;

        var subject = $"¡Alerta: Stock bajo para {producto.Nombre}!";
        var body = $@"
        <html>
        <head>
            <style>
                body {{ font-family: Arial, sans-serif; line-height: 1.6; }}
                .container {{ max-width: 600px; margin: 20px auto; padding: 20px; border: 1px solid #ddd; border-radius: 5px; }}
                .alert {{ color: #d9534f; font-weight: bold; }}
            </style>
        </head>
        <body>
            <div class='container'>
                <h2>Alerta de Stock Bajo</h2>
                <p class='alert'>El producto <strong>{producto.Nombre}</strong> tiene un stock bajo.</p>
                <p><strong>Cantidad actual:</strong> {cantidadActual}</p>
                <p><strong>Categoría:</strong> {producto.Categoria}</p>
                <p>Por favor, considere realizar un nuevo pedido al proveedor.</p>
                
                {(producto.Proveedor != null ?
                    $@"<h3>Información del Proveedor:</h3>
                    <p><strong>Nombre:</strong> {producto.Proveedor.Nombre}</p>
                    <p><strong>Contacto:</strong> {producto.Proveedor.Telefono}</p>
                    <p><strong>Teléfono:</strong> {producto.Proveedor.Telefono}</p>"
                    : "<p>No hay información de proveedor asociada.</p>")}
            </div>
        </body>
        </html>";

        // Siempre enviar al SenderEmail (angelnataren16@gmail.com en tu configuración)
        var senderEmail = _configuration["EmailSettings:SenderEmail"];
        if (!string.IsNullOrEmpty(senderEmail))
        {
            await SendEmailAsync(senderEmail, subject, body, isBodyHtml: true);
        }

        // Opcional: Enviar al proveedor si tiene email
        if (producto.Proveedor != null && !string.IsNullOrEmpty(producto.Proveedor.Correo))
        {
            var providerSubject = $"Solicitud de reposición: {producto.Nombre}";
            await SendEmailAsync(producto.Proveedor.Correo, providerSubject, body, isBodyHtml: true);
        }
    }

    // Método para confirmación de pedido regular
    public async Task SendOrderConfirmationAsync(Guid pedidoId, string clienteEmail)
    {
        var pedido = await _context.Pedidos
            .Include(p => p.Detalles)
            .ThenInclude(d => d.Producto)
            .FirstOrDefaultAsync(p => p.Id == pedidoId);

        if (pedido == null) return;

        var subject = $"Confirmación de tu Pedido #{pedidoId.ToString().Substring(0, 8).ToUpper()}";

        // Función local para formatear cantidades en HNL
        string FormatHNL(decimal amount)
        {
            return amount.ToString("C", new System.Globalization.CultureInfo("es-HN"));
        }

        var detallesRows = string.Join("", pedido.Detalles.Select(d => $@"
        <tr>
            <td>{d.Producto?.Nombre ?? "Producto no disponible"}</td>
            <td>{d.Cantidad}</td>
            <td>{FormatHNL(d.PrecioUnitario)}</td>
            <td>{FormatHNL(d.Cantidad * d.PrecioUnitario)}</td>
        </tr>"));

        var body = $@"
    <html>
    <head>
        <style>
            body {{ font-family: Arial, sans-serif; line-height: 1.6; }}
            .container {{ max-width: 600px; margin: 20px auto; padding: 20px; border: 1px solid #ddd; border-radius: 5px; }}
            table {{ width: 100%; border-collapse: collapse; margin: 20px 0; }}
            th, td {{ padding: 10px; text-align: left; border-bottom: 1px solid #ddd; }}
            th {{ background-color: #f5f5f5; }}
            .total {{ font-weight: bold; font-size: 1.2em; }}
            .currency {{ text-align: right; }}
        </style>
    </head>
    <body>
        <div class='container'>
            <h2>¡Gracias por tu pedido!</h2>
            <p>Hemos recibido tu pedido correctamente. Aquí están los detalles:</p>
            
            <p><strong>Número de pedido:</strong> #{pedidoId.ToString().Substring(0, 8).ToUpper()}</p>
            <p><strong>Fecha:</strong> {pedido.FechaPedido.ToString("f", new System.Globalization.CultureInfo("es-HN"))}</p>
            <p><strong>Estado:</strong> {pedido.Estado}</p>
            
            <h3>Detalles del Pedido</h3>
            <table>
                <thead>
                    <tr>
                        <th>Producto</th>
                        <th>Cantidad</th>
                        <th class='currency'>Precio Unitario</th>
                        <th class='currency'>Subtotal</th>
                    </tr>
                </thead>
                <tbody>
                    {detallesRows}
                </tbody>
                <tfoot>
                    <tr>
                        <td colspan='3' style='text-align: right;'><strong>Total:</strong></td>
                        <td class='currency'><strong>{FormatHNL(pedido.Total)}</strong></td>
                    </tr>
                </tfoot>
            </table>
            
            <p>Te notificaremos cuando tu pedido haya sido enviado.</p>
            <p>Si tienes alguna pregunta, no dudes en contactarnos.</p>
        </div>
    </body>
    </html>";

        await SendEmailAsync(clienteEmail, subject, body, isBodyHtml: true);
    }

    // Nuevo método para confirmación de pedido personalizado
    public async Task SendCustomOrderConfirmationAsync(Guid pedidoId, string clienteEmail)
    {
        var pedido = await _context.Personalizados
            .Include(p => p.ClienteId)
            .FirstOrDefaultAsync(p => p.Id == pedidoId);

        if (pedido == null) return;

        var subject = pedido.Estado == "Completado"
            ? $"¡Tu pedido personalizado #{pedidoId.ToString().Substring(0, 8).ToUpper()} ha sido completado!"
            : $"Confirmación de tu Pedido Personalizado #{pedidoId.ToString().Substring(0, 8).ToUpper()}";

        // Función para formatear opciones booleanas
        string FormatOption(string value)
        {
            if (string.IsNullOrEmpty(value)) return "No";
            return value.Equals("Sí", StringComparison.OrdinalIgnoreCase) ||
                   value.Equals("Si", StringComparison.OrdinalIgnoreCase) ||
                   value.Equals("Yes", StringComparison.OrdinalIgnoreCase) ||
                   value.Equals("True", StringComparison.OrdinalIgnoreCase) ? "Sí" : "No";
        }

        var body = $@"
    <html>
    <head>
        <style>
            body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
            .container {{ max-width: 600px; margin: 20px auto; padding: 25px; border: 1px solid #e1e1e1; border-radius: 8px; }}
            .header {{ color: #2e7d32; font-size: 24px; text-align: center; margin-bottom: 20px; }}
            .completed {{ color: #2e7d32; font-weight: bold; font-size: 18px; }}
            .details-container {{ margin: 25px 0; background-color: #f9f9f9; padding: 20px; border-radius: 6px; }}
            .detail-item {{ margin-bottom: 12px; display: flex; }}
            .detail-label {{ font-weight: bold; min-width: 150px; }}
            .image-container {{ margin: 20px 0; text-align: center; }}
            .product-image {{ max-width: 100%; max-height: 300px; border-radius: 4px; }}
            .footer {{ margin-top: 25px; font-size: 14px; color: #666; text-align: center; }}
            .status-badge {{
                display: inline-block; 
                padding: 5px 10px; 
                border-radius: 4px; 
                font-weight: bold;
                color: #333;
            }}
        </style>
    </head>
    <body>
        <div class='container'>
            <div class='header'>
                {(pedido.Estado == "Completado" ? "¡Tu pedido está listo!" : "Confirmación de pedido personalizado")}
            </div>
            
            <p>Hola {(pedido.ClienteId?.FirstName ?? "Cliente")},</p>
            
            {(pedido.Estado == "Completado"
                    ? "<p class='completed'>¡Tu pedido personalizado ha sido completado y está listo para ser entregado!</p>"
                    : "<p>Hemos recibido tu solicitud de pedido personalizado correctamente.</p>")}
            
            <div class='details-container'>
                <div class='detail-item'>
                    <span class='detail-label'>Número de pedido:</span>
                    <span>#{pedidoId.ToString().Substring(0, 8).ToUpper()}</span>
                </div>
                
                <div class='detail-item'>
                    <span class='detail-label'>Estado:</span>
                    <span class='status-badge'>{pedido.Estado}</span>
                </div>
                
                <div class='detail-item'>
                    <span class='detail-label'>Fecha:</span>
                    <span>{pedido.FechaPedido.ToString("f", new CultureInfo("es-HN"))}</span>
                </div>
                
                <div class='detail-item'>
                    <span class='detail-label'>Tipo de Flor:</span>
                    <span>{pedido.TipoFlor}</span>
                </div>
                
                <div class='detail-item'>
                    <span class='detail-label'>Cantidad:</span>
                    <span>{pedido.Cantidad}</span>
                </div>
                
                <div class='detail-item'>
                    <span class='detail-label'>Incluir Presente:</span>
                    <span>{FormatOption(pedido.IncluirPresente)}</span>
                </div>
                
                {(FormatOption(pedido.IncluirPresente) == "Sí" ?
                    $@"<div class='detail-item'>
                    <span class='detail-label'>Tipo de Presente:</span>
                    <span>{pedido.TipoPresente}</span>
                </div>" : "")}
                
                <div class='detail-item'>
                    <span class='detail-label'>Incluir Base:</span>
                    <span>{FormatOption(pedido.IncluirBase)}</span>
                </div>
                
                {(FormatOption(pedido.IncluirBase) == "Sí" ?
                    $@"<div class='detail-item'>
                    <span class='detail-label'>Tipo de Base:</span>
                    <span>{pedido.TipoBase}</span>
                </div>" : "")}
            </div>
            
            {(pedido.Estado == "Completado"
                ? @"<p style='font-size: 16px;'>
                    Por favor contáctanos para coordinar la entrega de tu pedido personalizado. 
                    Horario de atención: Lunes a Viernes de 8:00 AM a 5:00 PM.
                   </p>"
                : @"<p style='font-size: 16px;'>
                    Estamos trabajando en tu pedido personalizado. Te notificaremos cuando 
                    esté en proceso y cuando haya sido completado.
                   </p>")}
            
            <div class='footer'>
                <p>¡Gracias por confiar en nosotros para crear algo especial!</p>
            </ div >
        </ div >
    </ body >
    </ html > ";

    await SendEmailAsync(clienteEmail, subject, body, isBodyHtml: true);
}

    public async Task SendOrderReadyEmailAsync(Pedido pedido)
    {
        // Obtener el pedido con toda la información necesaria
        var fullPedido = await _context.Pedidos
            .Include(p => p.Cliente)
            .Include(p => p.Detalles)
                .ThenInclude(d => d.Producto)
            .FirstOrDefaultAsync(p => p.Id == pedido.Id);

        if (fullPedido == null || fullPedido.Cliente == null)
            return;

        var toEmail = fullPedido.Cliente.Email;
        var subject = $"¡Tu pedido #{fullPedido.Id.ToString().Substring(0, 8).ToUpper()} está listo!";

        // Formateador de moneda
        var formatoMoneda = new CultureInfo("es-HN");
        formatoMoneda.NumberFormat.CurrencySymbol = "L";

        // Cuerpo del correo
        var body = $@"
            <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; line-height: 1.6; }}
                    .container {{ max-width: 600px; margin: 20px auto; padding: 20px; border: 1px solid #ddd; border-radius: 5px; }}
                    .header {{ color: #28a745; font-size: 24px; text-align: center; }}
                    table {{ width: 100%; border-collapse: collapse; margin: 20px 0; }}
                    th, td {{ padding: 10px; text-align: left; border-bottom: 1px solid #ddd; }}
                    th {{ background-color: #f5f5f5; }}
                    .total-row {{ font-weight: bold; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>¡Tu pedido está listo!</div>
                    
                    <p>Hola {fullPedido.Cliente.FirstName},</p>
                    
                    <p>Nos complace informarte que tu pedido ha sido completado y está listo para ser recolectado.</p>
                    
                    <p><strong>Número de pedido:</strong> #{fullPedido.Id.ToString().Substring(0, 8).ToUpper()}</p>
                    <p><strong>Fecha:</strong> {fullPedido.FechaPedido.ToString("f", new CultureInfo("es-HN"))}</p>
                    
                    <h3>Detalles del pedido:</h3>
                    <table>
                        <thead>
                            <tr>
                                <th>Producto</th>
                                <th>Cantidad</th>
                                <th>Precio unitario</th>
                                <th>Subtotal</th>
                            </tr>
                        </thead>
                        <tbody>
                            {string.Join("", fullPedido.Detalles.Select(d => $@"
                            <tr>
                                <td>{d.Producto?.Nombre ?? "Producto no disponible"}</td>
                                <td>{d.Cantidad}</td>
                                <td>{d.PrecioUnitario.ToString("C", formatoMoneda)}</td>
                                <td>{(d.Cantidad * d.PrecioUnitario).ToString("C", formatoMoneda)}</td>
                            </tr>"))}
                        </tbody>
                        <tfoot>
                            <tr class='total-row'>
                                <td colspan='3' style='text-align: right;'>Total:</td>
                                <td>{fullPedido.Total.ToString("C", formatoMoneda)}</td>
                            </tr>
                        </tfoot>
                    </table>
                    
                    <p>Por favor acércate a nuestra tienda para recoger tu pedido. Horario de atención: Lunes a Viernes de 8:00 AM a 5:00 PM.</p>
                    
                    <p>¡Gracias por tu preferencia!</p>
                    <p>El equipo de <strong>{_configuration["EmailSettings:BusinessName"] ?? "Floralize"}</strong></p>
                </div>
            </body>
            </html>";

        await SendEmailAsync(toEmail, subject, body, isBodyHtml: true);
    }
}