namespace DataScraping
{
    using System.Globalization;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    public class DecimalToStringConverter : JsonConverter<decimal>
    {
        public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.GetDecimal();
        }

        public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
        {
            var stringValue = value.ToString(CultureInfo.InvariantCulture).Replace(",", ".");
            writer.WriteStringValue(stringValue.ToString());
        }
    }
}
