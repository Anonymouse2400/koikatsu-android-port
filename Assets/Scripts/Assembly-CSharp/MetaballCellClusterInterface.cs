public interface MetaballCellClusterInterface
{
	float BaseRadius { get; }

	int CellCount { get; }

	void DoForeachCell(ForeachCellDeleg deleg);
}
