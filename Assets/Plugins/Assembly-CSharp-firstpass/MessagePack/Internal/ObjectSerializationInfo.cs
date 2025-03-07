using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace MessagePack.Internal
{
	internal class ObjectSerializationInfo
	{
		public class EmittableMember
		{
			public bool IsProperty
			{
				get
				{
					return PropertyInfo != null;
				}
			}

			public bool IsField
			{
				get
				{
					return FieldInfo != null;
				}
			}

			public bool IsWritable { get; set; }

			public bool IsReadable { get; set; }

			public int IntKey { get; set; }

			public string StringKey { get; set; }

			public Type Type
			{
				get
				{
					return (!IsField) ? PropertyInfo.PropertyType : FieldInfo.FieldType;
				}
			}

			public FieldInfo FieldInfo { get; set; }

			public PropertyInfo PropertyInfo { get; set; }

			public bool IsValueType
			{
				get
				{
					MemberInfo memberInfo = ((!IsProperty) ? ((MemberInfo)FieldInfo) : ((MemberInfo)PropertyInfo));
					return memberInfo.DeclaringType.GetTypeInfo().IsValueType;
				}
			}

			public void EmitLoadValue(ILGenerator il)
			{
				if (IsProperty)
				{
					il.EmitCall(PropertyInfo.GetGetMethod());
				}
				else
				{
					il.Emit(OpCodes.Ldfld, FieldInfo);
				}
			}

			public void EmitStoreValue(ILGenerator il)
			{
				if (IsProperty)
				{
					il.EmitCall(PropertyInfo.GetSetMethod());
				}
				else
				{
					il.Emit(OpCodes.Stfld, FieldInfo);
				}
			}
		}

		public bool IsIntKey { get; set; }

		public bool IsStringKey
		{
			get
			{
				return !IsIntKey;
			}
		}

		public bool IsClass { get; set; }

		public bool IsStruct
		{
			get
			{
				return !IsClass;
			}
		}

		public ConstructorInfo BestmatchConstructor { get; set; }

		public EmittableMember[] ConstructorParameters { get; set; }

		public EmittableMember[] Members { get; set; }

		private ObjectSerializationInfo()
		{
		}

		public static ObjectSerializationInfo CreateOrNull(Type type, bool forceStringKey)
		{
			TypeInfo typeInfo = type.GetTypeInfo();
			bool isClass = typeInfo.IsClass;
			MessagePackObjectAttribute customAttribute = typeInfo.GetCustomAttribute<MessagePackObjectAttribute>();
			if (customAttribute == null && !forceStringKey)
			{
				return null;
			}
			bool flag = true;
			Dictionary<int, EmittableMember> dictionary = new Dictionary<int, EmittableMember>();
			Dictionary<string, EmittableMember> dictionary2 = new Dictionary<string, EmittableMember>();
			if (forceStringKey || customAttribute.KeyAsPropertyName)
			{
				flag = false;
				int num = 0;
				PropertyInfo[] runtimeProperties = type.GetRuntimeProperties();
				foreach (PropertyInfo propertyInfo in runtimeProperties)
				{
					if (propertyInfo.GetCustomAttribute<IgnoreMemberAttribute>(true) == null && propertyInfo.GetCustomAttribute<IgnoreDataMemberAttribute>(true) == null)
					{
						EmittableMember emittableMember = new EmittableMember();
						emittableMember.PropertyInfo = propertyInfo;
						emittableMember.IsReadable = propertyInfo.GetGetMethod() != null && propertyInfo.GetGetMethod().IsPublic && !propertyInfo.GetGetMethod().IsStatic;
						emittableMember.IsWritable = propertyInfo.GetSetMethod() != null && propertyInfo.GetSetMethod().IsPublic && !propertyInfo.GetSetMethod().IsStatic;
						emittableMember.StringKey = propertyInfo.Name;
						EmittableMember emittableMember2 = emittableMember;
						if (emittableMember2.IsReadable || emittableMember2.IsWritable)
						{
							emittableMember2.IntKey = num++;
							dictionary2.Add(emittableMember2.StringKey, emittableMember2);
						}
					}
				}
				FieldInfo[] runtimeFields = type.GetRuntimeFields();
				foreach (FieldInfo fieldInfo in runtimeFields)
				{
					if (fieldInfo.GetCustomAttribute<IgnoreMemberAttribute>(true) == null && fieldInfo.GetCustomAttribute<IgnoreDataMemberAttribute>(true) == null && fieldInfo.GetCustomAttribute<CompilerGeneratedAttribute>(true) == null && !fieldInfo.IsStatic)
					{
						EmittableMember emittableMember = new EmittableMember();
						emittableMember.FieldInfo = fieldInfo;
						emittableMember.IsReadable = fieldInfo.IsPublic;
						emittableMember.IsWritable = fieldInfo.IsPublic && !fieldInfo.IsInitOnly;
						emittableMember.StringKey = fieldInfo.Name;
						EmittableMember emittableMember3 = emittableMember;
						if (emittableMember3.IsReadable || emittableMember3.IsWritable)
						{
							emittableMember3.IntKey = num++;
							dictionary2.Add(emittableMember3.StringKey, emittableMember3);
						}
					}
				}
			}
			else
			{
				bool flag2 = true;
				int num2 = 0;
				PropertyInfo[] runtimeProperties2 = type.GetRuntimeProperties();
				foreach (PropertyInfo propertyInfo2 in runtimeProperties2)
				{
					if (propertyInfo2.GetCustomAttribute<IgnoreMemberAttribute>(true) != null || propertyInfo2.GetCustomAttribute<IgnoreDataMemberAttribute>(true) != null)
					{
						continue;
					}
					EmittableMember emittableMember = new EmittableMember();
					emittableMember.PropertyInfo = propertyInfo2;
					emittableMember.IsReadable = propertyInfo2.GetGetMethod() != null && propertyInfo2.GetGetMethod().IsPublic && !propertyInfo2.GetGetMethod().IsStatic;
					emittableMember.IsWritable = propertyInfo2.GetSetMethod() != null && propertyInfo2.GetSetMethod().IsPublic && !propertyInfo2.GetSetMethod().IsStatic;
					EmittableMember emittableMember4 = emittableMember;
					if (!emittableMember4.IsReadable && !emittableMember4.IsWritable)
					{
						continue;
					}
					KeyAttribute customAttribute2 = propertyInfo2.GetCustomAttribute<KeyAttribute>(true);
					if (customAttribute2 == null)
					{
						throw new MessagePackDynamicObjectResolverException("all public members must mark KeyAttribute or IgnoreMemberAttribute. type: " + type.FullName + " member:" + propertyInfo2.Name);
					}
					if (!customAttribute2.IntKey.HasValue && customAttribute2.StringKey == null)
					{
						throw new MessagePackDynamicObjectResolverException("both IntKey and StringKey are null. type: " + type.FullName + " member:" + propertyInfo2.Name);
					}
					if (flag2)
					{
						flag2 = false;
						flag = customAttribute2.IntKey.HasValue;
					}
					else if ((flag && !customAttribute2.IntKey.HasValue) || (!flag && customAttribute2.StringKey == null))
					{
						throw new MessagePackDynamicObjectResolverException("all members key type must be same. type: " + type.FullName + " member:" + propertyInfo2.Name);
					}
					if (flag)
					{
						emittableMember4.IntKey = customAttribute2.IntKey.Value;
						if (dictionary.ContainsKey(emittableMember4.IntKey))
						{
							throw new MessagePackDynamicObjectResolverException("key is duplicated, all members key must be unique. type: " + type.FullName + " member:" + propertyInfo2.Name);
						}
						dictionary.Add(emittableMember4.IntKey, emittableMember4);
						continue;
					}
					emittableMember4.StringKey = customAttribute2.StringKey;
					if (dictionary2.ContainsKey(emittableMember4.StringKey))
					{
						throw new MessagePackDynamicObjectResolverException("key is duplicated, all members key must be unique. type: " + type.FullName + " member:" + propertyInfo2.Name);
					}
					emittableMember4.IntKey = num2++;
					dictionary2.Add(emittableMember4.StringKey, emittableMember4);
				}
				FieldInfo[] runtimeFields2 = type.GetRuntimeFields();
				foreach (FieldInfo fieldInfo2 in runtimeFields2)
				{
					if (fieldInfo2.GetCustomAttribute<IgnoreMemberAttribute>(true) != null || fieldInfo2.GetCustomAttribute<IgnoreDataMemberAttribute>(true) != null || fieldInfo2.GetCustomAttribute<CompilerGeneratedAttribute>(true) != null || fieldInfo2.IsStatic)
					{
						continue;
					}
					EmittableMember emittableMember = new EmittableMember();
					emittableMember.FieldInfo = fieldInfo2;
					emittableMember.IsReadable = fieldInfo2.IsPublic;
					emittableMember.IsWritable = fieldInfo2.IsPublic && !fieldInfo2.IsInitOnly;
					EmittableMember emittableMember5 = emittableMember;
					if (!emittableMember5.IsReadable && !emittableMember5.IsWritable)
					{
						continue;
					}
					KeyAttribute customAttribute3 = fieldInfo2.GetCustomAttribute<KeyAttribute>(true);
					if (customAttribute3 == null)
					{
						throw new MessagePackDynamicObjectResolverException("all public members must mark KeyAttribute or IgnoreMemberAttribute. type: " + type.FullName + " member:" + fieldInfo2.Name);
					}
					if (!customAttribute3.IntKey.HasValue && customAttribute3.StringKey == null)
					{
						throw new MessagePackDynamicObjectResolverException("both IntKey and StringKey are null. type: " + type.FullName + " member:" + fieldInfo2.Name);
					}
					if (flag2)
					{
						flag2 = false;
						flag = customAttribute3.IntKey.HasValue;
					}
					else if ((flag && !customAttribute3.IntKey.HasValue) || (!flag && customAttribute3.StringKey == null))
					{
						throw new MessagePackDynamicObjectResolverException("all members key type must be same. type: " + type.FullName + " member:" + fieldInfo2.Name);
					}
					if (flag)
					{
						emittableMember5.IntKey = customAttribute3.IntKey.Value;
						if (dictionary.ContainsKey(emittableMember5.IntKey))
						{
							throw new MessagePackDynamicObjectResolverException("key is duplicated, all members key must be unique. type: " + type.FullName + " member:" + fieldInfo2.Name);
						}
						dictionary.Add(emittableMember5.IntKey, emittableMember5);
						continue;
					}
					emittableMember5.StringKey = customAttribute3.StringKey;
					if (dictionary2.ContainsKey(emittableMember5.StringKey))
					{
						throw new MessagePackDynamicObjectResolverException("key is duplicated, all members key must be unique. type: " + type.FullName + " member:" + fieldInfo2.Name);
					}
					emittableMember5.IntKey = num2++;
					dictionary2.Add(emittableMember5.StringKey, emittableMember5);
				}
			}
			ConstructorInfo constructorInfo = typeInfo.DeclaredConstructors.Where((ConstructorInfo x) => x.IsPublic).SingleOrDefault((ConstructorInfo x) => x.GetCustomAttribute<SerializationConstructorAttribute>(false) != null);
			if (constructorInfo == null)
			{
				constructorInfo = (from x in typeInfo.DeclaredConstructors
					where x.IsPublic
					orderby x.GetParameters().Length
					select x).FirstOrDefault();
			}
			if (constructorInfo == null && isClass)
			{
				throw new MessagePackDynamicObjectResolverException("can't find public constructor. type:" + type.FullName);
			}
			List<EmittableMember> list = new List<EmittableMember>();
			if (constructorInfo != null)
			{
				ILookup<string, KeyValuePair<string, EmittableMember>> lookup = dictionary2.ToLookup((KeyValuePair<string, EmittableMember> x) => x.Key, (KeyValuePair<string, EmittableMember> x) => x, StringComparer.OrdinalIgnoreCase);
				int num3 = 0;
				ParameterInfo[] parameters = constructorInfo.GetParameters();
				foreach (ParameterInfo parameterInfo in parameters)
				{
					EmittableMember value;
					if (flag)
					{
						if (!dictionary.TryGetValue(num3, out value))
						{
							throw new MessagePackDynamicObjectResolverException("can't find matched constructor parameter, index not found. type:" + type.FullName + " parameterIndex:" + num3);
						}
						if (parameterInfo.ParameterType != value.Type || !value.IsReadable)
						{
							throw new MessagePackDynamicObjectResolverException("can't find matched constructor parameter, parameterType mismatch. type:" + type.FullName + " parameterIndex:" + num3 + " paramterType:" + parameterInfo.ParameterType.Name);
						}
						list.Add(value);
					}
					else
					{
						IEnumerable<KeyValuePair<string, EmittableMember>> source = lookup[parameterInfo.Name];
						switch (source.Count())
						{
						default:
							throw new MessagePackDynamicObjectResolverException("duplicate matched constructor parameter name:" + type.FullName + " parameterName:" + parameterInfo.Name + " paramterType:" + parameterInfo.ParameterType.Name);
						case 1:
							value = source.First().Value;
							if (parameterInfo.ParameterType == value.Type && value.IsReadable)
							{
								list.Add(value);
								break;
							}
							throw new MessagePackDynamicObjectResolverException("can't find matched constructor parameter, parameterType mismatch. type:" + type.FullName + " parameterName:" + parameterInfo.Name + " paramterType:" + parameterInfo.ParameterType.Name);
						case 0:
							throw new MessagePackDynamicObjectResolverException("can't find matched constructor parameter, index not found. type:" + type.FullName + " parameterName:" + parameterInfo.Name);
						}
					}
					num3++;
				}
			}
			ObjectSerializationInfo objectSerializationInfo = new ObjectSerializationInfo();
			objectSerializationInfo.IsClass = isClass;
			objectSerializationInfo.BestmatchConstructor = constructorInfo;
			objectSerializationInfo.ConstructorParameters = list.ToArray();
			objectSerializationInfo.IsIntKey = flag;
			objectSerializationInfo.Members = ((!flag) ? dictionary2.Values.ToArray() : dictionary.Values.ToArray());
			return objectSerializationInfo;
		}
	}
}
