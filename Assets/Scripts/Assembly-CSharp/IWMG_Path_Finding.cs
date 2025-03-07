using System.Collections.Generic;

public interface IWMG_Path_Finding
{
	List<WMG_Link> FindShortestPathBetweenNodes(WMG_Node fromNode, WMG_Node toNode);

	List<WMG_Link> FindShortestPathBetweenNodesWeighted(WMG_Node fromNode, WMG_Node toNode, bool includeRadii);
}
