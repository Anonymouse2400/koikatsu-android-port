using System;
using System.Reflection;
using System.Reflection.Emit;

namespace MessagePack.Internal
{
	internal class DynamicAssembly
	{
		private readonly AssemblyBuilder assemblyBuilder;

		private readonly ModuleBuilder moduleBuilder;

		public ModuleBuilder ModuleBuilder
		{
			get
			{
				return moduleBuilder;
			}
		}

		public DynamicAssembly(string moduleName)
		{
			assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(moduleName), AssemblyBuilderAccess.Run);
			moduleBuilder = assemblyBuilder.DefineDynamicModule(moduleName);
		}
	}
}
