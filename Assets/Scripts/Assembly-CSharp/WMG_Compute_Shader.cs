using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class WMG_Compute_Shader : MonoBehaviour
{
	public ComputeShader computeShader;

	public float[] pointVals = new float[4000];

	private int kernelHandle;

	private RenderTexture renderTexture;

	private int texSize = 512;

	private RawImage rawImg;

	private bool hasInit;

	public void Init()
	{
		if (!hasInit)
		{
			hasInit = true;
			kernelHandle = computeShader.FindKernel("CSMain");
			rawImg = base.gameObject.GetComponent<RawImage>();
			renderTexture = new RenderTexture(texSize, texSize, 24);
			renderTexture.enableRandomWrite = true;
			renderTexture.Create();
		}
	}

	private void Start()
	{
		Init();
	}

	public void dispatchAndUpdateImage()
	{
		computeShader.SetInt("texSize", texSize);
		computeShader.SetTexture(kernelHandle, "Result", renderTexture);
		computeShader.Dispatch(kernelHandle, texSize / 8, texSize / 8, 1);
		rawImg.texture = renderTexture;
	}
}
