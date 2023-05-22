using System.Net;
using System.Reflection;
using Namotion.Reflection;
using Newtonsoft.Json.Linq;
using SwaggerDocumentationGenerator.Data;
using SwaggerDocumentationGenerator.Helpers;
using SwaggerDocumentationGenerator.Temp;

namespace SwaggerDocumentationGenerator
{
	internal static class PathsGenerator
	{
		public static JObject GetPaths(IEnumerable<Type> controllers, string apiVersion)
		{
			var result = new JObject();
			var pathsDescriptions = GetPathsDescriptions(controllers, apiVersion).ToArray();
			foreach (var (path, description) in pathsDescriptions)
			{
				result.Add(path, new JObject
								 {
									 { description.HttpMethod, JObject.FromObject(description, CommonHelpers.Serializer) }
								 });
			}

			return result;
		}

		private static IEnumerable<(string path, PathDescription description)> GetPathsDescriptions(IEnumerable<Type> controllers, string version)
		{
			foreach (var controller in controllers)
			{
				var methods = controller.GetMethods();
				var apiMeta = controller.GetCustomAttribute<ApiMetaAttribute>();
				foreach (var methodInfo in methods)
				{
					var controllerName = controller.Name.ExcludePrefix(apiMeta.InternalControllerPrefix).ExcludeSuffix("Controller");
					var path = $"/{apiMeta.ExternalRootPath.Decapitalize()}/v{version}/{controllerName.Decapitalize()}";
					var httpMethod = HttpStatusCodeHelpers.GetHttpMethod(methodInfo);

					var summary = methodInfo.GetSummary();
					if (summary == null)
						throw new Exception($"Нет документации для метода [{methodInfo.Name}] контроллера [{controller.Name}]");
					var swaggerResponses = new SwaggerResponses(TypeHelpers.GetReturnValues(methodInfo).Select(x => GetResponseInfo(x, methodInfo)).ToArray());

					var documentationAttribute = controller.GetCustomAttribute<NativeMobileDocumentationAttribute>();
					var tag = documentationAttribute != null ? documentationAttribute.Tag : controllerName;
					var pathDescription = new PathDescription
										  {
											  Tags = new[] { tag },
											  HttpMethod = httpMethod,
											  Summary = summary,
											  Description = methodInfo.GetRemarks(),
											  Responses = swaggerResponses
										  };
					if (HttpStatusCodeHelpers.IsHttpGet(methodInfo))
						FillParameters(methodInfo, pathDescription);
					else
					{
						FillRequestBody(pathDescription, methodInfo, controllerName);
						TypeCollection.AddIfNotExists(CommonHelpers.GetSchemasPath(controllerName, methodInfo.Name),
													  DefinitionsGenerator.GetDefinition( methodInfo.GetParameters()));
					}
					yield return ($"{path}/{methodInfo.Name.Decapitalize()}", pathDescription);
				}
			}
		}

		private static void FillRequestBody(PathDescription pathDescription, MethodInfo methodInfo, string controllerName)
		{
			pathDescription.RequestBody = new RequestBody
										  {
											  Description = methodInfo.GetRemarks(),
											  Required = true,
											  Content = new Content
														{
															ContentType = CommonHelpers.ContentTypeJson,
															ContentDescription = new ContentDescription
																				 {
																					 Schema = Schema.ForRequestBodyRef(controllerName, methodInfo.Name),
																				 }
														}
										  };
		}

		private static void FillParameters(MethodInfo methodInfo, PathDescription pathDescription)
		{
			var parameters = methodInfo.GetParameters()
									   .Select(GetParameterDescription)
									   .ToArray();
			if(parameters.Any())
				pathDescription.Parameters = parameters;
		}

		private static ResponseSchema GetResponseInfo(Type type, MethodInfo methodInfo)
		{
			var statusCode = ElbaJsonResult.GetStatusCode(type) ?? HttpStatusCode.OK;
			/*if (type == typeof(FileResult))
			{
				var fileResultAttribute = methodInfo.GetCustomAttribute<FileResultAttribute>();
				if (fileResultAttribute == null)
					throw new
						Exception($"Если метод возвращает [{nameof(FileResult)}], у него должен быть атрибут [{nameof(FileResultAttribute)}]. " +
								  $"Метод [{methodInfo.MemberType}]");
				return ResponseSchema.ForFileResult(fileResultAttribute.ContentType);
			}*/
			return ResponseSchema.ForJsonValue(SchemaGenerator.GetSchema(type).FillDescription(type), statusCode);
		}

		private static ParameterDescription GetParameterDescription(ParameterInfo parameterInfo)
		{
			var isNullable = parameterInfo.ToContextualParameter().IsNullableType;
			var type = isNullable ? parameterInfo.ParameterType.UnwrapNullableType() : parameterInfo.ParameterType;
			var description = new ParameterDescription
							  {
								  In = "query",
								  Name = parameterInfo.Name,
								  Description = parameterInfo.GetSummary(),
								  Required = !isNullable
							  };

			var primitiveTypeDescription = DataTypeNames.GetForPrimitiveType(type);
			if (primitiveTypeDescription.HasValue)
			{
				description.Schema = SchemaGenerator.GetSchema(type).FillDescription(type);
				return description;
			}

			if (type.IsEnum)
				description.Schema = Schema.ForEnum(type);
			else
				description.Schema = Schema.ForReusableType(type);
			return description;
		}
	}
}
