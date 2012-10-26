using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerControllerX : MonoBehaviour
{

    public float MovementFactor = 7.5f;
    public float RotateFactor = 10.0f;
    public Vector3 CameraOffset = new Vector3(3.0f, 7.5f, 3.0f);

    private bool bIsMoving = false;

    private Vector3 CurMoveDirection = Vector3.zero;
    private Vector3 TargetDirection = Vector3.zero;
    private Vector3 TargetPosition = Vector3.zero;

    private AnimationController AnimController = null;
    private CharacterController CharactoerContllor = null;

    public bool IsMoving()
    {
        return bIsMoving;
    }

    // Use this for initialization
    void Start()
    {
        CurMoveDirection = transform.TransformDirection(Vector3.forward);

        AnimController = GetComponent<AnimationController>();
        CharactoerContllor = GetComponent<CharacterController>();

        UpdateCamera();
    }

    void Update()
    {
        UpdateSmoothRotation();

        UpdateMovment();

        //UpdateAnimation();

        DrawDebug();
    }

    void LateUpdate()
    {
        UpdateCamera();
    }

    bool IsFinishRotating()
    {
        CurMoveDirection = transform.TransformDirection(Vector3.forward);
        return Vector3.Angle(CurMoveDirection, TargetDirection) < 1.0f;
    }

    void UpdateSmoothRotation()
    {
        if (InputController.IsScreenTouched())
        {
            TargetPosition      = InputController.GetCursorWorldPosition();
            TargetDirection   = TargetPosition - transform.position;
            TargetDirection.y = 0.0f;
        }

        if (!IsFinishRotating())
        {
            if (AnimController.IsSpecialAnimationFinished())
                transform.rotation = Quaternion.LookRotation(
                    Vector3.Lerp(CurMoveDirection, TargetDirection, Time.deltaTime * RotateFactor));
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
        bIsMoving = false;

        Vector3 CurPosition = transform.position;
        Vector3 DistOffset = TargetPosition - CurPosition;
        DistOffset.y = 0.0f;

        if (DistOffset.magnitude > 0.1f && IsFinishRotating())
        {
            DistOffset = DistOffset.normalized;

            // Apply gravity
            VerticalMovment.y -= 20.0f * Time.deltaTime;
            Vector3 Movment = (DistOffset * MovementFactor + VerticalMovment) * Time.deltaTime;

            // Move the controller
            CollisionFlags Flags = CharactoerContllor.Move(Movment);
            bool bGrounded = (Flags & CollisionFlags.CollidedBelow) != 0;
            if (bGrounded)
                VerticalMovment.y = 0.0f;

            bIsMoving = true;
        }
    }

    void DrawDebug()
    {
        Debug.DrawLine(transform.position, transform.position + CurMoveDirection);
    }
}
