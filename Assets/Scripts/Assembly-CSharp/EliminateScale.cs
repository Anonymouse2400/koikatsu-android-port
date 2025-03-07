using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class EliminateScale : MonoBehaviour
{
	public enum EliminateScaleKind
	{
		ALL = 0,
		X = 1,
		Y = 2,
		Z = 3,
		XY = 4,
		XZ = 5,
		YZ = 6,
		NONE = 7
	}

	[Serializable]
	public class ShapeMove
	{
		public int numShape;

		public Vector3 posMax;

		public Vector3 posMin;

		public Vector3 rotMax;

		public Vector3 rotMin;
	}

	[Tooltip("省きたいスケール軸")]
	public EliminateScaleKind kind;

	public List<ShapeMove> lstShape = new List<ShapeMove>();

	public ChaControl chaCtrl;

	[Header("Debug表示")]
	[SerializeField]
	private Vector3 defScale = Vector3.one;

	[SerializeField]
	private Vector3 defPositon = Vector3.zero;

	[SerializeField]
	private Quaternion defRotation = Quaternion.identity;

	[SerializeField]
	[Header("Debug表示 出現時のTransform")]
	private Vector3 oldScale = Vector3.one;

	[SerializeField]
	private Vector3 oldPositon = Vector3.zero;

	[SerializeField]
	private Quaternion oldRotation = Quaternion.identity;

	private Dictionary<int, Dictionary<int, ShapeMove>> dicEliminate = new Dictionary<int, Dictionary<int, ShapeMove>>();

	private void Awake()
	{
		defScale = base.transform.lossyScale;
		defPositon = base.transform.localPosition;
		defRotation = base.transform.localRotation;
	}

	private void OnEnable()
	{
		oldScale = base.transform.lossyScale;
		oldPositon = base.transform.localPosition;
		oldRotation = base.transform.localRotation;
	}

	private void OnDisable()
	{
		base.transform.localScale = defScale;
		base.transform.localPosition = defPositon;
		base.transform.localRotation = defRotation;
		oldScale = Vector3.one;
		oldPositon = Vector3.zero;
		oldRotation = Quaternion.identity;
	}

	private void LateUpdate()
	{
		Vector3 lossyScale = base.transform.lossyScale;
		Vector3 localScale = base.transform.localScale;
		base.transform.localScale = new Vector3((kind != 0 && kind != EliminateScaleKind.X && kind != EliminateScaleKind.XY && kind != EliminateScaleKind.XZ) ? localScale.x : (localScale.x / lossyScale.x * oldScale.x), (kind != 0 && kind != EliminateScaleKind.Y && kind != EliminateScaleKind.XY && kind != EliminateScaleKind.YZ) ? localScale.y : (localScale.y / lossyScale.y * oldScale.y), (kind != 0 && kind != EliminateScaleKind.Z && kind != EliminateScaleKind.XZ && kind != EliminateScaleKind.YZ) ? localScale.z : (localScale.z / lossyScale.z * oldScale.z));
		if (chaCtrl != null)
		{
			Vector3 zero = Vector3.zero;
			Vector3 zero2 = Vector3.zero;
			for (int i = 0; i < lstShape.Count; i++)
			{
				float shapeBodyValue = chaCtrl.GetShapeBodyValue(Mathf.Max(lstShape[i].numShape, 0));
				zero += ((!(shapeBodyValue >= 0.5f)) ? Vector3.Lerp(lstShape[i].posMin, Vector3.zero, Mathf.InverseLerp(0f, 0.5f, shapeBodyValue)) : Vector3.Lerp(Vector3.zero, lstShape[i].posMax, Mathf.InverseLerp(0.5f, 1f, shapeBodyValue)));
				zero2 += ((!(shapeBodyValue >= 0.5f)) ? Vector3.Lerp(lstShape[i].rotMin, Vector3.zero, Mathf.InverseLerp(0f, 0.5f, shapeBodyValue)) : Vector3.Lerp(Vector3.zero, lstShape[i].rotMax, Mathf.InverseLerp(0.5f, 1f, shapeBodyValue)));
			}
			base.transform.localPosition = oldPositon + zero;
			base.transform.localRotation = oldRotation * Quaternion.Euler(zero2);
		}
	}

	public bool LoadList(int _idObj)
	{
		string text = GlobalMethod.LoadAllListText("h/list/", "EliminateScale_AibuItem_" + _idObj.ToString("00"));
		dicEliminate.Clear();
		if (text.IsNullOrEmpty())
		{
			return false;
		}
		string[,] data;
		GlobalMethod.GetListString(text, out data);
		int length = data.GetLength(0);
		for (int i = 0; i < length; i++)
		{
			int num = 0;
			int result = 0;
			int result2 = 0;
			int.TryParse(data[i, num++], out result2);
			int.TryParse(data[i, num++], out result);
			if (!dicEliminate.ContainsKey(result2))
			{
				dicEliminate.Add(result2, new Dictionary<int, ShapeMove>());
			}
			Dictionary<int, ShapeMove> dictionary = dicEliminate[result2];
			if (!dictionary.ContainsKey(result))
			{
				dictionary.Add(result, new ShapeMove());
			}
			ShapeMove shapeMove = dictionary[result];
			shapeMove.numShape = int.Parse(data[i, num++]);
			shapeMove.posMin = new Vector3(float.Parse(data[i, num++]), float.Parse(data[i, num++]), float.Parse(data[i, num++]));
			shapeMove.rotMin = new Vector3(float.Parse(data[i, num++]), float.Parse(data[i, num++]), float.Parse(data[i, num++]));
			shapeMove.posMax = new Vector3(float.Parse(data[i, num++]), float.Parse(data[i, num++]), float.Parse(data[i, num++]));
			shapeMove.rotMax = new Vector3(float.Parse(data[i, num++]), float.Parse(data[i, num++]), float.Parse(data[i, num++]));
		}
		return true;
	}

	public bool SetShapeList(int _idArea)
	{
		if (!dicEliminate.ContainsKey(_idArea))
		{
			lstShape = new List<ShapeMove>();
			return false;
		}
		lstShape = dicEliminate[_idArea].Values.ToList();
		return true;
	}

	private void SaveText(StreamWriter _writer)
	{
		foreach (ShapeMove item in lstShape)
		{
			_writer.Write(item.numShape + "\t");
			_writer.Write(item.posMin.x + "\t");
			_writer.Write(item.posMin.y + "\t");
			_writer.Write(item.posMin.z + "\t");
			_writer.Write(item.rotMin.x + "\t");
			_writer.Write(item.rotMin.y + "\t");
			_writer.Write(item.rotMin.z + "\t");
			_writer.Write(item.posMax.x + "\t");
			_writer.Write(item.posMax.y + "\t");
			_writer.Write(item.posMax.z + "\t");
			_writer.Write(item.rotMax.x + "\t");
			_writer.Write(item.rotMax.y + "\t");
			_writer.Write(item.rotMax.z + "\t");
			_writer.Write("\n");
		}
	}
}
