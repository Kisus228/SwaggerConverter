using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Kontur.Elba.Core.Utilities.Helpers;

namespace Kontur.Elba.SwaggerDocumentationGenerator.Helpers
{
	internal static class HttpStatusCodeHelpers
	{
		public static string GetHttpMethod(MethodInfo info)
		{
			return GetHttpMethodAttribute(info).Name
											   .ExcludePrefix("Http")
											   .ExcludeSuffix("Attribute")
											   .ToLower();
		}

		public static bool IsHttpGet(MethodInfo methodInfo)
		{
			return GetHttpMethodAttribute(methodInfo) == typeof(HttpGetAttribute);
		}

		private static Type GetHttpMethodAttribute(MethodInfo info)
		{
			return info.CustomAttributes
					   .SingleOrDefault(x => x.AttributeType.BaseType == typeof(ActionMethodSelectorAttribute))?.AttributeType ?? typeof(HttpGetAttribute);
		}

		public static string GetDescription(HttpStatusCode httpStatusCode)
		{
			var statusCode = ((int)httpStatusCode).ToString();
			foreach (var description in ResponseDescriptionMap)
			{
				if (Regex.IsMatch(statusCode, description.Key))
					return description.Value;
			}

			return "Error";
		}

		private static readonly Dictionary<string, string> ResponseDescriptionMap = new()
																					{
																						{ "1\\d{2}", "Information" },
																						{ "2\\d{2}", "Success" },
																						{ "304", "Not Modified" },
																						{ "3\\d{2}", "Redirect" },
																						{ "400", "Bad Request" },
																						{ "401", "Unauthorized" },
																						{ "403", "Forbidden" },
																						{ "404", "Not Found" },
																						{ "405", "Method Not Allowed" },
																						{ "406", "Not Acceptable" },
																						{ "408", "Request Timeout" },
																						{ "409", "Conflict" },
																						{ "4\\d{2}", "Client Error" },
																						{ "5\\d{2}", "Server Error" }
																					};
	}
}
