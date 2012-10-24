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
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (Joystick.GJoysticks != null)
                return Joystick.GJoysticks.Position;
        }
        else
        {
            Vector2 Movement = new Vector2();
            Movement.x = Input.GetAxis("Horizontal");
            Movement.y = Input.GetAxis("Vertical");
            return Movement;
        }

        return Vector2.zero;
    }

    public static bool IsScreenTouched()
    {
        return Input.GetMouseButtonDown(0);
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
