using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerControllerX : MonoBehaviour
{

    public float    MovementSpeedFactor = 7.5f;
    public float    RotateSpeedFactor   = 5.0f;
    public Vector3  CameraOffset = new Vector3(3.0f, 7.5f, 3.0f);

    private Vector3 CurMoveDirection    = Vector3.zero;
    private Vector3 TargetDirection     = Vector3.zero;
    private Vector3 TargetPosition      = Vector3.zero;

    private AnimationController AnimController = null;
    private CharacterController CharactoerContllor = null;

    private bool bIsMoving = false;
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
        InputController.Update();

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
            TargetPosition    = InputController.GetCursorWorldPosition();
            TargetDirection   = TargetPosition - transform.position;
            TargetDirection.y = 0.0f;
        }

        if (!IsFinishRotating())
        {
            if (AnimController.IsSpecialAnimationFinished())
            {
                Vector3 Rotation = Vector3.Lerp(CurMoveDirection, TargetDirection, Time.deltaTime * RotateSpeedFactor);
                if (Rotation != Vector3.zero)
                    transform.rotation = Quaternion.LookRotation(Rotation);
            }
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

        if (DistOffset.magnitude > 0.1f)
        {
            DistOffset = DistOffset.normalized;

            // Apply gravity
            VerticalMovment.y   -= 20.0f * Time.deltaTime;
            Vector3 Movment     = (DistOffset * MovementSpeedFactor + VerticalMovment) * Time.deltaTime;

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
