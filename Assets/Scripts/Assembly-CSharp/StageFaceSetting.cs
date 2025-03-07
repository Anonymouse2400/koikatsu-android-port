using Manager;
using UnityEngine;

public class StageFaceSetting : MonoBehaviour
{
	public SaveData.Heroine heroine;

	public string[] keyFaceString;

	private void FaceSet(int _face)
	{
		if (heroine != null && !(heroine.chaCtrl == null) && keyFaceString.Length > _face)
		{
			Game.Expression expression = Singleton<Game>.Instance.expCharaDic[heroine.FixCharaIDOrPersonality][keyFaceString[_face]];
			heroine.chaCtrl.ChangeEyebrowPtn(expression.eyebrow.ptn);
			heroine.chaCtrl.ChangeEyesPtn(expression.eyes.ptn);
			heroine.chaCtrl.ChangeMouthPtn(expression.mouth.ptn);
			heroine.chaCtrl.ChangeHohoAkaRate(expression.hohoAkaRate);
		}
	}
}
