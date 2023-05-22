using System;
using Kontur.Elba.SwaggerDocumentationGenerator.Data;
using Kontur.Elba.SwaggerDocumentationGenerator.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Kontur.Elba.SwaggerDocumentationGenerator.Converters
{
	public class ResponseInfoJsonConverter: JsonConverter<SwaggerResponses>
	{
		public override void WriteJson(JsonWriter writer, SwaggerResponses? value, JsonSerializer serializer)
		{
			if(value == null)
				return;

			var result = new JObject();
			foreach (var response in value.ResponsesSchemas)
			{
				var responseInfo = new JObject
							  {
								  { "description", response.Description },
							  };
				if (response.Content != null)
				{
					responseInfo.Add(
						"content", new JObject
								   {
									   {
										   response.Content.ContentType, JToken.FromObject(response.Content.ContentDescription, CommonHelpers.Serializer)
									   }
								   }
					);
				}
				result.Add(((int)response.HttpStatusCode).ToString(), responseInfo);
			}
			serializer.Serialize(writer, result);
		}

		public override SwaggerResponses ReadJson(JsonReader reader,
												  Type objectType,
												  SwaggerResponses? existingValue,
												  bool hasExistingValue,
												  JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
	}
}
