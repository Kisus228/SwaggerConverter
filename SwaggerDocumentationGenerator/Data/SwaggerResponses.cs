using System.Net;
using Newtonsoft.Json;
using SwaggerDocumentationGenerator.Converters;
using SwaggerDocumentationGenerator.Helpers;

namespace SwaggerDocumentationGenerator.Data
{
	[JsonConverter(typeof(ResponseInfoJsonConverter))]
	public class SwaggerResponses
	{
		public SwaggerResponses(ResponseSchema[] responseInfos)
		{
			if (!responseInfos.Any())
				ResponsesSchemas = new[]
								{
									new ResponseSchema
									{
										HttpStatusCode = HttpStatusCode.OK,
										Description = HttpStatusCodeHelpers.GetDescription(HttpStatusCode.OK)
									}
								};
			else
			{
				ResponsesSchemas = responseInfos.ToArray();
			}
		}

		public ResponseSchema[] ResponsesSchemas { get; set; }
	}
}
