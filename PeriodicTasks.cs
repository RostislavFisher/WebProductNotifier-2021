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

    private  string Schedule => "*/50 * * * * *"; //Runs every 10 seconds

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
            await Task.Delay(5000, stoppingToken); //5 seconds delay
        }
        while (!stoppingToken.IsCancellationRequested);
    }

    private void Process()
    {
        Console.WriteLine("====");
        IDictionary<string, SearchInterface> dict = new Dictionary<string, SearchInterface>();
        dict["Rozetka"] = new Rozetka();
        dict["Foxtrot"] = new Foxtrot();


        using (var scope = scopeFactory.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            foreach (var applicationUser in dbContext.Users.Include(s => s.Products).Where(i => i.Products.Any()))
            {
                foreach (var product in applicationUser.Products)
                {
                    ProductFullInformationObject ProductFullInformationObject = dict[product.ShopKey].getProductFullInformationObject(
                        new ObjectToSearch {shopKey = product.ShopKey, ItemID = product.ItemID}
                    );
                    if (product.Price > ProductFullInformationObject.PriceUAH)
                    {
                        Console.WriteLine("=== Уведомить на почту ===");
                        product.Price = ProductFullInformationObject.PriceUAH;
                        MailWork.SendMessage(applicationUser.Email, applicationUser, ProductFullInformationObject);
                        dbContext.Attach(product);
                    }
                }
            }

            dbContext.SaveChanges();


        }
        

    }
}