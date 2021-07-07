using System;
using System.Net;
using System.Net.Mail;
using WebProductNotifier.Classes;
using WebProductNotifier.Models;

namespace WebProductNotifier.Logic
{
    public class MailWork
    {
        // Внимание! Не забывайте открывать доступ к небезопасным приложениям!
        public static string from = "myeamil@myemail.com"; // Введите рабочую почту сюда
        public static string fromPassword = "MyPassword123!"; // Введите рабочую почту сюда
        
        public static void SendMessage(string to, ApplicationUser applicationUser, ProductFullInformationObject product)
        {
            // Создание письма
            MailMessage message = new MailMessage(from, to);
            message.Subject = "WebProductNotifier";
            message.Body = $"Hey, {applicationUser.UserName}! Product {product.Title} has become cheaper!";
            SmtpClient client = new SmtpClient {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(from, fromPassword)
                    };
            try
            {
                client.Send(message); // Выслать письмо
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught {0}", ex.ToString());
            }

            
        }

    }
}