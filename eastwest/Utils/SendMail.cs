using System.Net;
using System.Net.Mail;

namespace eastwest.Utils
{
    public class SendMail
    {
        public SendMail()
        {
        }

        public void emailResetPasword(string email, string token, string name)
        {
            var fromAddress = new MailAddress("thanhantran21@gmail.com", "Eastwest Warehouse");

            var toAddress = new MailAddress(email, name);

            const string fromPassword = "okifhhhxhjasrioe";

            const string subject = "Reset Password Request";

            const string uiUrl = "http://localhost:4200";

            const string apiUrl = "http://localhost:5233";

            var body = @"
                < div style=' font - family: Avenir, Helvetica, sans - serif; box - sizing: border - box; background - color: #f5f8fa; color: #74787e; height: 100%; line-height: 1.4; margin: 0; width: 100% !important; word-break: break-word;'>
                    < table width='100 % ' cellpadding='0' cellspacing='0' style=' font - family: Avenir, Helvetica, sans - serif; box - sizing: border - box; background - color: #f5f8fa; margin: 0; padding: 0; width: 100%; '>
                        < tbody>
                            < tr>
                                < td align='center' style=' font-family: Avenir, Helvetica, sans-serif; box-sizing: border-box; '>
                                    < table width='100%' cellpadding='0' cellspacing='0' style=' font-family: Avenir, Helvetica, sans-serif; box-sizing: border-box; margin: 0; padding: 0; width: 100%; '>
                                        < tbody>
                                            < tr>
                                                < td style=' font-family: Avenir, Helvetica, sans-serif; box-sizing: border-box; padding: 25px 0; text-align: center; '>
                                                    <a a href='{uiUrl}/ ' style=' font - family: Avenir, Helvetica, sans - serif; box - sizing: border - box; color: #25af61; font-size: 19px; font-weight: bold; text-decoration: none; ' target='_blank'>Welcome to Eastwest Warehouse</a>
                                                </ td>
                                            </ tr>
                                            < tr>
                                                < td width='100%' cellpadding='0' cellspacing='0' style=' font-family: Avenir, Helvetica, sans-serif; box-sizing: border-box; background-color: #ffffff; border-bottom: 1px solid #edeff2; border-top: 1px solid #edeff2; margin: 0; padding: 0; width: 100%; '>
                                                    < table align='center' width='570' cellpadding='0' cellspacing='0' style=' font-family: Avenir, Helvetica, sans-serif; box-sizing: border-box; background-color: #ffffff; margin: 0 auto; padding: 0; width: 570px; '>
                                                        < tbody>
                                                            < tr>
                                                                < td style=' font-family: Avenir, Helvetica, sans-serif; box-sizing: border-box; padding: 35px; '>
                                                                    < p style=' font-family: Avenir, Helvetica, sans-serif; box-sizing: border-box; color: #74787e; font-size: 16px; line-height: 1.5em; margin-top: 0; text-align: left; font-weight:bold'> Hi,
                                                                        < span style='text-transform: capitalize;'> ` {name} `</ span>
                                                                    </ p>
                                                                    < p style=' font-family: Avenir, Helvetica, sans-serif; box-sizing: border-box; color: #74787e; font-size: 16px; line-height: 1.5em; margin-top: 0; text-align: left; '> You have requested to reset your password.</ p>
                                                                    < p style=' font-family: Avenir, Helvetica, sans-serif; box-sizing: border-box; color: #74787e; font-size: 16px; line-height: 1.5em; margin-top: 0; text-align: left; '> Please click the button below to reset your password!</ p>
                                                                    < table align='center' width='100%' cellpadding='0' cellspacing='0' style=' font-family: Avenir, Helvetica, sans-serif; box-sizing: border-box; margin: 30px auto; padding: 0; text-align: center; width: 100%; '>
                                                                        < tbody>
                                                                            < tr>
                                                                                < td align='center' style=' font-family: Avenir, Helvetica, sans-serif; box-sizing: border-box; '>
                                                                                    < table width='100%' border='0' cellpadding='0' cellspacing='0' style=' font-family: Avenir, Helvetica, sans-serif; box-sizing: border-box; '>
                                                                                        < tbody>
                                                                                            < tr>
                                                                                                < td align='center' style=' font-family: Avenir, Helvetica, sans-serif; box-sizing: border-box; '>
                                                                                                    < table border='0' cellpadding='0' cellspacing='0' style=' font-family: Avenir, Helvetica, sans-serif; box-sizing: border-box; '>
                                                                                                        < tbody>
                                                                                                            < tr>
                                                                                                                < td style=' font-family: Avenir, Helvetica, sans-serif; box-sizing: border-box; '>
                                                                                                                    <a href='{uiUrl}/ reset-password /{token}' style=' font - family: Avenir, Helvetica, sans - serif; box - sizing: border - box; border - radius: 3px; color: #fff; display: inline-block; text-decoration: none; background-color: #2ab27b; border-top: 10px solid #2ab27b; border-right: 18px solid #2ab27b; border-bottom: 10px solid #2ab27b; border-left: 18px solid #2ab27b; ' target='_blank'> Reset Your Password</a> 
                                                                                                                </ td>
                                                                                                            </ tr>
                                                                                                        </ tbody>
                                                                                                    </ table>
                                                                                                </ td>
                                                                                            </ tr>
                                                                                        </ tbody>
                                                                                    </ table>
                                                                                </ td>
                                                                            </ tr>
                                                                        </ tbody>
                                                                    </ table>
                                                                    < p style=' font-family: Avenir, Helvetica, sans-serif; box-sizing: border-box; color: #74787e; font-size: 16px; line-height: 1.5em; margin-top: 0; text-align: left; '> Keep Creating,
                                                                        < br /> The Eastwest Warehouse Team </ p>
                                                                    </ td>
                                                            </ tr>
                                                        </ tbody>
                                                    </ table>
                                                </ td>
                                            </ tr>
                                            < tr>
                                                < td style=' font-family: Avenir, Helvetica, sans-serif; box-sizing: border-box; '>
                                                    < table align='center' width='570' cellpadding='0' cellspacing='0' style=' font-family: Avenir, Helvetica, sans-serif; box-sizing: border-box; margin: 0 auto; padding: 0; text-align: center; width: 570px; '>
                                                        < tbody>
                                                            < tr>
                                                                < td align='center' style=' font-family: Avenir, Helvetica, sans-serif; box-sizing: border-box; padding: 35px; '>
                                                                    < p style=' font-family: Avenir, Helvetica, sans-serif; box-sizing: border-box; line-height: 1.5em; margin-top: 0; color: #aeaeae; font-size: 12px; text-align: center; '>© 2023 Eastwest Warehouse. All rights reserved.</ p>
                                                                </ td>
                                                            </ tr>
                                                        </ tbody>
                                                    </ table>
                                                </ td>
                                            </ tr>
                                        </ tbody>
                                    </ table>
                                </ td>
                            </ tr>
                        </ tbody>
                    </ table>
                </ div>
            ";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
            {
                smtp.Send(message);
            }
        }

    }
}