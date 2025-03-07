namespace Studio
{
	public static class AddObjectCommand
	{
		public class AddItemCommand : ICommand
		{
			private int group = -1;

			private int category = -1;

			private int no = -1;

			private int dicKey = -1;

			private int initialPosition;

			private TreeNodeObject tno;

			public AddItemCommand(int _group, int _category, int _no, int _dicKey, int _initialPosition)
			{
				group = _group;
				category = _category;
				no = _no;
				dicKey = _dicKey;
				initialPosition = _initialPosition;
			}

			public void Do()
			{
				OCIItem oCIItem = AddObjectItem.Load(new OIItemInfo(group, category, no, dicKey), null, null, true, initialPosition);
				tno = ((oCIItem == null) ? null : oCIItem.treeNodeObject);
			}

			public void Undo()
			{
				Studio.DeleteNode(tno);
				tno = null;
			}

			public void Redo()
			{
				Do();
				Studio.SetNewIndex(dicKey);
			}
		}

		public class AddLightCommand : ICommand
		{
			private int no = -1;

			private int dicKey = -1;

			private int initialPosition;

			private TreeNodeObject tno;

			public AddLightCommand(int _no, int _dicKey, int _initialPosition)
			{
				no = _no;
				dicKey = _dicKey;
				initialPosition = _initialPosition;
			}

			public void Do()
			{
				OCILight oCILight = AddObjectLight.Load(new OILightInfo(no, dicKey), null, null, true, initialPosition);
				tno = ((oCILight == null) ? null : oCILight.treeNodeObject);
			}

			public void Undo()
			{
				Studio.DeleteNode(tno);
				tno = null;
			}

			public void Redo()
			{
				Do();
				Studio.SetNewIndex(dicKey);
			}
		}
	}
}
