using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class CharactorInputControllor : MonoBehaviour {

    private Camera CharactorCamera  = null;
    private NavMeshAgent NavAgent   = null;
    public Vector3 TargetPosition  = Vector3.zero;

    private AnimationController AnimControl = null;
	// Use this for initialization
	void Start () {
        NavAgent        = gameObject.GetComponent<NavMeshAgent>();
        CharactorCamera = gameObject.GetComponent<CameraFollow>().BindCamera;
        AnimControl     = gameObject.GetComponent<AnimationController>();

        StartCoroutine(StartNavigation());
	}
	
	// Update is called once per frame
	void Update () {
	    if(Input.GetMouseButtonDown(0)) 
        {
            RaycastHit HitInfo;
            if (RayCast(CharactorCamera, Input.mousePosition, out HitInfo)) 
            {
                UpdateAgentTarget(HitInfo.point);
            }
        }
	}

    static Vector3 GizmoSize = new Vector3(0.5f, 0.5f, 0.5f);
    void OnDrawGizmos()
    {
        Gizmos.DrawCube(TargetPosition, GizmoSize);
    }

    bool RayCast(Camera Cam, Vector3 MousePosition, out RaycastHit Hit)
    {
        Ray ARay = Cam.ScreenPointToRay(MousePosition);
        return Physics.Raycast(ARay.origin, ARay.direction, out Hit); 
    }

    //////////////////////////////////////////////////////////////////////////
    // For NavMesh
    //////////////////////////////////////////////////////////////////////////

    private string CurNavigationState = "NavigationState_Strand";

    IEnumerator StartNavigation()
    {
        Debug.Log("Start Navigation");
        while(Application.isPlaying)
            yield return StartCoroutine(CurNavigationState);
    }

    void UpdateAgentTarget(Vector3 TargetPos)
    {
        NavAgent.destination = TargetPos;
        TargetPosition = TargetPos;
    }

    IEnumerator NavigationState_Strand()
    {
        do
        {
            UpdateAnimation();
            yield return null;
        } while (NavAgent.remainingDistance == 0.0f);

        CurNavigationState = "NavigationState_Move";
    }

    IEnumerator NavigationState_Move()
    {
        do
        {
            UpdateAnimation();
            yield return null;
        } while (NavAgent.remainingDistance != 0.0f);

        CurNavigationState = "NavigationState_Strand";
    }

    void UpdateAnimation()
    {
        Vector3 velocityXZ = new Vector3(NavAgent.velocity.x, 0.0f, NavAgent.velocity.z);
	    float speed  = velocityXZ.magnitude;

        if (speed > 0.1f)
            AnimControl.PlayAnimation(AnimControl.RunAnimName);
        else
            AnimControl.PlayAnimation(AnimControl.IdleAnimName);
    }
}
