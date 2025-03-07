using System;
using UniRx;

namespace FileListUI
{
	[Serializable]
	internal class TypeReactiveProperty : ReactiveProperty<VisibleType>
	{
		public TypeReactiveProperty()
		{
		}

		public TypeReactiveProperty(VisibleType initialValue)
			: base(initialValue)
		{
		}
	}
}
