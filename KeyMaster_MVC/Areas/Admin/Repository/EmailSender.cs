using System.Net.Mail;
using System.Net;

namespace KeyMaster_MVC.Areas.Admin.Repository
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)

        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true, //bật bảo mật
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("xuanlong2k414@gmail.com", "jgucgdojzdwksohx")
            };

            return client.SendMailAsync(
                new MailMessage(from: "xuanlong2k414@gmail.com",
                                    to: email,
                                    subject,
                                    message)
                );
        }
    }
}
