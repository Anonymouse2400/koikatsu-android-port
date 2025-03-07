using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Studio
{
	public static class AssetBundleCheck
	{
		public static bool IsSimulation
		{
			get
			{
				return false;
			}
		}

		public static string[] FindAllAssetName(string assetBundleName, string _regex, bool _WithExtension = true, RegexOptions _options = RegexOptions.None)
		{
			_regex = _regex.ToLower();
			string[] array = null;
			AssetBundle assetBundle = AssetBundle.LoadFromFile(AssetBundleManager.BaseDownloadingURL + assetBundleName);
			array = ((!_WithExtension) ? (from v in assetBundle.GetAllAssetNames().Select(Path.GetFileNameWithoutExtension)
				where CheckRegex(v, _regex, _options)
				select v).ToArray() : (from v in assetBundle.GetAllAssetNames().Select(Path.GetFileName)
				where CheckRegex(v, _regex, _options)
				select v).ToArray());
			assetBundle.Unload(true);
			return array;
		}

		private static bool CheckRegex(string _value, string _regex, RegexOptions _options)
		{
			Match match = Regex.Match(_value, _regex, _options);
			return match.Success;
		}

		public static bool FindFile(string _assetBundleName, string _fineName, bool _WithExtension = false)
		{
			_fineName = _fineName.ToLower();
			bool flag = false;
			AssetBundle assetBundle = AssetBundle.LoadFromFile(AssetBundleManager.BaseDownloadingURL + _assetBundleName);
			if (assetBundle == null)
			{
				return false;
			}
			flag = ((!_WithExtension) ? (assetBundle.GetAllAssetNames().Select(Path.GetFileNameWithoutExtension).ToList()
				.FindIndex((string s) => s == _fineName) != -1) : (assetBundle.GetAllAssetNames().Select(Path.GetFileName).ToList()
				.FindIndex((string s) => s == _fineName) != -1));
			assetBundle.Unload(true);
			return flag;
		}

		public static string[] GetAllFileName(string _assetBundleName, bool _WithExtension = false)
		{
			string error;
			LoadedAssetBundle loadedAssetBundle = AssetBundleManager.GetLoadedAssetBundle(_assetBundleName, out error);
			AssetBundle assetBundle = ((loadedAssetBundle == null) ? AssetBundle.LoadFromFile(AssetBundleManager.BaseDownloadingURL + _assetBundleName) : loadedAssetBundle.m_AssetBundle);
			string[] array = null;
			array = ((!_WithExtension) ? assetBundle.GetAllAssetNames().Select(Path.GetFileNameWithoutExtension).ToArray() : assetBundle.GetAllAssetNames().Select(Path.GetFileName).ToArray());
			if (loadedAssetBundle == null)
			{
				assetBundle.Unload(true);
			}
			return array;
		}
	}
}
