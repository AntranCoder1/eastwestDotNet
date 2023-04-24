using System.Net;
using System.Net.Mail;

namespace eastwest.Utils
{
    public class SendMail
    {
        public SendMail()
        {
        }

        public static void emailResetPasword(string email, string token, string name)
        {
            var fromAddress = new MailAddress("thanhantran21@gmail.com", "Eastwest Warehouse");
            var toAddress = new MailAddress(email, name);
            const string fromPassword = "okifhhhxhjasrioe";
            const string subject = "Reset Password Request";
        }

    }
}