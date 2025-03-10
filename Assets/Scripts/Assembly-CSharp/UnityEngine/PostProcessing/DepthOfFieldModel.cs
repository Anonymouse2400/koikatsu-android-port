using System;

namespace UnityEngine.PostProcessing
{
	[Serializable]
	public class DepthOfFieldModel : PostProcessingModel
	{
		public enum KernelSize
		{
			Small = 0,
			Medium = 1,
			Large = 2,
			VeryLarge = 3
		}

		[Serializable]
		public struct Settings
		{
			[Tooltip("Distance to the point of focus.")]
			[Min(0.1f)]
			public float focusDistance;

			[Tooltip("Ratio of aperture (known as f-stop or f-number). The smaller the value is, the shallower the depth of field is.")]
			[Range(0.05f, 32f)]
			public float aperture;

			[Tooltip("Distance between the lens and the film. The larger the value is, the shallower the depth of field is.")]
			[Range(1f, 300f)]
			public float focalLength;

			[Tooltip("Calculate the focal length automatically from the field-of-view value set on the camera. Using this setting isn't recommended.")]
			public bool useCameraFov;

			[Tooltip("Convolution kernel size of the bokeh filter, which determines the maximum radius of bokeh. It also affects the performance (the larger the kernel is, the longer the GPU time is required).")]
			public KernelSize kernelSize;

			public static Settings defaultSettings
			{
				get
				{
					Settings result = default(Settings);
					result.focusDistance = 10f;
					result.aperture = 5.6f;
					result.focalLength = 50f;
					result.useCameraFov = false;
					result.kernelSize = KernelSize.Medium;
					return result;
				}
			}
		}

		[SerializeField]
		private Settings m_Settings = Settings.defaultSettings;

		public Settings settings
		{
			get
			{
				return m_Settings;
			}
			set
			{
				m_Settings = value;
			}
		}

		public override void Reset()
		{
			m_Settings = Settings.defaultSettings;
		}
	}
}
