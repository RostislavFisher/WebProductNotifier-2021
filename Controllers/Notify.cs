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
                ApplicationUser person = _context.Users.FirstOrDefault(i => i.Id == personID);
                if (person == null)
                {
                    return Content(JsonConvert.SerializeObject(new JsonSimpleResult{key = "result", value="error: personID is invalid"}));  
                }
                else
                {
                    var section = _context.Users
                        .Include(s => s.Products)
                        .FirstOrDefault(s => s.Id == personID);

                    string result = (!section.Products.Where(i => i.ShopKey == ShopKey && i.ItemID == ItemID)
                        .IsNullOrEmpty()).ToString();
                    return Content(JsonConvert.SerializeObject(new JsonSimpleResult{key = "result", value=result}));
                    
                }
            }
        }

        public ActionResult DeleteFromNotify(string ItemID, string ShopKey, string personID)
        {
            using (var context = new ProductDbContext())
            {
                ApplicationUser person = _context.Users.FirstOrDefault(i => i.Id == personID);
                if (person == null)
                {
                    return Content(JsonConvert.SerializeObject(new JsonSimpleResult{key = "result", value="error: personID is invalid"}));
                }
                else
                {
                    context.Product.RemoveRange(context.Product.Where(x => x.ShopKey == ShopKey && x.ItemID == ItemID));
                }

                context.SaveChanges();
                _context.SaveChanges();
                return Content(JsonConvert.SerializeObject(new JsonSimpleResult{key = "result", value="SuccessfullyDeleted"}));
            } 
        }
        public ActionResult AddToNotify(string ItemID, string ShopKey, int Price, string personID)
        {
            using (var context = new ProductDbContext())
            {
                ApplicationUser person = _context.Users.FirstOrDefault(i => i.Id == personID);
                if (person == null)
                {
                    return Content(JsonConvert.SerializeObject(new JsonSimpleResult{key = "result", value="error: personID is invalid"}));
                }
                else
                {
                    var newProduct = new Product () {Price = Price, ShopKey = ShopKey, ItemID = ItemID};
                    context.Product.RemoveRange(context.Product.Where(x => x.ShopKey == ShopKey && x.Price == Price && x.ItemID == ItemID));
                    context.Product.Add(newProduct);
                    var newListOfUsersProducts = new List<Product> { newProduct };
                    person.Products = newListOfUsersProducts;
                    
                }

                context.SaveChanges();
                _context.SaveChanges();
            }
            return Content(JsonConvert.SerializeObject(new JsonSimpleResult{key = "result", value="SuccessfullyAdded"}));
        }
        
    }
}