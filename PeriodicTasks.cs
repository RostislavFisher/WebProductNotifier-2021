using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NCrontab;
using WebProductNotifier.Classes;
using WebProductNotifier.Data;
using WebProductNotifier.Logic;
using WebProductNotifier.Models;

public class PeriodicTasksService : BackgroundService
{
    private CrontabSchedule _schedule;
    private DateTime _nextRun;

    private  string Schedule => "* 0 */1 * * *"; //Runs every hour

    private readonly IServiceScopeFactory scopeFactory;
    public PeriodicTasksService(IServiceScopeFactory scopeFactory)
    {
        this.scopeFactory = scopeFactory;
        _schedule = CrontabSchedule.Parse(Schedule,new CrontabSchedule.ParseOptions { IncludingSeconds = true });
        _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        do
        {
            var now = DateTime.Now;
            var nextrun = _schedule.GetNextOccurrence(now);
            if (now > _nextRun)
            {
                 Process();
                _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
            }
            await Task.Delay(5000, stoppingToken); 
        }
        while (!stoppingToken.IsCancellationRequested);
    }

    private void Process()
    {
        // Создание списка с магазинами
        IDictionary<string, SearchInterface> dict = new Dictionary<string, SearchInterface>();
        dict["Rozetka"] = new Rozetka();
        dict["Foxtrot"] = new Foxtrot();

        using (var scope = scopeFactory.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            // Обработка каждого пользователя, у которого есть товары в списке отслеживаний 
            foreach (var applicationUser in dbContext.Users.Include(s => s.Products).Where(i => i.Products.Any()))
            {
                // Обработка каждого товара пользователя
                foreach (var product in applicationUser.Products)
                {
                    // Получение актуальной информации о продукте
                    ProductFullInformationObject ProductFullInformationObject = dict[product.ShopKey].getProductFullInformationObject(
                        new ObjectToSearch {shopKey = product.ShopKey, ItemID = product.ItemID}
                    );
                    // Если старая цена > новая цена - значит подешевело
                    if (product.Price > ProductFullInformationObject.PriceUAH)
                    {
                        // Обновление информации о конкретном товаре
                        product.Price = ProductFullInformationObject.PriceUAH;
                        
                        // Уведомить пользователя
                        MailWork.SendMessage(applicationUser.Email, applicationUser, ProductFullInformationObject);
                        dbContext.Attach(product);
                    }
                }
            }

            dbContext.SaveChanges();


        }
        

    }
}