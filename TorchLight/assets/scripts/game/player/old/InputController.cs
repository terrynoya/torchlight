using UnityEngine;
using System.Collections;

public class InputController
{

    private static Plane PlayerPlane = new Plane(new Vector3(0.0f, 1.0f, 0.0f), 0.0f);

    public static Vector3 PlaneRayIntersection (Plane InPlane, Ray InRay) 
    {
	    float Dist;
	    InPlane.Raycast(InRay, out Dist);
	    return InRay.GetPoint(Dist);
    }

    public static Vector3 ScreenPointToWorldPoint(Vector3 ScreenPoint)
    {
        Ray ray = Camera.mainCamera.ScreenPointToRay(ScreenPoint);
        return PlaneRayIntersection(PlayerPlane, ray);
    }

    public static Vector2 GetMoveDirection()
    {
        Vector2 Movement = new Vector2();
        Movement.x = Input.GetAxis("Horizontal");
        Movement.y = Input.GetAxis("Vertical");
        return Movement;
    }

    public static Vector3 GetCursorWorldPosition()
    {
        Vector2 ScreenPos = InputController.MousePosition();
        return InputController.ScreenPointToWorldPoint(ScreenPos);
    }

    public static Vector3 GetCursorOffsetInWorldPosition(Vector3 Center)
    {
        Vector2 ScreenPos = InputController.MousePosition();
        Vector3 CursorWorldPosition = InputController.ScreenPointToWorldPoint(ScreenPos);

        Vector3 Offset = CursorWorldPosition - Center;
        Offset.y = 0.0f;
        Offset = Offset.normalized;
        return Offset;
    }

    private static bool bTouchScreen = false;
    public static void Update()
    {
        if (Input.GetMouseButtonDown(0))
            bTouchScreen = true;

        if (Input.GetMouseButtonUp(0))
            bTouchScreen = false;
    }

    public static bool IsScreenTouched()
    {
        //return Input.GetMouseButtonDown(0);
        return bTouchScreen;
    }

    public static Vector2 MousePosition()
    {
        return Input.mousePosition;
    }

    public static bool GetButton(string Key)
    {
        return Input.GetButton(Key);
    }
}
