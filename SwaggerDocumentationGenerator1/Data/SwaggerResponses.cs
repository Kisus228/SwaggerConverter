using System.Collections.Generic;
using System.Linq;
using System.Net;
using Kontur.Elba.SwaggerDocumentationGenerator.Converters;
using Kontur.Elba.SwaggerDocumentationGenerator.Helpers;
using Newtonsoft.Json;

namespace Kontur.Elba.SwaggerDocumentationGenerator.Data
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
