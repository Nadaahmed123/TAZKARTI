using System.Net;
using System.Net.Mail;
namespace Tazkarti.Helpers
{
    public static class EmailSettings
    {
        public static void SendEmail(Email email){
            var client=new SmtpClient("smtp.gmail.com",587);
             client.EnableSsl = true;
             client.Credentials =new NetworkCredential("na6594276@gmail.com","ekthtlgscgqmcjom");
             client.Send("na6594276@gmail.com",email.To,email.Title,email.Body);
        }

    }
}