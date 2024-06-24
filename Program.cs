using Enviar_Correo;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;

class Program
{
    static void Main(string[] args)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        try
        {
            // Obtener configuraciones específicas del archivo appsettings.json
            SmtpSettings smtpSettings = configuration.GetSection("SmtpSettings").Get<SmtpSettings>();
            EmailSettings emailSettings = configuration.GetSection("EmailSettings").Get<EmailSettings>();
            FilePathSettings filePathSettings = configuration.GetSection("FilePathSettings").Get<FilePathSettings>();

            string directoryPath = filePathSettings.DirectoryPath;

            // Obtener archivos de la ruta especificada
            string[] files = Directory.GetFiles(directoryPath);
            if (files.Any())
            {
                // Obtener el último archivo de la ruta especificada
                string lastFilePath = files.OrderByDescending(f => new FileInfo(f).LastWriteTime).FirstOrDefault();

                if (lastFilePath != null)
                {
                    SendEmailWithAttachment(smtpSettings, emailSettings, lastFilePath);
                    Console.WriteLine("Correo enviado correctamente.");
                }
                else
                {
                    Console.WriteLine("No se encontraron archivos en la ruta especificada.");
                }
            }
            else
            {
                Console.WriteLine("No se encontraron archivos en la ruta especificada.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        Console.WriteLine("Presiona cualquier tecla para salir.");
        Console.ReadKey();
    }

    static void SendEmailWithAttachment(SmtpSettings smtpSettings, EmailSettings emailSettings, string attachmentPath)
    {
        using (var message = new MailMessage())
        {
            foreach (var toAddress in emailSettings.ToAddresses)
            {
                message.To.Add(toAddress);
            }
            message.From = new MailAddress(emailSettings.FromAddress);
            message.Subject = emailSettings.Subject;
            message.Body = emailSettings.Body;

            // Adjuntar archivo al correo
            if (!string.IsNullOrEmpty(attachmentPath))
            {
                message.Attachments.Add(new Attachment(attachmentPath));
            }

            using (var client = new SmtpClient(smtpSettings.SmtpServer, smtpSettings.Port))
            {
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(smtpSettings.UserName, smtpSettings.Password);
                client.EnableSsl = smtpSettings.EnableSsl;

                client.Send(message);
            }
        }
    }
}
