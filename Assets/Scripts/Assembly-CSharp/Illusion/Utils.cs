using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using Illusion.Extensions;
using UniRx;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

namespace Illusion
{
	public static class Utils
	{
		public static class Animator
		{
			public static string GetControllerName(UnityEngine.Animator animator)
			{
				return GetControllerName(animator.runtimeAnimatorController);
			}

			public static string GetControllerName(RuntimeAnimatorController runtimeAnimatorController)
			{
				return (!(runtimeAnimatorController == null)) ? runtimeAnimatorController.name : null;
			}

			public static AnimatorControllerParameter[] GetAnimeParams(UnityEngine.Animator animator)
			{
				return Enumerable.Range(0, animator.parameterCount).Select(animator.GetParameter).ToArray();
			}

			public static AnimatorControllerParameter GetAnimeParam(string name, UnityEngine.Animator animator)
			{
				return GetAnimeParam(name, GetAnimeParams(animator));
			}

			public static AnimatorControllerParameter GetAnimeParam(string name, AnimatorControllerParameter[] param)
			{
				return param.FirstOrDefault((AnimatorControllerParameter item) => item.name == name);
			}

			public static bool AnimeParamFindSet(UnityEngine.Animator animator, Tuple<string, object> nameValue)
			{
				return AnimeParamFindSet(animator, nameValue.Item1, nameValue.Item2, GetAnimeParams(animator));
			}

			public static bool AnimeParamFindSet(UnityEngine.Animator animator, string name, object value)
			{
				return AnimeParamFindSet(animator, name, value, GetAnimeParams(animator));
			}

			public static bool AnimeParamFindSet(UnityEngine.Animator animator, Tuple<string, object>[] nameValues)
			{
				return AnimeParamFindSet(animator, nameValues, GetAnimeParams(animator));
			}

			public static bool AnimeParamFindSet(UnityEngine.Animator animator, Tuple<string, object> nameValue, AnimatorControllerParameter[] animParams)
			{
				return AnimeParamFindSet(animator, nameValue.Item1, nameValue.Item2, animParams);
			}

			public static bool AnimeParamFindSet(UnityEngine.Animator animator, string name, object value, AnimatorControllerParameter[] animParams)
			{
				return animParams.FirstOrDefault((AnimatorControllerParameter p) => p.name == name).SafeProc(delegate(AnimatorControllerParameter param)
				{
					AnimeParamSet(animator, name, value, param.type);
				});
			}

			public static bool AnimeParamFindSet(UnityEngine.Animator animator, Tuple<string, object>[] nameValues, AnimatorControllerParameter[] animParams)
			{
				bool flag = false;
				foreach (var item in nameValues.Select(delegate(Tuple<string, object> v)
				{
					AnimatorControllerParameter animatorControllerParameter = animParams.FirstOrDefault((AnimatorControllerParameter p) => p.name == v.Item1);
					return (animatorControllerParameter != null) ? new
					{
						type = animatorControllerParameter.type,
						value = v
					} : null;
				}))
				{
					flag |= AnimeParamSet(animator, item.value, item.type);
				}
				return flag;
			}

			public static bool AnimeParamSet(UnityEngine.Animator animator, Tuple<string, object> nameValue)
			{
				return AnimeParamSet(animator, nameValue.Item1, nameValue.Item2);
			}

			public static bool AnimeParamSet(UnityEngine.Animator animator, string name, object value)
			{
				if (value is float)
				{
					animator.SetFloat(name, (float)value);
				}
				else if (value is int)
				{
					animator.SetInteger(name, (int)value);
				}
				else
				{
					if (!(value is bool))
					{
						return false;
					}
					animator.SetBool(name, (bool)value);
				}
				return true;
			}

			public static bool AnimeParamSet(UnityEngine.Animator animator, Tuple<string, object> nameValue, AnimatorControllerParameterType type)
			{
				return AnimeParamSet(animator, nameValue.Item1, nameValue.Item2, type);
			}

			public static bool AnimeParamSet(UnityEngine.Animator animator, string name, object value, AnimatorControllerParameterType type)
			{
				switch (type)
				{
				case AnimatorControllerParameterType.Float:
					animator.SetFloat(name, (float)value);
					break;
				case AnimatorControllerParameterType.Int:
					animator.SetInteger(name, (int)value);
					break;
				case AnimatorControllerParameterType.Bool:
					animator.SetBool(name, (bool)value);
					break;
				case AnimatorControllerParameterType.Trigger:
					if (value != null)
					{
						if (value is bool)
						{
							if ((bool)value)
							{
								animator.SetTrigger(name);
							}
							else
							{
								animator.ResetTrigger(name);
							}
						}
						else if (value is int)
						{
							if ((int)value != 0)
							{
								animator.SetTrigger(name);
							}
							else
							{
								animator.ResetTrigger(name);
							}
						}
						else
						{
							animator.SetTrigger(name);
						}
					}
					else
					{
						animator.ResetTrigger(name);
					}
					break;
				default:
					return false;
				}
				return true;
			}

			public static AnimatorOverrideController SetupAnimatorOverrideController(RuntimeAnimatorController src, RuntimeAnimatorController over)
			{
				if (src == null || over == null)
				{
					return null;
				}
				AnimatorOverrideController animatorOverrideController = new AnimatorOverrideController(src);
				AnimationClip[] animationClips = new AnimatorOverrideController(over).animationClips;
				foreach (AnimationClip animationClip in animationClips)
				{
					animatorOverrideController[animationClip.name] = animationClip;
				}
				animatorOverrideController.name = over.name;
				return animatorOverrideController;
			}
		}

		public static class Comparer
		{
			public enum Type
			{
				Equal = 0,
				NotEqual = 1,
				Over = 2,
				Under = 3,
				Greater = 4,
				Lesser = 5
			}

			public static readonly string[] STR = new string[6] { "==", "!=", ">=", "<=", ">", "<" };

			public static readonly string[] LABEL = new string[6] { "一致", "不一致", "以上", "以下", "より大きい", "より小さい" };

			public static bool Check<T>(T v1, string compStr, T v2) where T : IComparable
			{
				return Check(v1, (Type)STR.Check((string s) => s == compStr), v2);
			}

			public static bool Check<T>(T v1, Type compEnum, T v2) where T : IComparable
			{
				int num = v1.CompareTo(v2);
				switch (compEnum)
				{
				case Type.Equal:
					return num == 0;
				case Type.NotEqual:
					return num != 0;
				case Type.Over:
					return num >= 0;
				case Type.Under:
					return num <= 0;
				case Type.Greater:
					return num > 0;
				case Type.Lesser:
					return num < 0;
				default:
					return false;
				}
			}

			public static Type Cast(string str, out string v)
			{
				int findIndex = -1;
				int num = STR.Check(delegate(string s)
				{
					findIndex = str.IndexOf(s);
					return findIndex != -1;
				});
				v = str.Substring(findIndex + STR[num].Length);
				return (Type)num;
			}

			public static Tuple<Type, string>[] Cast(params string[] strs)
			{
				string v;
				return (from i in Enumerable.Range(0, strs.Length)
					select Tuple.Create(Cast(strs[i], out v), v)).ToArray();
			}
		}

		public static class Crypto
		{
			private const string AesInitVector = "1234567890abcdefghujklmnopqrstuv";

			private const string AesKey = "piyopiyopiyopiyopiyopiyopiyopiyo";

			private const int BlockSize = 256;

			private const int KeySize = 256;

			public static byte[] Encrypt(byte[] binData)
			{
				RijndaelManaged rijndaelManaged = new RijndaelManaged();
				rijndaelManaged.Padding = PaddingMode.Zeros;
				rijndaelManaged.Mode = CipherMode.CBC;
				rijndaelManaged.KeySize = 256;
				rijndaelManaged.BlockSize = 256;
				byte[] bytes = Encoding.UTF8.GetBytes("piyopiyopiyopiyopiyopiyopiyopiyo");
				byte[] bytes2 = Encoding.UTF8.GetBytes("1234567890abcdefghujklmnopqrstuv");
				ICryptoTransform transform = rijndaelManaged.CreateEncryptor(bytes, bytes2);
				MemoryStream memoryStream = new MemoryStream();
				using (CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write))
				{
					cryptoStream.Write(binData, 0, binData.Length);
				}
				memoryStream.Close();
				return memoryStream.ToArray();
			}

			public static byte[] Decrypt(byte[] binData)
			{
				RijndaelManaged rijndaelManaged = new RijndaelManaged();
				rijndaelManaged.Padding = PaddingMode.Zeros;
				rijndaelManaged.Mode = CipherMode.CBC;
				rijndaelManaged.KeySize = 256;
				rijndaelManaged.BlockSize = 256;
				byte[] bytes = Encoding.UTF8.GetBytes("piyopiyopiyopiyopiyopiyopiyopiyo");
				byte[] bytes2 = Encoding.UTF8.GetBytes("1234567890abcdefghujklmnopqrstuv");
				ICryptoTransform transform = rijndaelManaged.CreateDecryptor(bytes, bytes2);
				byte[] array = new byte[binData.Length];
				using (MemoryStream stream = new MemoryStream(binData))
				{
					using (CryptoStream cryptoStream = new CryptoStream(stream, transform, CryptoStreamMode.Read))
					{
						cryptoStream.Read(array, 0, array.Length);
						return array;
					}
				}
			}
		}

		public static class Enum<T> where T : struct
		{
			public class EnumerateParameter
			{
				public IEnumerator<T> GetEnumerator()
				{
					IEnumerator enumerator = Enum<T>.Values.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							yield return (T)enumerator.Current;
						}
					}
					finally
					{
						IDisposable disposable;
						IDisposable disposable2 = (disposable = enumerator as IDisposable);
						if (disposable != null)
						{
							disposable2.Dispose();
						}
					}
				}
			}

			public static string[] Names
			{
				get
				{
					return Enum.GetNames(typeof(T));
				}
			}

			public static Array Values
			{
				get
				{
					return Enum.GetValues(typeof(T));
				}
			}

			public static int Length
			{
				get
				{
					return Values.Length;
				}
			}

			public static T Nothing
			{
				get
				{
					return default(T);
				}
			}

			public static T Everything
			{
				get
				{
					ulong sum = 0uL;
					Each(delegate(T o)
					{
						sum += Convert.ToUInt64(o);
					});
					return Cast(sum);
				}
			}

			public static void Each(Action<T> act)
			{
				foreach (T value in Values)
				{
					act(value);
				}
			}

			[Conditional("UNITY_ASSERTIONS")]
			private static void Check(bool condition, string message)
			{
			}

			public static bool Contains(string key, bool ignoreCase = false)
			{
				return FindIndex(key, ignoreCase) != -1;
			}

			public static int FindIndex(string key, bool ignoreCase = false)
			{
				string[] names = Names;
				for (int i = 0; i < names.Length; i++)
				{
					if (string.Compare(names[i], key, ignoreCase) == 0)
					{
						return i;
					}
				}
				return -1;
			}

			public static T Cast(string key)
			{
				return (T)Enum.Parse(typeof(T), key);
			}

			public static T Cast(int no)
			{
				System.Type typeFromHandle = typeof(T);
				return (T)Enum.ToObject(typeFromHandle, no);
			}

			public static T Cast(uint sum)
			{
				return (T)Enum.ToObject(typeof(T), sum);
			}

			public static T Cast(ulong sum)
			{
				return (T)Enum.ToObject(typeof(T), sum);
			}

			public static EnumerateParameter Enumerate()
			{
				return new EnumerateParameter();
			}

			public static T Normalize(T value)
			{
				return Cast((ulong)(Convert.ToInt64(value) & Convert.ToInt64(Everything)));
			}

			public static string ToString(T value)
			{
				StringBuilder text = new StringBuilder();
				Each(delegate(T e)
				{
					ulong num = Convert.ToUInt64(e);
					if ((Convert.ToUInt64(value) & num) == num)
					{
						text.AppendFormat("{0} | ", e);
					}
				});
				return text.ToString();
			}
		}

		public static class File
		{
			public static string[] Gets(string filePath, string searchFile)
			{
				List<string> list = new List<string>();
				if (Directory.Exists(filePath))
				{
					string[] directories = Directory.GetDirectories(filePath);
					foreach (string path in directories)
					{
						list.AddRange(Directory.GetFiles(path, searchFile).Select(ConvertPath));
					}
				}
				return list.ToArray();
			}

			public static void GetAllFiles(string folder, string searchPattern, ref List<string> files)
			{
				files.AddRange(Directory.GetFiles(folder, searchPattern).Select(ConvertPath));
				string[] directories = Directory.GetDirectories(folder);
				foreach (string folder2 in directories)
				{
					GetAllFiles(folder2, searchPattern, ref files);
				}
			}

			public static List<string> GetPaths(string[] paths, string ext, SearchOption option)
			{
				List<string> list = new List<string>();
				foreach (IGrouping<bool, string> item in paths.GroupBy(Directory.Exists))
				{
					if (item.Key)
					{
						foreach (string item2 in item)
						{
							list.AddRange(Directory.GetFiles(item2, "*" + ext, option).Select(ConvertPath));
						}
					}
					else
					{
						list.AddRange(item.Where((string path) => Path.GetExtension(path) == ext).Select(ConvertPath));
					}
				}
				return list;
			}

			public static string ConvertPath(string path)
			{
				return path.Replace("\\", "/");
			}

			public static object LoadFromBinaryFile(string path)
			{
				object obj = null;
				OpenRead(path, delegate(FileStream fs)
				{
					obj = new BinaryFormatter().Deserialize(fs);
				});
				return obj;
			}

			public static void SaveToBinaryFile(object obj, string path)
			{
				OpenWrite(path, false, delegate(FileStream fs)
				{
					new BinaryFormatter().Serialize(fs, obj);
				});
			}

			public static bool OpenRead(string filePath, Action<FileStream> act)
			{
				if (!System.IO.File.Exists(filePath))
				{
					return false;
				}
				using (FileStream obj = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
				{
					act(obj);
				}
				return true;
			}

			public static void OpenWrite(string filePath, bool isAppend, Action<FileStream> act)
			{
				if (!isAppend)
				{
					using (FileStream obj = new FileStream(filePath, FileMode.Create))
					{
						act(obj);
						return;
					}
				}
				using (FileStream obj2 = new FileStream(filePath, FileMode.Append, FileAccess.Write))
				{
					act(obj2);
				}
			}

			public static bool Read(string filePath, Action<StreamReader> act)
			{
				return OpenRead(filePath, delegate(FileStream fs)
				{
					using (StreamReader obj = new StreamReader(fs))
					{
						act(obj);
					}
				});
			}

			public static void Write(string filePath, bool isAppend, Action<StreamWriter> act)
			{
				OpenWrite(filePath, isAppend, delegate(FileStream fs)
				{
					using (StreamWriter obj = new StreamWriter(fs))
					{
						act(obj);
					}
				});
			}
		}

		public static class Gizmos
		{
			public static void Axis(Vector3 pos, Quaternion rot, float len = 0.25f)
			{
				UnityEngine.Gizmos.color = Color.red;
				UnityEngine.Gizmos.DrawRay(pos, rot * Vector3.right * len);
				UnityEngine.Gizmos.color = Color.green;
				UnityEngine.Gizmos.DrawRay(pos, rot * Vector3.up * len);
				UnityEngine.Gizmos.color = Color.blue;
				UnityEngine.Gizmos.DrawRay(pos, rot * Vector3.forward * len);
			}

			public static void Axis(Transform transform, float len = 0.25f)
			{
				Axis(transform.position, transform.rotation, len);
			}

			public static void PointLine(Vector3[] route, bool isLink = false)
			{
				if (route.Any())
				{
					route.Aggregate(delegate(Vector3 prev, Vector3 current)
					{
						UnityEngine.Gizmos.DrawLine(prev, current);
						return current;
					});
					if (isLink)
					{
						UnityEngine.Gizmos.DrawLine(route.Last(), route.First());
					}
				}
			}
		}

		public static class Hash
		{
			public static bool Equals(byte[] arg1, byte[] arg2)
			{
				if (arg1.Length != arg2.Length)
				{
					return false;
				}
				int num = -1;
				while (++num < arg1.Length && arg1[num] == arg2[num])
				{
				}
				return num == arg1.Length;
			}

			public static byte[] ComputeMD5(string source)
			{
				return new MD5CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(source));
			}

			public static byte[] ComputeSHA1(string source)
			{
				return new SHA1CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(source));
			}

			public static int Convert(byte[] bytes)
			{
				if (BitConverter.IsLittleEndian)
				{
					Array.Reverse(bytes);
				}
				return BitConverter.ToInt32(bytes, 0);
			}

			public static string Cast(byte[] source)
			{
				StringBuilder stringBuilder = new StringBuilder(source.Length);
				for (int i = 0; i < source.Length - 1; i++)
				{
					stringBuilder.Append(source[i].ToString("X2"));
				}
				return stringBuilder.ToString();
			}
		}

		public static class Math
		{
			public static class Cast
			{
				public static Vector2 ToVector2(float[] f)
				{
					return new Vector2(f[0], f[1]);
				}

				public static Vector3 ToVector3(float[] f)
				{
					return new Vector3(f[0], f[1], f[2]);
				}

				public static float[] ToArray(Vector2 v2)
				{
					return new float[2] { v2.x, v2.y };
				}

				public static float[] ToArray(Vector3 v3)
				{
					return new float[3] { v3.x, v3.y, v3.z };
				}

				public static string ToString(Vector3 v3)
				{
					return string.Format("({0},{1},{2})", v3.x, v3.y, v3.z);
				}
			}

			public static class Fuzzy
			{
				public static float Grade(float _value, float _x0, float _x1)
				{
					float num = 0f;
					if (_value <= _x0)
					{
						return 0f;
					}
					if (_value >= _x1)
					{
						return 1f;
					}
					return _value / (_x1 - _x0) - _x0 / (_x1 - _x0);
				}

				public static float ReverseGrade(float _value, float _x0, float _x1)
				{
					float num = 0f;
					if (_value <= _x0)
					{
						return 1f;
					}
					if (_value >= _x1)
					{
						return 0f;
					}
					return (0f - _value) / (_x1 - _x0) + _x1 / (_x1 - _x0);
				}

				public static float Triangle(float _value, float _x0, float _x1, float _x2)
				{
					float num = 0f;
					if (_value <= _x0)
					{
						return 0f;
					}
					if (_value == _x1)
					{
						return 1f;
					}
					if (_value > _x0 && _value < _x1)
					{
						return _value / (_x1 - _x0) - _x0 / (_x1 - _x0);
					}
					return (0f - _value) / (_x2 - _x1) + _x2 / (_x2 - _x1);
				}

				public static float Trapezoid(float _value, float _x0, float _x1, float _x2, float _x3)
				{
					float num = 0f;
					if (_value <= _x0)
					{
						return 0f;
					}
					if (_value >= _x1 && _value <= _x2)
					{
						return 1f;
					}
					if (_value > _x0 && _value < _x1)
					{
						return _value / (_x1 - _x0) - _x0 / (_x1 - _x0);
					}
					return (0f - _value) / (_x3 - _x2) + _x3 / (_x3 - _x2);
				}

				public static float AND(float _a, float _b)
				{
					return Mathf.Min(_a, _b);
				}

				public static float OR(float _a, float _b)
				{
					return Mathf.Max(_a, _b);
				}

				public static float NOT(float _a)
				{
					return 1f - _a;
				}
			}

			public delegate float Func(float x);

			public static Vector3 MoveSpeedPositionEnter(Vector3[] points, float moveSpeed)
			{
				for (int i = 0; i < points.Length - 1; i++)
				{
					Vector3 a = points[i];
					Vector3 b = points[i + 1];
					float num = Vector3.Distance(a, b);
					if (moveSpeed > num)
					{
						moveSpeed -= num;
						continue;
					}
					return Vector3.Lerp(a, b, Mathf.InverseLerp(num, 0f, num - moveSpeed));
				}
				return points[points.Length - 1];
			}

			public static int MinDistanceRouteIndex(Vector3[] route, Vector3 pos)
			{
				int result = -1;
				float num = float.MaxValue;
				for (int i = 0; i < route.Length; i++)
				{
					float sqrMagnitude = (route[i] - pos).sqrMagnitude;
					if (num > sqrMagnitude)
					{
						num = sqrMagnitude;
						result = i;
					}
				}
				return result;
			}

			public static void TargetFor(Transform from, Transform target, bool isHeight = false)
			{
				Vector3 position = target.position;
				if (!isHeight)
				{
					position.y = from.position.y;
				}
				from.LookAt(position);
			}

			public static float NewtonMethod(Func func, Func derive, float initX, int maxLoop)
			{
				float num = initX;
				for (int i = 0; i < maxLoop; i++)
				{
					float num2 = func(num);
					if (num2 < 1E-05f && num2 > -1E-05f)
					{
						break;
					}
					num -= num2 / derive(num);
				}
				return num;
			}
		}

		public static class Mesh
		{
			private static float ToRad(float angle, int index)
			{
				return angle * (float)index * ((float)System.Math.PI / 180f);
			}

			public static IEnumerable<Vector3> CalculateVertices(int verticesNum)
			{
				if (verticesNum <= 0)
				{
					return Enumerable.Empty<Vector3>();
				}
				float angle = 360f / (float)verticesNum;
				return from i in Enumerable.Range(0, verticesNum)
					select ToRad(angle, i) into r
					select new Vector3(Mathf.Sin(r), Mathf.Cos(r));
			}

			public static void Create(GameObject go, IEnumerable<Vector3> vertices)
			{
				if (vertices != null && vertices.Count() >= 3)
				{
					MeshFilter meshFilter = go.GetComponent<MeshFilter>();
					if (meshFilter == null)
					{
						meshFilter = go.AddComponent<MeshFilter>();
					}
					UnityEngine.Mesh mesh = meshFilter.mesh;
					mesh.Clear();
					mesh.vertices = vertices.ToArray();
					int[] array = new int[(mesh.vertices.Length - 2) * 3];
					int num = 0;
					int num2 = 0;
					while (num < array.Length)
					{
						array[num] = 0;
						array[num + 1] = num2 + 1;
						array[num + 2] = num2 + 2;
						num += 3;
						num2++;
					}
					mesh.triangles = array;
					mesh.RecalculateNormals();
					mesh.RecalculateBounds();
					meshFilter.sharedMesh = mesh;
				}
			}

			public static void RendererSet(GameObject go, Color color, string matName = "Sprites-Default.mat")
			{
				MeshRenderer meshRenderer = go.GetComponent<MeshRenderer>();
				if (meshRenderer == null)
				{
					meshRenderer = go.AddComponent<MeshRenderer>();
				}
				meshRenderer.material = Resources.GetBuiltinResource<Material>(matName);
				meshRenderer.material.color = color;
			}
		}

		public static class NavMesh
		{
			public static GameObject CreateDrawObject(Color? color)
			{
				NavMeshTriangulation navMeshTriangulation = UnityEngine.AI.NavMesh.CalculateTriangulation();
				UnityEngine.Mesh mesh = new UnityEngine.Mesh();
				mesh.vertices = navMeshTriangulation.vertices;
				mesh.triangles = navMeshTriangulation.indices;
				GameObject gameObject = new GameObject("NavMeshDrawObject");
				gameObject.AddComponent<MeshFilter>().mesh = mesh;
				gameObject.AddComponent<MeshRenderer>().material.color = ((!color.HasValue) ? Color.white : color.Value);
				return gameObject;
			}

			public static bool GetRandomPosition(Vector3 center, out Vector3 result, float range = 10f, int count = 30, float maxDistance = 1f, bool isY = true, int area = -1)
			{
				Func<Vector3> func = ((!isY) ? ((Func<Vector3>)delegate
				{
					Vector2 vector = UnityEngine.Random.insideUnitCircle * range;
					return new Vector3(vector.x, 0f, vector.y);
				}) : ((Func<Vector3>)(() => UnityEngine.Random.insideUnitSphere * range)));
				for (int i = 0; i < count; i++)
				{
					NavMeshHit hit;
					if (UnityEngine.AI.NavMesh.SamplePosition(center + func(), out hit, maxDistance, area))
					{
						result = hit.position;
						return true;
					}
				}
				result = Vector3.zero;
				return false;
			}
		}

		public static class Network
		{
			public static string MACAddress
			{
				get
				{
					string text = string.Empty;
					NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
					if (allNetworkInterfaces != null)
					{
						NetworkInterface[] array = allNetworkInterfaces;
						foreach (NetworkInterface networkInterface in array)
						{
							PhysicalAddress physicalAddress = networkInterface.GetPhysicalAddress();
							if (physicalAddress != null && physicalAddress.GetAddressBytes().Length == 6)
							{
								string text2 = physicalAddress.ToString();
								text += text2;
							}
						}
					}
					return text;
				}
			}
		}

		public static class ProbabilityCalclator
		{
			public static bool DetectFromPercent(float percent)
			{
				int num = 0;
				string text = percent.ToString();
				if (text.IndexOf(".") > 0)
				{
					num = text.Split('.')[1].Length;
				}
				int num2 = (int)Mathf.Pow(10f, num);
				int max = 100 * num2;
				int num3 = (int)((float)num2 * percent);
				return UnityEngine.Random.Range(0, max) < num3;
			}

			public static T DetermineFromDict<T>(Dictionary<T, int> targetDict)
			{
				if (!targetDict.Any())
				{
					return default(T);
				}
				float num = UnityEngine.Random.Range(0f, targetDict.Values.Sum());
				foreach (KeyValuePair<T, int> item in targetDict)
				{
					num -= (float)item.Value;
					if (num < 0f)
					{
						return item.Key;
					}
				}
				return targetDict.Keys.First();
			}

			public static T DetermineFromDict<T>(Dictionary<T, float> targetDict)
			{
				if (!targetDict.Any())
				{
					return default(T);
				}
				float num = UnityEngine.Random.Range(0f, targetDict.Values.Sum());
				foreach (KeyValuePair<T, float> item in targetDict)
				{
					num -= item.Value;
					if (num < 0f)
					{
						return item.Key;
					}
				}
				return targetDict.Keys.First();
			}
		}

		public static class String
		{
			public static string GetPropertyName<T>(Expression<Func<T>> e)
			{
				MemberExpression memberExpression = (MemberExpression)e.Body;
				return memberExpression.Member.Name;
			}
		}

		public static class Type
		{
			public static System.Type Get(string dllName, string typeName)
			{
				return Assembly.Load(dllName).GetType(typeName);
			}

			public static System.Type Get(string typeName)
			{
				Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
				foreach (Assembly assembly in assemblies)
				{
					System.Type[] types = assembly.GetTypes();
					foreach (System.Type type in types)
					{
						if (type.Name == typeName)
						{
							return type;
						}
					}
				}
				return null;
			}

			public static System.Type GetFull(string typeFullName)
			{
				Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
				foreach (Assembly assembly in assemblies)
				{
					System.Type[] types = assembly.GetTypes();
					foreach (System.Type type in types)
					{
						if (type.FullName == typeFullName)
						{
							return type;
						}
					}
				}
				return null;
			}

			public static string GetAssemblyQualifiedName(string typeName)
			{
				Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
				foreach (Assembly assembly in assemblies)
				{
					System.Type[] types = assembly.GetTypes();
					foreach (System.Type type in types)
					{
						if (type.Name == typeName)
						{
							return type.AssemblyQualifiedName;
						}
					}
				}
				return null;
			}

			public static string GetFullAssemblyQualifiedName(string typeFullName)
			{
				Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
				foreach (Assembly assembly in assemblies)
				{
					System.Type[] types = assembly.GetTypes();
					foreach (System.Type type in types)
					{
						if (type.FullName == typeFullName)
						{
							return type.AssemblyQualifiedName;
						}
					}
				}
				return null;
			}

			public static FieldInfo[] GetPublicFields(System.Type type)
			{
				return type.GetFields(BindingFlags.Instance | BindingFlags.Public);
			}

			public static PropertyInfo[] GetPublicProperties(System.Type type)
			{
				return type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
			}
		}

		public static class Value
		{
			public static int Check(int len, Func<int, bool> func)
			{
				int num = -1;
				while (++num < len && !func(num))
				{
				}
				return (num < len) ? num : (-1);
			}
		}

		public static class uGUI
		{
			public static bool isMouseHit
			{
				get
				{
					return HitList(Input.mousePosition).Count > 0;
				}
			}

			public static List<RaycastResult> HitList(Vector3 position)
			{
				List<RaycastResult> list = new List<RaycastResult>();
				EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current)
				{
					position = position
				}, list);
				return list;
			}
		}
	}
}
