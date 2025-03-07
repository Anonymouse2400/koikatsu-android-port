using System;
using System.Collections.Generic;
using System.IO;

namespace Studio
{
	public static class ObjectInfoAssist
	{
		public static void LoadChild(BinaryReader _reader, Version _version, List<ObjectInfo> _list, bool _import)
		{
			int num = _reader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				switch (_reader.ReadInt32())
				{
				case 0:
				{
					OICharInfo oICharInfo = new OICharInfo(null, _import ? Studio.GetNewIndex() : (-1));
					oICharInfo.Load(_reader, _version, _import);
					_list.Add(oICharInfo);
					break;
				}
				case 1:
				{
					OIItemInfo oIItemInfo = new OIItemInfo(-1, -1, -1, _import ? Studio.GetNewIndex() : (-1));
					oIItemInfo.Load(_reader, _version, _import);
					_list.Add(oIItemInfo);
					break;
				}
				case 2:
				{
					bool flag = !Studio.IsLightCheck();
					OILightInfo oILightInfo = new OILightInfo(-1, _import ? Studio.GetNewIndex() : (-1));
					oILightInfo.Load(_reader, _version, _import);
					if (flag)
					{
						oILightInfo.DeleteKey();
					}
					else
					{
						_list.Add(oILightInfo);
					}
					break;
				}
				case 3:
				{
					OIFolderInfo oIFolderInfo = new OIFolderInfo(_import ? Studio.GetNewIndex() : (-1));
					oIFolderInfo.Load(_reader, _version, _import);
					_list.Add(oIFolderInfo);
					break;
				}
				case 4:
				{
					OIRouteInfo oIRouteInfo = new OIRouteInfo(_import ? Studio.GetNewIndex() : (-1));
					oIRouteInfo.Load(_reader, _version, _import);
					_list.Add(oIRouteInfo);
					break;
				}
				case 5:
				{
					OICameraInfo oICameraInfo = new OICameraInfo(_import ? Studio.GetNewIndex() : (-1));
					oICameraInfo.Load(_reader, _version, _import);
					_list.Add(oICameraInfo);
					break;
				}
				}
			}
		}

		public static List<ObjectInfo> Find(int _kind)
		{
			List<ObjectInfo> _list = new List<ObjectInfo>();
			foreach (KeyValuePair<int, ObjectInfo> item in Singleton<Studio>.Instance.sceneInfo.dicObject)
			{
				FindLoop(ref _list, item.Value, _kind);
			}
			return _list;
		}

		private static void FindLoop(ref List<ObjectInfo> _list, ObjectInfo _src, int _kind)
		{
			if (_src == null)
			{
				return;
			}
			if (_src.kind == _kind)
			{
				_list.Add(_src);
			}
			switch (_src.kind)
			{
			case 0:
			{
				OICharInfo oICharInfo = _src as OICharInfo;
				{
					foreach (KeyValuePair<int, List<ObjectInfo>> item in oICharInfo.child)
					{
						foreach (ObjectInfo item2 in item.Value)
						{
							FindLoop(ref _list, item2, _kind);
						}
					}
					break;
				}
			}
			case 1:
			{
				foreach (ObjectInfo item3 in (_src as OIItemInfo).child)
				{
					FindLoop(ref _list, item3, _kind);
				}
				break;
			}
			case 2:
				break;
			case 3:
			{
				foreach (ObjectInfo item4 in (_src as OIFolderInfo).child)
				{
					FindLoop(ref _list, item4, _kind);
				}
				break;
			}
			case 4:
			{
				foreach (ObjectInfo item5 in (_src as OIRouteInfo).child)
				{
					FindLoop(ref _list, item5, _kind);
				}
				break;
			}
			case 5:
				break;
			case 6:
				break;
			}
		}
	}
}
