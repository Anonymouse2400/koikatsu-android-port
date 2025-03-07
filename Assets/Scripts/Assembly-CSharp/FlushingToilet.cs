using System.Collections;
using UnityEngine;

public class FlushingToilet : MonoBehaviour
{
	public GameObject ToiletWaterAnim;

	public GameObject ToiletWaterSurface;

	public GameObject Toilet;

	public AudioSource FlushAudio;

	private int FlushDone;

	private void Start()
	{
		ToiletWaterAnim.SetActive(false);
	}

	private void Update()
	{
		if (Input.GetButtonDown("Fire1") && FlushDone == 0)
		{
			StartCoroutine("ToiletFlush");
		}
	}

	private IEnumerator ToiletFlush()
	{
		FlushDone = 1;
		FlushAudio.Play();
		Toilet.GetComponent<Animation>().Play();
		yield return new WaitForSeconds(0.25f);
		ToiletWaterAnim.SetActive(false);
		ToiletWaterAnim.SetActive(true);
		ToiletWaterSurface.GetComponent<Animation>().Play();
		yield return new WaitForSeconds(5f);
		FlushDone = 0;
	}
}
