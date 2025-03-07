using System;
using System.Collections.Generic;
using UnityEngine;

public class MetaballBuilder
{
	private class MB3DCubeVector
	{
		public sbyte[] e = new sbyte[3];

		public sbyte axisIdx = -1;

		public sbyte x
		{
			get
			{
				return e[0];
			}
			set
			{
				e[0] = value;
			}
		}

		public sbyte y
		{
			get
			{
				return e[1];
			}
			set
			{
				e[1] = value;
			}
		}

		public sbyte z
		{
			get
			{
				return e[2];
			}
			set
			{
				e[2] = value;
			}
		}

		public MB3DCubeVector()
		{
		}

		public MB3DCubeVector(sbyte x_, sbyte y_, sbyte z_)
		{
			x = x_;
			y = y_;
			z = z_;
			axisIdx = -1;
			CalcAxis();
		}

		public static MB3DCubeVector operator +(MB3DCubeVector lh, MB3DCubeVector rh)
		{
			return new MB3DCubeVector((sbyte)(lh.x + rh.x), (sbyte)(lh.y + rh.y), (sbyte)(lh.z + rh.z));
		}

		private void CalcAxis()
		{
			for (sbyte b = 0; b < 3; b++)
			{
				if (e[b] != 0)
				{
					if (axisIdx != -1)
					{
						axisIdx = -1;
						break;
					}
					axisIdx = b;
				}
			}
		}

		public void SetAxisVector(sbyte axisIdx_, sbyte value)
		{
			sbyte b2 = (z = 0);
			b2 = (y = b2);
			x = b2;
			if (value == 0)
			{
				axisIdx = -1;
				return;
			}
			axisIdx = axisIdx_;
			e[axisIdx] = value;
		}

		public static MB3DCubeVector operator *(MB3DCubeVector lh, sbyte rh)
		{
			return new MB3DCubeVector((sbyte)(lh.x * rh), (sbyte)(lh.y * rh), (sbyte)(lh.z * rh));
		}
	}

	private class MB3DCubeInOut
	{
		public sbyte[,,] bFill = new sbyte[2, 2, 2];

		public int inCount;

		public MB3DCubeInOut()
		{
		}

		public MB3DCubeInOut(sbyte patternIdx)
		{
			sbyte[] array = new sbyte[8];
			for (int i = 0; i < 8; i++)
			{
				array[i] = (sbyte)((patternIdx >> i) & 1);
			}
			Init(array[0], array[1], array[2], array[3], array[4], array[5], array[6], array[7]);
		}

		public MB3DCubeInOut(sbyte a0, sbyte a1, sbyte a2, sbyte a3, sbyte a4, sbyte a5, sbyte a6, sbyte a7)
		{
			Init(a0, a1, a2, a3, a4, a5, a6, a7);
		}

		private void Init(sbyte a0, sbyte a1, sbyte a2, sbyte a3, sbyte a4, sbyte a5, sbyte a6, sbyte a7)
		{
			bFill[0, 0, 0] = a0;
			bFill[0, 0, 1] = a1;
			bFill[0, 1, 0] = a2;
			bFill[0, 1, 1] = a3;
			bFill[1, 0, 0] = a4;
			bFill[1, 0, 1] = a5;
			bFill[1, 1, 0] = a6;
			bFill[1, 1, 1] = a7;
			inCount = a0 + a1 + a2 + a3 + a4 + a5 + a6 + a7;
		}

		public sbyte At(MB3DCubeVector point)
		{
			return bFill[point.z, point.y, point.x];
		}
	}

	private struct MB3DCubePrimitivePattern
	{
		public MB3DCubeInOut InOut;

		public int[] IndexBuf;

		public int[] IndexBufAlter;

		public int IndexCount
		{
			get
			{
				if (IndexBuf != null)
				{
					return IndexBuf.Length;
				}
				return 0;
			}
		}

		public int IndexCountAlter
		{
			get
			{
				if (IndexBufAlter != null)
				{
					return IndexBufAlter.Length;
				}
				return 0;
			}
		}
	}

	private class MB3DPatternMatchingInfo
	{
		public int PrimaryPatternIndex;

		public bool bFlipInOut;

		public MB3DCubeVector Origin = new MB3DCubeVector();

		public MB3DCubeVector[] Axis = new MB3DCubeVector[3];

		public void Match(MB3DCubeInOut src)
		{
			PrimaryPatternIndex = -1;
			bFlipInOut = src.inCount > 4;
			for (int i = 0; i < 15; i++)
			{
				MB3DCubeInOut inOut = __primitivePatterns[i].InOut;
				if (bFlipInOut)
				{
					if (8 - src.inCount != inOut.inCount)
					{
						continue;
					}
				}
				else if (src.inCount != inOut.inCount)
				{
					continue;
				}
				sbyte[] array = new sbyte[3];
				Origin.x = 0;
				while (Origin.x < 2)
				{
					array[0] = (sbyte)((Origin.x == 0) ? 1 : (-1));
					Origin.y = 0;
					while (Origin.y < 2)
					{
						array[1] = (sbyte)((Origin.y == 0) ? 1 : (-1));
						Origin.z = 0;
						while (Origin.z < 2)
						{
							array[2] = (sbyte)((Origin.z == 0) ? 1 : (-1));
							sbyte b = (sbyte)(((Origin.x + Origin.y + Origin.z) % 2 == 0) ? 1 : 2);
							for (sbyte b2 = 0; b2 < 3; b2++)
							{
								Axis[0] = new MB3DCubeVector();
								Axis[1] = new MB3DCubeVector();
								Axis[2] = new MB3DCubeVector();
								Axis[0].SetAxisVector(b2, array[b2]);
								Axis[1].SetAxisVector((sbyte)((b2 + b) % 3), array[(b2 + b) % 3]);
								Axis[2].SetAxisVector((sbyte)((b2 + b + b) % 3), array[(b2 + b + b) % 3]);
								bool flag = true;
								for (sbyte b3 = 0; b3 < 2; b3++)
								{
									for (sbyte b4 = 0; b4 < 2; b4++)
									{
										for (sbyte b5 = 0; b5 < 2; b5++)
										{
											MB3DCubeVector point = SampleVertex(new MB3DCubeVector(b3, b4, b5));
											if (bFlipInOut == (src.At(point) == inOut.bFill[b5, b4, b3]))
											{
												flag = false;
											}
										}
									}
								}
								if (flag)
								{
									PrimaryPatternIndex = i;
									return;
								}
							}
							MB3DCubeVector origin = Origin;
							origin.z += 1;
						}
						MB3DCubeVector origin2 = Origin;
						origin2.y += 1;
					}
					MB3DCubeVector origin3 = Origin;
					origin3.x += 1;
				}
			}
		}

		public int[] GetTargetPrimitiveIndexBuffer()
		{
			return (!bFlipInOut || __primitivePatterns[PrimaryPatternIndex].IndexCountAlter <= 0) ? __primitivePatterns[PrimaryPatternIndex].IndexBuf : __primitivePatterns[PrimaryPatternIndex].IndexBufAlter;
		}

		public MB3DCubeVector SampleVertex(MB3DCubeVector primaryPos)
		{
			return Origin + Axis[0] * primaryPos.x + Axis[1] * primaryPos.y + Axis[2] * primaryPos.z;
		}

		public void SampleSegment(sbyte primarySegmentID, out sbyte out_axis, out sbyte out_a_1, out sbyte out_a_2)
		{
			sbyte b = (sbyte)(primarySegmentID / 4);
			sbyte b2 = (sbyte)(primarySegmentID % 2);
			sbyte b3 = (sbyte)(primarySegmentID / 2 % 2);
			out_axis = Axis[b].axisIdx;
			MB3DCubeVector mB3DCubeVector = Origin + Axis[(b + 1) % 3] * b2 + Axis[(b + 2) % 3] * b3;
			sbyte b4 = (sbyte)((out_axis + 1) % 3);
			sbyte b5 = (sbyte)((out_axis + 2) % 3);
			out_a_1 = mB3DCubeVector.e[b4];
			out_a_2 = mB3DCubeVector.e[b5];
		}
	}

	private class MB3DCubePattern
	{
		public int PatternIdx;

		public MB3DPatternMatchingInfo MatchingInfo = new MB3DPatternMatchingInfo();

		public int[] IndexBuf = new int[15];

		public void Init(int patternIdx)
		{
			PatternIdx = patternIdx;
			MB3DCubeInOut src = new MB3DCubeInOut((sbyte)patternIdx);
			MatchingInfo.Match(src);
			int[] targetPrimitiveIndexBuffer = MatchingInfo.GetTargetPrimitiveIndexBuffer();
			for (int i = 0; i < targetPrimitiveIndexBuffer.Length; i++)
			{
				sbyte out_axis;
				sbyte out_a_;
				sbyte out_a_2;
				MatchingInfo.SampleSegment((sbyte)targetPrimitiveIndexBuffer[i], out out_axis, out out_a_, out out_a_2);
				int num = ((!MatchingInfo.bFlipInOut) ? i : (targetPrimitiveIndexBuffer.Length - i - 1));
				IndexBuf[num] = out_axis * 4 + out_a_2 * 2 + out_a_;
			}
		}
	}

	public class MetaballPointInfo
	{
		public Vector3 center;

		public float radius;

		public float density;
	}

	private static bool __bCubePatternsInitialized;

	private static MB3DCubePattern[] __cubePatterns;

	private const int MB3D_PATTERN_COUNT = 15;

	private static MB3DCubePrimitivePattern[] __primitivePatterns;

	private const int _maxGridCellCount = 1000000;

	private const int _maxVertexCount = 300000;

	private static MetaballBuilder _instance;

	public static int MaxGridCellCount
	{
		get
		{
			return 1000000;
		}
	}

	public static int MaxVertexCount
	{
		get
		{
			return 300000;
		}
	}

	public static MetaballBuilder Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new MetaballBuilder();
			}
			return _instance;
		}
	}

	static MetaballBuilder()
	{
		__bCubePatternsInitialized = false;
		__cubePatterns = new MB3DCubePattern[256];
		__primitivePatterns = new MB3DCubePrimitivePattern[15]
		{
			new MB3DCubePrimitivePattern
			{
				InOut = new MB3DCubeInOut(0, 0, 0, 0, 0, 0, 0, 0),
				IndexBuf = new int[0]
			},
			new MB3DCubePrimitivePattern
			{
				InOut = new MB3DCubeInOut(1, 0, 0, 0, 0, 0, 0, 0),
				IndexBuf = new int[3] { 0, 4, 8 }
			},
			new MB3DCubePrimitivePattern
			{
				InOut = new MB3DCubeInOut(1, 0, 1, 0, 0, 0, 0, 0),
				IndexBuf = new int[6] { 1, 10, 0, 8, 0, 10 }
			},
			new MB3DCubePrimitivePattern
			{
				InOut = new MB3DCubeInOut(1, 0, 0, 0, 0, 0, 1, 0),
				IndexBuf = new int[6] { 0, 4, 8, 3, 5, 10 },
				IndexBufAlter = new int[12]
				{
					0, 3, 8, 5, 8, 3, 0, 4, 3, 10,
					3, 4
				}
			},
			new MB3DCubePrimitivePattern
			{
				InOut = new MB3DCubeInOut(1, 0, 0, 0, 0, 0, 0, 1),
				IndexBuf = new int[6] { 0, 4, 8, 3, 11, 7 }
			},
			new MB3DCubePrimitivePattern
			{
				InOut = new MB3DCubeInOut(0, 1, 1, 1, 0, 0, 0, 0),
				IndexBuf = new int[9] { 10, 4, 0, 10, 0, 9, 10, 9, 11 }
			},
			new MB3DCubePrimitivePattern
			{
				InOut = new MB3DCubeInOut(1, 0, 1, 0, 0, 0, 0, 1),
				IndexBuf = new int[9] { 1, 10, 0, 8, 0, 10, 3, 11, 7 },
				IndexBufAlter = new int[15]
				{
					3, 10, 7, 10, 8, 7, 8, 0, 7, 0,
					1, 7, 1, 11, 7
				}
			},
			new MB3DCubePrimitivePattern
			{
				InOut = new MB3DCubeInOut(0, 0, 1, 0, 1, 0, 0, 1),
				IndexBuf = new int[9] { 10, 4, 1, 2, 8, 5, 3, 11, 7 }
			},
			new MB3DCubePrimitivePattern
			{
				InOut = new MB3DCubeInOut(1, 1, 1, 1, 0, 0, 0, 0),
				IndexBuf = new int[6] { 10, 8, 11, 9, 11, 8 }
			},
			new MB3DCubePrimitivePattern
			{
				InOut = new MB3DCubeInOut(1, 1, 0, 1, 0, 1, 0, 0),
				IndexBuf = new int[12]
				{
					2, 7, 8, 8, 7, 4, 4, 7, 11, 4,
					11, 1
				}
			},
			new MB3DCubePrimitivePattern
			{
				InOut = new MB3DCubeInOut(1, 0, 0, 1, 1, 0, 0, 1),
				IndexBuf = new int[12]
				{
					2, 0, 5, 4, 5, 0, 3, 1, 7, 6,
					7, 1
				}
			},
			new MB3DCubePrimitivePattern
			{
				InOut = new MB3DCubeInOut(1, 1, 0, 1, 0, 0, 0, 1),
				IndexBuf = new int[12]
				{
					8, 9, 4, 4, 9, 3, 7, 3, 9, 1,
					4, 3
				}
			},
			new MB3DCubePrimitivePattern
			{
				InOut = new MB3DCubeInOut(0, 1, 1, 1, 1, 0, 0, 0),
				IndexBuf = new int[12]
				{
					2, 8, 5, 10, 4, 0, 10, 0, 9, 10,
					9, 11
				}
			},
			new MB3DCubePrimitivePattern
			{
				InOut = new MB3DCubeInOut(1, 0, 0, 1, 0, 1, 1, 0),
				IndexBuf = new int[12]
				{
					0, 4, 8, 3, 5, 10, 2, 7, 9, 11,
					1, 6
				}
			},
			new MB3DCubePrimitivePattern
			{
				InOut = new MB3DCubeInOut(0, 1, 1, 1, 0, 1, 0, 0),
				IndexBuf = new int[12]
				{
					2, 4, 0, 2, 11, 4, 7, 11, 2, 10,
					4, 11
				}
			}
		};
		__InitCubePatterns();
	}

	private static void __InitCubePatterns()
	{
		for (int i = 0; i < 256; i++)
		{
			__cubePatterns[i] = new MB3DCubePattern();
			__cubePatterns[i].Init((sbyte)i);
		}
		__bCubePatternsInitialized = true;
	}

	private static float CalcPower(Vector3 relativePos, float radius, float density)
	{
		float num = relativePos.magnitude / radius;
		if (num > 1f)
		{
			return 0f;
		}
		return density * Mathf.Max((1f - num) * (1f - num), 0f);
	}

	public string CreateMesh(MetaballCellClusterInterface rootCell, Transform root, float powerThreshold, float gridCellSize, Vector3 uDir, Vector3 vDir, Vector3 uvOffset, out Mesh out_mesh, Vector3 _fixedMin, Vector3 _fixedMax, out Bounds boundsShow, MetaballCellObject cellObjPrefab = null, bool bReverse = false, Bounds? fixedBounds = null, bool bAutoGridSize = false, float autoGridQuarity = 0.2f)
	{
		Mesh mesh = new Mesh();
		MetaballPointInfo[] ballPoints;
		AnalyzeCellCluster(rootCell, root, out boundsShow, out ballPoints, _fixedMin, _fixedMax, cellObjPrefab);
		if (fixedBounds.HasValue)
		{
			boundsShow = fixedBounds.Value;
		}
		if (bAutoGridSize)
		{
			int num = (int)(1000000f * Mathf.Clamp01(autoGridQuarity));
			gridCellSize = Mathf.Pow(boundsShow.size.x * boundsShow.size.y * boundsShow.size.z / (float)num, 1f / 3f);
		}
		float num2 = (int)(boundsShow.size.x / gridCellSize) * (int)(boundsShow.size.y / gridCellSize) * (int)(boundsShow.size.z / gridCellSize);
		if (num2 > 1000000f)
		{
			out_mesh = mesh;
			return "Too many grid cells for building mesh (" + num2 + " > " + 1000000 + " )." + Environment.NewLine + "Make the area smaller or set larger (MetaballSeedBase.gridSize)";
		}
		BuildMetaballMesh(mesh, boundsShow.center, boundsShow.extents, gridCellSize, ballPoints, powerThreshold, bReverse, uDir, vDir, uvOffset);
		out_mesh = mesh;
		return null;
	}

	public string CreateMeshWithSkeleton(SkinnedMetaballCell rootCell, Transform root, float powerThreshold, float gridCellSize, Vector3 uDir, Vector3 vDir, Vector3 uvOffset, out Mesh out_mesh, out Transform[] out_bones, MetaballCellObject cellObjPrefab = null, bool bReverse = false, Bounds? fixedBounds = null, bool bAutoGridSize = false, float autoGridQuarity = 0.2f)
	{
		Mesh mesh = new Mesh();
		Bounds bounds;
		Transform[] bones;
		Matrix4x4[] bindPoses;
		MetaballPointInfo[] ballPoints;
		AnalyzeCellClusterWithSkeleton(rootCell, root, out bounds, out bones, out bindPoses, out ballPoints, cellObjPrefab);
		if (fixedBounds.HasValue)
		{
			bounds = fixedBounds.Value;
		}
		if (bAutoGridSize)
		{
			int num = (int)(1000000f * Mathf.Clamp01(autoGridQuarity));
			gridCellSize = Mathf.Pow(bounds.size.x * bounds.size.y * bounds.size.z / (float)num, 1f / 3f);
		}
		mesh.bindposes = bindPoses;
		float num2 = (int)(bounds.size.x / gridCellSize) * (int)(bounds.size.y / gridCellSize) * (int)(bounds.size.z / gridCellSize);
		if (num2 > 1000000f)
		{
			out_mesh = mesh;
			out_bones = bones;
			return "Too many grid cells for building mesh (" + num2 + " > " + 1000000 + " )." + Environment.NewLine + "Make the area smaller or set larger (MetaballSeedBase.gridSize)";
		}
		BuildMetaballMesh(mesh, bounds.center, bounds.extents, gridCellSize, ballPoints, powerThreshold, bReverse, uDir, vDir, uvOffset);
		out_mesh = mesh;
		out_bones = bones;
		return null;
	}

	private void AnalyzeCellCluster(MetaballCellClusterInterface cellCluster, Transform root, out Bounds bounds, out MetaballPointInfo[] ballPoints, Vector3 _fixedMin, Vector3 _fixedMax, MetaballCellObject cellObjPrefab = null)
	{
		int cellCount = cellCluster.CellCount;
		Bounds tmpBounds = new Bounds(Vector3.zero, Vector3.zero);
		List<MetaballPointInfo> lsttmpBallPoints = new List<MetaballPointInfo>();
		bool isRange = true;
		int cellIdx = 0;
		cellCluster.DoForeachCell(delegate(MetaballCell c)
		{
			isRange = true;
			for (int i = 0; i < 3; i++)
			{
				float num = c.modelPosition[i] - c.radius;
				if (!MathfEx.IsRange(_fixedMin[i], num, _fixedMax[i], true) && isRange)
				{
					isRange = false;
				}
				num = Mathf.Clamp(num, _fixedMin[i], _fixedMax[i]);
				if (num < tmpBounds.min[i])
				{
					Vector3 min = tmpBounds.min;
					min[i] = num;
					tmpBounds.min = min;
				}
				num = c.modelPosition[i] + c.radius;
				if (!MathfEx.IsRange(_fixedMin[i], num, _fixedMax[i], true) && isRange)
				{
					isRange = false;
				}
				num = Mathf.Clamp(num, _fixedMin[i], _fixedMax[i]);
				if (num > tmpBounds.max[i])
				{
					Vector3 max = tmpBounds.max;
					max[i] = num;
					tmpBounds.max = max;
				}
			}
			if (isRange)
			{
				GameObject gameObject = null;
				if ((bool)cellObjPrefab)
				{
					gameObject = UnityEngine.Object.Instantiate(cellObjPrefab.gameObject);
					gameObject.GetComponent<MetaballCellObject>().Setup(c);
				}
				else
				{
					gameObject = new GameObject();
				}
				if (!string.IsNullOrEmpty(c.tag))
				{
					gameObject.name = c.tag + "_Bone";
				}
				else
				{
					gameObject.name = "Bone";
				}
				Transform transform = gameObject.transform;
				transform.parent = root;
				transform.localPosition = c.modelPosition;
				transform.localRotation = c.modelRotation;
				transform.localScale = Vector3.one;
				MetaballPointInfo item = new MetaballPointInfo
				{
					center = c.modelPosition,
					radius = c.radius,
					density = c.density
				};
				lsttmpBallPoints.Add(item);
				cellIdx++;
			}
		});
		bounds = tmpBounds;
		ballPoints = lsttmpBallPoints.ToArray();
	}

	private void AnalyzeCellClusterWithSkeleton(SkinnedMetaballCell rootCell, Transform root, out Bounds bounds, out Transform[] bones, out Matrix4x4[] bindPoses, out MetaballPointInfo[] ballPoints, MetaballCellObject cellObjPrefab = null)
	{
		int cellCount = rootCell.CellCount;
		Transform[] tmpBones = new Transform[cellCount];
		Matrix4x4[] tmpBindPoses = new Matrix4x4[cellCount];
		Bounds tmpBounds = new Bounds(Vector3.zero, Vector3.zero);
		MetaballPointInfo[] tmpBallPoints = new MetaballPointInfo[cellCount];
		Dictionary<SkinnedMetaballCell, int> boneDictionary = new Dictionary<SkinnedMetaballCell, int>();
		int cellIdx = 0;
		rootCell.DoForeachSkinnedCell(delegate(SkinnedMetaballCell c)
		{
			for (int i = 0; i < 3; i++)
			{
				if (c.modelPosition[i] - c.radius < tmpBounds.min[i])
				{
					Vector3 min = tmpBounds.min;
					min[i] = c.modelPosition[i] - c.radius;
					tmpBounds.min = min;
				}
				if (c.modelPosition[i] + c.radius > tmpBounds.max[i])
				{
					Vector3 max = tmpBounds.max;
					max[i] = c.modelPosition[i] + c.radius;
					tmpBounds.max = max;
				}
			}
			GameObject gameObject = null;
			if ((bool)cellObjPrefab)
			{
				gameObject = UnityEngine.Object.Instantiate(cellObjPrefab.gameObject);
				gameObject.GetComponent<MetaballCellObject>().Setup(c);
			}
			else
			{
				gameObject = new GameObject();
			}
			if (!string.IsNullOrEmpty(c.tag))
			{
				gameObject.name = c.tag + "_Bone";
			}
			else
			{
				gameObject.name = "Bone";
			}
			Transform transform = gameObject.transform;
			if (c.IsRoot)
			{
				transform.parent = root;
				transform.localPosition = Vector3.zero;
				transform.localRotation = c.modelRotation;
				transform.localScale = Vector3.one;
			}
			else
			{
				Transform parent = tmpBones[boneDictionary[c.parent]];
				transform.parent = root;
				transform.localPosition = c.parent.modelPosition;
				transform.localRotation = c.modelRotation;
				transform.localScale = Vector3.one;
				transform.parent = parent;
			}
			tmpBones[cellIdx] = transform;
			tmpBindPoses[cellIdx] = tmpBones[cellIdx].worldToLocalMatrix * root.localToWorldMatrix;
			boneDictionary.Add(c, cellIdx);
			MetaballPointInfo metaballPointInfo = new MetaballPointInfo
			{
				center = c.modelPosition,
				radius = c.radius,
				density = c.density
			};
			tmpBallPoints[cellIdx] = metaballPointInfo;
			cellIdx++;
		});
		bounds = tmpBounds;
		bones = tmpBones;
		bindPoses = tmpBindPoses;
		ballPoints = tmpBallPoints;
	}

	public Mesh CreateImplicitSurfaceMesh(int countX, int countY, int countZ, Vector3[] positionMap, float[] powerMap, bool bReverse, float threshold, Vector3 uDir, Vector3 vDir, Vector3 uvOffset)
	{
		if (!__bCubePatternsInitialized)
		{
			__InitCubePatterns();
		}
		int num = countX * countY * countZ;
		Vector3[] array = new Vector3[num];
		int[] array2 = new int[num * 3];
		bool[] array3 = new bool[num];
		int num2 = countX * countY;
		int num3 = countX * countY * countZ;
		for (int i = 0; i < num * 3; i++)
		{
			array2[i] = -1;
		}
		for (int j = 0; j < num; j++)
		{
			float num4 = powerMap[j] - threshold;
			array3[j] = num4 >= 0f;
			if (array3[j] && num4 < 0.001f)
			{
				powerMap[j] = threshold + 0.001f;
			}
		}
		Vector3 vector = default(Vector3);
		for (int k = 1; k < countZ - 1; k++)
		{
			for (int l = 1; l < countY - 1; l++)
			{
				for (int m = 1; m < countX - 1; m++)
				{
					int num5 = m + l * countX + k * num2;
					vector.x = powerMap[num5 + 1] - powerMap[num5 - 1];
					vector.y = powerMap[num5 + countX] - powerMap[num5 - countX];
					vector.z = powerMap[num5 + num2] - powerMap[num5 - num2];
					if (vector.sqrMagnitude > 0.001f)
					{
						vector.Normalize();
					}
					array[num5] = vector;
				}
			}
		}
		int num6 = 0;
		List<Vector3> list = new List<Vector3>();
		List<Vector3> list2 = new List<Vector3>();
		List<Vector2> list3 = new List<Vector2>();
		for (int n = 0; n < countZ; n++)
		{
			if (num6 >= 299999)
			{
				break;
			}
			for (int num7 = 0; num7 < countY; num7++)
			{
				if (num6 >= 299999)
				{
					break;
				}
				for (int num8 = 0; num8 < countX; num8++)
				{
					if (num6 >= 299999)
					{
						break;
					}
					for (int num9 = 0; num9 < 3; num9++)
					{
						if (num6 >= 299999)
						{
							break;
						}
						int num10 = ((num9 == 0) ? 1 : 0);
						int num11 = ((num9 == 1) ? 1 : 0);
						int num12 = ((num9 == 2) ? 1 : 0);
						if (n + num12 < countZ && num7 + num11 < countY && num8 + num10 < countX)
						{
							int num13 = num8 + num7 * countX + n * num2;
							int num14 = num8 + num10 + (num7 + num11) * countX + (n + num12) * num2;
							float num15 = powerMap[num13];
							float num16 = powerMap[num14];
							if ((num15 - threshold) * (num16 - threshold) < 0f)
							{
								float num17 = (threshold - num15) / (num16 - num15);
								Vector3 vector2 = positionMap[num14] * num17 + positionMap[num13] * (1f - num17);
								list.Add(vector2);
								Vector3 lhs = vector2 + uvOffset;
								list3.Add(new Vector2(Vector3.Dot(lhs, uDir), Vector3.Dot(lhs, vDir)));
								Vector3 vector3 = -(array[num14] * num17 + array[num13] * (1f - num17)).normalized;
								list2.Add((!bReverse) ? vector3 : (-vector3));
								array2[num9 * num3 + num13] = num6;
								num6++;
							}
						}
					}
				}
			}
		}
		int[] array4 = new int[15];
		int num18 = 0;
		List<int> list4 = new List<int>();
		if (num6 > 3)
		{
			for (int num19 = 0; num19 < countZ - 1; num19++)
			{
				for (int num20 = 0; num20 < countY - 1; num20++)
				{
					for (int num21 = 0; num21 < countX - 1; num21++)
					{
						byte b = 0;
						for (int num22 = 0; num22 < 2; num22++)
						{
							for (int num23 = 0; num23 < 2; num23++)
							{
								for (int num24 = 0; num24 < 2; num24++)
								{
									if (array3[num21 + num24 + (num20 + num23) * countX + (num19 + num22) * num2])
									{
										b |= (byte)(1 << num22 * 4 + num23 * 2 + num24);
									}
								}
							}
						}
						int[] array5 = new int[12];
						for (int num25 = 0; num25 < 3; num25++)
						{
							for (int num26 = 0; num26 < 2; num26++)
							{
								for (int num27 = 0; num27 < 2; num27++)
								{
									int num30;
									int num29;
									int num28;
									switch (num25)
									{
									case 0:
										num30 = num21;
										num29 = num20 + num26;
										num28 = num19 + num27;
										break;
									case 1:
										num30 = num21 + num27;
										num29 = num20;
										num28 = num19 + num26;
										break;
									case 2:
										num30 = num21 + num26;
										num29 = num20 + num27;
										num28 = num19;
										break;
									default:
										num30 = (num29 = (num28 = -1));
										break;
									}
									int num31 = num25 * 4 + num27 * 2 + num26;
									array5[num31] = array2[num25 * num3 + num30 + num29 * countX + num28 * num2];
								}
							}
						}
						int primaryPatternIndex = __cubePatterns[b].MatchingInfo.PrimaryPatternIndex;
						array4[primaryPatternIndex]++;
						if (0 == 0)
						{
							for (int num32 = 0; num32 < __cubePatterns[b].MatchingInfo.GetTargetPrimitiveIndexBuffer().Length; num32++)
							{
								list4.Add(array5[__cubePatterns[b].IndexBuf[num32]]);
								num18++;
							}
						}
					}
				}
			}
		}
		Mesh mesh = new Mesh();
		mesh.vertices = list.ToArray();
		mesh.uv = list3.ToArray();
		mesh.normals = list2.ToArray();
		if (!bReverse)
		{
			mesh.triangles = list4.ToArray();
		}
		else
		{
			list4.Reverse();
			mesh.triangles = list4.ToArray();
		}
		return mesh;
	}

	private void BuildMetaballMesh(Mesh mesh, Vector3 center, Vector3 extent, float cellSize, MetaballPointInfo[] points, float powerThreshold, bool bReverse, Vector3 uDir, Vector3 vDir, Vector3 uvOffset)
	{
		if (!__bCubePatternsInitialized)
		{
			__InitCubePatterns();
		}
		int num = Mathf.CeilToInt(extent.x / cellSize) + 1;
		int num2 = Mathf.CeilToInt(extent.y / cellSize) + 1;
		int num3 = Mathf.CeilToInt(extent.z / cellSize) + 1;
		int num4 = num * 2;
		int num5 = num2 * 2;
		int num6 = num3 * 2;
		int num7 = num4;
		int num8 = num5;
		int num9 = num6;
		Vector3 vector = new Vector3((float)num * cellSize, (float)num2 * cellSize, (float)num3 * cellSize);
		Vector3 vector2 = center - vector;
		float[] array = new float[num4 * num5 * num6];
		Vector3[] array2 = new Vector3[num4 * num5 * num6];
		Vector3[] array3 = new Vector3[num4 * num5 * num6];
		int[] array4 = new int[num4 * num5 * num6 * 3];
		bool[] array5 = new bool[num4 * num5 * num6];
		BoneWeight[] array6 = new BoneWeight[num4 * num5 * num6];
		int num10 = num7;
		int num11 = num7 * num8;
		int num12 = num7 * num8 * num9;
		for (int i = 0; i < num9; i++)
		{
			for (int j = 0; j < num8; j++)
			{
				for (int k = 0; k < num7; k++)
				{
					array2[k + j * num10 + i * num11] = vector2 + new Vector3(cellSize * (float)k, cellSize * (float)j, cellSize * (float)i);
				}
			}
		}
		for (int l = 0; l < 3 * num9 * num8 * num7; l++)
		{
			array4[l] = -1;
		}
		int num13 = 0;
		foreach (MetaballPointInfo metaballPointInfo in points)
		{
			int b = (int)Mathf.Floor((metaballPointInfo.center.x - center.x - metaballPointInfo.radius) / cellSize) + num;
			int b2 = (int)Mathf.Floor((metaballPointInfo.center.y - center.y - metaballPointInfo.radius) / cellSize) + num2;
			int b3 = (int)Mathf.Floor((metaballPointInfo.center.z - center.z - metaballPointInfo.radius) / cellSize) + num3;
			b = Mathf.Max(0, b);
			b2 = Mathf.Max(0, b2);
			b3 = Mathf.Max(0, b3);
			int b4 = (int)Mathf.Ceil((metaballPointInfo.center.x - center.x + metaballPointInfo.radius) / cellSize) + num;
			int b5 = (int)Mathf.Ceil((metaballPointInfo.center.y - center.y + metaballPointInfo.radius) / cellSize) + num2;
			int b6 = (int)Mathf.Ceil((metaballPointInfo.center.z - center.z + metaballPointInfo.radius) / cellSize) + num3;
			b4 = Mathf.Min(num7 - 1, b4);
			b5 = Mathf.Min(num8 - 1, b5);
			b6 = Mathf.Min(num9 - 1, b6);
			for (int n = b3; n <= b6; n++)
			{
				for (int num14 = b2; num14 <= b5; num14++)
				{
					for (int num15 = b; num15 <= b4; num15++)
					{
						int num16 = num15 + num14 * num10 + n * num11;
						Vector3 vector3 = array2[num16];
						float num17 = CalcPower(vector3 - metaballPointInfo.center, metaballPointInfo.radius, metaballPointInfo.density);
						array[num16] += num17;
						if (!(num17 > 0f))
						{
							continue;
						}
						BoneWeight boneWeight = array6[num16];
						if (boneWeight.weight0 < num17 || boneWeight.weight1 < num17)
						{
							if (boneWeight.weight0 < boneWeight.weight1)
							{
								boneWeight.weight0 = num17;
								boneWeight.boneIndex0 = num13;
							}
							else
							{
								boneWeight.weight1 = num17;
								boneWeight.boneIndex1 = num13;
							}
						}
						array6[num16] = boneWeight;
					}
				}
			}
			num13++;
		}
		for (int num18 = 0; num18 < num7 * num8 * num9; num18++)
		{
			array5[num18] = array[num18] >= powerThreshold;
			if (array5[num18])
			{
				float num19 = 0.001f;
				if (Mathf.Abs(array[num18] - powerThreshold) < num19)
				{
					array[num18] = ((!(array[num18] - powerThreshold >= 0f)) ? (powerThreshold - num19) : (powerThreshold + num19));
				}
			}
		}
		for (int num20 = 1; num20 < num9 - 1; num20++)
		{
			for (int num21 = 1; num21 < num8 - 1; num21++)
			{
				for (int num22 = 1; num22 < num7 - 1; num22++)
				{
					array3[num22 + num21 * num10 + num20 * num11].x = array[num22 + 1 + num21 * num10 + num20 * num11] - array[num22 - 1 + num21 * num10 + num20 * num11];
					array3[num22 + num21 * num10 + num20 * num11].y = array[num22 + (num21 + 1) * num10 + num20 * num11] - array[num22 + (num21 - 1) * num10 + num20 * num11];
					array3[num22 + num21 * num10 + num20 * num11].z = array[num22 + num21 * num10 + (num20 + 1) * num11] - array[num22 + num21 * num10 + (num20 - 1) * num11];
					int num23 = num22 + num21 * num10 + num20 * num11;
					if (array3[num23].sqrMagnitude > 0.001f)
					{
						array3[num23].Normalize();
					}
				}
			}
		}
		int num24 = 0;
		List<Vector3> list = new List<Vector3>();
		List<Vector3> list2 = new List<Vector3>();
		List<Vector2> list3 = new List<Vector2>();
		for (int num25 = 0; num25 < num9; num25++)
		{
			if (num24 >= 299999)
			{
				break;
			}
			for (int num26 = 0; num26 < num8; num26++)
			{
				if (num24 >= 299999)
				{
					break;
				}
				for (int num27 = 0; num27 < num7; num27++)
				{
					if (num24 >= 299999)
					{
						break;
					}
					for (int num28 = 0; num28 < 3; num28++)
					{
						if (num24 >= 299999)
						{
							break;
						}
						int num29 = ((num28 == 0) ? 1 : 0);
						int num30 = ((num28 == 1) ? 1 : 0);
						int num31 = ((num28 == 2) ? 1 : 0);
						if (num25 + num31 < num9 && num26 + num30 < num8 && num27 + num29 < num7)
						{
							int num32 = num27 + num26 * num10 + num25 * num11;
							int num33 = num27 + num29 + (num26 + num30) * num10 + (num25 + num31) * num11;
							float num34 = array[num32];
							float num35 = array[num33];
							if ((num34 - powerThreshold) * (num35 - powerThreshold) < 0f)
							{
								float num36 = (powerThreshold - num34) / (num35 - num34);
								Vector3 vector4 = array2[num33] * num36 + array2[num32] * (1f - num36);
								list.Add(vector4);
								Vector3 lhs = vector4 + uvOffset;
								list3.Add(new Vector2(Vector3.Dot(lhs, uDir), Vector3.Dot(lhs, vDir)));
								Vector3 vector5 = -(array3[num33] * num36 + array3[num32] * (1f - num36)).normalized;
								list2.Add((!bReverse) ? vector5 : (-vector5));
								array4[num28 * num12 + num32] = num24;
								num24++;
							}
						}
					}
				}
			}
		}
		int[] array7 = new int[15];
		int[] array8 = new int[12];
		List<int> list4 = new List<int>();
		if (num24 > 3)
		{
			for (int num37 = 0; num37 < num9 - 1; num37++)
			{
				for (int num38 = 0; num38 < num8 - 1; num38++)
				{
					for (int num39 = 0; num39 < num7 - 1; num39++)
					{
						byte b7 = 0;
						for (int num40 = 0; num40 < 2; num40++)
						{
							for (int num41 = 0; num41 < 2; num41++)
							{
								for (int num42 = 0; num42 < 2; num42++)
								{
									if (array5[num39 + num42 + (num38 + num41) * num10 + (num37 + num40) * num11])
									{
										b7 |= (byte)(1 << num40 * 4 + num41 * 2 + num42);
									}
								}
							}
						}
						for (int num43 = 0; num43 < 3; num43++)
						{
							for (int num44 = 0; num44 < 2; num44++)
							{
								for (int num45 = 0; num45 < 2; num45++)
								{
									int num48;
									int num47;
									int num46;
									switch (num43)
									{
									case 0:
										num48 = num39;
										num47 = num38 + num44;
										num46 = num37 + num45;
										break;
									case 1:
										num48 = num39 + num45;
										num47 = num38;
										num46 = num37 + num44;
										break;
									case 2:
										num48 = num39 + num44;
										num47 = num38 + num45;
										num46 = num37;
										break;
									default:
										num48 = (num47 = (num46 = -1));
										break;
									}
									int num49 = num43 * 4 + num45 * 2 + num44;
									array8[num49] = array4[num43 * num12 + num48 + num47 * num10 + num46 * num11];
								}
							}
						}
						int primaryPatternIndex = __cubePatterns[b7].MatchingInfo.PrimaryPatternIndex;
						array7[primaryPatternIndex]++;
						for (int num50 = 0; num50 < __cubePatterns[b7].MatchingInfo.GetTargetPrimitiveIndexBuffer().Length; num50++)
						{
							if (array8[__cubePatterns[b7].IndexBuf[num50]] < 0)
							{
								string text = string.Empty;
								for (int num51 = 0; num51 < 12; num51++)
								{
									text = text + array8[num51] + ",";
								}
								string text2 = string.Empty;
								for (int num52 = 0; num52 < 2; num52++)
								{
									for (int num53 = 0; num53 < 2; num53++)
									{
										for (int num54 = 0; num54 < 2; num54++)
										{
											int num55 = num39 + num54 + (num38 + num53) * num10 + (num37 + num52) * num11;
											text2 = text2 + array[num55] + ",";
										}
									}
								}
								throw new UnityException("vertex error");
							}
							list4.Add(array8[__cubePatterns[b7].IndexBuf[num50]]);
						}
					}
				}
			}
		}
		mesh.vertices = list.ToArray();
		mesh.uv = list3.ToArray();
		mesh.normals = list2.ToArray();
		if (!bReverse)
		{
			mesh.triangles = list4.ToArray();
			return;
		}
		list4.Reverse();
		mesh.triangles = list4.ToArray();
	}
}
