using System.Net;
using SwaggerDocumentationGenerator.Helpers;

namespace SwaggerDocumentationGenerator.Data
{
	public class ResponseSchema
	{
		public HttpStatusCode HttpStatusCode { get; init; }
		public string Description { get; init; }
		public Content? Content { get; private init; }

		public static ResponseSchema ForJsonValue(Schema schema, HttpStatusCode httpStatusCode)
		{
			return new ResponseSchema
				   {
					   Content = new Content
								 {
									 ContentType = CommonHelpers.ContentTypeJson,
									 ContentDescription = new ContentDescription
														  {
															  Schema = schema
														  }
								 },
					   HttpStatusCode = httpStatusCode,
					   Description = HttpStatusCodeHelpers.GetDescription(httpStatusCode)
				   };
		}

		public static ResponseSchema ForFileResult(string contentType)
		{
			return new ResponseSchema
				   {
					   Content = new Content
								 {
									 ContentType = contentType,
									 ContentDescription = new ContentDescription
														  {
															  Schema = Schema.ForFileResult()
														  }
								 },
					   HttpStatusCode = HttpStatusCode.OK,
					   Description = HttpStatusCodeHelpers.GetDescription(HttpStatusCode.OK)
				   };
		}
	}
}
