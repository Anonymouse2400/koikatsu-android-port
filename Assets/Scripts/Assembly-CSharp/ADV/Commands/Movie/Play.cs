using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class Play : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    private RawImage rawImage;
    private UnityEngine.RenderTexture videoRenderTexture;  // Renamed variable to avoid conflicts

    public void SetVideo(VideoClip videoClip)
    {
        if (videoPlayer == null)
        {
            videoPlayer = gameObject.AddComponent<VideoPlayer>();
        }

        // Create UI Canvas
        GameObject canvasObject = new GameObject("VideoCanvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        // Create RawImage to display the video
        GameObject rawImageObject = new GameObject("VideoDisplay");
        rawImage = rawImageObject.AddComponent<RawImage>();
        rawImage.transform.SetParent(canvasObject.transform);
        rawImage.rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);

        // Create RenderTexture and use full namespace
        videoRenderTexture = new UnityEngine.RenderTexture(1920, 1080, 16);
        videoPlayer.targetTexture = videoRenderTexture;
        rawImage.texture = videoRenderTexture;

        // Configure VideoPlayer
        videoPlayer.clip = videoClip;
        videoPlayer.playOnAwake = false;
        videoPlayer.isLooping = true;
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;

        // Play when ready
        videoPlayer.Prepare();
        videoPlayer.prepareCompleted += (vp) => { vp.Play(); };
    }

    public bool IsPlaying()
    {
        return videoPlayer != null && videoPlayer.isPlaying;
    }
}
