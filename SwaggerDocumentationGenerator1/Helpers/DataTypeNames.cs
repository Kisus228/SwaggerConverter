using System;
using System.Collections.Generic;
using Kontur.Elba.Core.Utilities.Collections;
using Kontur.Elba.Core.Utilities.Monads;

namespace Kontur.Elba.SwaggerDocumentationGenerator.Helpers
{
	public static class DataTypeNames
	{
		public const string Bool = "boolean";
		public const string Int = "integer";
		public const string Number = "number";
		public const string String = "string";
		public const string Object = "object";
		public const string Array = "array";

		public const string Key = "key";
		public const string Value = "value";

		public static Maybe<(string typeName, string? format)> GetForPrimitiveType(Type type)
		{
			return PrimitiveTypesAndFormats.TryGetValue(type, out var value)
					   ? value
					   : Maybe<(string typeName, string? format)>.Nothing();
		}

		private static readonly Dictionary<Type, (string typeName, string? format)> PrimitiveTypesAndFormats
			= new()
			  {
				  [typeof(bool)] = (Bool, null),
				  [typeof(byte)] = (Int, "int32"),
				  [typeof(sbyte)] = (Int, "int32"),
				  [typeof(short)] = (Int, "int32"),
				  [typeof(ushort)] = (Int, "int32"),
				  [typeof(int)] = (Int, "int32"),
				  [typeof(uint)] = (Int, "int32"),
				  [typeof(long)] = (Int, "int64"),
				  [typeof(ulong)] = (Int, "int64"),
				  [typeof(float)] = (Number, "float"),
				  [typeof(double)] = (Number, "double"),
				  [typeof(decimal)] = (Number, "double"),
				  [typeof(byte[])] = (String, "byte"),
				  [typeof(string)] = (String, null),
				  [typeof(char)] = (String, null),
				  [typeof(DateTime)] = (String, "date-time"),
				  [typeof(DateTimeOffset)] = (String, "date-time"),
				  [typeof(Guid)] = (String, "uuid"),
				  [typeof(Uri)] = (String, "uri"),
				  [typeof(TimeSpan)] = (String, "date-span")
			  };
	}
}
