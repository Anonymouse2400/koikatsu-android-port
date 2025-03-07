using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Unity.Linq.Sample
{
	public class SampleSceneScript : MonoBehaviour
	{
		private GameObject[] array = new GameObject[0];

		private void OnGUI()
		{
			GameObject gameObject = GameObject.Find("Origin");
			if (GUILayout.Button("Parent"))
			{
				GameObject gameObject2 = gameObject.Parent();
			}
			if (GUILayout.Button("Child"))
			{
				GameObject gameObject3 = gameObject.Child("Sphere_B");
			}
			using (new GUILayout.HorizontalScope())
			{
				if (GUILayout.Button("Ancestors"))
				{
					foreach (GameObject item in gameObject.Ancestors())
					{
					}
				}
				if (GUILayout.Button("AncestorsAndSelf"))
				{
					foreach (GameObject item2 in gameObject.AncestorsAndSelf())
					{
					}
				}
			}
			using (new GUILayout.HorizontalScope())
			{
				if (GUILayout.Button("Children"))
				{
					foreach (GameObject item3 in gameObject.Children())
					{
					}
				}
				if (GUILayout.Button("ChildrenAndSelf"))
				{
					foreach (GameObject item4 in gameObject.ChildrenAndSelf())
					{
					}
				}
			}
			using (new GUILayout.HorizontalScope())
			{
				if (GUILayout.Button("Descendants"))
				{
					foreach (GameObject item5 in gameObject.Descendants())
					{
					}
				}
				if (GUILayout.Button("DescendantsAndSelf"))
				{
					foreach (GameObject item6 in gameObject.DescendantsAndSelf())
					{
					}
				}
			}
			using (new GUILayout.HorizontalScope())
			{
				if (GUILayout.Button("Descendants:DescendIntoChildren"))
				{
					foreach (GameObject item7 in gameObject.Descendants((Transform x) => x.name != "Group"))
					{
					}
				}
			}
			using (new GUILayout.HorizontalScope())
			{
				if (GUILayout.Button("BeforeSelf"))
				{
					foreach (GameObject item8 in gameObject.BeforeSelf())
					{
					}
				}
				if (GUILayout.Button("BeforeSelfAndSelf"))
				{
					foreach (GameObject item9 in gameObject.BeforeSelfAndSelf())
					{
					}
				}
			}
			using (new GUILayout.HorizontalScope())
			{
				if (GUILayout.Button("AfterSelf"))
				{
					foreach (GameObject item10 in gameObject.AfterSelf())
					{
					}
				}
				if (GUILayout.Button("AfterSelf"))
				{
					foreach (GameObject item11 in gameObject.AfterSelfAndSelf())
					{
					}
				}
			}
			if (GUILayout.Button("OfComponent"))
			{
				foreach (SphereCollider item12 in gameObject.Descendants().OfComponent<SphereCollider>())
				{
				}
			}
			if (GUILayout.Button("LINQ"))
			{
				IEnumerable<GameObject> enumerable = from x in gameObject.Children()
					where x.name.EndsWith("B")
					select x;
				foreach (GameObject item13 in enumerable)
				{
				}
			}
			if (GUILayout.Button("Add"))
			{
				gameObject.Add(new GameObject("lastChild0"), TransformCloneType.KeepOriginal, null);
				gameObject.AddRange(new GameObject[3]
				{
					new GameObject("lastChild1"),
					new GameObject("lastChild2"),
					new GameObject("lastChild3")
				}, TransformCloneType.KeepOriginal, null);
				gameObject.AddFirstRange(new GameObject[3]
				{
					new GameObject("firstChild1"),
					new GameObject("firstChild2"),
					new GameObject("firstChild3")
				}, TransformCloneType.KeepOriginal, null);
				gameObject.AddFirst(new GameObject("firstChild0"), TransformCloneType.KeepOriginal, null);
				gameObject.AddBeforeSelf(new GameObject("beforeSelf0"), TransformCloneType.KeepOriginal, null);
				gameObject.AddBeforeSelfRange(new GameObject[3]
				{
					new GameObject("beforeSelf1"),
					new GameObject("beforeSelf2"),
					new GameObject("beforeSelf3")
				}, TransformCloneType.KeepOriginal, null);
				gameObject.AddAfterSelfRange(new GameObject[3]
				{
					new GameObject("afterSelf1"),
					new GameObject("afterSelf2"),
					new GameObject("afterSelf3")
				}, TransformCloneType.KeepOriginal, null);
				gameObject.AddAfterSelf(new GameObject("afterSelf0"), TransformCloneType.KeepOriginal, null);
				(from GameObject x in Resources.FindObjectsOfTypeAll<GameObject>()
					where x.Parent() == null && !x.name.Contains("Camera") && x.name != "Root" && x.name != string.Empty && x.name != "HandlesGO" && !x.name.StartsWith("Scene") && !x.name.Contains("Light") && !x.name.Contains("Materials")
					select x).Destroy();
			}
			if (GUILayout.Button("MoveTo"))
			{
				gameObject.MoveToLast(new GameObject("lastChild0(Original)"), TransformMoveType.DoNothing, null);
				gameObject.MoveToLastRange(new GameObject[3]
				{
					new GameObject("lastChild1(Original)"),
					new GameObject("lastChild2(Original)"),
					new GameObject("lastChild3(Original)")
				}, TransformMoveType.DoNothing, null);
				gameObject.MoveToFirstRange(new GameObject[3]
				{
					new GameObject("firstChild1(Original)"),
					new GameObject("firstChild2(Original)"),
					new GameObject("firstChild3(Original)")
				}, TransformMoveType.DoNothing, null);
				gameObject.MoveToFirst(new GameObject("firstChild0(Original)"), TransformMoveType.DoNothing, null);
				gameObject.MoveToBeforeSelf(new GameObject("beforeSelf0(Original)"), TransformMoveType.DoNothing, null);
				gameObject.MoveToBeforeSelfRange(new GameObject[3]
				{
					new GameObject("beforeSelf1(Original)"),
					new GameObject("beforeSelf2(Original)"),
					new GameObject("beforeSelf3(Original)")
				}, TransformMoveType.DoNothing, null);
				gameObject.MoveToAfterSelfRange(new GameObject[3]
				{
					new GameObject("afterSelf1(Original)"),
					new GameObject("afterSelf2(Original)"),
					new GameObject("afterSelf3(Original)")
				}, TransformMoveType.DoNothing, null);
				gameObject.MoveToAfterSelf(new GameObject("afterSelf0(Original)"), TransformMoveType.DoNothing, null);
			}
			if (GUILayout.Button("Destroy"))
			{
				(from x in gameObject.transform.root.gameObject.Descendants()
					where x.name.EndsWith("(Clone)") || x.name.EndsWith("(Original)")
					select x).Destroy();
			}
			using (new GUILayout.HorizontalScope())
			{
				if (GUILayout.Button("ToArrayNonAlloc"))
				{
					int num = gameObject.Children().ToArrayNonAlloc(ref array);
					for (int i = 0; i < num; i++)
					{
					}
				}
				if (GUILayout.Button("ToArrayNonAlloc(with filter)"))
				{
					int num2 = gameObject.Children().ToArrayNonAlloc((GameObject x) => x.name.EndsWith("B"), ref array);
					for (int j = 0; j < num2; j++)
					{
					}
				}
			}
		}
	}
}
