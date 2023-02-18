namespace DataScraping
{
    using System.Text.Json.Serialization;

    public class Product
    {
        private double rating;

        [JsonPropertyName("productName")]
        public string Name { get; set; }

        public decimal Price { get; set; }

        public double Rating
        {
            get
            {
                return rating;
            }
            set
            {
                if (value > 5)
                {
                    var devider = 2;

                    while (value / devider > 5)
                    {
                        devider++;
                    }

                    rating = Math.Round(value / devider, 1);
                }
                else
                {
                    rating = value;
                }
            }
        }
    }
}
