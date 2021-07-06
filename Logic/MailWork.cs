using System;
using System.Net;
using System.Net.Mail;
using WebProductNotifier.Classes;
using WebProductNotifier.Models;

namespace WebProductNotifier.Logic
{
    public class MailWork
    {
        public static void SendMessage(string to, ApplicationUser applicationUser, ProductFullInformationObject product)
        {
            string from = "myeamil@myemail.com"; // put your email here
            MailMessage message = new MailMessage(from, to);
            message.Subject = "WebProductNotifier";
            message.Body = $"Hey, {applicationUser.UserName}! Product {product.Title} has become cheaper!";
            SmtpClient client = new SmtpClient {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(from, "myPassword") // put your password here
                    };
            try
            {
                client.Send(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in CreateTestMessage2(): {0}",
                    ex.ToString());
            }

            
        }

    }
}