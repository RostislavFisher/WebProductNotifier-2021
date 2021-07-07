using System;
using Microsoft.AspNetCore.Identity;

using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebProductNotifier.Classes;
using WebProductNotifier.Data;
using WebProductNotifier.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using JsonResult = Microsoft.AspNetCore.Mvc.JsonResult;

namespace WebProductNotifier.Controllers
{
    public class Notify : Controller
    {
        ApplicationDbContext _context;
        public Notify(ApplicationDbContext context)
        {
            _context = context;
        }
        

        public ActionResult CheckIfAlreadyAdded(string ItemID, string ShopKey, string personID)
        {
            using (var context = new ProductDbContext())
            {
                // Найти пользователя по его ID. Если пользователя нет - возвращается null
                ApplicationUser person = _context.Users.FirstOrDefault(i => i.Id == personID);
                // Пользователя нет
                if (person == null)
                {
                    // результат:
                    // {
                    //     "result": "error: personID is invalid"
                    // }
                    return Content(JsonConvert.SerializeObject(new JsonSimpleResult{key = "result", value="error: personID is invalid"}));  
                }
                
                
                // Пользователь есть - код продолжается
                // Получить список товаров пользователя по его ID
                var section = _context.Users
                    .Include(s => s.Products)
                    .FirstOrDefault(s => s.Id == personID);

                // Проверить, есть ли конкретно этот товар у пользователя в списке
                string result = (!section.Products.Where(i => i.ShopKey == ShopKey && i.ItemID == ItemID)
                    .IsNullOrEmpty()).ToString();
                // результат:
                // {
                //     "result": "True"
                // }
                return Content(JsonConvert.SerializeObject(new JsonSimpleResult{key = "result", value=result}));
                    
                
            }
        }

        public ActionResult DeleteFromNotify(string ItemID, string ShopKey, string personID)
        {
            using (var context = new ProductDbContext())
            {
                // Найти пользователя по его ID. Если пользователя нет - возвращается null
                ApplicationUser person = _context.Users.FirstOrDefault(i => i.Id == personID);
                // Пользователя нет
                if (person == null)
                {
                    // результат:
                    // {
                    //     "result": "error: personID is invalid"
                    // }
                    return Content(JsonConvert.SerializeObject(new JsonSimpleResult{key = "result", value="error: personID is invalid"}));
                }
                
                
                // Пользователь есть - код продолжается
                // Получить список товаров пользователя по его ID
                var section = _context.Users
                    .Include(s => s.Products)
                    .FirstOrDefault(s => s.Id == personID);

                // Удалить товар
                context.Product.RemoveRange(section.Products.Where(i => i.ShopKey == ShopKey && i.ItemID == ItemID));
                context.SaveChanges();
                _context.SaveChanges();
   
                // результат:
                // {
                //     "result": "True"
                // }
                return Content(JsonConvert.SerializeObject(new JsonSimpleResult{key = "result", value="SuccessfullyDeleted"}));
            } 
        }
        
        public ActionResult AddToNotify(string ItemID, string ShopKey, int Price, string personID)
        {
            using (var context = new ProductDbContext())
            {
                // Найти пользователя по его ID. Если пользователя нет - возвращается null
                ApplicationUser person = _context.Users.FirstOrDefault(i => i.Id == personID);
                // Пользователя нет
                if (person == null)
                {
                    // результат:
                    // {
                    //     "result": "error: personID is invalid"
                    // }
                    return Content(JsonConvert.SerializeObject(new JsonSimpleResult{key = "result", value="error: personID is invalid"}));
                }
                
                // Пользователь есть - код продолжается
                // Получить список товаров пользователя по его ID
                var section = _context.Users
                    .Include(s => s.Products)
                    .FirstOrDefault(s => s.Id == personID);
                
                
                // Создание экземляра объекта Product
                var newProduct = new Product () {Price = Price, ShopKey = ShopKey, ItemID = ItemID};
                
                // Удаляем повторы товара в список товаров пользователя до этого: (если объекта в БД нету - код не выдаст ошибку)
                context.Product.RemoveRange(section.Products.Where(i => i.ShopKey == ShopKey && i.ItemID == ItemID));
                
                // Добавляем товар в список товаров пользователя
                context.Product.Add(newProduct);
                person.Products = new List<Product> { newProduct };

                context.SaveChanges();
                _context.SaveChanges();
            }
            // результат:
            // {
            //     "result": "SuccessfullyAdded"
            // }
            return Content(JsonConvert.SerializeObject(new JsonSimpleResult{key = "result", value="SuccessfullyAdded"}));
        }
        
    }
}