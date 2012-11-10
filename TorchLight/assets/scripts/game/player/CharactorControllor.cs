using UnityEngine;
using System.Collections;

public class CharactorControllor : MonoBehaviour {

	// Use this for initialization
	void Start () {

        CameraFollow CamFollow                      = gameObject.AddComponent<CameraFollow>();
        CamFollow.Init(transform);

        AnimationController AnimControllor          = gameObject.AddComponent<AnimationController>();

        CharactorInputControllor InputControllor    = gameObject.AddComponent<CharactorInputControllor>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
