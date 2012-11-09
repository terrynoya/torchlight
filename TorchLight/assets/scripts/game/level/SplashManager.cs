using UnityEngine;
using System.Collections.Generic;

public class SplashManager : MonoBehaviour
{

    public List<Texture2D> SplashImages = new List<Texture2D>();

	// Use this for initialization
	void Start () {
        if (guiTexture == null)
        {
            Debug.LogError("Splash Manager MUST Attach to a GUITexture");
            return;
        }

        if (SplashImages.Count > 0)
        {
            guiTexture.transform.position = Vector3.zero;
            guiTexture.pixelInset = new Rect(0.0f, 0.0f, Screen.width, Screen.height);
            guiTexture.texture = SplashImages[Random.Range(0, SplashImages.Count)];
        }
	}

    public void HideSplash()
    {
        gameObject.SetActiveRecursively(false);
    }
}
