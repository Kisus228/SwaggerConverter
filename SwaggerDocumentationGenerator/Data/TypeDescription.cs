using Newtonsoft.Json;
using SwaggerDocumentationGenerator.Converters;

namespace SwaggerDocumentationGenerator.Data
{
	[JsonConverter(typeof(TypeDescriptionJsonConverter))]
	public class TypeDescription
	{
		public string[]? Required { get; set; }
		public string? Type { get; set; }
		public Dictionary<string, Schema>? Properties { get; set; }
		public string[]? Enum { get; set; }
		public string? Description { get; set; }
	}
}
