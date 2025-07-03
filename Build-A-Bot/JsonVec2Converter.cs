using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Build_A_Bot
{
    /*
     * Klasa za konverzija na Vector2 vo JSON i obratno
     * Vector2 se pretvara vo JSON objekt so atributi "X" i "Y"
     */
    public class JsonVec2Converter : JsonConverter<Vector2>
    {
        public override Vector2 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Vector2 res = new Vector2(0f);

            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                    break;

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    string propName = reader.GetString();
                    reader.Read();
                    switch (propName)
                    {
                        case "X":
                            res.X = reader.GetSingle();
                            break;
                        case "Y":
                            res.Y = reader.GetSingle();
                            break;
                    }
                }
            }

            return res;
            
        }

        public override void Write(Utf8JsonWriter writer, Vector2 value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteNumber("X", value.X);
            writer.WriteNumber("Y", value.Y);
            writer.WriteEndObject();
        }
    }
}
