using System.Collections.Generic;
using System.Text;


namespace TestCase.SimpleWine
{
    public class Product
    {
        public string Name { get; set; }

        public int Price { get; set; }

        public int OldPrice { get; set; }

        public float Rating { get; set; }

        public string Volume { get; set; }

        public int Articul { get; set; }

        public string Region { get; set; }

        public string Url { get; set; }

        public List<string> Pictures { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Name: {Name}");
            sb.AppendLine($"Price: {Price}");
            sb.AppendLine($"OldPrice: {OldPrice}");
            sb.AppendLine($"Rating: {Rating}");
            sb.AppendLine($"Volume: {Volume}");
            sb.AppendLine($"Articul: {Articul}");
            sb.AppendLine($"Region: {Region}");
            sb.AppendLine($"Url: {Url}");
            sb.AppendLine("Pictures: " + (Pictures != null ? string.Join(", ", Pictures) : "None"));

            return sb.ToString();

        }
    }
}