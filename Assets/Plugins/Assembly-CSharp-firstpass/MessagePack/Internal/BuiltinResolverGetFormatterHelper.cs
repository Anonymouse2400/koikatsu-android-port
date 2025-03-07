using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using MessagePack.Formatters;

namespace MessagePack.Internal
{
	internal static class BuiltinResolverGetFormatterHelper
	{
		private static readonly Dictionary<Type, object> formatterMap = new Dictionary<Type, object>
		{
			{
				typeof(short),
				Int16Formatter.Instance
			},
			{
				typeof(int),
				Int32Formatter.Instance
			},
			{
				typeof(long),
				Int64Formatter.Instance
			},
			{
				typeof(ushort),
				UInt16Formatter.Instance
			},
			{
				typeof(uint),
				UInt32Formatter.Instance
			},
			{
				typeof(ulong),
				UInt64Formatter.Instance
			},
			{
				typeof(float),
				SingleFormatter.Instance
			},
			{
				typeof(double),
				DoubleFormatter.Instance
			},
			{
				typeof(bool),
				BooleanFormatter.Instance
			},
			{
				typeof(byte),
				ByteFormatter.Instance
			},
			{
				typeof(sbyte),
				SByteFormatter.Instance
			},
			{
				typeof(DateTime),
				DateTimeFormatter.Instance
			},
			{
				typeof(char),
				CharFormatter.Instance
			},
			{
				typeof(short?),
				NullableInt16Formatter.Instance
			},
			{
				typeof(int?),
				NullableInt32Formatter.Instance
			},
			{
				typeof(long?),
				NullableInt64Formatter.Instance
			},
			{
				typeof(ushort?),
				NullableUInt16Formatter.Instance
			},
			{
				typeof(uint?),
				NullableUInt32Formatter.Instance
			},
			{
				typeof(ulong?),
				NullableUInt64Formatter.Instance
			},
			{
				typeof(float?),
				NullableSingleFormatter.Instance
			},
			{
				typeof(double?),
				NullableDoubleFormatter.Instance
			},
			{
				typeof(bool?),
				NullableBooleanFormatter.Instance
			},
			{
				typeof(byte?),
				NullableByteFormatter.Instance
			},
			{
				typeof(sbyte?),
				NullableSByteFormatter.Instance
			},
			{
				typeof(DateTime?),
				NullableDateTimeFormatter.Instance
			},
			{
				typeof(char?),
				NullableCharFormatter.Instance
			},
			{
				typeof(string),
				NullableStringFormatter.Instance
			},
			{
				typeof(decimal),
				DecimalFormatter.Instance
			},
			{
				typeof(decimal?),
				new StaticNullableFormatter<decimal>(DecimalFormatter.Instance)
			},
			{
				typeof(TimeSpan),
				TimeSpanFormatter.Instance
			},
			{
				typeof(TimeSpan?),
				new StaticNullableFormatter<TimeSpan>(TimeSpanFormatter.Instance)
			},
			{
				typeof(DateTimeOffset),
				DateTimeOffsetFormatter.Instance
			},
			{
				typeof(DateTimeOffset?),
				new StaticNullableFormatter<DateTimeOffset>(DateTimeOffsetFormatter.Instance)
			},
			{
				typeof(Guid),
				GuidFormatter.Instance
			},
			{
				typeof(Guid?),
				new StaticNullableFormatter<Guid>(GuidFormatter.Instance)
			},
			{
				typeof(Uri),
				UriFormatter.Instance
			},
			{
				typeof(Version),
				VersionFormatter.Instance
			},
			{
				typeof(StringBuilder),
				StringBuilderFormatter.Instance
			},
			{
				typeof(BitArray),
				BitArrayFormatter.Instance
			},
			{
				typeof(byte[]),
				ByteArrayFormatter.Instance
			},
			{
				typeof(Nil),
				NilFormatter.Instance
			},
			{
				typeof(Nil?),
				NullableNilFormatter.Instance
			},
			{
				typeof(short[]),
				Int16ArrayFormatter.Instance
			},
			{
				typeof(int[]),
				Int32ArrayFormatter.Instance
			},
			{
				typeof(long[]),
				Int64ArrayFormatter.Instance
			},
			{
				typeof(ushort[]),
				UInt16ArrayFormatter.Instance
			},
			{
				typeof(uint[]),
				UInt32ArrayFormatter.Instance
			},
			{
				typeof(ulong[]),
				UInt64ArrayFormatter.Instance
			},
			{
				typeof(float[]),
				SingleArrayFormatter.Instance
			},
			{
				typeof(double[]),
				DoubleArrayFormatter.Instance
			},
			{
				typeof(bool[]),
				BooleanArrayFormatter.Instance
			},
			{
				typeof(sbyte[]),
				SByteArrayFormatter.Instance
			},
			{
				typeof(DateTime[]),
				DateTimeArrayFormatter.Instance
			},
			{
				typeof(char[]),
				CharArrayFormatter.Instance
			},
			{
				typeof(string[]),
				NullableStringArrayFormatter.Instance
			},
			{
				typeof(List<short>),
				new ListFormatter<short>()
			},
			{
				typeof(List<int>),
				new ListFormatter<int>()
			},
			{
				typeof(List<long>),
				new ListFormatter<long>()
			},
			{
				typeof(List<ushort>),
				new ListFormatter<ushort>()
			},
			{
				typeof(List<uint>),
				new ListFormatter<uint>()
			},
			{
				typeof(List<ulong>),
				new ListFormatter<ulong>()
			},
			{
				typeof(List<float>),
				new ListFormatter<float>()
			},
			{
				typeof(List<double>),
				new ListFormatter<double>()
			},
			{
				typeof(List<bool>),
				new ListFormatter<bool>()
			},
			{
				typeof(List<byte>),
				new ListFormatter<byte>()
			},
			{
				typeof(List<sbyte>),
				new ListFormatter<sbyte>()
			},
			{
				typeof(List<DateTime>),
				new ListFormatter<DateTime>()
			},
			{
				typeof(List<char>),
				new ListFormatter<char>()
			},
			{
				typeof(List<string>),
				new ListFormatter<string>()
			},
			{
				typeof(ArraySegment<byte>),
				ByteArraySegmentFormatter.Instance
			},
			{
				typeof(ArraySegment<byte>?),
				new StaticNullableFormatter<ArraySegment<byte>>(ByteArraySegmentFormatter.Instance)
			}
		};

		internal static object GetFormatter(Type t)
		{
			object value;
			if (formatterMap.TryGetValue(t, out value))
			{
				return value;
			}
			return null;
		}
	}
}
