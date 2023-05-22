using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Kontur.Elba.Core.Utilities.Collections;
using Kontur.Elba.Core.Utilities.Helpers;
using Namotion.Reflection;

namespace Kontur.Elba.SwaggerDocumentationGenerator.Helpers
{
	internal static class SummaryHelpers
	{
		private static readonly Dictionary<Type, object> Examples = new()
																	{
																		[typeof(string)] = "строка",
																		[typeof(int)] = 42,
																		[typeof(double)] = 15.50,
																		[typeof(decimal)] = 15.50,
																		[typeof(DateTime)] = new DateTime(2021, 01, 2, 3, 4, 5),
																		[typeof(bool)] = true,
																	};
		public static string? GetSummary(this Type type) => ToSingleLine(type.GetXmlDocsSummary());
		public static string? GetRemarks(this Type type) => ToSingleLine(type.GetXmlDocsRemarks());

		public static object? GetExample(this Type type) => ToSingleLine(type.GetXmlDocsTag(exampleTagName))
															?? type.GetDefaultExample();

		public static string? GetSummary(this PropertyInfo propertyInfo) => ToSingleLine(propertyInfo.GetXmlDocsSummary());
		public static object? GetExample(this PropertyInfo propertyInfo) => ToSingleLine(propertyInfo.GetXmlDocsTag(exampleTagName))
																			?? propertyInfo.PropertyType.GetDefaultExample();

		public static string? GetSummary(this MethodInfo methodInfo) => ToSingleLine(methodInfo.GetXmlDocsSummary());
		public static string? GetRemarks(this MethodInfo methodInfo) => ToSingleLine(methodInfo.GetXmlDocsRemarks());

		public static string? GetSummary(this ParameterInfo parameterInfo) => ToSingleLine(parameterInfo.GetXmlDocs());

		public static object? GetExample(this ParameterInfo parameterInfo) => ToSingleLine(parameterInfo.ToContextualParameter().GetXmlDocsTag(exampleTagName))
																			  ?? parameterInfo.ParameterType.GetDefaultExample();

		private const string exampleTagName = "example";
		private static string? ToSingleLine(string? documentation)
		{
			return documentation?.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries)
								.Select(x => x.Trim())
								.JoinStrings("<br/>")
								.NullIfEmpty();
		}

		private static object? GetDefaultExample(this Type type)
		{
			return Examples.TryGetValue(type.UnwrapNullableType(), out var value)
					   ? value
					   : null;
		}
	}
}
