using UnityEngine;
using System.Collections.Generic;

public class SplashManager : MonoBehaviour
{

    public List<Texture2D> SplashImages = new List<Texture2D>();

    private Camera BindCamera = null;

	// Use this for initialization
	void Start () {
        if (guiTexture == null)
        {
            Debug.LogError("Splash Manager MUST Attach to a GUITexture");
            return;
        }

        //BindCamera = gameObject.GetComponent<Camera>();
        //if (BindCamera == null)
        //    BindCamera = gameObject.AddComponent<Camera>();
        //BindCamera.transform.position = new Vector3(-999999.0f, -999999.0f, -999999.0f);

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
