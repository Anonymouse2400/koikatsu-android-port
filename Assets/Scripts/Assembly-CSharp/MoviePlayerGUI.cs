using UnityEngine;
using UnityEngine.Video;

public class MoviePlayerGUI : MonoBehaviour
{
    public VideoPlayer movie;
    private RenderTexture renderTexture;

    private void Start()
    {
        
        if (movie == null)
        {
            movie = gameObject.AddComponent<VideoPlayer>();
            movie.Play();
        }

        // Create a RenderTexture
        renderTexture = new RenderTexture(Screen.width, Screen.height, 16);
        movie.targetTexture = renderTexture;

        // Set video properties
        movie.playOnAwake = false;
        movie.isLooping = true;
        movie.renderMode = VideoRenderMode.RenderTexture;

        // Set video file (must be placed inside StreamingAssets folder)
        movie.url = "file://" + Application.streamingAssetsPath + "/video.mp4";

        // Play the video
        movie.Prepare();
        movie.prepareCompleted += (vp) => { vp.Play(); };
    }

    private void OnGUI()
    {
        if (movie.isPlaying && renderTexture != null)
        {
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), renderTexture);
        }
    }
}
