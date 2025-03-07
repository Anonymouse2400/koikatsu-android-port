using System.Collections;
using UnityEngine;

namespace H
{
	public class Toilet : MonoBehaviour
	{
		public GameObject ToiletWaterAnim;

		public Animation animToiletWaterSurface;

		public Animation animToilet;

		private int FlushDone;

		private void Start()
		{
			ToiletWaterAnim.SetActive(false);
		}

		private IEnumerator ToiletFlush()
		{
			FlushDone = 1;
			animToilet.Play();
			yield return new WaitForSeconds(0.25f);
			ToiletWaterAnim.SetActive(false);
			ToiletWaterAnim.SetActive(true);
			animToiletWaterSurface.Play();
			yield return new WaitForSeconds(5f);
			FlushDone = 0;
		}

		public void ToiletStart()
		{
			if (FlushDone == 0)
			{
				StartCoroutine("ToiletFlush");
			}
		}
	}
}
