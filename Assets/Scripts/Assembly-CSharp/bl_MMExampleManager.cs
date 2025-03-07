using UnityEngine;
using UnityEngine.SceneManagement;

public class bl_MMExampleManager : MonoBehaviour
{
	public int MapID = 2;

	public const string MMName = "MMManagerExample";

	public GameObject[] Maps;

	private bool Rotation = true;

	public bl_MiniMap GetActiveMiniMap
	{
		get
		{
			return Maps[MapID].GetComponentInChildren<bl_MiniMap>();
		}
	}

	private void Awake()
	{
		MapID = PlayerPrefs.GetInt("MMExampleMapID", 0);
		ApplyMap();
	}

	private void ApplyMap()
	{
		for (int i = 0; i < Maps.Length; i++)
		{
			Maps[i].SetActive(false);
		}
		Maps[MapID].SetActive(true);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.R))
		{
			ChangeRotation();
		}
	}

	private void ChangeRotation()
	{
		Rotation = !Rotation;
		Maps[MapID].GetComponentInChildren<bl_MiniMap>().RotationMap(Rotation);
	}

	public void ChangeMap(int i)
	{
		PlayerPrefs.SetInt("MMExampleMapID", i);
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
	}
}
