using System.Linq;
using System.Reflection.Emit;

namespace System.Reflection
{
	public static class ReflectionExtensions
	{
		public static MethodInfo GetRuntimeMethod(this Type type, string name, Type[] types)
		{
			return type.GetMethod(name, types);
		}

		public static TypeInfo GetTypeInfo(this Type type)
		{
			return new TypeInfo(type);
		}

		public static TypeInfo CreateTypeInfo(this TypeBuilder type)
		{
			return new TypeInfo(type.CreateType());
		}

		public static MethodInfo GetRuntimeMethods(this Type type, string name)
		{
			return type.GetMethod(name);
		}

		public static MethodInfo[] GetRuntimeMethods(this Type type)
		{
			return type.GetMethods();
		}

		public static PropertyInfo GetRuntimeProperty(this Type type, string name)
		{
			return type.GetProperty(name);
		}

		public static PropertyInfo[] GetRuntimeProperties(this Type type)
		{
			return type.GetProperties();
		}

		public static FieldInfo GetRuntimeField(this Type type, string name)
		{
			return type.GetField(name);
		}

		public static FieldInfo[] GetRuntimeFields(this Type type)
		{
			return type.GetFields();
		}

		public static T GetCustomAttribute<T>(this FieldInfo type, bool inherit) where T : Attribute
		{
			return type.GetCustomAttributes(inherit).OfType<T>().FirstOrDefault();
		}

		public static T GetCustomAttribute<T>(this PropertyInfo type, bool inherit) where T : Attribute
		{
			return type.GetCustomAttributes(inherit).OfType<T>().FirstOrDefault();
		}

		public static T GetCustomAttribute<T>(this ConstructorInfo type, bool inherit) where T : Attribute
		{
			return type.GetCustomAttributes(inherit).OfType<T>().FirstOrDefault();
		}
	}
}
