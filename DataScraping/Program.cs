using System.Globalization;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using DataScraping;
using HtmlAgilityPack;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

var pdfFilePath = "../../../../Test task - Web Scraping Specialist.pdf";

var pdfText = ExtractPdfText(pdfFilePath);

var productsList = GetProductsFromPdf(pdfText);

var productsAsJson = SerializeCollectionOfProducts(productsList);

Console.WriteLine(productsAsJson);

static string ExtractPdfText(string pdfFilePath)
{
    var pdfText = new StringBuilder();

    using (var reader = new PdfReader(pdfFilePath))
    {
        for (int i = 1; i <= reader.NumberOfPages; i++)
        {
            var currentLine = PdfTextExtractor.GetTextFromPage(reader, i);
            pdfText.AppendLine(currentLine);
        }
    }

    return pdfText.ToString();
}

static IEnumerable<Product> GetProductsFromPdf(string pdfText)
{
    var products = new List<Product>();

    var htmlDocument = new HtmlDocument();
    htmlDocument.LoadHtml(pdfText);

    var productEmementsCollection = htmlDocument
        .DocumentNode
        .SelectNodes("div")
        .Where(node => node.Attributes["class"].Value != null
                    && node.Attributes["class"].Value == "item")
        .ToList();

    foreach (var element in productEmementsCollection)
    {
        var format = new NumberFormatInfo();
        format.NumberDecimalSeparator = ".";
        format.NumberGroupSeparator = ",";

        var ratingAsString = element
            .GetAttributeValue("rating", "")
            .ToString();

        var rating = double.Parse(ratingAsString, NumberStyles.Any, format);

        var encodedProductName = element
            .Descendants("img")
            .Select(atributes => atributes.Attributes["alt"].Value)
            .FirstOrDefault();

        string productName = WebUtility.HtmlDecode(encodedProductName);

        var priceSpanElement = element
            .Descendants("span")
            .Where(atributes => atributes.Attributes["class"].Value != null
                                && atributes.Attributes["class"].Value == "price-display formatted")
            .FirstOrDefault();

        var priceAsString = priceSpanElement
            .FirstChild
            .InnerText
            .ToString()
            .Remove(0, 1);

        var price = decimal.Parse(priceAsString, NumberStyles.Any, format);

        var currentProduct = new Product()
        {
            Name = productName,
            Price = price,
            Rating = rating,
        };

        products.Add(currentProduct);
    }

    return products;
}

static string SerializeCollectionOfProducts(IEnumerable<Product> productsList)
{
    var settings = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        Converters = { new DecimalToStringConverter(), new DoubleToStringConverter() },
        WriteIndented = true,
    };

    return JsonSerializer.Serialize(productsList, settings);
}