using UnityEngine;
using System.Collections;

public class AnimationController : MonoBehaviour {

    private AnimationState LastSpecialAnimState = null;
    private PlayerController PlayerController = null;


	// Use this for initialization
	void Start () 
    {
        PlayerController = GetComponent<PlayerController>();
	}
	
	// Update is called once per frame
	void Update()
    {
        if (IsSpecialAnimationFinished())
        {
            if (PlayerController.IsMoving())
                PlayAnimation("run");
            else
                PlayAnimation("idle");
        }
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
        LastSpecialAnimState = animation.CrossFadeQueued(AnimName, 0.1f, QueueMode.PlayNow);
    }

    public bool IsSpecialAnimationFinished()
    {
        return LastSpecialAnimState == null || LastSpecialAnimState.time > LastSpecialAnimState.length * 0.9f;
    }

    public void PlayAnimation(string AnimName)
    {
        animation.CrossFade(AnimName, 0.1f);
    }

    public bool IsPlaying(string AnimName)
    {
        return animation.IsPlaying(AnimName);
    }
}
