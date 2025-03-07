using Manager;
using UnityEngine;

namespace Studio
{
	public class OCILight : ObjectCtrlInfo
	{
		public GameObject objectLight;

		protected Light m_Light;

		public Info.LightLoadInfo.Target lightTarget;

		public LightColor lightColor;

		public OILightInfo lightInfo
		{
			get
			{
				return objectInfo as OILightInfo;
			}
		}

		public Light light
		{
			get
			{
				if (m_Light == null)
				{
					m_Light = objectLight.GetComponentInChildren<Light>();
				}
				return m_Light;
			}
		}

		public LightType lightType
		{
			get
			{
				return (!(light != null)) ? LightType.Directional : light.type;
			}
		}

		public void SetColor(Color _color)
		{
			lightInfo.color = _color;
			light.color = lightInfo.color;
			if ((bool)lightColor)
			{
				lightColor.color = lightInfo.color;
			}
		}

		public bool SetIntensity(float _value, bool _force = false)
		{
			if (!Utility.SetStruct(ref lightInfo.intensity, _value) && !_force)
			{
				return false;
			}
			if ((bool)light)
			{
				light.intensity = lightInfo.intensity;
			}
			return true;
		}

		public bool SetRange(float _value, bool _force = false)
		{
			if (!Utility.SetStruct(ref lightInfo.range, _value) && !_force)
			{
				return false;
			}
			if ((bool)light)
			{
				light.range = lightInfo.range;
			}
			return true;
		}

		public bool SetSpotAngle(float _value, bool _force = false)
		{
			if (!Utility.SetStruct(ref lightInfo.spotAngle, _value) && !_force)
			{
				return false;
			}
			if ((bool)light)
			{
				light.spotAngle = lightInfo.spotAngle;
			}
			return true;
		}

		public bool SetEnable(bool _value, bool _force = false)
		{
			if (!Utility.SetStruct(ref lightInfo.enable, _value) && !_force)
			{
				return false;
			}
			if ((bool)light)
			{
				light.enabled = lightInfo.enable;
			}
			return true;
		}

		public bool SetDrawTarget(bool _value, bool _force = false)
		{
			if (!Utility.SetStruct(ref lightInfo.drawTarget, _value) && !_force)
			{
				return false;
			}
			Singleton<GuideObjectManager>.Instance.drawLightLine.SetEnable(light, lightInfo.drawTarget);
			guideObject.visible = lightInfo.drawTarget;
			return true;
		}

		public bool SetShadow(bool _value, bool _force = false)
		{
			if (!Utility.SetStruct(ref lightInfo.shadow, _value) && !_force)
			{
				return false;
			}
			if ((bool)light)
			{
				light.shadows = (lightInfo.shadow ? LightShadows.Soft : LightShadows.None);
			}
			return true;
		}

		public void Update()
		{
			SetColor(lightInfo.color);
			SetIntensity(lightInfo.intensity, true);
			SetRange(lightInfo.range, true);
			SetSpotAngle(lightInfo.spotAngle, true);
			SetEnable(lightInfo.enable, true);
			SetDrawTarget(lightInfo.drawTarget, true);
			SetShadow(lightInfo.shadow, true);
		}

		public override void OnDelete()
		{
			Singleton<GuideObjectManager>.Instance.Delete(guideObject);
			Object.Destroy(objectLight);
			if (parentInfo != null)
			{
				parentInfo.OnDetachChild(this);
			}
			Studio.DeleteInfo(objectInfo);
		}

		public override void OnAttach(TreeNodeObject _parent, ObjectCtrlInfo _child)
		{
		}

		public override void OnLoadAttach(TreeNodeObject _parent, ObjectCtrlInfo _child)
		{
		}

		public override void OnDetach()
		{
			parentInfo.OnDetachChild(this);
			guideObject.parent = null;
			Studio.AddInfo(objectInfo, this);
			objectLight.transform.SetParent(Singleton<Scene>.Instance.commonSpace.transform);
			objectInfo.changeAmount.pos = objectLight.transform.localPosition;
			objectInfo.changeAmount.rot = objectLight.transform.localEulerAngles;
			treeNodeObject.ResetVisible();
		}

		public override void OnDetachChild(ObjectCtrlInfo _child)
		{
		}

		public override void OnSelect(bool _select)
		{
		}

		public override void OnSavePreprocessing()
		{
			base.OnSavePreprocessing();
		}

		public override void OnVisible(bool _visible)
		{
		}
	}
}
