using System;
using Kontur.Elba.SwaggerDocumentationGenerator.Data;
using Kontur.Elba.SwaggerDocumentationGenerator.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Kontur.Elba.SwaggerDocumentationGenerator.Converters
{
	public class AdditionalPropertiesDescriptionConverter: JsonConverter<AdditionalPropertiesDescription>
	{
		public override void WriteJson(JsonWriter writer, AdditionalPropertiesDescription? value, JsonSerializer serializer)
		{
			if(value == null)
				return;
			var description = new JObject
							  {
								  { "type", value.Type },
							  };

			var properties = new JObject();
			foreach (var pair in value.Properties)
			{
				properties.Add(pair.Key, JToken.FromObject(pair.Value, CommonHelpers.Serializer));
			}

			description.Add("properties", properties);
			description.WriteTo(writer);
		}

		public override AdditionalPropertiesDescription ReadJson(JsonReader reader,
																 Type objectType,
																 AdditionalPropertiesDescription? existingValue,
																 bool hasExistingValue,
																 JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
	}
}
