using System.Linq;
using UnityEngine;

public class AssetBundleLoadAssetOperationSimulation : AssetBundleLoadAssetOperation
{
	private Object[] m_SimulatedObjects;

	public AssetBundleLoadAssetOperationSimulation(Object simulatedObject)
	{
		m_SimulatedObjects = new Object[1] { simulatedObject };
	}

	public AssetBundleLoadAssetOperationSimulation(Object[] simulatedObjects)
	{
		m_SimulatedObjects = simulatedObjects;
	}

	public override bool IsEmpty()
	{
		return m_SimulatedObjects == null || m_SimulatedObjects.Length == 0 || m_SimulatedObjects[0] == null;
	}

	public override T GetAsset<T>()
	{
		return m_SimulatedObjects[0] as T;
	}

	public override T[] GetAllAssets<T>()
	{
		return m_SimulatedObjects.OfType<T>().ToArray();
	}

	public override bool Update()
	{
		return false;
	}

	public override bool IsDone()
	{
		return true;
	}
}
