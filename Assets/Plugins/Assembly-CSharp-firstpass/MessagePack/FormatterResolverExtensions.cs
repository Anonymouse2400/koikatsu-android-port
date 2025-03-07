using System;
using System.Reflection;
using MessagePack.Formatters;

namespace MessagePack
{
	public static class FormatterResolverExtensions
	{
		public static IMessagePackFormatter<T> GetFormatterWithVerify<T>(this IFormatterResolver resolver)
		{
			IMessagePackFormatter<T> formatter;
			try
			{
				formatter = resolver.GetFormatter<T>();
			}
			catch (TypeInitializationException ex)
			{
				Exception ex2 = ex;
				while (ex2.InnerException != null)
				{
					ex2 = ex2.InnerException;
				}
				throw ex2;
			}
			if (formatter == null)
			{
				throw new FormatterNotRegisteredException(typeof(T).FullName + " is not registered in this resolver. resolver:" + resolver.GetType().Name);
			}
			return formatter;
		}

		public static object GetFormatterDynamic(this IFormatterResolver resolver, Type type)
		{
			MethodInfo runtimeMethod = typeof(IFormatterResolver).GetRuntimeMethod("GetFormatter", Type.EmptyTypes);
			return runtimeMethod.MakeGenericMethod(type).Invoke(resolver, null);
		}
	}
}
