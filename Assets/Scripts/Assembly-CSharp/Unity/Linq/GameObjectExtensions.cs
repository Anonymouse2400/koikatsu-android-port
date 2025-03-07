using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.Linq
{
	public static class GameObjectExtensions
	{
		public struct ChildrenEnumerable : IEnumerable<GameObject>, IEnumerable
		{
			public struct Enumerator : IEnumerator<GameObject>, IEnumerator, IDisposable
			{
				private readonly int childCount;

				private readonly Transform originTransform;

				private readonly bool canRun;

				private bool withSelf;

				private int currentIndex;

				private GameObject current;

				object IEnumerator.Current
				{
					get
					{
						return current;
					}
				}

				public GameObject Current
				{
					get
					{
						return current;
					}
				}

				internal Enumerator(Transform originTransform, bool withSelf, bool canRun)
				{
					this.originTransform = originTransform;
					this.withSelf = withSelf;
					childCount = (canRun ? originTransform.childCount : 0);
					currentIndex = -1;
					this.canRun = canRun;
					current = null;
				}

				public bool MoveNext()
				{
					if (!canRun)
					{
						return false;
					}
					if (withSelf)
					{
						current = originTransform.gameObject;
						withSelf = false;
						return true;
					}
					currentIndex++;
					if (currentIndex < childCount)
					{
						Transform child = originTransform.GetChild(currentIndex);
						current = child.gameObject;
						return true;
					}
					return false;
				}

				public void Dispose()
				{
				}

				public void Reset()
				{
					throw new NotSupportedException();
				}
			}

			public struct OfComponentEnumerable<T> : IEnumerable<T>, IEnumerable where T : Component
			{
				private ChildrenEnumerable parent;

				public OfComponentEnumerable(ref ChildrenEnumerable parent)
				{
					this.parent = parent;
				}

				public OfComponentEnumerator<T> GetEnumerator()
				{
					return new OfComponentEnumerator<T>(ref parent);
				}

				IEnumerator<T> IEnumerable<T>.GetEnumerator()
				{
					return GetEnumerator();
				}

				IEnumerator IEnumerable.GetEnumerator()
				{
					return GetEnumerator();
				}

				public void ForEach(Action<T> action)
				{
					OfComponentEnumerator<T> enumerator = GetEnumerator();
					while (enumerator.MoveNext())
					{
						action(enumerator.Current);
					}
				}

				public T First()
				{
					OfComponentEnumerator<T> enumerator = GetEnumerator();
					if (enumerator.MoveNext())
					{
						return enumerator.Current;
					}
					throw new InvalidOperationException("sequence is empty.");
				}

				public T FirstOrDefault()
				{
					OfComponentEnumerator<T> enumerator = GetEnumerator();
					return (!enumerator.MoveNext()) ? ((T)null) : enumerator.Current;
				}

				public T[] ToArray()
				{
					T[] array = new T[parent.GetChildrenSize()];
					int num = ToArrayNonAlloc(ref array);
					if (array.Length != num)
					{
						Array.Resize(ref array, num);
					}
					return array;
				}

				public int ToArrayNonAlloc(ref T[] array)
				{
					int num = 0;
					OfComponentEnumerator<T> enumerator = GetEnumerator();
					while (enumerator.MoveNext())
					{
						if (array.Length == num)
						{
							int newSize = ((num != 0) ? (num * 2) : parent.GetChildrenSize());
							Array.Resize(ref array, newSize);
						}
						array[num++] = enumerator.Current;
					}
					return num;
				}
			}

			public struct OfComponentEnumerator<T> : IEnumerator<T>, IEnumerator, IDisposable where T : Component
			{
				private Enumerator enumerator;

				private T current;

				object IEnumerator.Current
				{
					get
					{
						return current;
					}
				}

				public T Current
				{
					get
					{
						return current;
					}
				}

				public OfComponentEnumerator(ref ChildrenEnumerable parent)
				{
					enumerator = parent.GetEnumerator();
					current = (T)null;
				}

				public bool MoveNext()
				{
					while (enumerator.MoveNext())
					{
						T component = enumerator.Current.GetComponent<T>();
						if (component != null)
						{
							current = component;
							return true;
						}
					}
					return false;
				}

				public void Dispose()
				{
				}

				public void Reset()
				{
					throw new NotSupportedException();
				}
			}

			private readonly GameObject origin;

			private readonly bool withSelf;

			public ChildrenEnumerable(GameObject origin, bool withSelf)
			{
				this.origin = origin;
				this.withSelf = withSelf;
			}

			public OfComponentEnumerable<T> OfComponent<T>() where T : Component
			{
				return new OfComponentEnumerable<T>(ref this);
			}

			public void Destroy(bool useDestroyImmediate = false, bool detachParent = false)
			{
				Enumerator enumerator = GetEnumerator();
				while (enumerator.MoveNext())
				{
					enumerator.Current.Destroy(useDestroyImmediate, false);
				}
				if (detachParent)
				{
					origin.transform.DetachChildren();
					if (withSelf)
					{
						origin.transform.SetParent(null);
					}
				}
			}

			public void Destroy(Func<GameObject, bool> predicate, bool useDestroyImmediate = false)
			{
				Enumerator enumerator = GetEnumerator();
				while (enumerator.MoveNext())
				{
					GameObject current = enumerator.Current;
					if (predicate(current))
					{
						current.Destroy(useDestroyImmediate, false);
					}
				}
			}

			public Enumerator GetEnumerator()
			{
				return (!(origin == null)) ? new Enumerator(origin.transform, withSelf, true) : new Enumerator(null, withSelf, false);
			}

			IEnumerator<GameObject> IEnumerable<GameObject>.GetEnumerator()
			{
				return GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			private int GetChildrenSize()
			{
				return origin.transform.childCount + (withSelf ? 1 : 0);
			}

			public void ForEach(Action<GameObject> action)
			{
				Enumerator enumerator = GetEnumerator();
				while (enumerator.MoveNext())
				{
					action(enumerator.Current);
				}
			}

			public int ToArrayNonAlloc(ref GameObject[] array)
			{
				int num = 0;
				Enumerator enumerator = GetEnumerator();
				while (enumerator.MoveNext())
				{
					GameObject current = enumerator.Current;
					if (array.Length == num)
					{
						int newSize = ((num != 0) ? (num * 2) : GetChildrenSize());
						Array.Resize(ref array, newSize);
					}
					array[num++] = current;
				}
				return num;
			}

			public int ToArrayNonAlloc(Func<GameObject, bool> filter, ref GameObject[] array)
			{
				int num = 0;
				Enumerator enumerator = GetEnumerator();
				while (enumerator.MoveNext())
				{
					GameObject current = enumerator.Current;
					if (filter(current))
					{
						if (array.Length == num)
						{
							int newSize = ((num != 0) ? (num * 2) : GetChildrenSize());
							Array.Resize(ref array, newSize);
						}
						array[num++] = current;
					}
				}
				return num;
			}

			public int ToArrayNonAlloc<T>(Func<GameObject, T> selector, ref T[] array)
			{
				int num = 0;
				Enumerator enumerator = GetEnumerator();
				while (enumerator.MoveNext())
				{
					GameObject current = enumerator.Current;
					if (array.Length == num)
					{
						int newSize = ((num != 0) ? (num * 2) : GetChildrenSize());
						Array.Resize(ref array, newSize);
					}
					array[num++] = selector(current);
				}
				return num;
			}

			public int ToArrayNonAlloc<T>(Func<GameObject, bool> filter, Func<GameObject, T> selector, ref T[] array)
			{
				int num = 0;
				Enumerator enumerator = GetEnumerator();
				while (enumerator.MoveNext())
				{
					GameObject current = enumerator.Current;
					if (filter(current))
					{
						if (array.Length == num)
						{
							int newSize = ((num != 0) ? (num * 2) : GetChildrenSize());
							Array.Resize(ref array, newSize);
						}
						array[num++] = selector(current);
					}
				}
				return num;
			}

			public int ToArrayNonAlloc<TState, T>(Func<GameObject, TState> let, Func<TState, bool> filter, Func<TState, T> selector, ref T[] array)
			{
				int num = 0;
				Enumerator enumerator = GetEnumerator();
				while (enumerator.MoveNext())
				{
					GameObject current = enumerator.Current;
					TState arg = let(current);
					if (filter(arg))
					{
						if (array.Length == num)
						{
							int newSize = ((num != 0) ? (num * 2) : GetChildrenSize());
							Array.Resize(ref array, newSize);
						}
						array[num++] = selector(arg);
					}
				}
				return num;
			}

			public GameObject[] ToArray()
			{
				GameObject[] array = new GameObject[GetChildrenSize()];
				int num = ToArrayNonAlloc(ref array);
				if (array.Length != num)
				{
					Array.Resize(ref array, num);
				}
				return array;
			}

			public GameObject[] ToArray(Func<GameObject, bool> filter)
			{
				GameObject[] array = new GameObject[GetChildrenSize()];
				int num = ToArrayNonAlloc(filter, ref array);
				if (array.Length != num)
				{
					Array.Resize(ref array, num);
				}
				return array;
			}

			public T[] ToArray<T>(Func<GameObject, T> selector)
			{
				T[] array = new T[GetChildrenSize()];
				int num = ToArrayNonAlloc(selector, ref array);
				if (array.Length != num)
				{
					Array.Resize(ref array, num);
				}
				return array;
			}

			public T[] ToArray<T>(Func<GameObject, bool> filter, Func<GameObject, T> selector)
			{
				T[] array = new T[GetChildrenSize()];
				int num = ToArrayNonAlloc(filter, selector, ref array);
				if (array.Length != num)
				{
					Array.Resize(ref array, num);
				}
				return array;
			}

			public T[] ToArray<TState, T>(Func<GameObject, TState> let, Func<TState, bool> filter, Func<TState, T> selector)
			{
				T[] array = new T[GetChildrenSize()];
				int num = ToArrayNonAlloc(let, filter, selector, ref array);
				if (array.Length != num)
				{
					Array.Resize(ref array, num);
				}
				return array;
			}

			public GameObject First()
			{
				Enumerator enumerator = GetEnumerator();
				if (enumerator.MoveNext())
				{
					return enumerator.Current;
				}
				throw new InvalidOperationException("sequence is empty.");
			}

			public GameObject FirstOrDefault()
			{
				Enumerator enumerator = GetEnumerator();
				return (!enumerator.MoveNext()) ? null : enumerator.Current;
			}
		}

		public struct AncestorsEnumerable : IEnumerable<GameObject>, IEnumerable
		{
			public struct Enumerator : IEnumerator<GameObject>, IEnumerator, IDisposable
			{
				private readonly bool canRun;

				private GameObject current;

				private Transform currentTransform;

				private bool withSelf;

				object IEnumerator.Current
				{
					get
					{
						return current;
					}
				}

				public GameObject Current
				{
					get
					{
						return current;
					}
				}

				internal Enumerator(GameObject origin, Transform originTransform, bool withSelf, bool canRun)
				{
					current = origin;
					currentTransform = originTransform;
					this.withSelf = withSelf;
					this.canRun = canRun;
				}

				public bool MoveNext()
				{
					if (!canRun)
					{
						return false;
					}
					if (withSelf)
					{
						withSelf = false;
						return true;
					}
					Transform parent = currentTransform.parent;
					if (parent != null)
					{
						current = parent.gameObject;
						currentTransform = parent;
						return true;
					}
					return false;
				}

				public void Dispose()
				{
				}

				public void Reset()
				{
					throw new NotSupportedException();
				}
			}

			public struct OfComponentEnumerable<T> : IEnumerable<T>, IEnumerable where T : Component
			{
				private AncestorsEnumerable parent;

				public OfComponentEnumerable(ref AncestorsEnumerable parent)
				{
					this.parent = parent;
				}

				public OfComponentEnumerator<T> GetEnumerator()
				{
					return new OfComponentEnumerator<T>(ref parent);
				}

				IEnumerator<T> IEnumerable<T>.GetEnumerator()
				{
					return GetEnumerator();
				}

				IEnumerator IEnumerable.GetEnumerator()
				{
					return GetEnumerator();
				}

				public void ForEach(Action<T> action)
				{
					OfComponentEnumerator<T> enumerator = GetEnumerator();
					while (enumerator.MoveNext())
					{
						action(enumerator.Current);
					}
				}

				public T First()
				{
					OfComponentEnumerator<T> enumerator = GetEnumerator();
					if (enumerator.MoveNext())
					{
						return enumerator.Current;
					}
					throw new InvalidOperationException("sequence is empty.");
				}

				public T FirstOrDefault()
				{
					OfComponentEnumerator<T> enumerator = GetEnumerator();
					return (!enumerator.MoveNext()) ? ((T)null) : enumerator.Current;
				}

				public T[] ToArray()
				{
					T[] array = new T[4];
					int num = ToArrayNonAlloc(ref array);
					if (array.Length != num)
					{
						Array.Resize(ref array, num);
					}
					return array;
				}

				public int ToArrayNonAlloc(ref T[] array)
				{
					int num = 0;
					OfComponentEnumerator<T> enumerator = GetEnumerator();
					while (enumerator.MoveNext())
					{
						if (array.Length == num)
						{
							int newSize = ((num != 0) ? (num * 2) : 4);
							Array.Resize(ref array, newSize);
						}
						array[num++] = enumerator.Current;
					}
					return num;
				}
			}

			public struct OfComponentEnumerator<T> : IEnumerator<T>, IEnumerator, IDisposable where T : Component
			{
				private Enumerator enumerator;

				private T current;

				object IEnumerator.Current
				{
					get
					{
						return current;
					}
				}

				public T Current
				{
					get
					{
						return current;
					}
				}

				public OfComponentEnumerator(ref AncestorsEnumerable parent)
				{
					enumerator = parent.GetEnumerator();
					current = (T)null;
				}

				public bool MoveNext()
				{
					while (enumerator.MoveNext())
					{
						T component = enumerator.Current.GetComponent<T>();
						if (component != null)
						{
							current = component;
							return true;
						}
					}
					return false;
				}

				public void Dispose()
				{
				}

				public void Reset()
				{
					throw new NotSupportedException();
				}
			}

			private readonly GameObject origin;

			private readonly bool withSelf;

			public AncestorsEnumerable(GameObject origin, bool withSelf)
			{
				this.origin = origin;
				this.withSelf = withSelf;
			}

			public OfComponentEnumerable<T> OfComponent<T>() where T : Component
			{
				return new OfComponentEnumerable<T>(ref this);
			}

			public void Destroy(bool useDestroyImmediate = false)
			{
				Enumerator enumerator = GetEnumerator();
				while (enumerator.MoveNext())
				{
					enumerator.Current.Destroy(useDestroyImmediate, false);
				}
			}

			public void Destroy(Func<GameObject, bool> predicate, bool useDestroyImmediate = false)
			{
				Enumerator enumerator = GetEnumerator();
				while (enumerator.MoveNext())
				{
					GameObject current = enumerator.Current;
					if (predicate(current))
					{
						current.Destroy(useDestroyImmediate, false);
					}
				}
			}

			public Enumerator GetEnumerator()
			{
				return (!(origin == null)) ? new Enumerator(origin, origin.transform, withSelf, true) : new Enumerator(null, null, withSelf, false);
			}

			IEnumerator<GameObject> IEnumerable<GameObject>.GetEnumerator()
			{
				return GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			public void ForEach(Action<GameObject> action)
			{
				Enumerator enumerator = GetEnumerator();
				while (enumerator.MoveNext())
				{
					action(enumerator.Current);
				}
			}

			public int ToArrayNonAlloc(ref GameObject[] array)
			{
				int num = 0;
				Enumerator enumerator = GetEnumerator();
				while (enumerator.MoveNext())
				{
					GameObject current = enumerator.Current;
					if (array.Length == num)
					{
						int newSize = ((num != 0) ? (num * 2) : 4);
						Array.Resize(ref array, newSize);
					}
					array[num++] = current;
				}
				return num;
			}

			public int ToArrayNonAlloc(Func<GameObject, bool> filter, ref GameObject[] array)
			{
				int num = 0;
				Enumerator enumerator = GetEnumerator();
				while (enumerator.MoveNext())
				{
					GameObject current = enumerator.Current;
					if (filter(current))
					{
						if (array.Length == num)
						{
							int newSize = ((num != 0) ? (num * 2) : 4);
							Array.Resize(ref array, newSize);
						}
						array[num++] = current;
					}
				}
				return num;
			}

			public int ToArrayNonAlloc<T>(Func<GameObject, T> selector, ref T[] array)
			{
				int num = 0;
				Enumerator enumerator = GetEnumerator();
				while (enumerator.MoveNext())
				{
					GameObject current = enumerator.Current;
					if (array.Length == num)
					{
						int newSize = ((num != 0) ? (num * 2) : 4);
						Array.Resize(ref array, newSize);
					}
					array[num++] = selector(current);
				}
				return num;
			}

			public int ToArrayNonAlloc<T>(Func<GameObject, bool> filter, Func<GameObject, T> selector, ref T[] array)
			{
				int num = 0;
				Enumerator enumerator = GetEnumerator();
				while (enumerator.MoveNext())
				{
					GameObject current = enumerator.Current;
					if (filter(current))
					{
						if (array.Length == num)
						{
							int newSize = ((num != 0) ? (num * 2) : 4);
							Array.Resize(ref array, newSize);
						}
						array[num++] = selector(current);
					}
				}
				return num;
			}

			public int ToArrayNonAlloc<TState, T>(Func<GameObject, TState> let, Func<TState, bool> filter, Func<TState, T> selector, ref T[] array)
			{
				int num = 0;
				Enumerator enumerator = GetEnumerator();
				while (enumerator.MoveNext())
				{
					GameObject current = enumerator.Current;
					TState arg = let(current);
					if (filter(arg))
					{
						if (array.Length == num)
						{
							int newSize = ((num != 0) ? (num * 2) : 4);
							Array.Resize(ref array, newSize);
						}
						array[num++] = selector(arg);
					}
				}
				return num;
			}

			public GameObject[] ToArray()
			{
				GameObject[] array = new GameObject[4];
				int num = ToArrayNonAlloc(ref array);
				if (array.Length != num)
				{
					Array.Resize(ref array, num);
				}
				return array;
			}

			public GameObject[] ToArray(Func<GameObject, bool> filter)
			{
				GameObject[] array = new GameObject[4];
				int num = ToArrayNonAlloc(filter, ref array);
				if (array.Length != num)
				{
					Array.Resize(ref array, num);
				}
				return array;
			}

			public T[] ToArray<T>(Func<GameObject, T> selector)
			{
				T[] array = new T[4];
				int num = ToArrayNonAlloc(selector, ref array);
				if (array.Length != num)
				{
					Array.Resize(ref array, num);
				}
				return array;
			}

			public T[] ToArray<T>(Func<GameObject, bool> filter, Func<GameObject, T> selector)
			{
				T[] array = new T[4];
				int num = ToArrayNonAlloc(filter, selector, ref array);
				if (array.Length != num)
				{
					Array.Resize(ref array, num);
				}
				return array;
			}

			public T[] ToArray<TState, T>(Func<GameObject, TState> let, Func<TState, bool> filter, Func<TState, T> selector)
			{
				T[] array = new T[4];
				int num = ToArrayNonAlloc(let, filter, selector, ref array);
				if (array.Length != num)
				{
					Array.Resize(ref array, num);
				}
				return array;
			}

			public GameObject First()
			{
				Enumerator enumerator = GetEnumerator();
				if (enumerator.MoveNext())
				{
					return enumerator.Current;
				}
				throw new InvalidOperationException("sequence is empty.");
			}

			public GameObject FirstOrDefault()
			{
				Enumerator enumerator = GetEnumerator();
				return (!enumerator.MoveNext()) ? null : enumerator.Current;
			}
		}

		public struct DescendantsEnumerable : IEnumerable<GameObject>, IEnumerable
		{
			internal class InternalUnsafeRefStack
			{
				public static Queue<InternalUnsafeRefStack> RefStackPool = new Queue<InternalUnsafeRefStack>();

				public int size;

				public Enumerator[] array;

				public InternalUnsafeRefStack(int initialStackDepth)
				{
					array = new Enumerator[initialStackDepth];
				}

				public void Push(ref Enumerator e)
				{
					if (size == array.Length)
					{
						Array.Resize(ref array, array.Length * 2);
					}
					array[size++] = e;
				}

				public void Reset()
				{
					size = 0;
				}
			}

			public struct Enumerator : IEnumerator<GameObject>, IEnumerator, IDisposable
			{
				private readonly int childCount;

				private readonly Transform originTransform;

				private bool canRun;

				private bool withSelf;

				private int currentIndex;

				private GameObject current;

				private InternalUnsafeRefStack sharedStack;

				private Func<Transform, bool> descendIntoChildren;

				object IEnumerator.Current
				{
					get
					{
						return current;
					}
				}

				public GameObject Current
				{
					get
					{
						return current;
					}
				}

				internal Enumerator(Transform originTransform, bool withSelf, bool canRun, InternalUnsafeRefStack sharedStack, Func<Transform, bool> descendIntoChildren)
				{
					this.originTransform = originTransform;
					this.withSelf = withSelf;
					childCount = (canRun ? originTransform.childCount : 0);
					currentIndex = -1;
					this.canRun = canRun;
					current = null;
					this.sharedStack = sharedStack;
					this.descendIntoChildren = descendIntoChildren;
				}

				public bool MoveNext()
				{
					if (!canRun)
					{
						return false;
					}
					while (sharedStack.size != 0)
					{
						if (sharedStack.array[sharedStack.size - 1].MoveNextCore(true, out current))
						{
							return true;
						}
					}
					if (!withSelf && !descendIntoChildren(originTransform))
					{
						canRun = false;
						InternalUnsafeRefStack.RefStackPool.Enqueue(sharedStack);
						return false;
					}
					if (MoveNextCore(false, out current))
					{
						return true;
					}
					canRun = false;
					InternalUnsafeRefStack.RefStackPool.Enqueue(sharedStack);
					return false;
				}

				private bool MoveNextCore(bool peek, out GameObject current)
				{
					if (withSelf)
					{
						current = originTransform.gameObject;
						withSelf = false;
						return true;
					}
					currentIndex++;
					if (currentIndex < childCount)
					{
						Transform child = originTransform.GetChild(currentIndex);
						if (descendIntoChildren(child))
						{
							Enumerator e = new Enumerator(child, true, true, sharedStack, descendIntoChildren);
							sharedStack.Push(ref e);
							return sharedStack.array[sharedStack.size - 1].MoveNextCore(true, out current);
						}
						current = child.gameObject;
						return true;
					}
					if (peek)
					{
						sharedStack.size--;
					}
					current = null;
					return false;
				}

				public void Dispose()
				{
					if (canRun)
					{
						canRun = false;
						InternalUnsafeRefStack.RefStackPool.Enqueue(sharedStack);
					}
				}

				public void Reset()
				{
					throw new NotSupportedException();
				}
			}

			public struct OfComponentEnumerable<T> : IEnumerable<T>, IEnumerable where T : Component
			{
				private DescendantsEnumerable parent;

				public OfComponentEnumerable(ref DescendantsEnumerable parent)
				{
					this.parent = parent;
				}

				public OfComponentEnumerator<T> GetEnumerator()
				{
					return new OfComponentEnumerator<T>(ref parent);
				}

				IEnumerator<T> IEnumerable<T>.GetEnumerator()
				{
					return GetEnumerator();
				}

				IEnumerator IEnumerable.GetEnumerator()
				{
					return GetEnumerator();
				}

				public T First()
				{
					OfComponentEnumerator<T> enumerator = GetEnumerator();
					try
					{
						if (enumerator.MoveNext())
						{
							return enumerator.Current;
						}
						throw new InvalidOperationException("sequence is empty.");
					}
					finally
					{
						enumerator.Dispose();
					}
				}

				public T FirstOrDefault()
				{
					OfComponentEnumerator<T> enumerator = GetEnumerator();
					try
					{
						return (!enumerator.MoveNext()) ? ((T)null) : enumerator.Current;
					}
					finally
					{
						enumerator.Dispose();
					}
				}

				public void ForEach(Action<T> action)
				{
					if (parent.withSelf)
					{
						T val = (T)null;
						val = parent.origin.GetComponent<T>();
						if (val != null)
						{
							action(val);
						}
					}
					Transform transform = parent.origin.transform;
					OfComponentDescendantsCore(ref transform, ref action);
				}

				public T[] ToArray()
				{
					T[] array = new T[4];
					int num = ToArrayNonAlloc(ref array);
					if (array.Length != num)
					{
						Array.Resize(ref array, num);
					}
					return array;
				}

				private void OfComponentDescendantsCore(ref Transform transform, ref Action<T> action)
				{
					if (!parent.descendIntoChildren(transform))
					{
						return;
					}
					int childCount = transform.childCount;
					for (int i = 0; i < childCount; i++)
					{
						Transform transform2 = transform.GetChild(i);
						T val = (T)null;
						val = transform2.GetComponent<T>();
						if (val != null)
						{
							action(val);
						}
						OfComponentDescendantsCore(ref transform2, ref action);
					}
				}

				private void OfComponentDescendantsCore(ref Transform transform, ref int index, ref T[] array)
				{
					if (!parent.descendIntoChildren(transform))
					{
						return;
					}
					int childCount = transform.childCount;
					for (int i = 0; i < childCount; i++)
					{
						Transform transform2 = transform.GetChild(i);
						T val = (T)null;
						val = transform2.GetComponent<T>();
						if (val != null)
						{
							if (array.Length == index)
							{
								int newSize = ((index != 0) ? (index * 2) : 4);
								Array.Resize(ref array, newSize);
							}
							array[index++] = val;
						}
						OfComponentDescendantsCore(ref transform2, ref index, ref array);
					}
				}

				public int ToArrayNonAlloc(ref T[] array)
				{
					int index = 0;
					if (parent.withSelf)
					{
						T val = (T)null;
						val = parent.origin.GetComponent<T>();
						if (val != null)
						{
							if (array.Length == index)
							{
								int newSize = ((index != 0) ? (index * 2) : 4);
								Array.Resize(ref array, newSize);
							}
							array[index++] = val;
						}
					}
					Transform transform = parent.origin.transform;
					OfComponentDescendantsCore(ref transform, ref index, ref array);
					return index;
				}
			}

			public struct OfComponentEnumerator<T> : IEnumerator<T>, IEnumerator, IDisposable where T : Component
			{
				private Enumerator enumerator;

				private T current;

				object IEnumerator.Current
				{
					get
					{
						return current;
					}
				}

				public T Current
				{
					get
					{
						return current;
					}
				}

				public OfComponentEnumerator(ref DescendantsEnumerable parent)
				{
					enumerator = parent.GetEnumerator();
					current = (T)null;
				}

				public bool MoveNext()
				{
					while (enumerator.MoveNext())
					{
						T component = enumerator.Current.GetComponent<T>();
						if (component != null)
						{
							current = component;
							return true;
						}
					}
					return false;
				}

				public void Dispose()
				{
					enumerator.Dispose();
				}

				public void Reset()
				{
					throw new NotSupportedException();
				}
			}

			private static readonly Func<Transform, bool> alwaysTrue = (Transform _) => true;

			private readonly GameObject origin;

			private readonly bool withSelf;

			private readonly Func<Transform, bool> descendIntoChildren;

			public DescendantsEnumerable(GameObject origin, bool withSelf, Func<Transform, bool> descendIntoChildren)
			{
				this.origin = origin;
				this.withSelf = withSelf;
				this.descendIntoChildren = descendIntoChildren ?? alwaysTrue;
			}

			public OfComponentEnumerable<T> OfComponent<T>() where T : Component
			{
				return new OfComponentEnumerable<T>(ref this);
			}

			public void Destroy(bool useDestroyImmediate = false)
			{
				Enumerator enumerator = GetEnumerator();
				while (enumerator.MoveNext())
				{
					enumerator.Current.Destroy(useDestroyImmediate, false);
				}
			}

			public void Destroy(Func<GameObject, bool> predicate, bool useDestroyImmediate = false)
			{
				Enumerator enumerator = GetEnumerator();
				while (enumerator.MoveNext())
				{
					GameObject current = enumerator.Current;
					if (predicate(current))
					{
						current.Destroy(useDestroyImmediate, false);
					}
				}
			}

			public Enumerator GetEnumerator()
			{
				if (origin == null)
				{
					return new Enumerator(null, withSelf, false, null, descendIntoChildren);
				}
				InternalUnsafeRefStack internalUnsafeRefStack;
				if (InternalUnsafeRefStack.RefStackPool.Count != 0)
				{
					internalUnsafeRefStack = InternalUnsafeRefStack.RefStackPool.Dequeue();
					internalUnsafeRefStack.Reset();
				}
				else
				{
					internalUnsafeRefStack = new InternalUnsafeRefStack(6);
				}
				return new Enumerator(origin.transform, withSelf, true, internalUnsafeRefStack, descendIntoChildren);
			}

			IEnumerator<GameObject> IEnumerable<GameObject>.GetEnumerator()
			{
				return GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			private void ResizeArray<T>(ref int index, ref T[] array)
			{
				if (array.Length == index)
				{
					int newSize = ((index != 0) ? (index * 2) : 4);
					Array.Resize(ref array, newSize);
				}
			}

			private void DescendantsCore(ref Transform transform, ref Action<GameObject> action)
			{
				if (descendIntoChildren(transform))
				{
					int childCount = transform.childCount;
					for (int i = 0; i < childCount; i++)
					{
						Transform transform2 = transform.GetChild(i);
						action(transform2.gameObject);
						DescendantsCore(ref transform2, ref action);
					}
				}
			}

			private void DescendantsCore(ref Transform transform, ref int index, ref GameObject[] array)
			{
				if (descendIntoChildren(transform))
				{
					int childCount = transform.childCount;
					for (int i = 0; i < childCount; i++)
					{
						Transform transform2 = transform.GetChild(i);
						ResizeArray(ref index, ref array);
						array[index++] = transform2.gameObject;
						DescendantsCore(ref transform2, ref index, ref array);
					}
				}
			}

			private void DescendantsCore(ref Func<GameObject, bool> filter, ref Transform transform, ref int index, ref GameObject[] array)
			{
				if (!descendIntoChildren(transform))
				{
					return;
				}
				int childCount = transform.childCount;
				for (int i = 0; i < childCount; i++)
				{
					Transform transform2 = transform.GetChild(i);
					GameObject gameObject = transform2.gameObject;
					if (filter(gameObject))
					{
						ResizeArray(ref index, ref array);
						array[index++] = gameObject;
					}
					DescendantsCore(ref filter, ref transform2, ref index, ref array);
				}
			}

			private void DescendantsCore<T>(ref Func<GameObject, T> selector, ref Transform transform, ref int index, ref T[] array)
			{
				if (descendIntoChildren(transform))
				{
					int childCount = transform.childCount;
					for (int i = 0; i < childCount; i++)
					{
						Transform transform2 = transform.GetChild(i);
						ResizeArray(ref index, ref array);
						array[index++] = selector(transform2.gameObject);
						DescendantsCore(ref selector, ref transform2, ref index, ref array);
					}
				}
			}

			private void DescendantsCore<T>(ref Func<GameObject, bool> filter, ref Func<GameObject, T> selector, ref Transform transform, ref int index, ref T[] array)
			{
				if (!descendIntoChildren(transform))
				{
					return;
				}
				int childCount = transform.childCount;
				for (int i = 0; i < childCount; i++)
				{
					Transform transform2 = transform.GetChild(i);
					GameObject gameObject = transform2.gameObject;
					if (filter(gameObject))
					{
						ResizeArray(ref index, ref array);
						array[index++] = selector(gameObject);
					}
					DescendantsCore(ref filter, ref selector, ref transform2, ref index, ref array);
				}
			}

			private void DescendantsCore<TState, T>(ref Func<GameObject, TState> let, ref Func<TState, bool> filter, ref Func<TState, T> selector, ref Transform transform, ref int index, ref T[] array)
			{
				if (!descendIntoChildren(transform))
				{
					return;
				}
				int childCount = transform.childCount;
				for (int i = 0; i < childCount; i++)
				{
					Transform transform2 = transform.GetChild(i);
					TState arg = let(transform2.gameObject);
					if (filter(arg))
					{
						ResizeArray(ref index, ref array);
						array[index++] = selector(arg);
					}
					DescendantsCore(ref let, ref filter, ref selector, ref transform2, ref index, ref array);
				}
			}

			public void ForEach(Action<GameObject> action)
			{
				if (withSelf)
				{
					action(origin);
				}
				Transform transform = origin.transform;
				DescendantsCore(ref transform, ref action);
			}

			public int ToArrayNonAlloc(ref GameObject[] array)
			{
				int index = 0;
				if (withSelf)
				{
					ResizeArray(ref index, ref array);
					array[index++] = origin;
				}
				Transform transform = origin.transform;
				DescendantsCore(ref transform, ref index, ref array);
				return index;
			}

			public int ToArrayNonAlloc(Func<GameObject, bool> filter, ref GameObject[] array)
			{
				int index = 0;
				if (withSelf && filter(origin))
				{
					ResizeArray(ref index, ref array);
					array[index++] = origin;
				}
				Transform transform = origin.transform;
				DescendantsCore(ref filter, ref transform, ref index, ref array);
				return index;
			}

			public int ToArrayNonAlloc<T>(Func<GameObject, T> selector, ref T[] array)
			{
				int index = 0;
				if (withSelf)
				{
					ResizeArray(ref index, ref array);
					array[index++] = selector(origin);
				}
				Transform transform = origin.transform;
				DescendantsCore(ref selector, ref transform, ref index, ref array);
				return index;
			}

			public int ToArrayNonAlloc<T>(Func<GameObject, bool> filter, Func<GameObject, T> selector, ref T[] array)
			{
				int index = 0;
				if (withSelf && filter(origin))
				{
					ResizeArray(ref index, ref array);
					array[index++] = selector(origin);
				}
				Transform transform = origin.transform;
				DescendantsCore(ref filter, ref selector, ref transform, ref index, ref array);
				return index;
			}

			public int ToArrayNonAlloc<TState, T>(Func<GameObject, TState> let, Func<TState, bool> filter, Func<TState, T> selector, ref T[] array)
			{
				int index = 0;
				if (withSelf)
				{
					TState arg = let(origin);
					if (filter(arg))
					{
						ResizeArray(ref index, ref array);
						array[index++] = selector(arg);
					}
				}
				Transform transform = origin.transform;
				DescendantsCore(ref let, ref filter, ref selector, ref transform, ref index, ref array);
				return index;
			}

			public GameObject[] ToArray()
			{
				GameObject[] array = new GameObject[4];
				int num = ToArrayNonAlloc(ref array);
				if (array.Length != num)
				{
					Array.Resize(ref array, num);
				}
				return array;
			}

			public GameObject[] ToArray(Func<GameObject, bool> filter)
			{
				GameObject[] array = new GameObject[4];
				int num = ToArrayNonAlloc(filter, ref array);
				if (array.Length != num)
				{
					Array.Resize(ref array, num);
				}
				return array;
			}

			public T[] ToArray<T>(Func<GameObject, T> selector)
			{
				T[] array = new T[4];
				int num = ToArrayNonAlloc(selector, ref array);
				if (array.Length != num)
				{
					Array.Resize(ref array, num);
				}
				return array;
			}

			public T[] ToArray<T>(Func<GameObject, bool> filter, Func<GameObject, T> selector)
			{
				T[] array = new T[4];
				int num = ToArrayNonAlloc(filter, selector, ref array);
				if (array.Length != num)
				{
					Array.Resize(ref array, num);
				}
				return array;
			}

			public T[] ToArray<TState, T>(Func<GameObject, TState> let, Func<TState, bool> filter, Func<TState, T> selector)
			{
				T[] array = new T[4];
				int num = ToArrayNonAlloc(let, filter, selector, ref array);
				if (array.Length != num)
				{
					Array.Resize(ref array, num);
				}
				return array;
			}

			public GameObject First()
			{
				Enumerator enumerator = GetEnumerator();
				try
				{
					if (enumerator.MoveNext())
					{
						return enumerator.Current;
					}
					throw new InvalidOperationException("sequence is empty.");
				}
				finally
				{
					enumerator.Dispose();
				}
			}

			public GameObject FirstOrDefault()
			{
				Enumerator enumerator = GetEnumerator();
				try
				{
					return (!enumerator.MoveNext()) ? null : enumerator.Current;
				}
				finally
				{
					enumerator.Dispose();
				}
			}
		}

		public struct BeforeSelfEnumerable : IEnumerable<GameObject>, IEnumerable
		{
			public struct Enumerator : IEnumerator<GameObject>, IEnumerator, IDisposable
			{
				private readonly int childCount;

				private readonly Transform originTransform;

				private bool canRun;

				private bool withSelf;

				private int currentIndex;

				private GameObject current;

				private Transform parent;

				object IEnumerator.Current
				{
					get
					{
						return current;
					}
				}

				public GameObject Current
				{
					get
					{
						return current;
					}
				}

				internal Enumerator(Transform originTransform, bool withSelf, bool canRun)
				{
					this.originTransform = originTransform;
					this.withSelf = withSelf;
					currentIndex = -1;
					this.canRun = canRun;
					current = null;
					parent = originTransform.parent;
					childCount = ((parent != null) ? parent.childCount : 0);
				}

				public bool MoveNext()
				{
					if (!canRun)
					{
						return false;
					}
					if (!(parent == null))
					{
						currentIndex++;
						if (currentIndex < childCount)
						{
							Transform child = parent.GetChild(currentIndex);
							if (!(child == originTransform))
							{
								current = child.gameObject;
								return true;
							}
						}
					}
					if (withSelf)
					{
						current = originTransform.gameObject;
						withSelf = false;
						canRun = false;
						return true;
					}
					return false;
				}

				public void Dispose()
				{
				}

				public void Reset()
				{
					throw new NotSupportedException();
				}
			}

			public struct OfComponentEnumerable<T> : IEnumerable<T>, IEnumerable where T : Component
			{
				private BeforeSelfEnumerable parent;

				public OfComponentEnumerable(ref BeforeSelfEnumerable parent)
				{
					this.parent = parent;
				}

				public OfComponentEnumerator<T> GetEnumerator()
				{
					return new OfComponentEnumerator<T>(ref parent);
				}

				IEnumerator<T> IEnumerable<T>.GetEnumerator()
				{
					return GetEnumerator();
				}

				IEnumerator IEnumerable.GetEnumerator()
				{
					return GetEnumerator();
				}

				public void ForEach(Action<T> action)
				{
					OfComponentEnumerator<T> enumerator = GetEnumerator();
					while (enumerator.MoveNext())
					{
						action(enumerator.Current);
					}
				}

				public T First()
				{
					OfComponentEnumerator<T> enumerator = GetEnumerator();
					if (enumerator.MoveNext())
					{
						return enumerator.Current;
					}
					throw new InvalidOperationException("sequence is empty.");
				}

				public T FirstOrDefault()
				{
					OfComponentEnumerator<T> enumerator = GetEnumerator();
					return (!enumerator.MoveNext()) ? ((T)null) : enumerator.Current;
				}

				public T[] ToArray()
				{
					T[] array = new T[4];
					int num = ToArrayNonAlloc(ref array);
					if (array.Length != num)
					{
						Array.Resize(ref array, num);
					}
					return array;
				}

				public int ToArrayNonAlloc(ref T[] array)
				{
					int num = 0;
					OfComponentEnumerator<T> enumerator = GetEnumerator();
					while (enumerator.MoveNext())
					{
						if (array.Length == num)
						{
							int newSize = ((num != 0) ? (num * 2) : 4);
							Array.Resize(ref array, newSize);
						}
						array[num++] = enumerator.Current;
					}
					return num;
				}
			}

			public struct OfComponentEnumerator<T> : IEnumerator<T>, IEnumerator, IDisposable where T : Component
			{
				private Enumerator enumerator;

				private T current;

				object IEnumerator.Current
				{
					get
					{
						return current;
					}
				}

				public T Current
				{
					get
					{
						return current;
					}
				}

				public OfComponentEnumerator(ref BeforeSelfEnumerable parent)
				{
					enumerator = parent.GetEnumerator();
					current = (T)null;
				}

				public bool MoveNext()
				{
					while (enumerator.MoveNext())
					{
						T component = enumerator.Current.GetComponent<T>();
						if (component != null)
						{
							current = component;
							return true;
						}
					}
					return false;
				}

				public void Dispose()
				{
				}

				public void Reset()
				{
					throw new NotSupportedException();
				}
			}

			private readonly GameObject origin;

			private readonly bool withSelf;

			public BeforeSelfEnumerable(GameObject origin, bool withSelf)
			{
				this.origin = origin;
				this.withSelf = withSelf;
			}

			public OfComponentEnumerable<T> OfComponent<T>() where T : Component
			{
				return new OfComponentEnumerable<T>(ref this);
			}

			public void Destroy(bool useDestroyImmediate = false)
			{
				Enumerator enumerator = GetEnumerator();
				while (enumerator.MoveNext())
				{
					enumerator.Current.Destroy(useDestroyImmediate, false);
				}
			}

			public void Destroy(Func<GameObject, bool> predicate, bool useDestroyImmediate = false)
			{
				Enumerator enumerator = GetEnumerator();
				while (enumerator.MoveNext())
				{
					GameObject current = enumerator.Current;
					if (predicate(current))
					{
						current.Destroy(useDestroyImmediate, false);
					}
				}
			}

			public Enumerator GetEnumerator()
			{
				return (!(origin == null)) ? new Enumerator(origin.transform, withSelf, true) : new Enumerator(null, withSelf, false);
			}

			IEnumerator<GameObject> IEnumerable<GameObject>.GetEnumerator()
			{
				return GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			public void ForEach(Action<GameObject> action)
			{
				Enumerator enumerator = GetEnumerator();
				while (enumerator.MoveNext())
				{
					action(enumerator.Current);
				}
			}

			public int ToArrayNonAlloc(ref GameObject[] array)
			{
				int num = 0;
				Enumerator enumerator = GetEnumerator();
				while (enumerator.MoveNext())
				{
					GameObject current = enumerator.Current;
					if (array.Length == num)
					{
						int newSize = ((num != 0) ? (num * 2) : 4);
						Array.Resize(ref array, newSize);
					}
					array[num++] = current;
				}
				return num;
			}

			public int ToArrayNonAlloc(Func<GameObject, bool> filter, ref GameObject[] array)
			{
				int num = 0;
				Enumerator enumerator = GetEnumerator();
				while (enumerator.MoveNext())
				{
					GameObject current = enumerator.Current;
					if (filter(current))
					{
						if (array.Length == num)
						{
							int newSize = ((num != 0) ? (num * 2) : 4);
							Array.Resize(ref array, newSize);
						}
						array[num++] = current;
					}
				}
				return num;
			}

			public int ToArrayNonAlloc<T>(Func<GameObject, T> selector, ref T[] array)
			{
				int num = 0;
				Enumerator enumerator = GetEnumerator();
				while (enumerator.MoveNext())
				{
					GameObject current = enumerator.Current;
					if (array.Length == num)
					{
						int newSize = ((num != 0) ? (num * 2) : 4);
						Array.Resize(ref array, newSize);
					}
					array[num++] = selector(current);
				}
				return num;
			}

			public int ToArrayNonAlloc<T>(Func<GameObject, bool> filter, Func<GameObject, T> selector, ref T[] array)
			{
				int num = 0;
				Enumerator enumerator = GetEnumerator();
				while (enumerator.MoveNext())
				{
					GameObject current = enumerator.Current;
					if (filter(current))
					{
						if (array.Length == num)
						{
							int newSize = ((num != 0) ? (num * 2) : 4);
							Array.Resize(ref array, newSize);
						}
						array[num++] = selector(current);
					}
				}
				return num;
			}

			public int ToArrayNonAlloc<TState, T>(Func<GameObject, TState> let, Func<TState, bool> filter, Func<TState, T> selector, ref T[] array)
			{
				int num = 0;
				Enumerator enumerator = GetEnumerator();
				while (enumerator.MoveNext())
				{
					GameObject current = enumerator.Current;
					TState arg = let(current);
					if (filter(arg))
					{
						if (array.Length == num)
						{
							int newSize = ((num != 0) ? (num * 2) : 4);
							Array.Resize(ref array, newSize);
						}
						array[num++] = selector(arg);
					}
				}
				return num;
			}

			public GameObject[] ToArray()
			{
				GameObject[] array = new GameObject[4];
				int num = ToArrayNonAlloc(ref array);
				if (array.Length != num)
				{
					Array.Resize(ref array, num);
				}
				return array;
			}

			public GameObject[] ToArray(Func<GameObject, bool> filter)
			{
				GameObject[] array = new GameObject[4];
				int num = ToArrayNonAlloc(filter, ref array);
				if (array.Length != num)
				{
					Array.Resize(ref array, num);
				}
				return array;
			}

			public T[] ToArray<T>(Func<GameObject, T> selector)
			{
				T[] array = new T[4];
				int num = ToArrayNonAlloc(selector, ref array);
				if (array.Length != num)
				{
					Array.Resize(ref array, num);
				}
				return array;
			}

			public T[] ToArray<T>(Func<GameObject, bool> filter, Func<GameObject, T> selector)
			{
				T[] array = new T[4];
				int num = ToArrayNonAlloc(filter, selector, ref array);
				if (array.Length != num)
				{
					Array.Resize(ref array, num);
				}
				return array;
			}

			public T[] ToArray<TState, T>(Func<GameObject, TState> let, Func<TState, bool> filter, Func<TState, T> selector)
			{
				T[] array = new T[4];
				int num = ToArrayNonAlloc(let, filter, selector, ref array);
				if (array.Length != num)
				{
					Array.Resize(ref array, num);
				}
				return array;
			}

			public GameObject First()
			{
				Enumerator enumerator = GetEnumerator();
				if (enumerator.MoveNext())
				{
					return enumerator.Current;
				}
				throw new InvalidOperationException("sequence is empty.");
			}

			public GameObject FirstOrDefault()
			{
				Enumerator enumerator = GetEnumerator();
				return (!enumerator.MoveNext()) ? null : enumerator.Current;
			}
		}

		public struct AfterSelfEnumerable : IEnumerable<GameObject>, IEnumerable
		{
			public struct Enumerator : IEnumerator<GameObject>, IEnumerator, IDisposable
			{
				private readonly int childCount;

				private readonly Transform originTransform;

				private readonly bool canRun;

				private bool withSelf;

				private int currentIndex;

				private GameObject current;

				private Transform parent;

				object IEnumerator.Current
				{
					get
					{
						return current;
					}
				}

				public GameObject Current
				{
					get
					{
						return current;
					}
				}

				internal Enumerator(Transform originTransform, bool withSelf, bool canRun)
				{
					this.originTransform = originTransform;
					this.withSelf = withSelf;
					currentIndex = ((originTransform != null) ? (originTransform.GetSiblingIndex() + 1) : 0);
					this.canRun = canRun;
					current = null;
					parent = originTransform.parent;
					childCount = ((parent != null) ? parent.childCount : 0);
				}

				public bool MoveNext()
				{
					if (!canRun)
					{
						return false;
					}
					if (withSelf)
					{
						current = originTransform.gameObject;
						withSelf = false;
						return true;
					}
					if (currentIndex < childCount)
					{
						current = parent.GetChild(currentIndex).gameObject;
						currentIndex++;
						return true;
					}
					return false;
				}

				public void Dispose()
				{
				}

				public void Reset()
				{
					throw new NotSupportedException();
				}
			}

			public struct OfComponentEnumerable<T> : IEnumerable<T>, IEnumerable where T : Component
			{
				private AfterSelfEnumerable parent;

				public OfComponentEnumerable(ref AfterSelfEnumerable parent)
				{
					this.parent = parent;
				}

				public OfComponentEnumerator<T> GetEnumerator()
				{
					return new OfComponentEnumerator<T>(ref parent);
				}

				IEnumerator<T> IEnumerable<T>.GetEnumerator()
				{
					return GetEnumerator();
				}

				IEnumerator IEnumerable.GetEnumerator()
				{
					return GetEnumerator();
				}

				public void ForEach(Action<T> action)
				{
					OfComponentEnumerator<T> enumerator = GetEnumerator();
					while (enumerator.MoveNext())
					{
						action(enumerator.Current);
					}
				}

				public T First()
				{
					OfComponentEnumerator<T> enumerator = GetEnumerator();
					if (enumerator.MoveNext())
					{
						return enumerator.Current;
					}
					throw new InvalidOperationException("sequence is empty.");
				}

				public T FirstOrDefault()
				{
					OfComponentEnumerator<T> enumerator = GetEnumerator();
					return (!enumerator.MoveNext()) ? ((T)null) : enumerator.Current;
				}

				public T[] ToArray()
				{
					T[] array = new T[4];
					int num = ToArrayNonAlloc(ref array);
					if (array.Length != num)
					{
						Array.Resize(ref array, num);
					}
					return array;
				}

				public int ToArrayNonAlloc(ref T[] array)
				{
					int num = 0;
					OfComponentEnumerator<T> enumerator = GetEnumerator();
					while (enumerator.MoveNext())
					{
						if (array.Length == num)
						{
							int newSize = ((num != 0) ? (num * 2) : 4);
							Array.Resize(ref array, newSize);
						}
						array[num++] = enumerator.Current;
					}
					return num;
				}
			}

			public struct OfComponentEnumerator<T> : IEnumerator<T>, IEnumerator, IDisposable where T : Component
			{
				private Enumerator enumerator;

				private T current;

				object IEnumerator.Current
				{
					get
					{
						return current;
					}
				}

				public T Current
				{
					get
					{
						return current;
					}
				}

				public OfComponentEnumerator(ref AfterSelfEnumerable parent)
				{
					enumerator = parent.GetEnumerator();
					current = (T)null;
				}

				public bool MoveNext()
				{
					while (enumerator.MoveNext())
					{
						T component = enumerator.Current.GetComponent<T>();
						if (component != null)
						{
							current = component;
							return true;
						}
					}
					return false;
				}

				public void Dispose()
				{
				}

				public void Reset()
				{
					throw new NotSupportedException();
				}
			}

			private readonly GameObject origin;

			private readonly bool withSelf;

			public AfterSelfEnumerable(GameObject origin, bool withSelf)
			{
				this.origin = origin;
				this.withSelf = withSelf;
			}

			public OfComponentEnumerable<T> OfComponent<T>() where T : Component
			{
				return new OfComponentEnumerable<T>(ref this);
			}

			public void Destroy(bool useDestroyImmediate = false)
			{
				Enumerator enumerator = GetEnumerator();
				while (enumerator.MoveNext())
				{
					enumerator.Current.Destroy(useDestroyImmediate, false);
				}
			}

			public void Destroy(Func<GameObject, bool> predicate, bool useDestroyImmediate = false)
			{
				Enumerator enumerator = GetEnumerator();
				while (enumerator.MoveNext())
				{
					GameObject current = enumerator.Current;
					if (predicate(current))
					{
						current.Destroy(useDestroyImmediate, false);
					}
				}
			}

			public Enumerator GetEnumerator()
			{
				return (!(origin == null)) ? new Enumerator(origin.transform, withSelf, true) : new Enumerator(null, withSelf, false);
			}

			IEnumerator<GameObject> IEnumerable<GameObject>.GetEnumerator()
			{
				return GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			public void ForEach(Action<GameObject> action)
			{
				Enumerator enumerator = GetEnumerator();
				while (enumerator.MoveNext())
				{
					action(enumerator.Current);
				}
			}

			public int ToArrayNonAlloc(ref GameObject[] array)
			{
				int num = 0;
				Enumerator enumerator = GetEnumerator();
				while (enumerator.MoveNext())
				{
					GameObject current = enumerator.Current;
					if (array.Length == num)
					{
						int newSize = ((num != 0) ? (num * 2) : 4);
						Array.Resize(ref array, newSize);
					}
					array[num++] = current;
				}
				return num;
			}

			public int ToArrayNonAlloc(Func<GameObject, bool> filter, ref GameObject[] array)
			{
				int num = 0;
				Enumerator enumerator = GetEnumerator();
				while (enumerator.MoveNext())
				{
					GameObject current = enumerator.Current;
					if (filter(current))
					{
						if (array.Length == num)
						{
							int newSize = ((num != 0) ? (num * 2) : 4);
							Array.Resize(ref array, newSize);
						}
						array[num++] = current;
					}
				}
				return num;
			}

			public int ToArrayNonAlloc<T>(Func<GameObject, T> selector, ref T[] array)
			{
				int num = 0;
				Enumerator enumerator = GetEnumerator();
				while (enumerator.MoveNext())
				{
					GameObject current = enumerator.Current;
					if (array.Length == num)
					{
						int newSize = ((num != 0) ? (num * 2) : 4);
						Array.Resize(ref array, newSize);
					}
					array[num++] = selector(current);
				}
				return num;
			}

			public int ToArrayNonAlloc<T>(Func<GameObject, bool> filter, Func<GameObject, T> selector, ref T[] array)
			{
				int num = 0;
				Enumerator enumerator = GetEnumerator();
				while (enumerator.MoveNext())
				{
					GameObject current = enumerator.Current;
					if (filter(current))
					{
						if (array.Length == num)
						{
							int newSize = ((num != 0) ? (num * 2) : 4);
							Array.Resize(ref array, newSize);
						}
						array[num++] = selector(current);
					}
				}
				return num;
			}

			public int ToArrayNonAlloc<TState, T>(Func<GameObject, TState> let, Func<TState, bool> filter, Func<TState, T> selector, ref T[] array)
			{
				int num = 0;
				Enumerator enumerator = GetEnumerator();
				while (enumerator.MoveNext())
				{
					GameObject current = enumerator.Current;
					TState arg = let(current);
					if (filter(arg))
					{
						if (array.Length == num)
						{
							int newSize = ((num != 0) ? (num * 2) : 4);
							Array.Resize(ref array, newSize);
						}
						array[num++] = selector(arg);
					}
				}
				return num;
			}

			public GameObject[] ToArray()
			{
				GameObject[] array = new GameObject[4];
				int num = ToArrayNonAlloc(ref array);
				if (array.Length != num)
				{
					Array.Resize(ref array, num);
				}
				return array;
			}

			public GameObject[] ToArray(Func<GameObject, bool> filter)
			{
				GameObject[] array = new GameObject[4];
				int num = ToArrayNonAlloc(filter, ref array);
				if (array.Length != num)
				{
					Array.Resize(ref array, num);
				}
				return array;
			}

			public T[] ToArray<T>(Func<GameObject, T> selector)
			{
				T[] array = new T[4];
				int num = ToArrayNonAlloc(selector, ref array);
				if (array.Length != num)
				{
					Array.Resize(ref array, num);
				}
				return array;
			}

			public T[] ToArray<T>(Func<GameObject, bool> filter, Func<GameObject, T> selector)
			{
				T[] array = new T[4];
				int num = ToArrayNonAlloc(filter, selector, ref array);
				if (array.Length != num)
				{
					Array.Resize(ref array, num);
				}
				return array;
			}

			public T[] ToArray<TState, T>(Func<GameObject, TState> let, Func<TState, bool> filter, Func<TState, T> selector)
			{
				T[] array = new T[4];
				int num = ToArrayNonAlloc(let, filter, selector, ref array);
				if (array.Length != num)
				{
					Array.Resize(ref array, num);
				}
				return array;
			}

			public GameObject First()
			{
				Enumerator enumerator = GetEnumerator();
				if (enumerator.MoveNext())
				{
					return enumerator.Current;
				}
				throw new InvalidOperationException("sequence is empty.");
			}

			public GameObject FirstOrDefault()
			{
				Enumerator enumerator = GetEnumerator();
				return (!enumerator.MoveNext()) ? null : enumerator.Current;
			}
		}

		public static IEnumerable<GameObject> Ancestors(this IEnumerable<GameObject> source)
		{
			foreach (GameObject item in source)
			{
				AncestorsEnumerable.Enumerator e = item.Ancestors().GetEnumerator();
				while (e.MoveNext())
				{
					yield return e.Current;
				}
			}
		}

		public static IEnumerable<GameObject> AncestorsAndSelf(this IEnumerable<GameObject> source)
		{
			foreach (GameObject item in source)
			{
				AncestorsEnumerable.Enumerator e = item.AncestorsAndSelf().GetEnumerator();
				while (e.MoveNext())
				{
					yield return e.Current;
				}
			}
		}

		public static IEnumerable<GameObject> Descendants(this IEnumerable<GameObject> source, Func<Transform, bool> descendIntoChildren = null)
		{
			foreach (GameObject item in source)
			{
				DescendantsEnumerable.Enumerator e = item.Descendants(descendIntoChildren).GetEnumerator();
				while (e.MoveNext())
				{
					yield return e.Current;
				}
			}
		}

		public static IEnumerable<GameObject> DescendantsAndSelf(this IEnumerable<GameObject> source, Func<Transform, bool> descendIntoChildren = null)
		{
			foreach (GameObject item in source)
			{
				DescendantsEnumerable.Enumerator e = item.DescendantsAndSelf(descendIntoChildren).GetEnumerator();
				while (e.MoveNext())
				{
					yield return e.Current;
				}
			}
		}

		public static IEnumerable<GameObject> Children(this IEnumerable<GameObject> source)
		{
			foreach (GameObject item in source)
			{
				ChildrenEnumerable.Enumerator e = item.Children().GetEnumerator();
				while (e.MoveNext())
				{
					yield return e.Current;
				}
			}
		}

		public static IEnumerable<GameObject> ChildrenAndSelf(this IEnumerable<GameObject> source)
		{
			foreach (GameObject item in source)
			{
				ChildrenEnumerable.Enumerator e = item.ChildrenAndSelf().GetEnumerator();
				while (e.MoveNext())
				{
					yield return e.Current;
				}
			}
		}

		public static void Destroy(this IEnumerable<GameObject> source, bool useDestroyImmediate = false, bool detachParent = false)
		{
			if (detachParent)
			{
				List<GameObject> list = new List<GameObject>(source);
				List<GameObject>.Enumerator enumerator = list.GetEnumerator();
				while (enumerator.MoveNext())
				{
					enumerator.Current.Destroy(useDestroyImmediate, true);
				}
				return;
			}
			foreach (GameObject item in source)
			{
				item.Destroy(useDestroyImmediate);
			}
		}

		public static IEnumerable<T> OfComponent<T>(this IEnumerable<GameObject> source) where T : Component
		{
			foreach (GameObject item in source)
			{
				T component = item.GetComponent<T>();
				if (component != null)
				{
					yield return component;
				}
			}
		}

		public static int ToArrayNonAlloc<T>(this IEnumerable<T> source, ref T[] array)
		{
			int num = 0;
			foreach (T item in source)
			{
				if (array.Length == num)
				{
					int newSize = ((num != 0) ? (num * 2) : 4);
					Array.Resize(ref array, newSize);
				}
				array[num++] = item;
			}
			return num;
		}

		private static GameObject GetGameObject<T>(T obj) where T : UnityEngine.Object
		{
			GameObject gameObject = obj as GameObject;
			if (gameObject == null)
			{
				Component component = obj as Component;
				if (component == null)
				{
					return null;
				}
				gameObject = component.gameObject;
			}
			return gameObject;
		}

		public static T Add<T>(this GameObject parent, T childOriginal, TransformCloneType cloneType = TransformCloneType.KeepOriginal, bool? setActive = null, string specifiedName = null, bool setLayer = false) where T : UnityEngine.Object
		{
			if (parent == null)
			{
				throw new ArgumentNullException("parent");
			}
			if (childOriginal == null)
			{
				throw new ArgumentNullException("childOriginal");
			}
			T val = UnityEngine.Object.Instantiate(childOriginal);
			GameObject gameObject = GetGameObject(val);
			Transform transform = gameObject.transform;
			RectTransform rectTransform = transform as RectTransform;
			if (rectTransform != null)
			{
				rectTransform.SetParent(parent.transform, false);
			}
			else
			{
				Transform transform3 = (transform.parent = parent.transform);
				switch (cloneType)
				{
				case TransformCloneType.FollowParent:
					transform.localPosition = transform3.localPosition;
					transform.localScale = transform3.localScale;
					transform.localRotation = transform3.localRotation;
					break;
				case TransformCloneType.Origin:
					transform.localPosition = Vector3.zero;
					transform.localScale = Vector3.one;
					transform.localRotation = Quaternion.identity;
					break;
				case TransformCloneType.KeepOriginal:
				{
					GameObject gameObject2 = GetGameObject(childOriginal);
					Transform transform4 = gameObject2.transform;
					transform.localPosition = transform4.localPosition;
					transform.localScale = transform4.localScale;
					transform.localRotation = transform4.localRotation;
					break;
				}
				}
			}
			if (setLayer)
			{
				gameObject.layer = parent.layer;
			}
			if (setActive.HasValue)
			{
				gameObject.SetActive(setActive.Value);
			}
			if (specifiedName != null)
			{
				val.name = specifiedName;
			}
			return val;
		}

		public static T[] AddRange<T>(this GameObject parent, IEnumerable<T> childOriginals, TransformCloneType cloneType = TransformCloneType.KeepOriginal, bool? setActive = null, string specifiedName = null, bool setLayer = false) where T : UnityEngine.Object
		{
			if (parent == null)
			{
				throw new ArgumentNullException("parent");
			}
			if (childOriginals == null)
			{
				throw new ArgumentNullException("childOriginals");
			}
			T[] array = childOriginals as T[];
			if (array != null)
			{
				T[] array2 = new T[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					T val = parent.Add(array[i], cloneType, setActive, specifiedName, setLayer);
					array2[i] = val;
				}
				return array2;
			}
			IList<T> list = childOriginals as IList<T>;
			if (list != null)
			{
				T[] array3 = new T[list.Count];
				for (int j = 0; j < list.Count; j++)
				{
					T val2 = parent.Add(list[j], cloneType, setActive, specifiedName, setLayer);
					array3[j] = val2;
				}
				return array3;
			}
			List<T> list2 = new List<T>();
			foreach (T childOriginal in childOriginals)
			{
				T item = parent.Add(childOriginal, cloneType, setActive, specifiedName, setLayer);
				list2.Add(item);
			}
			return list2.ToArray();
		}

		public static T AddFirst<T>(this GameObject parent, T childOriginal, TransformCloneType cloneType = TransformCloneType.KeepOriginal, bool? setActive = null, string specifiedName = null, bool setLayer = false) where T : UnityEngine.Object
		{
			T val = parent.Add(childOriginal, cloneType, setActive, specifiedName, setLayer);
			GameObject gameObject = GetGameObject(val);
			if (gameObject == null)
			{
				return val;
			}
			gameObject.transform.SetAsFirstSibling();
			return val;
		}

		public static T[] AddFirstRange<T>(this GameObject parent, IEnumerable<T> childOriginals, TransformCloneType cloneType = TransformCloneType.KeepOriginal, bool? setActive = null, string specifiedName = null, bool setLayer = false) where T : UnityEngine.Object
		{
			T[] array = parent.AddRange(childOriginals, cloneType, setActive, specifiedName, setLayer);
			for (int num = array.Length - 1; num >= 0; num--)
			{
				GameObject gameObject = GetGameObject(array[num]);
				if (!(gameObject == null))
				{
					gameObject.transform.SetAsFirstSibling();
				}
			}
			return array;
		}

		public static T AddBeforeSelf<T>(this GameObject parent, T childOriginal, TransformCloneType cloneType = TransformCloneType.KeepOriginal, bool? setActive = null, string specifiedName = null, bool setLayer = false) where T : UnityEngine.Object
		{
			GameObject gameObject = parent.Parent();
			if (gameObject == null)
			{
				throw new InvalidOperationException("The parent root is null");
			}
			int siblingIndex = parent.transform.GetSiblingIndex();
			T val = gameObject.Add(childOriginal, cloneType, setActive, specifiedName, setLayer);
			GameObject gameObject2 = GetGameObject(val);
			if (gameObject2 == null)
			{
				return val;
			}
			gameObject2.transform.SetSiblingIndex(siblingIndex);
			return val;
		}

		public static T[] AddBeforeSelfRange<T>(this GameObject parent, IEnumerable<T> childOriginals, TransformCloneType cloneType = TransformCloneType.KeepOriginal, bool? setActive = null, string specifiedName = null, bool setLayer = false) where T : UnityEngine.Object
		{
			GameObject gameObject = parent.Parent();
			if (gameObject == null)
			{
				throw new InvalidOperationException("The parent root is null");
			}
			int siblingIndex = parent.transform.GetSiblingIndex();
			T[] array = gameObject.AddRange(childOriginals, cloneType, setActive, specifiedName, setLayer);
			for (int num = array.Length - 1; num >= 0; num--)
			{
				GameObject gameObject2 = GetGameObject(array[num]);
				if (!(gameObject2 == null))
				{
					gameObject2.transform.SetSiblingIndex(siblingIndex);
				}
			}
			return array;
		}

		public static T AddAfterSelf<T>(this GameObject parent, T childOriginal, TransformCloneType cloneType = TransformCloneType.KeepOriginal, bool? setActive = null, string specifiedName = null, bool setLayer = false) where T : UnityEngine.Object
		{
			GameObject gameObject = parent.Parent();
			if (gameObject == null)
			{
				throw new InvalidOperationException("The parent root is null");
			}
			int siblingIndex = parent.transform.GetSiblingIndex() + 1;
			T val = gameObject.Add(childOriginal, cloneType, setActive, specifiedName, setLayer);
			GameObject gameObject2 = GetGameObject(val);
			if (gameObject2 == null)
			{
				return val;
			}
			gameObject2.transform.SetSiblingIndex(siblingIndex);
			return val;
		}

		public static T[] AddAfterSelfRange<T>(this GameObject parent, IEnumerable<T> childOriginals, TransformCloneType cloneType = TransformCloneType.KeepOriginal, bool? setActive = null, string specifiedName = null, bool setLayer = false) where T : UnityEngine.Object
		{
			GameObject gameObject = parent.Parent();
			if (gameObject == null)
			{
				throw new InvalidOperationException("The parent root is null");
			}
			int siblingIndex = parent.transform.GetSiblingIndex() + 1;
			T[] array = gameObject.AddRange(childOriginals, cloneType, setActive, specifiedName, setLayer);
			for (int num = array.Length - 1; num >= 0; num--)
			{
				GameObject gameObject2 = GetGameObject(array[num]);
				if (!(gameObject2 == null))
				{
					gameObject2.transform.SetSiblingIndex(siblingIndex);
				}
			}
			return array;
		}

		public static T MoveToLast<T>(this GameObject parent, T child, TransformMoveType moveType = TransformMoveType.DoNothing, bool? setActive = null, bool setLayer = false) where T : UnityEngine.Object
		{
			if (parent == null)
			{
				throw new ArgumentNullException("parent");
			}
			if (child == null)
			{
				throw new ArgumentNullException("child");
			}
			GameObject gameObject = GetGameObject(child);
			if (child == null)
			{
				return child;
			}
			Transform transform = gameObject.transform;
			RectTransform rectTransform = transform as RectTransform;
			if (rectTransform != null)
			{
				rectTransform.SetParent(parent.transform, false);
			}
			else
			{
				Transform transform3 = (transform.parent = parent.transform);
				switch (moveType)
				{
				case TransformMoveType.FollowParent:
					transform.localPosition = transform3.localPosition;
					transform.localScale = transform3.localScale;
					transform.localRotation = transform3.localRotation;
					break;
				case TransformMoveType.Origin:
					transform.localPosition = Vector3.zero;
					transform.localScale = Vector3.one;
					transform.localRotation = Quaternion.identity;
					break;
				}
			}
			if (setLayer)
			{
				gameObject.layer = parent.layer;
			}
			if (setActive.HasValue)
			{
				gameObject.SetActive(setActive.Value);
			}
			return child;
		}

		public static T[] MoveToLastRange<T>(this GameObject parent, IEnumerable<T> childs, TransformMoveType moveType = TransformMoveType.DoNothing, bool? setActive = null, bool setLayer = false) where T : UnityEngine.Object
		{
			if (parent == null)
			{
				throw new ArgumentNullException("parent");
			}
			if (childs == null)
			{
				throw new ArgumentNullException("childs");
			}
			T[] array = childs as T[];
			if (array != null)
			{
				T[] array2 = new T[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					T val = parent.MoveToLast(array[i], moveType, setActive, setLayer);
					array2[i] = val;
				}
				return array2;
			}
			IList<T> list = childs as IList<T>;
			if (list != null)
			{
				T[] array3 = new T[list.Count];
				for (int j = 0; j < list.Count; j++)
				{
					T val2 = parent.MoveToLast(list[j], moveType, setActive, setLayer);
					array3[j] = val2;
				}
				return array3;
			}
			List<T> list2 = new List<T>();
			foreach (T child in childs)
			{
				T item = parent.MoveToLast(child, moveType, setActive, setLayer);
				list2.Add(item);
			}
			return list2.ToArray();
		}

		public static T MoveToFirst<T>(this GameObject parent, T child, TransformMoveType moveType = TransformMoveType.DoNothing, bool? setActive = null, bool setLayer = false) where T : UnityEngine.Object
		{
			parent.MoveToLast(child, moveType, setActive, setLayer);
			GameObject gameObject = GetGameObject(child);
			if (gameObject == null)
			{
				return child;
			}
			gameObject.transform.SetAsFirstSibling();
			return child;
		}

		public static T[] MoveToFirstRange<T>(this GameObject parent, IEnumerable<T> childs, TransformMoveType moveType = TransformMoveType.DoNothing, bool? setActive = null, bool setLayer = false) where T : UnityEngine.Object
		{
			T[] array = parent.MoveToLastRange(childs, moveType, setActive, setLayer);
			for (int num = array.Length - 1; num >= 0; num--)
			{
				GameObject gameObject = GetGameObject(array[num]);
				if (!(gameObject == null))
				{
					gameObject.transform.SetAsFirstSibling();
				}
			}
			return array;
		}

		public static T MoveToBeforeSelf<T>(this GameObject parent, T child, TransformMoveType moveType = TransformMoveType.DoNothing, bool? setActive = null, bool setLayer = false) where T : UnityEngine.Object
		{
			GameObject gameObject = parent.Parent();
			if (gameObject == null)
			{
				throw new InvalidOperationException("The parent root is null");
			}
			int siblingIndex = parent.transform.GetSiblingIndex();
			gameObject.MoveToLast(child, moveType, setActive, setLayer);
			GameObject gameObject2 = GetGameObject(child);
			if (gameObject2 == null)
			{
				return child;
			}
			gameObject2.transform.SetSiblingIndex(siblingIndex);
			return child;
		}

		public static T[] MoveToBeforeSelfRange<T>(this GameObject parent, IEnumerable<T> childs, TransformMoveType moveType = TransformMoveType.DoNothing, bool? setActive = null, bool setLayer = false) where T : UnityEngine.Object
		{
			GameObject gameObject = parent.Parent();
			if (gameObject == null)
			{
				throw new InvalidOperationException("The parent root is null");
			}
			int siblingIndex = parent.transform.GetSiblingIndex();
			T[] array = gameObject.MoveToLastRange(childs, moveType, setActive, setLayer);
			for (int num = array.Length - 1; num >= 0; num--)
			{
				GameObject gameObject2 = GetGameObject(array[num]);
				if (!(gameObject2 == null))
				{
					gameObject2.transform.SetSiblingIndex(siblingIndex);
				}
			}
			return array;
		}

		public static T MoveToAfterSelf<T>(this GameObject parent, T child, TransformMoveType moveType = TransformMoveType.DoNothing, bool? setActive = null, bool setLayer = false) where T : UnityEngine.Object
		{
			GameObject gameObject = parent.Parent();
			if (gameObject == null)
			{
				throw new InvalidOperationException("The parent root is null");
			}
			int siblingIndex = parent.transform.GetSiblingIndex() + 1;
			gameObject.MoveToLast(child, moveType, setActive, setLayer);
			GameObject gameObject2 = GetGameObject(child);
			if (gameObject2 == null)
			{
				return child;
			}
			gameObject2.transform.SetSiblingIndex(siblingIndex);
			return child;
		}

		public static T[] MoveToAfterSelfRange<T>(this GameObject parent, IEnumerable<T> childs, TransformMoveType moveType = TransformMoveType.DoNothing, bool? setActive = null, bool setLayer = false) where T : UnityEngine.Object
		{
			GameObject gameObject = parent.Parent();
			if (gameObject == null)
			{
				throw new InvalidOperationException("The parent root is null");
			}
			int siblingIndex = parent.transform.GetSiblingIndex() + 1;
			T[] array = gameObject.MoveToLastRange(childs, moveType, setActive, setLayer);
			for (int num = array.Length - 1; num >= 0; num--)
			{
				GameObject gameObject2 = GetGameObject(array[num]);
				if (!(gameObject2 == null))
				{
					gameObject2.transform.SetSiblingIndex(siblingIndex);
				}
			}
			return array;
		}

		public static void Destroy(this GameObject self, bool useDestroyImmediate = false, bool detachParent = false)
		{
			if (!(self == null))
			{
				if (detachParent)
				{
					self.transform.SetParent(null);
				}
				if (useDestroyImmediate)
				{
					UnityEngine.Object.DestroyImmediate(self);
				}
				else
				{
					UnityEngine.Object.Destroy(self);
				}
			}
		}

		public static GameObject Parent(this GameObject origin)
		{
			if (origin == null)
			{
				return null;
			}
			Transform parent = origin.transform.parent;
			if (parent == null)
			{
				return null;
			}
			return parent.gameObject;
		}

		public static GameObject Child(this GameObject origin, string name)
		{
			if (origin == null)
			{
				return null;
			}
			Transform transform = origin.transform.Find(name);
			if (transform == null)
			{
				return null;
			}
			return transform.gameObject;
		}

		public static ChildrenEnumerable Children(this GameObject origin)
		{
			return new ChildrenEnumerable(origin, false);
		}

		public static ChildrenEnumerable ChildrenAndSelf(this GameObject origin)
		{
			return new ChildrenEnumerable(origin, true);
		}

		public static AncestorsEnumerable Ancestors(this GameObject origin)
		{
			return new AncestorsEnumerable(origin, false);
		}

		public static AncestorsEnumerable AncestorsAndSelf(this GameObject origin)
		{
			return new AncestorsEnumerable(origin, true);
		}

		public static DescendantsEnumerable Descendants(this GameObject origin, Func<Transform, bool> descendIntoChildren = null)
		{
			return new DescendantsEnumerable(origin, false, descendIntoChildren);
		}

		public static DescendantsEnumerable DescendantsAndSelf(this GameObject origin, Func<Transform, bool> descendIntoChildren = null)
		{
			return new DescendantsEnumerable(origin, true, descendIntoChildren);
		}

		public static BeforeSelfEnumerable BeforeSelf(this GameObject origin)
		{
			return new BeforeSelfEnumerable(origin, false);
		}

		public static BeforeSelfEnumerable BeforeSelfAndSelf(this GameObject origin)
		{
			return new BeforeSelfEnumerable(origin, true);
		}

		public static AfterSelfEnumerable AfterSelf(this GameObject origin)
		{
			return new AfterSelfEnumerable(origin, false);
		}

		public static AfterSelfEnumerable AfterSelfAndSelf(this GameObject origin)
		{
			return new AfterSelfEnumerable(origin, true);
		}
	}
}
