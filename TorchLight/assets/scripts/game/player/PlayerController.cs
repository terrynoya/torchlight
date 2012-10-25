using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour {

    public float MovementFactor = 10.0f;
    public float RotateFactor   = 100.0f;
    public Vector3 CameraOffset = new Vector3(3.0f, 3.0f, 10.0f);

    private bool bIsMoving = false;

    private Vector3 MoveDirection = Vector3.zero;
    private Vector3 TargetDirection = Vector3.zero;

    private AnimationController AnimController = null;

    private CharacterController CharactoerContllor = null;

    public Transform EffectSocket = null;
    public GameObject EffectPerfab = null;

    Quaternion screenMovementSpace;
	Vector3 screenMovementForward;
	Vector3 screenMovementRight;

    public bool IsMoving()
    {
        return bIsMoving;
    }

	// Use this for initialization
	void Start () 
    {
        MoveDirection = transform.TransformDirection(Vector3.forward);

        AnimController = GetComponent<AnimationController>();
        CharactoerContllor = GetComponent<CharacterController>();

        UpdateCamera();

        screenMovementSpace     = Quaternion.Euler(0, Camera.mainCamera.transform.eulerAngles.y, 0);
        screenMovementForward   = screenMovementSpace * Vector3.forward;
        screenMovementRight     = screenMovementSpace * Vector3.right;	
	}

    Vector3 GetTouchPosition()
    {
        Vector2 ScreenPos           = InputController.MousePosition();
        Vector3 CursorWorldPosition = InputController.ScreenPointToWorldPoint(ScreenPos);

        Vector3 TargetDirection = CursorWorldPosition - transform.position;
        TargetDirection.y = 0.0f;
        TargetDirection = TargetDirection.normalized;

        return TargetDirection;
    }

	void Update () 
    {
        UpdateSmoothRotation();

        UpdateAnimation();

        UpdateMovment();

        UpdateCamera();
	}

    void UpdateSmoothRotation()
    {
        Vector2 Movement = InputController.GetMoveDirection();

        //Vector3 FaceForward = transform.TransformDirection(Vector3.forward);
        //FaceForward.y = 0.0f;
        //FaceForward = FaceForward.normalized;

        //Vector3 FaceRight       = new Vector3(FaceForward.z, 0.0f, -FaceForward.x);

        TargetDirection = Movement.x * screenMovementRight + Movement.y * screenMovementForward;

        float RotateSpeedFactor = RotateFactor * Mathf.Deg2Rad * Time.deltaTime;

        if (InputController.IsScreenTouched())
        {
            TargetDirection = GetTouchPosition();
            RotateSpeedFactor *= 200;
        }

        bIsMoving = Mathf.Abs(Movement.x) > 0.1f || Mathf.Abs(Movement.y) > 0.1f;

        if (TargetDirection != Vector3.zero)
        {
            MoveDirection = Vector3.RotateTowards(MoveDirection, TargetDirection, RotateSpeedFactor, 1000);
            MoveDirection = MoveDirection.normalized;

            if (AnimController.IsSpecialAnimationFinished())
                transform.rotation = Quaternion.LookRotation(MoveDirection);
        }
    }

    void UpdateCamera()
    {
        Camera MainCamera = Camera.mainCamera;
        MainCamera.transform.position = transform.position + CameraOffset;
        MainCamera.transform.LookAt(transform);  
    }

    private Vector3 VerticalMovment = Vector3.zero;
    void UpdateMovment()
    {
        if (IsMoving() && AnimController.IsSpecialAnimationFinished())
        {
            // Apply gravity
            VerticalMovment.y -= 20.0f * Time.deltaTime;

            Vector3 Movment = (MoveDirection * MovementFactor + VerticalMovment) * Time.deltaTime;

            // Move the controller
            CollisionFlags flags = CharactoerContllor.Move(Movment);
	        bool grounded = (flags & CollisionFlags.CollidedBelow) != 0;
            if (grounded)
            {
                VerticalMovment.y = 0.0f;
            }
        }
    }

    void UpdateAnimation()
    {
        if (InputController.GetButton("Fire1"))
        {
            if (AnimController.IsSpecialAnimationFinished())
                AnimController.PlaySpecialAnimation("rwand1");
        }
    }

    public void SpecialAnimationEffect()
    {
        if (EffectSocket != null)
        {
            GameObject Effect = Instantiate(EffectPerfab, EffectSocket.position, EffectSocket.rotation) as GameObject;
            EffectController EffController = Effect.GetComponent<EffectController>();
            if (EffController != null)
                EffController.SetDiretion(EffectSocket.position - transform.position);
        }
    }
}
