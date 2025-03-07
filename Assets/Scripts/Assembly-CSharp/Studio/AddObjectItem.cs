using System;
using System.Collections.Generic;
using System.Linq;
using IllusionUtility.GetUtility;
using Manager;
using Studio.Sound;
using UnityEngine;

namespace Studio
{
	public static class AddObjectItem
	{
		public static OCIItem Add(int _group, int _category, int _no)
		{
			int newIndex = Studio.GetNewIndex();
			Singleton<UndoRedoManager>.Instance.Do(new AddObjectCommand.AddItemCommand(_group, _category, _no, newIndex, Studio.optionSystem.initialPosition));
			return Studio.GetCtrlInfo(newIndex) as OCIItem;
		}

		public static OCIItem Load(OIItemInfo _info, ObjectCtrlInfo _parent, TreeNodeObject _parentNode)
		{
			ChangeAmount source = _info.changeAmount.Clone();
			OCIItem oCIItem = Load(_info, _parent, _parentNode, false, -1);
			_info.changeAmount.Copy(source);
			AddObjectAssist.LoadChild(_info.child, oCIItem, null);
			return oCIItem;
		}

		public static OCIItem Load(OIItemInfo _info, ObjectCtrlInfo _parent, TreeNodeObject _parentNode, bool _addInfo, int _initialPosition)
		{
			OCIItem oCIItem = new OCIItem();
			Info.ItemLoadInfo loadInfo = GetLoadInfo(_info.group, _info.category, _info.no);
			if (loadInfo == null)
			{
				loadInfo = GetLoadInfo(0, 0, 0);
			}
			oCIItem.objectInfo = _info;
			GameObject gameObject = CommonLib.LoadAsset<GameObject>(loadInfo.bundlePath, loadInfo.fileName, true, loadInfo.manifest);
			if (gameObject == null)
			{
				Studio.DeleteIndex(_info.dicKey);
				return null;
			}
			gameObject.transform.SetParent(Singleton<Scene>.Instance.commonSpace.transform);
			oCIItem.objectItem = gameObject;
			oCIItem.arrayRender = (from v in gameObject.GetComponentsInChildren<Renderer>()
				where v.enabled
				select v).ToArray();
			ParticleSystem[] componentsInChildren = gameObject.GetComponentsInChildren<ParticleSystem>();
			if (!componentsInChildren.IsNullOrEmpty())
			{
				oCIItem.arrayParticle = componentsInChildren.Where((ParticleSystem v) => v.isPlaying).ToArray();
			}
			MeshCollider component = gameObject.GetComponent<MeshCollider>();
			if ((bool)component)
			{
				component.enabled = false;
			}
			oCIItem.dynamicBones = gameObject.GetComponentsInChildren<DynamicBone>();
			GuideObject guideObject = Singleton<GuideObjectManager>.Instance.Add(gameObject.transform, _info.dicKey);
			guideObject.isActive = false;
			guideObject.scaleSelect = 0.1f;
			guideObject.scaleRot = 0.05f;
			guideObject.isActiveFunc = (GuideObject.IsActiveFunc)Delegate.Combine(guideObject.isActiveFunc, new GuideObject.IsActiveFunc(oCIItem.OnSelect));
			guideObject.enableScale = loadInfo.isScale;
			guideObject.SetVisibleCenter(true);
			oCIItem.guideObject = guideObject;
			if (!loadInfo.childRoot.IsNullOrEmpty())
			{
				GameObject gameObject2 = gameObject.transform.FindLoop(loadInfo.childRoot);
				if ((bool)gameObject2)
				{
					oCIItem.childRoot = gameObject2.transform;
				}
			}
			if (oCIItem.childRoot == null)
			{
				oCIItem.childRoot = gameObject.transform;
			}
			oCIItem.animator = gameObject.GetComponent<Animator>();
			if ((bool)oCIItem.animator)
			{
				oCIItem.animator.enabled = loadInfo.isAnime;
			}
			oCIItem.itemComponent = gameObject.GetComponent<ItemComponent>();
			if (oCIItem.itemComponent != null)
			{
				oCIItem.itemComponent.SetFlag(loadInfo.color, loadInfo.pattren);
				oCIItem.itemComponent.SetLine();
				oCIItem.itemComponent.SetEmission();
				if (_addInfo)
				{
					Color[] defColorMain = oCIItem.itemComponent.defColorMain;
					for (int i = 0; i < 3; i++)
					{
						_info.color[i] = defColorMain[i];
					}
					defColorMain = oCIItem.itemComponent.defColorPattern;
					for (int j = 0; j < 3; j++)
					{
						_info.color[j + 3] = defColorMain[j];
					}
					_info.color[6] = oCIItem.itemComponent.defShadow;
					for (int k = 0; k < 3; k++)
					{
						_info.pattern[k].clamp = oCIItem.itemComponent.info[k].defClamp;
						_info.pattern[k].uv = oCIItem.itemComponent.info[k].defUV;
						_info.pattern[k].rot = oCIItem.itemComponent.info[k].defRot;
					}
					_info.lineColor = oCIItem.itemComponent.defLineColor;
					_info.lineWidth = oCIItem.itemComponent.defLineWidth;
					_info.emissionColor = oCIItem.itemComponent.defEmissionColor;
					_info.emissionPower = oCIItem.itemComponent.defEmissionPower;
					_info.lightCancel = oCIItem.itemComponent.defLightCancel;
				}
			}
			else
			{
				oCIItem.chaAccessoryComponent = gameObject.GetComponent<ChaAccessoryComponent>();
				if ((bool)oCIItem.chaAccessoryComponent && _addInfo)
				{
					_info.color[0] = oCIItem.chaAccessoryComponent.defColor01;
					_info.color[1] = oCIItem.chaAccessoryComponent.defColor02;
					_info.color[2] = oCIItem.chaAccessoryComponent.defColor03;
					_info.color[7] = oCIItem.chaAccessoryComponent.defColor04;
				}
			}
			oCIItem.particleComponent = gameObject.GetComponent<ParticleComponent>();
			if ((bool)oCIItem.particleComponent && _addInfo)
			{
				_info.color[0] = oCIItem.particleComponent.defColor01;
			}
			oCIItem.enableEmission = loadInfo.isEmission;
			oCIItem.iconComponent = gameObject.GetComponent<IconComponent>();
			oCIItem.panelComponent = gameObject.GetComponent<PanelComponent>();
			oCIItem.seComponent = gameObject.GetComponent<SEComponent>();
			if (_addInfo)
			{
				Studio.AddInfo(_info, oCIItem);
			}
			else
			{
				Studio.AddObjectCtrlInfo(oCIItem);
			}
			TreeNodeObject parent = ((_parentNode != null) ? _parentNode : ((_parent == null) ? null : _parent.treeNodeObject));
			TreeNodeObject treeNodeObject = Studio.AddNode(loadInfo.name, parent);
			treeNodeObject.treeState = _info.treeState;
			treeNodeObject.onVisible = (TreeNodeObject.OnVisibleFunc)Delegate.Combine(treeNodeObject.onVisible, new TreeNodeObject.OnVisibleFunc(oCIItem.OnVisible));
			treeNodeObject.enableVisible = true;
			treeNodeObject.visible = _info.visible;
			guideObject.guideSelect.treeNodeObject = treeNodeObject;
			oCIItem.treeNodeObject = treeNodeObject;
			if (!loadInfo.bones.IsNullOrEmpty())
			{
				oCIItem.itemFKCtrl = gameObject.AddComponent<ItemFKCtrl>();
				oCIItem.itemFKCtrl.InitBone(oCIItem, loadInfo, _addInfo);
			}
			else
			{
				oCIItem.itemFKCtrl = null;
			}
			if (_initialPosition == 1)
			{
				_info.changeAmount.pos = Singleton<Studio>.Instance.cameraCtrl.targetPos;
			}
			_info.changeAmount.OnChange();
			Studio.AddCtrlInfo(oCIItem);
			if (_parent != null)
			{
				_parent.OnLoadAttach((!(_parentNode != null)) ? _parent.treeNodeObject : _parentNode, oCIItem);
			}
			if ((bool)oCIItem.animator)
			{
				oCIItem.animator.speed = _info.animeSpeed;
				if (_info.animeNormalizedTime != 0f && oCIItem.animator.layerCount != 0)
				{
					oCIItem.animator.Update(1f);
					AnimatorStateInfo currentAnimatorStateInfo = oCIItem.animator.GetCurrentAnimatorStateInfo(0);
					oCIItem.animator.Play(currentAnimatorStateInfo.shortNameHash, 0, _info.animeNormalizedTime);
				}
			}
			oCIItem.SetupPatternTex();
			oCIItem.SetMainTex();
			oCIItem.UpdateColor();
			oCIItem.ActiveFK(oCIItem.itemInfo.enableFK);
			oCIItem.UpdateFKColor();
			oCIItem.ActiveDynamicBone(oCIItem.itemInfo.enableDynamicBone);
			return oCIItem;
		}

		private static Info.ItemLoadInfo GetLoadInfo(int _group, int _category, int _no)
		{
			Dictionary<int, Dictionary<int, Info.ItemLoadInfo>> value = null;
			if (!Singleton<Info>.Instance.dicItemLoadInfo.TryGetValue(_group, out value))
			{
				return null;
			}
			Dictionary<int, Info.ItemLoadInfo> value2 = null;
			if (!value.TryGetValue(_category, out value2))
			{
				return null;
			}
			Info.ItemLoadInfo value3 = null;
			if (!value2.TryGetValue(_no, out value3))
			{
				return null;
			}
			return value3;
		}
	}
}
