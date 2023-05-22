using System;
using System.Collections.Generic;
using System.Linq;
using Kontur.Elba.SwaggerDocumentationGenerator.Data;
using Kontur.Elba.SwaggerDocumentationGenerator.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Kontur.Elba.SwaggerDocumentationGenerator.Converters
{
	public class TypeDescriptionJsonConverter: JsonConverter<TypeDescription>
	{
		public override void WriteJson(JsonWriter writer, TypeDescription? value, JsonSerializer serializer)
		{
			if(value == null)
				return;

			var result = new JObject ();

			if (value.Properties != null)
				result.Add("properties", PropertiesToJObject(value.Properties));

			if (value.Type != null)
				result.Add("type", value.Type);

			if (value.Required != null && value.Required.Any())
				result.Add("required", JArray.FromObject(value.Required));

			serializer.Serialize(writer, result);
		}

		private static JToken PropertiesToJObject(Dictionary<string, Schema> typeInfoProperties)
		{
			var result = new JObject();
			foreach (var typeInfoProperty in typeInfoProperties)
			{
				if (typeInfoProperty.Value.Ref == null || typeInfoProperty.Value.Description == null)
					result.Add(typeInfoProperty.Key, JToken.FromObject(typeInfoProperty.Value, CommonHelpers.Serializer));
				else
				{
					result.Add(typeInfoProperty.Key, new JObject
													 {
														 { "description", typeInfoProperty.Value.Description },
														 {
															 "allOf", new JArray(new JObject
																				 {
																					 { "$ref", typeInfoProperty.Value.Ref }
																				 })
														 }
													 });
				}
			}


			return result;
			}

		public override TypeDescription ReadJson(JsonReader reader,
												 Type objectType,
												 TypeDescription? existingValue,
												 bool hasExistingValue,
												 JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
	}
}
