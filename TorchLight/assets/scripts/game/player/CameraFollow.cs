using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

    // The target we are following
    public Transform Target = null;
    public Camera BindCamera = null;

    public bool bLockRotation = false;

    // The distance in the x-z plane to the target
    public float Distance = 10.0f;
    // the height we want the camera to be above the target
    public float Height = 15.0f;
    // How much we 
    float HeightDamping = 2.0f;
    float RotationDamping = 3.0f;

	// Use this for initialization
	void Start () {

	}

    public void Init(Transform InTarget)
    {
        Target = InTarget;

        GameObject CamObj = new GameObject("CharactorCamera");
        CamObj.transform.rotation = Quaternion.Euler(new Vector3(90.0f, 0.0f, 0.0f));
        CamObj.transform.position = InTarget.position + new Vector3(0.0f, Height, 0.0f);
        BindCamera = CamObj.AddComponent<Camera>();
        BindCamera.backgroundColor = Color.black;

        // We need add a component for drawing GUI
        CamObj.AddComponent<GUILayer>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void LateUpdate () {
	    // Early out if we don't have a target
        if (!Target)
		    return;

        Transform Trans = BindCamera.transform;
	
	    // Calculate the current rotation angles
        float WantedRotationAngle = Target.eulerAngles.y;
        float WantedHeight = Target.position.y + Height;

        float CurrentRotationAngle = Trans.eulerAngles.y;
        float CurrentHeight = Trans.position.y;
	
	    // Damp the rotation around the y-axis
	    CurrentRotationAngle = Mathf.LerpAngle (CurrentRotationAngle, WantedRotationAngle, RotationDamping * Time.deltaTime);

	    // Damp the height
	    CurrentHeight = Mathf.Lerp (CurrentHeight, WantedHeight, HeightDamping * Time.deltaTime);

	    // Convert the angle into a rotation
	    Quaternion CurrentRotation = Quaternion.Euler (0, CurrentRotationAngle, 0);
	
	    // Set the position of the camera on the x-z plane to:
	    // distance meters behind the target
        Trans.position = Target.position;
        Trans.position -= Vector3.forward * Distance;

	    // Set the height of the camera
        Vector3 Position = Trans.position;
        Position.y = CurrentHeight;
        Trans.position = Position;
	
	    // Always look at the target
        Trans.LookAt(Target);
    }
}
