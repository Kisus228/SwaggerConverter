using Newtonsoft.Json;

namespace SwaggerDocumentationGenerator.Data
{
	public class ParameterDescription
	{
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string? In { get; set; }
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string? Name { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string? Description { get; set; }
		public bool Required { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public Schema? Schema { get; set; }
	}
}
