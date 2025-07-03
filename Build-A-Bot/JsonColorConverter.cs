using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Build_A_Bot
{
    /*
     * Klasa za konverzija na Color vo JSON i obratno 
     * Color se pretvara vo JSON objekt so parametar "ARGB"
     */
    public class JsonColorConverter : JsonConverter<Color>
    {
        public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Color res = Segment.DEFAULT_COLOUR;

            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                    break;

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    string propName = reader.GetString();
                    if (propName.Equals("ARGB"))
                    {
                        reader.Read();
                        res = Color.FromArgb(reader.GetInt32());
                    }
                }
            }

            return res;
        }

        public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteNumber("ARGB", value.ToArgb());
            writer.WriteEndObject();
        }
    }
}
