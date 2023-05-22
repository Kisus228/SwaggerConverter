using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SwaggerDocumentationGenerator.Helpers;

namespace SwaggerDocumentationGenerator.Data
{
	public class PathDescription
	{
		[JsonIgnore]
		public string HttpMethod { get; set; }
		public string[] Tags { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string Summary { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string? Description { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public ParameterDescription[]? Parameters { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public RequestBody? RequestBody { get; set; }

		public SwaggerResponses Responses { get; set; }
	}

	public class RequestBody
	{
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string? Description { get; set; }
		public bool Required { get; set; } = true;
		public Content Content { get; set; }
	}

	public class ContentConverter: JsonConverter<Content>
	{
		public override void WriteJson(JsonWriter writer, Content? value, JsonSerializer serializer)
		{
			if(value == null)
				return;
			var result = new JObject
						 {
							 { value.ContentType, JToken.FromObject(value.ContentDescription, CommonHelpers.Serializer) }
						 };
			result.WriteTo(writer);
		}

		public override Content ReadJson(JsonReader reader, Type objectType, Content? existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
	}

	[JsonConverter(typeof(ContentConverter))]
	public class Content
	{
		public ContentDescription ContentDescription { get; set; }

		public string ContentType { get; set; }

	}

	public class ContentDescription
	{
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string? Description { get; set; }
		public Schema Schema { get; set; }
	}
}
