using System;
using System.Collections.Generic;

namespace ActionGame
{
	public class ObjectPool<T> where T : new()
	{
		private readonly Stack<T> _stack = new Stack<T>();

		private readonly Action<T> _actionOnGet;

		private readonly Action<T> _actionOnRelease;

		public int CountAll { get; private set; }

		public int CountInactive
		{
			get
			{
				return _stack.Count;
			}
		}

		public int CountActive
		{
			get
			{
				return CountAll - CountInactive;
			}
		}

		public ObjectPool(Action<T> actionOnGet, Action<T> actionOnRelease)
		{
			_actionOnGet = actionOnGet;
			_actionOnRelease = actionOnRelease;
		}

		public T Get()
		{
			T val;
			if (_stack.Count == 0)
			{
				val = new T();
				CountAll++;
			}
			else
			{
				val = _stack.Pop();
			}
			if (_actionOnGet != null)
			{
				_actionOnGet(val);
			}
			return val;
		}

		public void Release(T element)
		{
			if (_stack.Count <= 0 || object.ReferenceEquals(_stack.Peek(), element))
			{
			}
			if (_actionOnRelease != null)
			{
				_actionOnRelease(element);
			}
			_stack.Push(element);
		}
	}
}
