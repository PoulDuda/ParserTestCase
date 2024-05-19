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
            ParserPages parserMoscow = new ParserPages();
            Console.WriteLine("Запускаем парсинг");
            List<Product> productsMoscow = await parserMoscow.ParseAllProducts();

            string jsonResultMoscow = JsonConvert.SerializeObject(productsMoscow, Formatting.Indented);
            System.IO.File.WriteAllText("Moscow.json", jsonResultMoscow);

            Console.WriteLine("То же самое для славного города Сочи");
            ParserPages parserSochi = new ParserPages("/catalog/shampanskoe_i_igristoe_vino/?setVisitorCityId=5");
            List<Product> productsSochi = await parserSochi.ParseAllProducts();

            string jsonResultSochi = JsonConvert.SerializeObject(productsSochi, Formatting.Indented);
            System.IO.File.WriteAllText("Sochi.json", jsonResultSochi);

            Console.WriteLine("I'm done...");
            Console.ReadKey();
        }
    }

}
