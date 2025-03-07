using System.Collections.Generic;
using System.Linq;

namespace System.Reflection
{
	public class TypeInfo
	{
		private readonly Type type;

		public bool IsClass
		{
			get
			{
				return type.IsClass;
			}
		}

		public bool IsPublic
		{
			get
			{
				return type.IsPublic;
			}
		}

		public bool IsValueType
		{
			get
			{
				return type.IsValueType;
			}
		}

		public bool IsNestedPublic
		{
			get
			{
				return type.IsNestedPublic;
			}
		}

		public IEnumerable<ConstructorInfo> DeclaredConstructors
		{
			get
			{
				return type.GetConstructors().AsEnumerable();
			}
		}

		public bool IsGenericType
		{
			get
			{
				return type.IsGenericType;
			}
		}

		public Type[] GenericTypeArguments
		{
			get
			{
				return type.GetGenericArguments();
			}
		}

		public bool IsEnum
		{
			get
			{
				return type.IsEnum;
			}
		}

		public Type[] ImplementedInterfaces
		{
			get
			{
				return type.GetInterfaces();
			}
		}

		public TypeInfo(Type type)
		{
			this.type = type;
		}

		public Type GetGenericTypeDefinition()
		{
			return type.GetGenericTypeDefinition();
		}

		public Type AsType()
		{
			return type;
		}

		public MethodInfo GetDeclaredMethod(string name)
		{
			return type.GetMethod(name);
		}

		public bool IsConstructedGenericType()
		{
			return type.IsGenericType && !type.IsGenericTypeDefinition;
		}

		public MethodInfo[] GetRuntimeMethods()
		{
			return type.GetMethods();
		}

		public T GetCustomAttribute<T>(bool inherit = true) where T : Attribute
		{
			return type.GetCustomAttributes(inherit).OfType<T>().FirstOrDefault();
		}

		public IEnumerable<T> GetCustomAttributes<T>(bool inherit = true) where T : Attribute
		{
			return type.GetCustomAttributes(inherit).OfType<T>();
		}
	}
}
