using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace Illusion.Component.UI
{
	[RequireComponent(typeof(MouseButtonCheck))]
	public class RepeatButton : SelectUI, ISubmitHandler, IEventSystemHandler
	{
		public UnityEvent call = new UnityEvent();

		private bool push;

		private ActionScene scene;

		protected virtual void Process(bool push)
		{
			if (push && base.isSelect)
			{
				call.Invoke();
			}
		}

		public void OnSubmit(BaseEventData eventData)
		{
			Process(true);
		}

		protected virtual void Awake()
		{
			push = false;
		}

		private void Start()
		{
			MouseButtonCheck component = GetComponent<MouseButtonCheck>();
			component.onPointerDown.AddListener(delegate
			{
				push = true;
			});
			component.onPointerUp.AddListener(delegate
			{
				push = false;
			});
		}

		private void Update()
		{
			Process(push);
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			push = false;
		}
	}
}
