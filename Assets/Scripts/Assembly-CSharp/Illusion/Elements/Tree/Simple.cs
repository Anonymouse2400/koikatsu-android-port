using System;
using System.Collections.Generic;
using System.Linq;

namespace Illusion.Elements.Tree
{
	public class Simple<T>
	{
		public int level { get; private set; }

		public Simple<T> parent { get; private set; }

		public List<Simple<T>> children { get; private set; }

		public T data { get; private set; }

		public Simple(T data, int level = 0)
		{
			this.level = level;
			this.data = data;
			children = new List<Simple<T>>();
		}

		public void RootAction(Action<Simple<T>> act)
		{
			for (Simple<T> simple = this; simple != null; simple = simple.parent)
			{
				act(simple);
			}
		}

		public bool RootCheck(Func<Simple<T>, bool> func)
		{
			for (Simple<T> simple = this; simple != null; simple = simple.parent)
			{
				if (func(simple))
				{
					return true;
				}
			}
			return false;
		}

		public Simple<T> AddChild(T child)
		{
			Simple<T> simple = new Simple<T>(child, level + 1);
			simple.parent = this;
			children.Add(simple);
			return simple;
		}

		public bool RemoveChild(T child)
		{
			return children.Remove(children.FirstOrDefault((Simple<T> p) => p.data.Equals(child)));
		}
	}
}
