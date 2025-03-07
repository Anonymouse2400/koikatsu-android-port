using Manager;
using UnityEngine;

public class HExpSprite : MonoBehaviour
{
	public HExpGauge mune;

	public HExpGauge kokan;

	public HExpGauge anal;

	public HExpGauge siri;

	public HExpGauge nip;

	public HExpGauge p_kokan;

	public HExpGauge p_anal;

	public HExpGauge houshi;

	private void Update()
	{
		if (!(Singleton<Scene>.Instance.NowSceneNames[0] != "HProc") && (Input.GetKeyDown(KeyCode.F6) || Input.GetMouseButtonDown(1)))
		{
			base.gameObject.SetActive(false);
		}
	}

	public void SetHeroine(SaveData.Heroine _heroine, HScene.AddParameter _add)
	{
		if (_heroine != null)
		{
			if ((bool)mune)
			{
				mune.Set(_heroine.hAreaExps[1], _add.aibus[0]);
			}
			if ((bool)kokan)
			{
				kokan.Set(_heroine.hAreaExps[2], _add.aibus[1]);
			}
			if ((bool)anal)
			{
				anal.Set(_heroine.hAreaExps[3], _add.aibus[2]);
			}
			if ((bool)siri)
			{
				siri.Set(_heroine.hAreaExps[4], _add.aibus[3]);
			}
			if ((bool)nip)
			{
				nip.Set(_heroine.hAreaExps[5], _add.aibus[4]);
			}
			if ((bool)p_kokan)
			{
				p_kokan.Set(_heroine.countKokanH, _add.sonyus[0]);
			}
			if ((bool)p_anal)
			{
				p_anal.Set(_heroine.countAnalH, _add.sonyus[1]);
			}
			if ((bool)houshi)
			{
				houshi.Set(_heroine.houshiExp, _add.houshi);
			}
		}
	}
}
