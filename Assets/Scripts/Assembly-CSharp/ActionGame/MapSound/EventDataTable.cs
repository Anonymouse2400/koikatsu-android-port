using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ActionGame.MapSound
{
	[Serializable]
	public class EventDataTable
	{
		[SerializeField]
		private EventData[] _events;

		public IEnumerable<EventData> Events
		{
			get
			{
				return _events;
			}
			set
			{
				_events = value.ToArray();
			}
		}
	}
}
