namespace DataScraping
{
    using System.Text.Json.Serialization;
    using System.Text.Json;
    using System.Globalization;

    public class DoubleToStringConverter : JsonConverter<double>
    {
        public override double Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.GetDouble();
        }

        public override void Write(Utf8JsonWriter writer, double value, JsonSerializerOptions options)
        {
            var stringValue = value.ToString(CultureInfo.InvariantCulture).Replace(",", ".");
            writer.WriteStringValue(stringValue.ToString());
        }
    }
}
