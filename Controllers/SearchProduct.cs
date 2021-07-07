using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebProductNotifier.Classes;

namespace WebProductNotifier.Controllers
{
    public class SearchProduct : Controller
    {
        public ActionResult Search(string ShopTitle, string Manufacturer, string FirstPriceScope, string SecondPriceScope, string Title)
        {
            // Создание списка с магазинами
            IDictionary<string, SearchInterface> dict = new Dictionary<string, SearchInterface>();
            dict["Rozetka"] = new Rozetka();
            dict["Foxtrot"] = new Foxtrot();
            
            // Сборка аргументов в один объект
            ObjectSearch objectSearch = new ObjectSearch {ShopTitle = ShopTitle, Manufacturer = Manufacturer, FirstPriceScope = FirstPriceScope, SecondPriceScope=SecondPriceScope, Title=Title};
            // Получение списка товаров по коду магазина и объекту сборки аргументов
            List<ProductObject> productSearchList =  dict[ShopTitle].searchProduct(objectSearch);
            string result = JsonConvert.SerializeObject(productSearchList);

            return Content(result);
        }
        
        public ActionResult SearchInShopByCode(string shopKey, string ItemID)
        {
            // Создание списка с магазинами
            IDictionary<string, SearchInterface> dict = new Dictionary<string, SearchInterface>();
            dict["Rozetka"] = new Rozetka();
            dict["Foxtrot"] = new Foxtrot();
            
            // Получение товара по коду магазина и его коду.
            ProductFullInformationObject ProductFullInformationObject = dict[shopKey].getProductFullInformationObject(
                new ObjectToSearch {shopKey = shopKey, ItemID = ItemID}
                );
            return Content(JsonConvert.SerializeObject(ProductFullInformationObject));
        }
    }
}