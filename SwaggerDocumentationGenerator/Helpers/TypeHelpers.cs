using System.Collections;
using System.Reflection;
using Namotion.Reflection;
using SwaggerDocumentationGenerator.Temp;

namespace SwaggerDocumentationGenerator.Helpers
{
	internal static class TypeHelpers
	{
		public static IEnumerable<Type> GetReturnValues(MethodInfo methodInfo)
		{
			var returnType = methodInfo.ReturnType;
			
			if (returnType == typeof(void))
				yield break;

			yield return returnType;
		}

		public static string[] GetEnumMembers(Type type)
		{
			return GetEnumMembersInternal(type).Select(x => x.Name.Decapitalize()).ToArray();
		}

		public static string GetEnumMembersDescription(Type type)
		{
			var members = GetEnumMembersInternal(type);
			return $"{type.GetSummary()}{GetEnumMembersDescription(members)}";
		}

		private static string GetEnumMembersDescription(IEnumerable<FieldInfo> fields)
		{
			var fieldsDescription = fields.Select(x => (name: x.Name, description: GetEnumMemberDescription(x)))
										  .Where(x => x.description != null)
										  .Select(x => $"<li><i>{x.name}</i> - {x.description}</li>")
										  .ToArray();
			return fieldsDescription.Any()
					   ? $" <ul>{fieldsDescription.JoinStrings(" ")}</ul>"
					   : "";
		}

		private static string? GetEnumMemberDescription(FieldInfo field)
		{
			var summary = field.GetXmlDocsSummary();
			if (summary.IsFilled())
				return summary;
			/*var attribute = field.GetCustomAttributeOrNull<EnumMemberDescAttribute>();
			if (attribute != null)
				return attribute.Text;*/
			return null;
		}

		private static IEnumerable<FieldInfo> GetEnumMembersInternal(Type type)
		{
			return type.GetFields().Where(x => x.IsStatic);
		}

		public static bool IsSupportedDictionary(Type type, out Type keyType, out Type valueType)
		{
			if (type.IsConstructedFrom(typeof(IDictionary<,>), out Type constructedType)
				|| type.IsConstructedFrom(typeof(IReadOnlyDictionary<,>), out constructedType))
			{
				keyType = constructedType.GenericTypeArguments[0];
				valueType = constructedType.GenericTypeArguments[1];
				return true;
			}

			if (typeof(IDictionary).IsAssignableFrom(type))
			{
				keyType = valueType = typeof(object);
				return true;
			}

			keyType = valueType = null;
			return false;
		}

		public static bool IsSupportedCollection(Type type, out Type itemType)
		{
			if (type.IsConstructedFrom(typeof(IEnumerable<>), out Type constructedType))
			{
				itemType = constructedType.GenericTypeArguments[0];
				return true;
			}


			if (typeof(IEnumerable).IsAssignableFrom(type))
			{
				itemType = typeof(object);
				return true;
			}

			itemType = null;
			return false;
		}

		private static bool IsConstructedFrom(this Type type, Type genericType, out Type constructedType)
		{
			constructedType = Enumerable.Union(new[] { type }
												   .Union(type.GetInheritanceChain()), type.GetInterfaces())
										.FirstOrDefault(i => i.IsConstructedGenericType && i.GetGenericTypeDefinition() == genericType);

			return (constructedType != null);
		}

		public static Type[] GetInheritanceChain(this Type type)
		{
			var inheritanceChain = new List<Type>();

			var current = type;
			while (current.BaseType != null)
			{
				inheritanceChain.Add(current.BaseType);
				current = current.BaseType;
			}

			return inheritanceChain.ToArray();
		}

		public static bool IsEnumerable(this Type type)
		{
			if (!(type != typeof(string)))
				return false;
			return type.IsArray || type.GetDefinition() == typeof(IEnumerable<>) || type.GetDefinition() == typeof(List<>) ||
				   type.GetDefinition() == typeof(IList<>);
		}

		public static bool IsDictionary(this Type type)
		{
			return type.IsConstructedFrom(typeof(IDictionary<,>), out _)
				   || type.IsConstructedFrom(typeof(IReadOnlyDictionary<,>), out _);
		}


		public static bool IsSystemType(this Type type)
		{
			return type.Namespace != null && type.Namespace.StartsWith("System");
		}

		public static bool IsFileResultType(this Type type)
		{
			/*var fileResultTypes = new []{typeof(FileResult), typeof(BasicFile), typeof(Microsoft.AspNetCore.Mvc.FileResult)};
			return fileResultTypes.Contains(type);*/
			return true;
		}

		public static string GetName(this Type type)
		{
			if (type.ToContextualType().IsNullableType)
				return type.UnwrapNullableType().Name.Decapitalize();
			if (type.IsEnumerable())
				return type.UnwrapEnumerableType().Name.Decapitalize();
			return type.Name.Decapitalize();
		}

		/*public static Maybe<IEnumerable<Type>> UnwrapApiResultTypes(this MethodInfo methodInfo)
		{
			if (methodInfo.ReturnType.GetInterface(nameof(IPublicApiResult)) != null)
			{
				return methodInfo.ReturnType.GetGenericArguments();
			}

			return Maybe<IEnumerable<Type>>.Nothing();
		}*/

		public static Type UnwrapEnumerableType(this Type type)
		{
			if (type.IsArray)
				return type.GetElementType();
			return type.GetDefinition() == typeof(IEnumerable<>) || type.GetDefinition() == typeof(List<>) ||
				   type.GetDefinition() == typeof(IList<>)
					   ? type.GetGenericArguments().Single<Type>()
					   : type;
		}

		public static Type UnwrapNullableType(this Type type)
		{
			var contextualType = type.ToContextualType();
			return contextualType.IsNullableType ? type.GetGenericArguments()[0] : type;
		}

		public static (Type keyType, Type valueType) UnwrapDictionaryTypes(this Type type)
		{
			return (type.GenericTypeArguments[0], type.GenericTypeArguments[1]);
		}

		public static Type GetDefinition(this Type type) =>
			!type.IsGenericType || type.IsGenericTypeDefinition ? type : type.GetGenericTypeDefinition();

		public static bool IsNumber(this Type type) => type == typeof(sbyte)
													   || type == typeof(byte)
													   || type == typeof(short)
													   || type == typeof(ushort)
													   || type == typeof(int)
													   || type == typeof(uint)
													   || type == typeof(long)
													   || type == typeof(ulong)
													   || type == typeof(float)
													   || type == typeof(double)
													   || type == typeof(decimal);




		public static IEnumerable<Type> GetPropertyTypes(IEnumerable<Type> types)
		{
			var parameterAndReturnTypes = types.Distinct()
											   .ToArray();
			return parameterAndReturnTypes.SelectMany(GetPropertyTypes)
										  .Where(x => !x.IsSystemType());
		}

		private static IEnumerable<Type> GetPropertyTypes(Type type)
		{
			yield return type;
			var properties = type.GetProperties().Where(x => x.SetMethod != null).ToArray();
			if(properties.Any())
				foreach (var property in properties)
				{
					foreach (var innerPropertyType in GetPropertyTypes(property.PropertyType))
						yield return innerPropertyType;
				}
		}
	}
}
