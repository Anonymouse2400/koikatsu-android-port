using System;
using UnityEngine;
using UnityEngine.UI;

public class CustomShapeSample : MonoBehaviour
{
	[Serializable]
	public class CustomCtrl
	{
		private bool InitEnd;

		public GameObject objSample;

		private ShapeInfoBase sibSample;

		private float[] value;

		public bool CheckInitEnd()
		{
			return InitEnd;
		}

		public void Update()
		{
			if (sibSample != null)
			{
				sibSample.Update();
			}
		}

		public void SetValue(int no, float val)
		{
			value[no] = val;
			if (sibSample != null)
			{
				sibSample.ChangeValue(no, val);
			}
		}

		public float GetValue(int no)
		{
			return value[no];
		}

		public void Initialize()
		{
			sibSample = new ShapeInfoSample();
			int num = ShapeSampleDefine.shapename.Length;
			value = new float[num];
			if (sibSample != null && null != objSample)
			{
				sibSample.InitShapeInfo("sample.unity3d", "sample.unity3d", "anmShapeSample", "customSample", objSample.transform);
				for (int i = 0; i < num; i++)
				{
					SetValue(i, 0.5f);
				}
				sibSample.Update();
			}
			InitEnd = true;
		}
	}

	public CustomCtrl cctrl;

	public Transform trfPanel;

	private Slider[] sldCustom = new Slider[ShapeSampleDefine.shapename.Length];

	public Transform trfSample;

	public Transform trfDemo;

	private Animator anmDemo;

	public WireFrameRender wfr;

	private void Start()
	{
		if (cctrl != null)
		{
			cctrl.Initialize();
		}
		if ((bool)trfPanel)
		{
			Transform transform = null;
			for (int i = 0; i < ShapeSampleDefine.shapename.Length; i++)
			{
				transform = trfPanel.transform.Find("Parts" + i.ToString("00"));
				if (null == trfPanel)
				{
					continue;
				}
				Transform transform2 = transform.Find("Slider");
				if (!(null == transform2))
				{
					sldCustom[i] = transform2.GetComponent<Slider>();
					if (cctrl != null && cctrl.CheckInitEnd())
					{
						sldCustom[i].value = cctrl.GetValue(i);
					}
				}
			}
		}
		if ((bool)trfDemo)
		{
			anmDemo = trfDemo.GetComponent<Animator>();
			if ((bool)anmDemo)
			{
				anmDemo.Play(anmDemo.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, 0.5f);
			}
		}
	}

	public void OnWireFrameDraw(Toggle tgl)
	{
		if ((bool)wfr)
		{
			wfr.wireFrameDraw = tgl.isOn;
		}
	}

	public void OnObjectPosition(Toggle tgl)
	{
		float[] array = new float[2];
		if (tgl.isOn)
		{
			array[0] = -0.2f;
			array[1] = 0.1f;
		}
		else
		{
			array[0] = 0f;
			array[1] = 0f;
		}
		if ((bool)trfSample)
		{
			trfSample.position = new Vector3(array[0], 0f, 0f);
		}
		if ((bool)trfDemo)
		{
			trfDemo.position = new Vector3(array[1], 0f, 0f);
		}
	}

	public void OnPushButton(int id)
	{
		float num = 0f;
		switch (id)
		{
		case 1:
			num = 0.5f;
			break;
		case 2:
			num = 1f;
			break;
		}
		for (int i = 0; i < ShapeSampleDefine.shapename.Length; i++)
		{
			sldCustom[i].value = num;
		}
		if ((bool)anmDemo)
		{
			anmDemo.Play(anmDemo.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, num);
		}
	}

	private void Update()
	{
		if (cctrl == null || !cctrl.CheckInitEnd())
		{
			return;
		}
		for (int i = 0; i < ShapeSampleDefine.shapename.Length; i++)
		{
			if (null != sldCustom[i])
			{
				cctrl.SetValue(i, sldCustom[i].value);
			}
		}
		cctrl.Update();
	}
}
