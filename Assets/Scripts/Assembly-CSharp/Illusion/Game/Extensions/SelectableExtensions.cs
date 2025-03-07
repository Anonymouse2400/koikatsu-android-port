using System;
using System.Collections.Generic;
using System.Linq;
using Illusion.Component.UI;
using Illusion.Extensions;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Illusion.Game.Extensions
{
	public static class SelectableExtensions
	{
		public static IObservable<IList<double>> DoubleInterval<T>(this IObservable<T> source, float interval, bool isHot = true)
		{
			return isHot ? CreateDoubleIntervalStream(source, interval).Share() : CreateDoubleIntervalStream(source, interval);
		}

		public static IConnectableObservable<IList<double>> DoubleIntervalPublish<T>(this IObservable<T> source, float interval)
		{
			return CreateDoubleIntervalStream(source, interval).Publish();
		}

		private static IObservable<IList<double>> CreateDoubleIntervalStream<T>(IObservable<T> source, float interval)
		{
			return from list in (from t in source.TimeInterval()
					select t.Interval.TotalMilliseconds).Buffer(2, 1)
				where list[0] > (double)interval
				where list[1] <= (double)interval
				select list;
		}

		public static IDisposable SubscribeToText(this IObservable<string> source, TextMeshProUGUI text)
		{
			return source.SubscribeWithState(text, delegate(string x, TextMeshProUGUI t)
			{
				t.text = x;
			});
		}

		public static List<Tuple<Selectable, SelectUI>> SelectSEAdd<T>(this T _, params Selectable[] selectables)
		{
			return selectables.Select(SelectSE).ToList();
		}

		public static Tuple<Selectable, SelectUI> SelectSE(this Selectable selectable)
		{
			SelectUI selectUI = selectable.GetOrAddComponent<SelectUI>();
			(from sel in selectable.ObserveEveryValueChanged((Selectable _) => selectUI.isSelect && selectable.IsInteractable()).TakeUntilDestroy(selectable)
				where sel
				select sel).Subscribe(delegate
			{
				Utils.Sound.Play(SystemSE.sel);
			});
			return Tuple.Create(selectable, selectUI);
		}

		public static void FocusAdd(this UnityEngine.Component component, bool isTabKey, Func<bool> isFocus, Func<int> next, Func<Tuple<bool, int>> direct, int firstIndex, params Selectable[] sels)
		{
			if (sels.IsNullOrEmpty())
			{
				return;
			}
			Selectable lastCurrent = sels.SafeGet(firstIndex);
			(from go in component.ObserveEveryValueChanged((UnityEngine.Component _) => EventSystem.current.currentSelectedGameObject)
				select (!(go != null)) ? null : go.GetComponent<Selectable>()).Subscribe(delegate(Selectable sel)
			{
				if (sels.Contains(sel))
				{
					lastCurrent = sel;
				}
				else if (isFocus() && lastCurrent != null)
				{
					lastCurrent.Select();
				}
			});
			Action<int, bool> focus = delegate(int index, bool isDirect)
			{
				bool flag = index >= 0;
				if (!isDirect)
				{
					index += sels.Check((Selectable v) => v == lastCurrent);
				}
				MathfEx.LoopValue(ref index, 0, sels.Length - 1);
				if (!sels[index].IsInteractable() && sels.Any((Selectable p) => p.IsInteractable()))
				{
					if (!flag)
					{
						index = Mathf.Min(sels.Length, index + 1);
					}
					IEnumerable<int> enumerable = Enumerable.Range(index, sels.Length - index);
					IEnumerable<int> enumerable2 = Enumerable.Range(0, index);
					index = ((!flag) ? enumerable2.Reverse().Concat(enumerable.Reverse()) : enumerable.Concat(enumerable2)).FirstOrDefault((int i) => sels[i].IsInteractable());
				}
				sels[index].Select();
			};
			if (isTabKey)
			{
				(from _ in component.UpdateAsObservable()
					where isFocus()
					where Input.GetKeyDown(KeyCode.Tab)
					select _).Subscribe(delegate
				{
					focus((!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift)) ? 1 : (-1), false);
				});
			}
			if (!next.IsNullOrEmpty())
			{
				(from _ in component.UpdateAsObservable()
					where isFocus()
					select _).Subscribe(delegate
				{
					int num = next();
					if (num != 0)
					{
						focus(num, false);
					}
				});
			}
			if (direct.IsNullOrEmpty())
			{
				return;
			}
			(from _ in component.UpdateAsObservable()
				where isFocus()
				select _).Subscribe(delegate
			{
				Tuple<bool, int> tuple = direct();
				if (tuple.Item1)
				{
					focus(tuple.Item2, true);
				}
			});
		}
	}
}
