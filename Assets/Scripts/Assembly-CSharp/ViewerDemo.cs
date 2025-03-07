using UnityEngine;

[RequireComponent(typeof(EMTransition))]
public class ViewerDemo : MonoBehaviour
{
	[SerializeField]
	private Texture2D[] gradations;

	[SerializeField]
	private Texture2D[] textures;

	[SerializeField]
	private int current;

	private EMTransition emTransition;

	private void Start()
	{
		emTransition = GetComponent<EMTransition>();
		emTransition.SetGradationTexture(gradations[current]);
	}

	private void Update()
	{
		if ((bool)gradations[current])
		{
			if (Input.GetKeyDown(KeyCode.LeftArrow))
			{
				current = ((current-- <= 0) ? (gradations.Length - 1) : current);
				emTransition.SetGradationTexture(gradations[current]);
			}
			if (Input.GetKeyDown(KeyCode.RightArrow))
			{
				current = ((++current < gradations.Length) ? current : 0);
				emTransition.SetGradationTexture(gradations[current]);
			}
		}
	}

	private void OnGUI()
	{
		if ((bool)gradations[current])
		{
			GUI.Label(new Rect(20f, 10f, 100f, 20f), "GRADATION:");
			if (GUI.Button(new Rect(110f, 10f, 30f, 20f), "<"))
			{
				current = ((current-- <= 0) ? (gradations.Length - 1) : current);
				emTransition.SetGradationTexture(gradations[current]);
			}
			if (GUI.Button(new Rect(150f, 10f, 30f, 20f), ">"))
			{
				current = ((++current < gradations.Length) ? current : 0);
				emTransition.SetGradationTexture(gradations[current]);
			}
			GUI.Label(new Rect(190f, 10f, 200f, 20f), gradations[current].name + " / 040");
			GUI.Label(new Rect(20f, 40f, 100f, 20f), "COLOR:");
			if (GUI.Button(new Rect(110f, 40f, 80f, 20f), "black"))
			{
				emTransition.SetColor(Color.black);
			}
			if (GUI.Button(new Rect(200f, 40f, 80f, 20f), "white"))
			{
				emTransition.SetColor(Color.white);
			}
			if (GUI.Button(new Rect(290f, 40f, 80f, 20f), "red"))
			{
				emTransition.SetColor(Color.red);
			}
			if (GUI.Button(new Rect(380f, 40f, 80f, 20f), "green"))
			{
				emTransition.SetColor(Color.green);
			}
			if (GUI.Button(new Rect(470f, 40f, 80f, 20f), "blue"))
			{
				emTransition.SetColor(Color.blue);
			}
			if (GUI.Button(new Rect(560f, 40f, 80f, 20f), "random"))
			{
				Color color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
				emTransition.SetColor(color);
			}
			GUI.Label(new Rect(20f, 70f, 100f, 20f), "TEXTURE:");
			if (GUI.Button(new Rect(110f, 70f, 80f, 20f), "none"))
			{
				emTransition.SetTexture(textures[0]);
			}
			if (GUI.Button(new Rect(200f, 70f, 80f, 20f), "tile"))
			{
				emTransition.SetTexture(textures[1]);
				emTransition.SetColor(Color.white);
			}
			if (GUI.Button(new Rect(290f, 70f, 80f, 20f), "wood"))
			{
				emTransition.Play();
			}
		}
	}
}
