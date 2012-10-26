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
    private Vector3 TargetOritenation = Vector3.zero;

    private AnimationController AnimController      = null;
    private CharacterController CharactoerContllor  = null;

    private Quaternion ScreenMovementSpace = Quaternion.identity;
    private Vector3 ScreenMovementForward = Vector3.zero;
    private Vector3 ScreenMovementRight = Vector3.zero;

    public bool IsMoving()
    {
        return bIsMoving;
    }

	// Use this for initialization
	void Start () 
    {
        MoveDirection = transform.TransformDirection(Vector3.forward);

        AnimController      = GetComponent<AnimationController>();
        CharactoerContllor  = GetComponent<CharacterController>();

        UpdateCamera();

        ScreenMovementSpace     = Quaternion.Euler(0, Camera.mainCamera.transform.eulerAngles.y, 0);
        ScreenMovementForward   = ScreenMovementSpace * Vector3.forward;
        ScreenMovementRight     = ScreenMovementSpace * Vector3.right;	
	}

	void Update () 
    {
        UpdateSmoothRotation();

        UpdateMovment();

        UpdateAnimation();

        DrawDebug();
	}

    void LateUpdate()
    {
        UpdateCamera();
    }

    bool IsFinishRotating()
    {
        return Vector3.Angle(MoveDirection, TargetOritenation) < 1.0f;
    }

    void UpdateSmoothRotation()
    {
        if (InputController.IsScreenTouched())
        {
            TargetOritenation = InputController.GetCursorOffsetInWorldPosition(transform.position);
        }

        if (!IsFinishRotating())
        {
            if (AnimController.IsSpecialAnimationFinished())
                transform.rotation = Quaternion.LookRotation(Vector3.Lerp(MoveDirection, TargetOritenation, Time.deltaTime * 15));
        }
    }

    bool bPlaySpecialAnimation = false;
    void UpdateAnimation()
    {
        if (InputController.GetButton("Fire1"))
        {
            bPlaySpecialAnimation = true;
        }

        if (bPlaySpecialAnimation && IsFinishRotating())
        {
            if (AnimController.IsSpecialAnimationFinished())
                AnimController.PlaySpecialAnimation("rwand1", 0.2f);

            bPlaySpecialAnimation = false;
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
        Vector2 Movement = InputController.GetMoveDirection();

        TargetDirection = Movement.x * ScreenMovementRight + Movement.y * ScreenMovementForward;

        float RotateSpeedFactor = RotateFactor * Mathf.Deg2Rad * Time.deltaTime;

        bIsMoving = Mathf.Abs(Movement.x) > 0.1f || Mathf.Abs(Movement.y) > 0.1f;

        if (TargetDirection != Vector3.zero)
        {
            MoveDirection = Vector3.RotateTowards(MoveDirection, TargetDirection, RotateSpeedFactor, 1000);
            MoveDirection = MoveDirection.normalized;

            if (AnimController.IsSpecialAnimationFinished())
                transform.rotation = Quaternion.LookRotation(MoveDirection);
        }

        if (IsMoving() && AnimController.IsSpecialAnimationFinished())
        {
            // Apply gravity
            VerticalMovment.y -= 20.0f * Time.deltaTime;
            Vector3 Movment = (MoveDirection * MovementFactor + VerticalMovment) * Time.deltaTime;

            // Move the controller
            CollisionFlags Flags = CharactoerContllor.Move(Movment);
	        bool bGrounded = (Flags & CollisionFlags.CollidedBelow) != 0;
            if (bGrounded)
                VerticalMovment.y = 0.0f;

            TargetOritenation = MoveDirection;
        }
        else
            MoveDirection = transform.TransformDirection(Vector3.forward);
    }

    void DrawDebug()
    {
        Debug.DrawLine(transform.position, transform.position + MoveDirection);
        Debug.DrawLine(transform.position, transform.position + TargetDirection);
    }
}
