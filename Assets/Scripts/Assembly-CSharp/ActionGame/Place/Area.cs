using Illusion.Component;
using UnityEngine;

namespace ActionGame.Place
{
	public class Area : TriggerEnterExitEvent
	{
		[SerializeField]
		private int _mapNo;

		[SerializeField]
		private string[] _names = new string[0];

		public int mapNo
		{
			get
			{
				return _mapNo;
			}
		}

		public string[] names
		{
			get
			{
				return _names;
			}
		}

		protected override void Awake()
		{
			base.Awake();
		}

		private void Start()
		{
			base.onTriggerEnter += delegate(Collider other)
			{
				IHitable component = other.GetComponent<IHitable>();
				if (component != null)
				{
					component.Enter(this);
				}
			};
			base.onTriggerExit += delegate(Collider other)
			{
				IHitable component2 = other.GetComponent<IHitable>();
				if (component2 != null)
				{
					component2.Exit(this);
				}
			};
		}
	}
}
