using System.Collections.Generic;
using Kontur.Elba.SwaggerDocumentationGenerator.Converters;
using Newtonsoft.Json;

namespace Kontur.Elba.SwaggerDocumentationGenerator.Data
{
	[JsonConverter(typeof(AdditionalPropertiesDescriptionConverter))]
	public class AdditionalPropertiesDescription
	{
		public string Type { get; set; }
		public Dictionary<string, Schema> Properties { get; set; }
	}
}
