using AngleSharp;
using AngleSharp.Io;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TestCase.SimpleWine;

namespace TestCase.Parser
{
    public class ParserPages
    {
        private readonly string _baseUrl;
        private readonly int _numberOfThreads;
        private readonly string _uri;
        public ParserPages(string uri = "/catalog/shampanskoe_i_igristoe_vino/")
        {
            _baseUrl = "https://simplewine.ru";
            _uri = uri;
            _numberOfThreads = 3;
        }

        private IBrowsingContext CreateBrowsingContext()
        {
            var config = Configuration.Default.WithDefaultLoader().WithDefaultCookies();
            return BrowsingContext.New(config);
        }

        private async Task<string> GetCity()
        {
            var context = CreateBrowsingContext();
            var document = await context.OpenAsync(_baseUrl + _uri);
            string city = document.QuerySelector(".location__current.dropdown__toggler").TextContent.Trim().Replace(" ", "");
            return city;
        }

        private async Task<int> GetPages()
        {
            try
            {
                var context = CreateBrowsingContext();
                var document = await context.OpenAsync(_baseUrl + _uri);
                var catalogElement = document.QuerySelector(".catalog.js-catalog.new-catalog-navigation");
                int totalPageCount = int.Parse(catalogElement.GetAttribute("data-page-total"));
                context.Dispose();
                return totalPageCount;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}.");
                return 0;
            }
        }

        private async Task<List<string>> ParseAllLinks()
        {
            List<string> allLinks = new List<string>();
            int pages = await GetPages();
            try
            {
                var tasks = Enumerable.Range(1, pages).Select(page => ParsePage(page)).ToList();
                var results = await Task.WhenAll(tasks);
                foreach (var result in results)
                {
                    allLinks.AddRange(result);
                }
                return allLinks;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}.");
                return null;
            }
        }

        public async Task<List<Product>> ParseAllProducts()
        {
            var links = await ParseAllLinks();
            var productChunks = links.Select((link, index) => new { link, index }).GroupBy(x => x.index % _numberOfThreads).Select(g => g.Select(x => x.link).ToList())
                                     .ToList();

            var tasks = productChunks.Select(chunk => Task.Run(async () =>
            {
                var products = new List<Product>();
                foreach (var link in chunk)
                {
                    var product = await ParseProduct(link);
                    if (product != null)
                    {
                        products.Add(product);
                    }
                }
                return products;
            })).ToArray();

            var results = await Task.WhenAll(tasks);
            return results.SelectMany(x => x).ToList();
        }

        private async Task<List<string>> ParsePage(int page)
        {
            try
            {
                string url = $"{_baseUrl + _uri}page{page}";
                var context = CreateBrowsingContext();
                var document = await context.OpenAsync(url);
                var links = document.QuerySelectorAll(".swiper-container").Select(x => x.GetAttribute("href")).ToList();
                context.Dispose();
                return links;
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}.");
                return null;
            }
        }

        private async Task<Product> ParseProduct(string url)
        {
            var context = CreateBrowsingContext();
            var document = await context.OpenAsync(_baseUrl + url);
            int oldPrice, price;

            if (document.QuerySelector("dd.product-brief__value a[href*='/filter/volume-']") == null)
            {
                return null;
            }
            var strOldPrice = document.QuerySelector(".product-buy__old-price")?.TextContent.Replace(" ", "");
            var strPrice = document.QuerySelector(".product-buy__price").TextContent.Replace(" ", "");
            oldPrice = strOldPrice != null ? int.Parse(Regex.Match(strOldPrice, @"\d+").Value) : int.Parse(Regex.Match(strPrice, @"\d+").Value);
            price = int.Parse(Regex.Match(strPrice, @"\d+").Value);

            var product = new Product()
            {
                Url = _baseUrl + url,
                Region = await GetCity(),
                Name = document.QuerySelector(".product-page__header").TextContent.Trim(),
                Articul = int.Parse(document.QuerySelector(".product-page__article.js-copy-article").TextContent.Replace("Артикул: ", "")),
                Rating = float.Parse(document.QuerySelector(".rating-stars__value").TextContent.Trim().Replace(".", ",")),
                Volume = document.QuerySelector("dd.product-brief__value a[href*='/filter/volume-']").TextContent,
                OldPrice = oldPrice,
                Price = price,
                Pictures = document.QuerySelectorAll(".product-slider picture img").Select(x => x.GetAttribute("src").ToString()).ToList()
            };
            Console.WriteLine(product.ToString());
            context.Dispose();
            return product;
        }
    }
}
