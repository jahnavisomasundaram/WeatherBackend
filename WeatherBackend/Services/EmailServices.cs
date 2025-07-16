//using MailKit.Net.Smtp;
//using MimeKit;
//using System.Net.Mail;

//namespace WeatherBackend.Services
//{
//    public interface IEmailService
//    {
//        Task SendEmailAsync(string to, string subject, string body);
//    }

//    public class EmailService : IEmailService
//    {
//        private readonly IConfiguration _config;

//        public EmailService(IConfiguration config)
//        {
//            _config = config;
//        }

//        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
//        {
//            var email = new MimeMessage();
//            email.From.Add(MailboxAddress.Parse(_config["EmailSettings:FromEmail"]));
//            email.To.Add(MailboxAddress.Parse(toEmail));
//            email.Subject = subject;
//            email.Body = new TextPart(MimeKit.Text.TextFormat.Plain) { Text = body };

//            using var smtp = new MailKit.Net.Smtp.SmtpClient();
//            try
//            {
//                await smtp.ConnectAsync(_config["EmailSettings:SmtpServer"], 587, MailKit.Security.SecureSocketOptions.StartTls);
//                await smtp.AuthenticateAsync(_config["EmailSettings:FromEmail"], _config["EmailSettings:Password"]);
//                await smtp.SendAsync(email);
//                await smtp.DisconnectAsync(true);
//                return true;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"❌ Email sending error: {ex.Message}");
//                return false;
//            }
//        }

//        Task IEmailService.SendEmailAsync(string to, string subject, string body)
//        {
//            return SendEmailAsync(to, subject, body);
//        }
//    }

//}
