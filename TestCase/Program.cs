using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestCase.Parser;
using TestCase.SimpleWine;


namespace TestCase
{
    internal class Program
    {
        static async Task Main()
        {
            ParserPages parser = new ParserPages();
            Console.WriteLine("Запускаем парсинг");
            List<Product> productsMoscow = await parser.ParseAllProducts();

            string jsonResultMoscow = JsonConvert.SerializeObject(productsMoscow, Formatting.Indented);
            System.IO.File.WriteAllText("Moscow.json", jsonResultMoscow);

            Console.WriteLine("I'm done...");
            Console.ReadKey();
        }
    }

}
