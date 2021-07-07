using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebProductNotifier.Classes
{
    public class Rozetka : SearchInterface
    {
        public List<ProductObject> searchProduct(ObjectSearch objectSearch)
        {
            string url = $"https://search.rozetka.com.ua/search/api/v6/?front-type=xl&country=UA&lang=ru&text={objectSearch.Title}&price={objectSearch.FirstPriceScope}-{objectSearch.SecondPriceScope}&producer={objectSearch.Manufacturer}";
            System.Net.WebClient wc = new System.Net.WebClient();
            string webData = wc.DownloadString(url);

            JObject json = JObject.Parse(webData);

            List<ProductObject> productObjects = new List<ProductObject>();
            foreach (var VARIABLE in json["data"]["goods"])
            {
                var Title = VARIABLE["title"].Value<string>();
                var id = VARIABLE["id"].Value<string>();
                var Link = VARIABLE["href"].Value<string>();
                var img = VARIABLE["image_main"].Value<string>();
                double PriceUAH = VARIABLE["price"].Value<double>();
                double PriceUSD= VARIABLE["price_pcs"].Value<double>();
                productObjects.Add(new ProductObject
                {
                    Title = Title, img = img, Link = Link,
                    ShopObject = new ShopObject{ShopCountry = "UA", ShopTitle = "Rozetka"}, ItemID = id,
                    PriceUAH = (PriceUAH), PriceUSD = (PriceUSD)
                });

            }

            return productObjects;
        }

        public ShopObject Shop()
        {
            return new ShopObject{ShopTitle = "Rozetka", ShopCountry = "UA"};
        }

        public ProductFullInformationObject getProductFullInformationObject(ObjectToSearch objectToSearch)
        {
            string url = $"https://product-api.rozetka.com.ua/v4/goods/get-main?front-type=xl&goodsId={objectToSearch.ItemID}]";
            System.Net.WebClient wc = new System.Net.WebClient();
            string webData = wc.DownloadString(url);

            JObject json = JObject.Parse(webData);
            var Title = json["data"]["article"].ToString();
            var id = objectToSearch.ItemID;
            var Link = json["data"]["href"].ToString();
            var img = json["data"]["images"][0]["base_action"]["url"].ToString();
            double PriceUAH = Convert.ToDouble(json["data"]["price"]);
            double PriceUSD= Convert.ToDouble(json["data"]["price_pcs"]);
            var description = json["data"]["description"]["text"].ToString();
            
            return new ProductFullInformationObject
            {
                ItemID = id, Title = Title, 
                Link = Link, img = img,
                ShopObject = new ShopObject{ShopCountry = "UA", ShopTitle = "Rozetka"}, PriceUAH = PriceUAH,
                PriceUSD = PriceUSD, description = description
            };

        }
    }

    // Поддержка Foxtrot сломалась. Когда я делал - все работало. Боюсь, поддержку Foxtrot в ближайшее время починить будет нельзя.
    // Ребята не используют REST Api, а, значит, их придется распаршивать, что, естественно, и будет делом долгим, тяжелым в тестировке,
    // тяжелым в получении данных, да и парсить мне леньки
    public class Foxtrot : SearchInterface
    {
        public List<ProductObject> searchProduct(ObjectSearch objectSearch)
        {
            string url = $"https://www.foxtrot.com.ua/ru/search?query={objectSearch.Title}&price=[{objectSearch.FirstPriceScope}-{objectSearch.SecondPriceScope}]";
            var web = new HtmlAgilityPack.HtmlWeb();
            HtmlDocument doc = web.Load(url);
            var metascore = doc.DocumentNode.SelectNodes("//div[contains(@class, 'card js-card')]");
            List<ProductObject> productObjects = new List<ProductObject>();

            foreach (HtmlNode link in metascore)
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(link.InnerHtml);

                var Name = htmlDoc.DocumentNode.SelectSingleNode("//a[contains(@class, 'card__title')]").Attributes["title"].Value;
                var ItemID = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(@class, 'card__head')]").Attributes["data-gid"].Value;
                var img = htmlDoc.DocumentNode.SelectSingleNode($"//img[contains(@alt, '{Name}')]").Attributes["data-src"].Value;
                var href = "https://www.foxtrot.com.ua" + htmlDoc.DocumentNode.SelectSingleNode("//a[contains(@class , 'card__title')]").Attributes["href"].Value;
                double priceUAH = Convert.ToDouble(htmlDoc.DocumentNode.SelectSingleNode("//div[contains(@class, 'card__head')]").Attributes["data-price"].Value);
                productObjects.Add(new ProductObject
                {
                    ItemID = ItemID,
                    Title = Name, img = img, Link = href, ShopObject = new ShopObject{ShopTitle = "Foxtrot", ShopCountry = "UA"},
                    PriceUAH = (priceUAH), PriceUSD = (priceUAH)
                });
                
            }
            return productObjects;
        }
        
        public ShopObject Shop()
        {
            return new ShopObject{ShopTitle = "Foxtrot", ShopCountry = "UA"};
        }
        public ProductFullInformationObject getProductFullInformationObject(ObjectToSearch objectToSearch)
        {
            string url = $"https://www.foxtrot.com.ua/ru/search?query={objectToSearch.ItemID}]";
            var web = new HtmlAgilityPack.HtmlWeb();
            HtmlDocument doc = web.Load(url);
            
            System.Console.WriteLine(doc.DocumentNode.InnerHtml);
            
            var metascore = doc.DocumentNode.SelectNodes("//main");

            var card_priceUAH = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'card-price')]").InnerText.Replace(" ", "").Replace("₴", ""); // priceUAH
            var card_priceENG = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'card-price')]").InnerText.Replace(" ", "").Replace("₴", ""); // priceENG
            var Title = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'page__title')]").InnerText; // Title
            var img = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'action-icon')]").Attributes["src"].Value; // img
            ShopObject shopObject = new ShopObject { ShopCountry = "UA", ShopTitle = "Foxtrot"}; // shopObject
            var Country = "UA";
            var description = "";
            
            return new ProductFullInformationObject
            {
                ItemID = objectToSearch.ItemID, Title = Title, 
                Link = url, img = img,
                ShopObject = shopObject, PriceUAH = Convert.ToDouble(card_priceUAH),
                PriceUSD = Convert.ToDouble(card_priceENG), description = description
            };

        }

    }
}