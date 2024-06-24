using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enviar_Correo
{
    public class SmtpSettings
    {
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool EnableSsl { get; set; }
    }

    public class EmailSettings
    {
        public string FromAddress { get; set; }
        public List<string> ToAddresses { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }

    public class FilePathSettings
    {
        public string DirectoryPath { get; set; }
    }

}
