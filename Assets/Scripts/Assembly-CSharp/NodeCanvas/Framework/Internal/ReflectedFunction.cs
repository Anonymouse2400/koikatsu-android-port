using System;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace NodeCanvas.Framework.Internal
{
	[Serializable]
	[SpoofAOT]
	public class ReflectedFunction<TResult> : ReflectedFunctionWrapper
	{
		private FunctionCall<TResult> call;

		[BlackboardOnly]
		public BBParameter<TResult> result = new BBParameter<TResult>();

		public override BBParameter[] GetVariables()
		{
			return new BBParameter[1] { result };
		}

		public override void Init(object instance)
		{
			call = GetMethod().RTCreateDelegate<FunctionCall<TResult>>(instance);
		}

		public override object Call()
		{
			TResult val = call();
			result.value = val;
			return val;
		}
	}
	[Serializable]
	public class ReflectedFunction<TResult, T1> : ReflectedFunctionWrapper
	{
		private FunctionCall<T1, TResult> call;

		public BBParameter<T1> p1 = new BBParameter<T1>();

		[BlackboardOnly]
		public BBParameter<TResult> result = new BBParameter<TResult>();

		public override BBParameter[] GetVariables()
		{
			return new BBParameter[2] { result, p1 };
		}

		public override void Init(object instance)
		{
			call = GetMethod().RTCreateDelegate<FunctionCall<T1, TResult>>(instance);
		}

		public override object Call()
		{
			TResult val = call(p1.value);
			result.value = val;
			return val;
		}
	}
	[Serializable]
	public class ReflectedFunction<TResult, T1, T2> : ReflectedFunctionWrapper
	{
		private FunctionCall<T1, T2, TResult> call;

		public BBParameter<T1> p1 = new BBParameter<T1>();

		public BBParameter<T2> p2 = new BBParameter<T2>();

		[BlackboardOnly]
		public BBParameter<TResult> result = new BBParameter<TResult>();

		public override BBParameter[] GetVariables()
		{
			return new BBParameter[3] { result, p1, p2 };
		}

		public override void Init(object instance)
		{
			call = GetMethod().RTCreateDelegate<FunctionCall<T1, T2, TResult>>(instance);
		}

		public override object Call()
		{
			TResult val = call(p1.value, p2.value);
			result.value = val;
			return val;
		}
	}
	[Serializable]
	public class ReflectedFunction<TResult, T1, T2, T3> : ReflectedFunctionWrapper
	{
		private FunctionCall<T1, T2, T3, TResult> call;

		public BBParameter<T1> p1 = new BBParameter<T1>();

		public BBParameter<T2> p2 = new BBParameter<T2>();

		public BBParameter<T3> p3 = new BBParameter<T3>();

		[BlackboardOnly]
		public BBParameter<TResult> result = new BBParameter<TResult>();

		public override BBParameter[] GetVariables()
		{
			return new BBParameter[4] { result, p1, p2, p3 };
		}

		public override void Init(object instance)
		{
			call = GetMethod().RTCreateDelegate<FunctionCall<T1, T2, T3, TResult>>(instance);
		}

		public override object Call()
		{
			TResult val = call(p1.value, p2.value, p3.value);
			result.value = val;
			return val;
		}
	}
	[Serializable]
	public class ReflectedFunction<TResult, T1, T2, T3, T4> : ReflectedFunctionWrapper
	{
		private FunctionCall<T1, T2, T3, T4, TResult> call;

		public BBParameter<T1> p1 = new BBParameter<T1>();

		public BBParameter<T2> p2 = new BBParameter<T2>();

		public BBParameter<T3> p3 = new BBParameter<T3>();

		public BBParameter<T4> p4 = new BBParameter<T4>();

		[BlackboardOnly]
		public BBParameter<TResult> result = new BBParameter<TResult>();

		public override BBParameter[] GetVariables()
		{
			return new BBParameter[5] { result, p1, p2, p3, p4 };
		}

		public override void Init(object instance)
		{
			call = GetMethod().RTCreateDelegate<FunctionCall<T1, T2, T3, T4, TResult>>(instance);
		}

		public override object Call()
		{
			TResult val = call(p1.value, p2.value, p3.value, p4.value);
			result.value = val;
			return val;
		}
	}
	[Serializable]
	public class ReflectedFunction<TResult, T1, T2, T3, T4, T5> : ReflectedFunctionWrapper
	{
		private FunctionCall<T1, T2, T3, T4, T5, TResult> call;

		public BBParameter<T1> p1 = new BBParameter<T1>();

		public BBParameter<T2> p2 = new BBParameter<T2>();

		public BBParameter<T3> p3 = new BBParameter<T3>();

		public BBParameter<T4> p4 = new BBParameter<T4>();

		public BBParameter<T5> p5 = new BBParameter<T5>();

		[BlackboardOnly]
		public BBParameter<TResult> result = new BBParameter<TResult>();

		public override BBParameter[] GetVariables()
		{
			return new BBParameter[6] { result, p1, p2, p3, p4, p5 };
		}

		public override void Init(object instance)
		{
			call = GetMethod().RTCreateDelegate<FunctionCall<T1, T2, T3, T4, T5, TResult>>(instance);
		}

		public override object Call()
		{
			TResult val = call(p1.value, p2.value, p3.value, p4.value, p5.value);
			result.value = val;
			return val;
		}
	}
	[Serializable]
	public class ReflectedFunction<TResult, T1, T2, T3, T4, T5, T6> : ReflectedFunctionWrapper
	{
		private FunctionCall<T1, T2, T3, T4, T5, T6, TResult> call;

		public BBParameter<T1> p1 = new BBParameter<T1>();

		public BBParameter<T2> p2 = new BBParameter<T2>();

		public BBParameter<T3> p3 = new BBParameter<T3>();

		public BBParameter<T4> p4 = new BBParameter<T4>();

		public BBParameter<T5> p5 = new BBParameter<T5>();

		public BBParameter<T6> p6 = new BBParameter<T6>();

		[BlackboardOnly]
		public BBParameter<TResult> result = new BBParameter<TResult>();

		public override BBParameter[] GetVariables()
		{
			return new BBParameter[7] { result, p1, p2, p3, p4, p5, p6 };
		}

		public override void Init(object instance)
		{
			call = GetMethod().RTCreateDelegate<FunctionCall<T1, T2, T3, T4, T5, T6, TResult>>(instance);
		}

		public override object Call()
		{
			TResult val = call(p1.value, p2.value, p3.value, p4.value, p5.value, p6.value);
			result.value = val;
			return val;
		}
	}
}
