using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Illusion.Elements.Culling
{
	public class CullingList<T> : List<T> where T : Transform
	{
		protected int activeFalseLevel;

		protected CullingGroup group;

		protected BoundingSphere[] bounds;

		protected float radius = 1.5f;

		public CullingList(Camera camera, float[] distances, int activeFalseLevel = int.MaxValue)
		{
			group = new CullingGroup();
			ChangeCamera(camera);
			SetDistanceLevel(distances);
			SetActiveFalseLevel(activeFalseLevel);
			group.onStateChanged = OnChange;
		}

		public void Release()
		{
			CullingGroup cullingGroup = group;
			cullingGroup.onStateChanged = (CullingGroup.StateChanged)Delegate.Remove(cullingGroup.onStateChanged, new CullingGroup.StateChanged(OnChange));
			group.Dispose();
			group = null;
		}

		public void Update()
		{
			for (int i = 0; i < base.Count; i++)
			{
				 BoundingSphere reference =  bounds[i];
				T val = base[i];
				reference.position = val.position;
			}
		}

		public void ChangeCamera(Camera camera)
		{
			group.targetCamera = camera;
			group.SetDistanceReferencePoint(camera.transform);
		}

		public void SetDistanceLevel(float[] distances)
		{
			group.SetBoundingDistances(distances);
		}

		public void SetActiveFalseLevel(int activeFalseLevel)
		{
			this.activeFalseLevel = activeFalseLevel;
		}

		public void SetRadius(float radius)
		{
			this.radius = radius;
			Set();
		}

		public new void Add(T item)
		{
			base.Add(item);
			Set();
		}

		public new void Remove(T item)
		{
			if (base.Remove(item))
			{
				Set();
			}
		}

		private void Set()
		{
			bounds = new BoundingSphere[base.Count];
			for (int i = 0; i < bounds.Length; i++)
			{
				bounds[i].radius = radius;
			}
			group.SetBoundingSpheres(bounds);
			group.SetBoundingSphereCount(base.Count);
		}

		protected virtual void OnChange(CullingGroupEvent ev)
		{
			bool flag = ev.isVisible;
			if (flag && ev.currentDistance >= activeFalseLevel)
			{
				flag = false;
			}
			T val = base[ev.index];
			GameObject gameObject = val.gameObject;
			ExecuteEvents.Execute(gameObject, null, delegate(ICullingListRecieveMessage recieve, BaseEventData y)
			{
				recieve.CullingGroupEvent(ev);
			});
			if (gameObject.activeSelf == flag)
			{
				return;
			}
			if (!flag)
			{
				ExecuteEvents.Execute(gameObject, null, delegate(ICullingListRecieveMessage recieve, BaseEventData y)
				{
					recieve.CullingGroupEventChangeFalse(ev);
				});
			}
			gameObject.SetActive(flag);
			if (flag)
			{
				ExecuteEvents.Execute(gameObject, null, delegate(ICullingListRecieveMessage recieve, BaseEventData y)
				{
					recieve.CullingGroupEventChangeTrue(ev);
				});
			}
		}
	}
}
