using UnityEngine;

public class DemoUI : MonoBehaviour
{
	protected SSAOPro m_SSAOPro;

	private void Start()
	{
		m_SSAOPro = GetComponent<SSAOPro>();
	}

	private void OnGUI()
	{
		GUI.Box(new Rect(10f, 10f, 130f, 194f), string.Empty);
		GUI.BeginGroup(new Rect(20f, 15f, 200f, 200f));
		m_SSAOPro.enabled = GUILayout.Toggle(m_SSAOPro.enabled, "Enable SSAO");
		m_SSAOPro.DebugAO = GUILayout.Toggle(m_SSAOPro.DebugAO, "Show AO Only");
		bool value = m_SSAOPro.Blur == SSAOPro.BlurMode.HighQualityBilateral;
		value = GUILayout.Toggle(value, "HQ Bilateral Blur");
		m_SSAOPro.Blur = (value ? SSAOPro.BlurMode.HighQualityBilateral : SSAOPro.BlurMode.None);
		GUILayout.Space(10f);
		bool value2 = m_SSAOPro.Samples == SSAOPro.SampleCount.VeryLow;
		value2 = GUILayout.Toggle(value2, "4 samples");
		m_SSAOPro.Samples = ((!value2) ? m_SSAOPro.Samples : SSAOPro.SampleCount.VeryLow);
		value2 = m_SSAOPro.Samples == SSAOPro.SampleCount.Low;
		value2 = GUILayout.Toggle(value2, "8 samples");
		m_SSAOPro.Samples = (value2 ? SSAOPro.SampleCount.Low : m_SSAOPro.Samples);
		value2 = m_SSAOPro.Samples == SSAOPro.SampleCount.Medium;
		value2 = GUILayout.Toggle(value2, "12 samples");
		m_SSAOPro.Samples = ((!value2) ? m_SSAOPro.Samples : SSAOPro.SampleCount.Medium);
		value2 = m_SSAOPro.Samples == SSAOPro.SampleCount.High;
		value2 = GUILayout.Toggle(value2, "16 samples");
		m_SSAOPro.Samples = ((!value2) ? m_SSAOPro.Samples : SSAOPro.SampleCount.High);
		value2 = m_SSAOPro.Samples == SSAOPro.SampleCount.Ultra;
		value2 = GUILayout.Toggle(value2, "20 samples");
		m_SSAOPro.Samples = ((!value2) ? m_SSAOPro.Samples : SSAOPro.SampleCount.Ultra);
		GUI.EndGroup();
	}
}
