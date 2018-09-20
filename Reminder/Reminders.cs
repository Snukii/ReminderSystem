using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Net;
using System.Net.Mime;
using System.Net.Mail;
using System.IO;

namespace Reminder
{
    class Reminders
    {
        //Reminder sent via Email
        public void SendEMAIL(string email, string message)
        {
            Console.WriteLine("\n" + "Sending email to: " + email);
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress("XXX@gmail.com", "Dentist");
                mail.To.Add(email);
                mail.Subject = "Reminder";
                mail.Body = message;
                mail.IsBodyHtml = true;

                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.Credentials = new NetworkCredential("XXX@gmail.com", "PASSWORDHERE");
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }
            }
        }

        //Reminder sent via SMS
        public string SendSMS(string msisdn, string message)
        {
            string username = "XXXXX";
            string apikey = "XXXXXXXXXXXXXXXXXXXXXXXXXXX";
            string basicauth = Convert.ToBase64String(Encoding.UTF8.GetBytes(username + ":" + apikey));

            string from = "Dentist";

            string url = $"https://{username}:{apikey}@api.cpsms.dk/v2/simplesend/45{msisdn}/{System.Uri.EscapeDataString(message)}/{System.Uri.EscapeDataString(from)}";


            Console.WriteLine("\n" + "Sending SMS to: " + msisdn + " : " + message);


            using (WebClient client = new WebClient())
            {
                client.Headers["Authorization"] = $"Basic {basicauth}";
                return client.DownloadString(url);

            }

        }

    }
}
