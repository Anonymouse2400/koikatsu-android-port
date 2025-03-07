using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using MessagePack.Formatters;

namespace MessagePack.Internal
{
	internal static class DynamicGenericResolverGetFormatterHelper
	{
		private static readonly Dictionary<Type, Type> formatterMap = new Dictionary<Type, Type>
		{
			{
				typeof(List<>),
				typeof(ListFormatter<>)
			},
			{
				typeof(LinkedList<>),
				typeof(LinkedListFormatter<>)
			},
			{
				typeof(Queue<>),
				typeof(QeueueFormatter<>)
			},
			{
				typeof(Stack<>),
				typeof(StackFormatter<>)
			},
			{
				typeof(HashSet<>),
				typeof(HashSetFormatter<>)
			},
			{
				typeof(ReadOnlyCollection<>),
				typeof(ReadOnlyCollectionFormatter<>)
			},
			{
				typeof(IList<>),
				typeof(InterfaceListFormatter<>)
			},
			{
				typeof(ICollection<>),
				typeof(InterfaceCollectionFormatter<>)
			},
			{
				typeof(IEnumerable<>),
				typeof(InterfaceEnumerableFormatter<>)
			},
			{
				typeof(Dictionary<, >),
				typeof(DictionaryFormatter<, >)
			},
			{
				typeof(IDictionary<, >),
				typeof(InterfaceDictionaryFormatter<, >)
			},
			{
				typeof(SortedDictionary<, >),
				typeof(SortedDictionaryFormatter<, >)
			},
			{
				typeof(SortedList<, >),
				typeof(SortedListFormatter<, >)
			},
			{
				typeof(ILookup<, >),
				typeof(InterfaceLookupFormatter<, >)
			},
			{
				typeof(IGrouping<, >),
				typeof(InterfaceGroupingFormatter<, >)
			}
		};

		internal static object GetFormatter(Type t)
		{
			TypeInfo typeInfo = t.GetTypeInfo();
			if (t.IsArray)
			{
				switch (t.GetArrayRank())
				{
				case 1:
					if (t.GetElementType() == typeof(byte[]))
					{
						return ByteArrayFormatter.Instance;
					}
					return Activator.CreateInstance(typeof(ArrayFormatter<>).MakeGenericType(t.GetElementType()));
				case 2:
					return Activator.CreateInstance(typeof(TwoDimentionalArrayFormatter<>).MakeGenericType(t.GetElementType()));
				case 3:
					return Activator.CreateInstance(typeof(ThreeDimentionalArrayFormatter<>).MakeGenericType(t.GetElementType()));
				case 4:
					return Activator.CreateInstance(typeof(FourDimentionalArrayFormatter<>).MakeGenericType(t.GetElementType()));
				default:
					return null;
				}
			}
			if (typeInfo.IsGenericType)
			{
				Type genericTypeDefinition = typeInfo.GetGenericTypeDefinition();
				TypeInfo typeInfo2 = genericTypeDefinition.GetTypeInfo();
				bool flag = typeInfo2.IsNullable();
				Type type = ((!flag) ? null : typeInfo.GenericTypeArguments[0]);
				if (genericTypeDefinition == typeof(KeyValuePair<, >))
				{
					return CreateInstance(typeof(KeyValuePairFormatter<, >), typeInfo.GenericTypeArguments);
				}
				if (flag && type.GetTypeInfo().IsConstructedGenericType() && type.GetGenericTypeDefinition() == typeof(KeyValuePair<, >))
				{
					return CreateInstance(typeof(NullableFormatter<>), new Type[1] { type });
				}
				if (genericTypeDefinition == typeof(ArraySegment<>))
				{
					if (typeInfo.GenericTypeArguments[0] == typeof(byte))
					{
						return ByteArraySegmentFormatter.Instance;
					}
					return CreateInstance(typeof(ArraySegmentFormatter<>), typeInfo.GenericTypeArguments);
				}
				if (flag && type.GetTypeInfo().IsConstructedGenericType() && type.GetGenericTypeDefinition() == typeof(ArraySegment<>))
				{
					if (type == typeof(ArraySegment<byte>))
					{
						return new StaticNullableFormatter<ArraySegment<byte>>(ByteArraySegmentFormatter.Instance);
					}
					return CreateInstance(typeof(NullableFormatter<>), new Type[1] { type });
				}
				Type value;
				if (formatterMap.TryGetValue(genericTypeDefinition, out value))
				{
					return CreateInstance(value, typeInfo.GenericTypeArguments);
				}
				if (typeInfo.GenericTypeArguments.Length == 1 && typeInfo.ImplementedInterfaces.Any((Type x) => x.GetTypeInfo().IsConstructedGenericType() && x.GetGenericTypeDefinition() == typeof(ICollection<>)) && typeInfo.DeclaredConstructors.Any((ConstructorInfo x) => x.GetParameters().Length == 0))
				{
					Type type2 = typeInfo.GenericTypeArguments[0];
					return CreateInstance(typeof(GenericCollectionFormatter<, >), new Type[2] { type2, t });
				}
				if (typeInfo.GenericTypeArguments.Length == 2 && typeInfo.ImplementedInterfaces.Any((Type x) => x.GetTypeInfo().IsConstructedGenericType() && x.GetGenericTypeDefinition() == typeof(IDictionary<, >)) && typeInfo.DeclaredConstructors.Any((ConstructorInfo x) => x.GetParameters().Length == 0))
				{
					Type type3 = typeInfo.GenericTypeArguments[0];
					Type type4 = typeInfo.GenericTypeArguments[1];
					return CreateInstance(typeof(GenericDictionaryFormatter<, , >), new Type[3] { type3, type4, t });
				}
			}
			return null;
		}

		private static object CreateInstance(Type genericType, Type[] genericTypeArguments, params object[] arguments)
		{
			return Activator.CreateInstance(genericType.MakeGenericType(genericTypeArguments), arguments);
		}
	}
}
