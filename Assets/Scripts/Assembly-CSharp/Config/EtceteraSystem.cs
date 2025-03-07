using UnityEngine;

namespace Config
{
	public class EtceteraSystem : BaseSystem
	{
		public bool Look = true;

		public bool SelfShadow = true;

		public bool DepthOfField = true;

		public bool Map = true;

		public int rampId = 1;

		public float shadowDepth = 0.26f;

		public float lineDepth = 1f;

		public float lineWidth = 0.307f;

		public bool ForegroundEyebrow;

		public bool ForegroundEyes;

		public bool SimpleBody;

		public bool VisibleSon = true;

		public bool VisibleBody = true;

		public bool hohoAka;

		public bool loadHeadAccessory = true;

		public bool loadAllAccessory;

		public Color SilhouetteColor = Color.blue;

		public int SemenType = 1;

		public bool IsMaleClothes = true;

		public bool IsMaleAccessoriesMain = true;

		public bool IsMaleAccessoriesSub = true;

		public bool IsMaleShoes = true;

		public bool FemaleEyesCamera;

		public bool FemaleNeckCamera;

		public bool FemaleEyesCamera1;

		public bool FemaleNeckCamera1;

		public bool HInitCamera = true;

		public bool Shield = true;

		public Color BackColor = Color.black;

		public float nipMax = 0.4f;

		public bool Fog = true;

		public bool Bloom = true;

		public bool Vignette = true;

		public bool AmplifyOcclus = true;

		public bool SunShafts = true;

		public EtceteraSystem(string elementName)
			: base(elementName)
		{
		}

		public override void Init()
		{
			Look = true;
			SelfShadow = true;
			DepthOfField = true;
			Map = true;
			rampId = 1;
			shadowDepth = 0.26f;
			lineDepth = 1f;
			lineWidth = 0.307f;
			ForegroundEyebrow = false;
			ForegroundEyes = false;
			SimpleBody = false;
			VisibleSon = true;
			VisibleBody = true;
			hohoAka = false;
			loadHeadAccessory = true;
			loadAllAccessory = false;
			SilhouetteColor = Color.blue;
			SemenType = 1;
			IsMaleClothes = true;
			IsMaleAccessoriesMain = true;
			IsMaleAccessoriesSub = true;
			IsMaleShoes = true;
			FemaleEyesCamera = false;
			FemaleNeckCamera = false;
			FemaleEyesCamera1 = false;
			FemaleNeckCamera1 = false;
			HInitCamera = true;
			Shield = true;
			BackColor = Color.black;
			Fog = true;
			Bloom = true;
			Vignette = true;
			AmplifyOcclus = true;
			SunShafts = true;
			nipMax = 0.4f;
		}
	}
}
