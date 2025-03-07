using System.Collections.Generic;
using ActionGame.Point;
using Illusion.Extensions;
using Localize.Translate;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace ActionGame
{
	public class ActionMoveUI : MonoBehaviour
	{
		[SerializeField]
		private GameObject moveIcon;

		[SerializeField]
		private TextMeshProUGUI text;

		public void Initialize(ActionScene actScene)
		{
			Dictionary<int, Data.Param> mapTexts = base.gameObject.LoadTranslater(Localize.Translate.Manager.SCENE_ID.MAP).Get(0);
			ActionMap map = actScene.Map;
			(from _ in actScene.UpdateAsObservable()
				where actScene.Player != null
				select actScene.Player.hitGateID).DistinctUntilChanged().Subscribe(delegate(int id)
			{
				bool flag = actScene.Player.isGateHit;
				GateInfo gateInfo = null;
				if (flag)
				{
					gateInfo = map.gateInfoDic[id];
					flag = gateInfo.moveType == 0;
				}
				moveIcon.SetActiveIfDifferent(flag);
				string title;
				if (flag)
				{
					int mapNo = gateInfo.mapNo;
					title = map.ConvertMapName(mapNo);
					mapTexts.SafeGetText(mapNo).SafeProc(delegate(string text)
					{
						title = text;
					});
				}
				else
				{
					title = string.Empty;
				}
				text.text = title;
			});
		}
	}
}
