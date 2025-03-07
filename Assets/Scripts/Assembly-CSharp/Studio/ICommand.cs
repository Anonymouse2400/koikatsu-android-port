namespace Studio
{
	public interface ICommand
	{
		void Do();

		void Undo();

		void Redo();
	}
}
