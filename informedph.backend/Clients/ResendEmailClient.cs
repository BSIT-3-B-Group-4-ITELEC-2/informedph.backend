using Resend;

namespace informedph.backend.Clients
{
    public class ResendEmailClient
    {
        private readonly IResend _resend;
        private readonly IConfiguration _configuration;

        public ResendEmailClient(
            IResend resend,
            IConfiguration configuration)
        {
            _resend = resend;
            _configuration = configuration;
        }

        public async Task SendAsync(
            string recipient,
            string subject,
            string htmlBody)
        {
            var fromEmail = Environment.GetEnvironmentVariable(
                    "RESEND_FROM_EMAIL"
                ) ?? throw new InvalidOperationException(
                    "Resend sender email is not configured."
                );

            var message = new EmailMessage
            {
                From = fromEmail,
                Subject = subject,
                HtmlBody = htmlBody
            };

            message.To.Add(recipient);

            await _resend.EmailSendAsync(message);
        }
    }
}
