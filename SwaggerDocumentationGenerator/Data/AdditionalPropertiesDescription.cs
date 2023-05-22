using Newtonsoft.Json;
using SwaggerDocumentationGenerator.Converters;

namespace SwaggerDocumentationGenerator.Data
{
	[JsonConverter(typeof(AdditionalPropertiesDescriptionConverter))]
	public class AdditionalPropertiesDescription
	{
		public string Type { get; set; }
		public Dictionary<string, Schema> Properties { get; set; }
	}
}
