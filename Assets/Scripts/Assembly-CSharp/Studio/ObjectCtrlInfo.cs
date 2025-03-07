namespace Studio
{
	public abstract class ObjectCtrlInfo
	{
		public ObjectInfo objectInfo;

		public TreeNodeObject treeNodeObject;

		public GuideObject guideObject;

		public ObjectCtrlInfo parentInfo;

		public int kind
		{
			get
			{
				return (objectInfo == null) ? (-1) : objectInfo.kind;
			}
		}

		public int[] kinds
		{
			get
			{
				return (objectInfo != null) ? objectInfo.kinds : new int[1] { -1 };
			}
		}

		public virtual float animeSpeed { get; set; }

		public virtual ObjectCtrlInfo this[int _idx]
		{
			get
			{
				return (_idx != 0) ? parentInfo : this;
			}
		}

		public abstract void OnDelete();

		public abstract void OnAttach(TreeNodeObject _parent, ObjectCtrlInfo _child);

		public abstract void OnDetach();

		public abstract void OnDetachChild(ObjectCtrlInfo _child);

		public abstract void OnSelect(bool _select);

		public abstract void OnLoadAttach(TreeNodeObject _parent, ObjectCtrlInfo _child);

		public abstract void OnVisible(bool _visible);

		public virtual void OnSavePreprocessing()
		{
			if (objectInfo != null && treeNodeObject != null)
			{
				objectInfo.treeState = treeNodeObject.treeState;
			}
			if (objectInfo != null && treeNodeObject != null)
			{
				objectInfo.visible = treeNodeObject.visible;
			}
		}
	}
}
