using UnityEngine;

public class MobSwim : MonoBehaviour
{
	[SerializeField]
	private Animator anmMob;

	[SerializeField]
	private Animator anmNull;

	[SerializeField]
	private float min = 0.8f;

	[SerializeField]
	private float max = 1.2f;

	private void Proc()
	{
		string[] array = new string[2] { "move_00_00", "move_00_01" };
		float normalizedTime = Random.Range(0f, 0.8f);
		float value = Random.Range(min, max);
		anmMob.SetFloat("MotionSpeed", value);
		anmNull.SetFloat("MotionSpeed", value);
		anmNull.Play(array[Random.Range(0, array.Length)], 0, normalizedTime);
	}

	private void Start()
	{
		Proc();
	}

	private void OnEnable()
	{
		Proc();
	}
}
