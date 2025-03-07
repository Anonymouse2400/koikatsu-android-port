using System.Collections.Generic;
using System.IO;
using Illusion.Extensions;
using Manager;
using Studio.Sound;
using UnityEngine;

namespace Studio
{
	public class OCIItem : ObjectCtrlInfo
	{
		public GameObject objectItem;

		public Transform childRoot;

		public Animator animator;

		public ItemComponent itemComponent;

		public ChaAccessoryComponent chaAccessoryComponent;

		public ParticleComponent particleComponent;

		private Texture2D[] texturePattern = new Texture2D[3];

		public bool enableEmission;

		public IconComponent iconComponent;

		public PanelComponent panelComponent;

		private Texture2D textureMain;

		public SEComponent seComponent;

		public ItemFKCtrl itemFKCtrl;

		public List<OCIChar.BoneInfo> listBones;

		public DynamicBone[] dynamicBones;

		private bool m_Visible = true;

		public Renderer[] arrayRender;

		public ParticleSystem[] arrayParticle;

		public OIItemInfo itemInfo
		{
			get
			{
				return objectInfo as OIItemInfo;
			}
		}

		public bool isAnime
		{
			get
			{
				return animator != null && animator.enabled;
			}
		}

		public bool isChangeColor
		{
			get
			{
				if ((bool)itemComponent)
				{
					return itemComponent.check;
				}
				if ((bool)chaAccessoryComponent)
				{
					return chaAccessoryComponent.useColor01 | chaAccessoryComponent.useColor02 | chaAccessoryComponent.useColor03 | !chaAccessoryComponent.rendAlpha.IsNullOrEmpty();
				}
				if ((bool)particleComponent)
				{
					return particleComponent.check;
				}
				return false;
			}
		}

		public bool[] useColor
		{
			get
			{
				if ((bool)itemComponent)
				{
					return itemComponent.useColor;
				}
				if ((bool)chaAccessoryComponent)
				{
					return new bool[3] { chaAccessoryComponent.useColor01, chaAccessoryComponent.useColor02, chaAccessoryComponent.useColor03 };
				}
				if ((bool)particleComponent)
				{
					return particleComponent.useColor;
				}
				return new bool[3];
			}
		}

		public bool useColor4
		{
			get
			{
				return (!itemComponent) ? (chaAccessoryComponent != null) : (!itemComponent.rendGlass.IsNullOrEmpty());
			}
		}

		public Color[] defColor
		{
			get
			{
				if ((bool)itemComponent)
				{
					return itemComponent.defColorMain;
				}
				if (!chaAccessoryComponent)
				{
					if (!particleComponent)
					{
						return new Color[3]
						{
							Color.white,
							Color.white,
							Color.white
						};
					}
					return new Color[1] { particleComponent.defColor01 };
				}
				return new Color[3] { chaAccessoryComponent.defColor01, chaAccessoryComponent.defColor02, chaAccessoryComponent.defColor03 };
			}
		}

		public bool[] usePattern
		{
			get
			{
				return (!(itemComponent == null)) ? itemComponent.usePattern : new bool[3];
			}
		}

		public bool checkAlpha
		{
			get
			{
				return (bool)itemComponent && itemComponent.checkAlpha;
			}
		}

		public bool checkLine
		{
			get
			{
				return (bool)itemComponent && itemComponent.checkLine;
			}
		}

		public bool checkEmission
		{
			get
			{
				return enableEmission && (bool)itemComponent && itemComponent.checkEmission;
			}
		}

		public bool checkEmissionColor
		{
			get
			{
				return (bool)itemComponent && itemComponent.checkEmissionColor;
			}
		}

		public bool checkEmissionPower
		{
			get
			{
				return (bool)itemComponent && itemComponent.checkEmissionPower;
			}
		}

		public bool checkLightCancel
		{
			get
			{
				return (bool)itemComponent && itemComponent.checkLightCancel;
			}
		}

		public bool checkShadow
		{
			get
			{
				return !(useColor4 | (bool)particleComponent) && useColor.Check((bool b) => b) != -1;
			}
		}

		public bool isParticle
		{
			get
			{
				return particleComponent;
			}
		}

		public bool checkPanel
		{
			get
			{
				return panelComponent != null;
			}
		}

		public bool isFK
		{
			get
			{
				return !listBones.IsNullOrEmpty();
			}
		}

		public bool isDynamicBone
		{
			get
			{
				return !(isFK & itemInfo.enableFK) && !dynamicBones.IsNullOrEmpty();
			}
		}

		public bool visible
		{
			get
			{
				return m_Visible;
			}
			set
			{
				m_Visible = value;
				for (int i = 0; i < arrayRender.Length; i++)
				{
					arrayRender[i].enabled = value;
				}
				if (!arrayParticle.IsNullOrEmpty())
				{
					for (int j = 0; j < arrayParticle.Length; j++)
					{
						if (value)
						{
							arrayParticle[j].Play();
						}
						else
						{
							arrayParticle[j].Pause();
						}
					}
				}
				if ((bool)seComponent)
				{
					seComponent.enabled = value;
				}
			}
		}

		public override float animeSpeed
		{
			get
			{
				return itemInfo.animeSpeed;
			}
			set
			{
				if (Utility.SetStruct(ref itemInfo.animeSpeed, value) && (bool)animator)
				{
					animator.speed = itemInfo.animeSpeed;
				}
			}
		}

		public override void OnDelete()
		{
			if (!listBones.IsNullOrEmpty())
			{
				for (int i = 0; i < listBones.Count; i++)
				{
					Singleton<GuideObjectManager>.Instance.Delete(listBones[i].guideObject);
				}
				listBones.Clear();
			}
			Singleton<GuideObjectManager>.Instance.Delete(guideObject);
			Object.Destroy(objectItem);
			if (parentInfo != null)
			{
				parentInfo.OnDetachChild(this);
			}
			Studio.DeleteInfo(objectInfo);
		}

		public override void OnAttach(TreeNodeObject _parent, ObjectCtrlInfo _child)
		{
			if (_child.parentInfo == null)
			{
				Studio.DeleteInfo(_child.objectInfo, false);
			}
			else
			{
				_child.parentInfo.OnDetachChild(_child);
			}
			if (!itemInfo.child.Contains(_child.objectInfo))
			{
				itemInfo.child.Add(_child.objectInfo);
			}
			_child.guideObject.transformTarget.SetParent(childRoot);
			_child.guideObject.parent = childRoot;
			_child.guideObject.mode = GuideObject.Mode.World;
			_child.guideObject.moveCalc = GuideMove.MoveCalc.TYPE2;
			_child.objectInfo.changeAmount.pos = _child.guideObject.transformTarget.localPosition;
			_child.objectInfo.changeAmount.rot = _child.guideObject.transformTarget.localEulerAngles;
			_child.parentInfo = this;
		}

		public override void OnLoadAttach(TreeNodeObject _parent, ObjectCtrlInfo _child)
		{
			if (_child.parentInfo == null)
			{
				Studio.DeleteInfo(_child.objectInfo, false);
			}
			else
			{
				_child.parentInfo.OnDetachChild(_child);
			}
			if (!itemInfo.child.Contains(_child.objectInfo))
			{
				itemInfo.child.Add(_child.objectInfo);
			}
			_child.guideObject.transformTarget.SetParent(childRoot, false);
			_child.guideObject.parent = childRoot;
			_child.guideObject.mode = GuideObject.Mode.World;
			_child.guideObject.moveCalc = GuideMove.MoveCalc.TYPE2;
			_child.objectInfo.changeAmount.OnChange();
			_child.parentInfo = this;
		}

		public override void OnDetach()
		{
			parentInfo.OnDetachChild(this);
			guideObject.parent = null;
			Studio.AddInfo(objectInfo, this);
			objectItem.transform.SetParent(Singleton<Scene>.Instance.commonSpace.transform);
			objectInfo.changeAmount.pos = objectItem.transform.localPosition;
			objectInfo.changeAmount.rot = objectItem.transform.localEulerAngles;
			guideObject.mode = GuideObject.Mode.Local;
			guideObject.moveCalc = GuideMove.MoveCalc.TYPE1;
			treeNodeObject.ResetVisible();
		}

		public override void OnSelect(bool _select)
		{
			int layer = LayerMask.NameToLayer((!_select) ? "Studio/Select" : "Studio/Col");
			if (!listBones.IsNullOrEmpty())
			{
				for (int i = 0; i < listBones.Count; i++)
				{
					listBones[i].layer = layer;
				}
			}
		}

		public override void OnDetachChild(ObjectCtrlInfo _child)
		{
			if (!itemInfo.child.Remove(_child.objectInfo))
			{
			}
			_child.parentInfo = null;
		}

		public override void OnSavePreprocessing()
		{
			base.OnSavePreprocessing();
			if (isAnime && animator.layerCount != 0)
			{
				AnimatorStateInfo currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
				itemInfo.animeNormalizedTime = currentAnimatorStateInfo.normalizedTime;
			}
		}

		public override void OnVisible(bool _visible)
		{
			visible = _visible;
		}

		public void SetColor(Color _color, int _idx)
		{
			itemInfo.color[_idx] = _color;
			UpdateColor();
		}

		public void SetupPatternTex()
		{
			for (int i = 0; i < 3; i++)
			{
				if (!itemInfo.pattern[i].filePath.IsNullOrEmpty())
				{
					string fileName = Path.GetFileName(itemInfo.pattern[i].filePath);
					SetPatternTex(i, UserData.Path + "pattern/" + fileName);
				}
				else
				{
					SetPatternTex(i, itemInfo.pattern[i].key);
				}
			}
		}

		public string SetPatternTex(int _idx, int _key)
		{
			if (_key <= 0)
			{
				itemInfo.pattern[_idx].key = _key;
				itemInfo.pattern[_idx].filePath = string.Empty;
				if ((bool)itemComponent)
				{
					itemComponent.SetPatternTex(_idx, null);
				}
				ReleasePatternTex(_idx);
				return "なし";
			}
			List<PatternSelectInfo> lstSelectInfo = Singleton<Studio>.Instance.patternSelectListCtrl.lstSelectInfo;
			PatternSelectInfo patternSelectInfo = lstSelectInfo.Find((PatternSelectInfo p) => p.index == _key);
			string result = "なし";
			if (patternSelectInfo != null)
			{
				if (patternSelectInfo.assetBundle.IsNullOrEmpty())
				{
					string path = UserData.Path + "pattern/" + patternSelectInfo.assetName;
					if (!File.Exists(path))
					{
						return "なし";
					}
					texturePattern[_idx] = PngAssist.LoadTexture(path);
					itemInfo.pattern[_idx].key = -1;
					itemInfo.pattern[_idx].filePath = patternSelectInfo.assetName;
					result = patternSelectInfo.assetName;
				}
				else
				{
					string assetBundleName = patternSelectInfo.assetBundle.Replace("thumb/", string.Empty);
					string assetName = patternSelectInfo.assetName.Replace("thumb_", string.Empty);
					texturePattern[_idx] = CommonLib.LoadAsset<Texture2D>(assetBundleName, assetName, false, string.Empty);
					itemInfo.pattern[_idx].key = _key;
					itemInfo.pattern[_idx].filePath = string.Empty;
					result = patternSelectInfo.name;
				}
			}
			itemComponent.SetPatternTex(_idx, texturePattern[_idx]);
			Resources.UnloadUnusedAssets();
			return result;
		}

		public void SetPatternTex(int _idx, string _path)
		{
			if (_path.IsNullOrEmpty())
			{
				itemInfo.pattern[_idx].key = 0;
				itemInfo.pattern[_idx].filePath = string.Empty;
				itemComponent.SetPatternTex(_idx, null);
				ReleasePatternTex(_idx);
				return;
			}
			itemInfo.pattern[_idx].key = -1;
			itemInfo.pattern[_idx].filePath = _path;
			if (File.Exists(_path))
			{
				texturePattern[_idx] = PngAssist.LoadTexture(_path);
			}
			itemComponent.SetPatternTex(_idx, texturePattern[_idx]);
			Resources.UnloadUnusedAssets();
		}

		private void ReleasePatternTex(int _idx)
		{
			texturePattern[_idx] = null;
		}

		public void SetPatternClamp(int _idx, bool _flag)
		{
			if (Utility.SetStruct(ref itemInfo.pattern[_idx].clamp, _flag))
			{
				UpdateColor();
			}
		}

		public void SetPatternUT(int _idx, float _value)
		{
			if (Utility.SetStruct(ref itemInfo.pattern[_idx].uv.x, _value))
			{
				UpdateColor();
			}
		}

		public void SetPatternVT(int _idx, float _value)
		{
			if (Utility.SetStruct(ref itemInfo.pattern[_idx].uv.y, _value))
			{
				UpdateColor();
			}
		}

		public void SetPatternUS(int _idx, float _value)
		{
			if (Utility.SetStruct(ref itemInfo.pattern[_idx].uv.z, _value))
			{
				UpdateColor();
			}
		}

		public void SetPatternVS(int _idx, float _value)
		{
			if (Utility.SetStruct(ref itemInfo.pattern[_idx].uv.w, _value))
			{
				UpdateColor();
			}
		}

		public void SetPatternRot(int _idx, float _value)
		{
			if (Utility.SetStruct(ref itemInfo.pattern[_idx].rot, _value))
			{
				UpdateColor();
			}
		}

		public void SetAlpha(float _value)
		{
			if (Utility.SetStruct(ref itemInfo.alpha, _value))
			{
				UpdateColor();
			}
		}

		public void SetEmissionColor(Color _color)
		{
			itemInfo.emissionColor = _color;
			UpdateColor();
		}

		public void SetEmissionPower(float _value)
		{
			itemInfo.emissionPower = _value;
			UpdateColor();
		}

		public void SetLightCancel(float _value)
		{
			itemInfo.lightCancel = _value;
			UpdateColor();
		}

		public void SetLineColor(Color _color)
		{
			itemInfo.lineColor = _color;
			UpdateColor();
		}

		public void ResetLineColor()
		{
			if (!(itemComponent == null))
			{
				SetLineColor(itemComponent.defLineColor);
			}
		}

		public void SetLineWidth(float _value)
		{
			if (Utility.SetStruct(ref itemInfo.lineWidth, _value))
			{
				UpdateColor();
			}
		}

		public void ResetLineWidth()
		{
			if (!(itemComponent == null))
			{
				SetLineWidth(itemComponent.defLineWidth);
			}
		}

		public void UpdateColor()
		{
			if ((bool)itemComponent && itemComponent.check)
			{
				itemComponent.UpdateColor(itemInfo);
			}
			else if ((bool)chaAccessoryComponent)
			{
				if (!chaAccessoryComponent.rendNormal.IsNullOrEmpty())
				{
					Renderer[] rendNormal = chaAccessoryComponent.rendNormal;
					foreach (Renderer renderer in rendNormal)
					{
						if (chaAccessoryComponent.useColor01)
						{
							renderer.material.SetColor(ChaShader._Color, itemInfo.color[0]);
						}
						if (chaAccessoryComponent.useColor02)
						{
							renderer.material.SetColor(ChaShader._Color2, itemInfo.color[1]);
						}
						if (chaAccessoryComponent.useColor03)
						{
							renderer.material.SetColor(ChaShader._Color3, itemInfo.color[2]);
						}
					}
				}
				if (!chaAccessoryComponent.rendAlpha.IsNullOrEmpty())
				{
					Renderer[] rendAlpha = chaAccessoryComponent.rendAlpha;
					foreach (Renderer renderer2 in rendAlpha)
					{
						renderer2.material.SetColor(ChaShader._Color4, itemInfo.color[7]);
						renderer2.gameObject.SetActiveIfDifferent((itemInfo.color[7].a != 0f) ? true : false);
					}
				}
			}
			else if ((bool)particleComponent && particleComponent.check)
			{
				particleComponent.UpdateColor(itemInfo);
			}
			else if ((bool)panelComponent)
			{
				panelComponent.UpdateColor(itemInfo);
			}
		}

		public void SetMainTex()
		{
			SetMainTex(itemInfo.panel.filePath);
		}

		public void SetMainTex(string _file)
		{
			if (panelComponent == null)
			{
				return;
			}
			if (_file.IsNullOrEmpty())
			{
				itemInfo.panel.filePath = string.Empty;
				panelComponent.SetMainTex(null);
				textureMain = null;
				return;
			}
			itemInfo.panel.filePath = _file;
			string path = UserData.Path + BackgroundList.dirName + "/" + _file;
			if (File.Exists(path))
			{
				textureMain = PngAssist.LoadTexture(path);
				panelComponent.SetMainTex(textureMain);
				Resources.UnloadUnusedAssets();
			}
		}

		public void ActiveFK(bool _active)
		{
			if (itemFKCtrl == null)
			{
				return;
			}
			itemFKCtrl.enabled = _active;
			itemInfo.enableFK = _active;
			bool enabled = !_active && itemInfo.enableDynamicBone;
			DynamicBone[] array = dynamicBones;
			foreach (DynamicBone dynamicBone in array)
			{
				dynamicBone.enabled = enabled;
			}
			foreach (OCIChar.BoneInfo listBone in listBones)
			{
				listBone.active = _active;
			}
		}

		public void UpdateFKColor()
		{
			if (listBones.IsNullOrEmpty())
			{
				return;
			}
			foreach (OCIChar.BoneInfo listBone in listBones)
			{
				listBone.color = Studio.optionSystem.colorFKItem;
			}
		}

		public void ActiveDynamicBone(bool _active)
		{
			itemInfo.enableDynamicBone = _active;
			if (!dynamicBones.IsNullOrEmpty() && !(isFK & itemInfo.enableFK))
			{
				DynamicBone[] array = dynamicBones;
				foreach (DynamicBone dynamicBone in array)
				{
					dynamicBone.enabled = _active;
				}
			}
		}

		public void RestartAnime()
		{
			if (isAnime && animator.layerCount != 0)
			{
				AnimatorStateInfo currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
				animator.Play(currentAnimatorStateInfo.shortNameHash, 0, 0f);
			}
		}
	}
}
