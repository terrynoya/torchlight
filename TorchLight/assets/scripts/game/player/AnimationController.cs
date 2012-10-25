using UnityEngine;
using System.Collections;

public class AnimationController : MonoBehaviour {

    public string               RunAnimName   = "run";
    public string               IdleAnimName  = "idle";

    private AnimationState      LastSpecialAnimState = null;
    private PlayerController    PlayerController = null;


	// Use this for initialization
	void Start () 
    {
        PlayerController    = GetComponent<PlayerController>();

        animation.Stop();
        animation.wrapMode  = WrapMode.Once;

        AnimationState AnimState = null;
        AnimState           = animation[RunAnimName];
        AnimState.wrapMode  = WrapMode.Loop;
        AnimState.layer     = -1;

        AnimState           = animation[IdleAnimName];
        AnimState.wrapMode  = WrapMode.Loop;
        AnimState.layer     = -1;

        animation.SyncLayer(-1);
	}
	
	// Update is called once per frame
	void Update()
    {
        if (PlayerController.IsMoving())
            PlayAnimation(RunAnimName);
        else
            PlayAnimation(IdleAnimName);
	}

    public bool CheckAnimation(string AnimName)
    {
        if (animation.GetClip(AnimName) == null)
        {
            Debug.Log(AnimName + " Not Found");
            return false;
        }
        return true;
    }

    public void PlaySpecialAnimation(string AnimName)
    {
        LastSpecialAnimState = animation.CrossFadeQueued(AnimName, 0.3f, QueueMode.PlayNow);
    }

    public bool IsSpecialAnimationFinished()
    {
        return LastSpecialAnimState == null || LastSpecialAnimState.time > LastSpecialAnimState.length - 0.1f;
    }

    public void PlayAnimation(string AnimName)
    {
        animation.CrossFade(AnimName);
    }

    public bool IsPlaying(string AnimName)
    {
        return animation.IsPlaying(AnimName);
    }
}
