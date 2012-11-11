using UnityEngine;
using System.Collections;

public class MiniMap : MonoBehaviour {

    public bool bEnableMinimap = true;

    /// <summary>
    /// Minimap (0, 0, 0) is TopLeft Corner
    /// </summary>
    public float CenterXInImage = 800.0f;   // 960, 640
    public float CenterYInImage = 320.0f;

    public float ChunkSizeInImage = 320;
    public float ChunkSizeInScene = 100;

    Transform CharactorTransform = null;

    private float SceneX = 1.0f;
    private float SceneY = 1.0f;

    private float ImageWidth = 1.0f;
    private float ImageHeight = 1.0f;

    private float ImageCenterX = 1.0f;
    private float ImageCenterY = 1.0f;

    private float ImageOffsetX = 0.0f;
    private float ImageOffsetY = 0.0f;

	// Use this for initialization
	void Start () {
        ImageWidth = guiTexture.texture.width;
        ImageHeight = guiTexture.texture.height;

        ImageOffsetX = Screen.width / 2 - ImageWidth / 2;
        ImageOffsetY = Screen.height / 2 - ImageHeight / 2;

        float ChunkXNum = ImageWidth / ChunkSizeInImage;
        float ChunkYNum = ImageHeight / ChunkSizeInImage;

        SceneX = ChunkXNum * ChunkSizeInScene;
        SceneY = ChunkYNum * ChunkSizeInScene;

        ImageCenterX = CenterXInImage / ImageWidth;
        ImageCenterY = CenterYInImage / ImageHeight;

        guiTexture.pixelInset = new Rect(0, 0, ImageWidth, ImageHeight);
        gameObject.transform.position = new Vector3(ImageOffsetX / Screen.width, ImageOffsetY / Screen.height, 0.0f);
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (CharactorTransform == null)
        {
            CameraFollow Follower = FindObjectOfType(typeof(CameraFollow)) as CameraFollow;
            CharactorTransform = Follower.Target;
        }
	}
	
    void OnGUI()
    {
        if (bEnableMinimap && CharactorTransform != null)
        {
            Vector3 CurPos = CharactorTransform.position;

            float PosX = ImageOffsetX + (ImageCenterX + CurPos.x / SceneX) * ImageWidth;
            float PosY = ImageOffsetY + (ImageCenterY - CurPos.z / SceneY) * ImageHeight;
			
			GUI.Label(new Rect(PosX - 10.0f, PosY - 10.0f, 20.0f, 20.0f), "+");
        }
    }
}
