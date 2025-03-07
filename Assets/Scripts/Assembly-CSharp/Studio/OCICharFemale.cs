using Manager;

namespace Studio
{
	public class OCICharFemale : OCIChar
	{
		public ChaControl female
		{
			get
			{
				return charInfo;
			}
		}

		public override void OnDelete()
		{
			base.OnDelete();
			Singleton<Character>.Instance.DeleteChara(female);
		}

		public override void SetSiruFlags(ChaFileDefine.SiruParts _parts, byte _state)
		{
			base.SetSiruFlags(_parts, _state);
			female.SetSiruFlags(_parts, _state);
		}

		public override byte GetSiruFlags(ChaFileDefine.SiruParts _parts)
		{
			return female.GetSiruFlags(_parts);
		}

		public override void SetNipStand(float _value)
		{
			base.SetNipStand(_value);
			base.oiCharInfo.nipple = _value;
			female.ChangeNipRate(_value);
		}

		public override void ChangeChara(string _path)
		{
			base.ChangeChara(_path);
			female.UpdateBustSoftnessAndGravity();
			optionItemCtrl.height = female.fileBody.shapeValueBody[0];
			female.setAnimatorParamFloat("height", female.fileBody.shapeValueBody[0]);
			if (isAnimeMotion)
			{
				female.setAnimatorParamFloat("breast", female.fileBody.shapeValueBody[1]);
			}
		}

		public override void SetCoordinateInfo(ChaFileDefine.CoordinateType _type, bool _force = false)
		{
			base.SetCoordinateInfo(_type, _force);
			female.UpdateBustSoftnessAndGravity();
			skirtDynamic = AddObjectFemale.GetSkirtDynamic(charInfo.objClothes);
			ActiveFK(OIBoneInfo.BoneGroup.Skirt, base.oiCharInfo.activeFK[6], base.oiCharInfo.enableFK);
		}

		public override void SetClothesStateAll(int _state)
		{
			base.SetClothesStateAll(_state);
			female.SetClothesStateAll((byte)_state);
		}

		public override void LoadClothesFile(string _path)
		{
			base.LoadClothesFile(_path);
			female.UpdateBustSoftnessAndGravity();
			skirtDynamic = AddObjectFemale.GetSkirtDynamic(charInfo.objClothes);
			ActiveFK(OIBoneInfo.BoneGroup.Skirt, base.oiCharInfo.activeFK[6], base.oiCharInfo.enableFK);
		}
	}
}
