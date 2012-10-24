using UnityEngine;
using System.Collections;

[RequireComponent(typeof(GUITexture))]
public class Joystick : MonoBehaviour
{

    static public Joystick GJoysticks = null;					// A static collection of all joysticks
    static private bool EnumeratedJoysticks = false;
    static private float TapTimeDelta = 0.3f;				// Time allowed between taps

    bool TouchPad = false; 									    // Is this a TouchPad?
    Rect TouchZone;
    float DeadZone = 0;									// Control when position is output
    bool Normalize = false; 							// Normalize output after the dead-zone?
    public Vector2 Position = Vector2.zero; 									// [-1, 1] in x,y
    int TapCount = 0;											// Current tap count

    private int lastFingerId = -1;							// Finger last used for this joystick
    private float tapTimeWindow;							// How much time there is left for a tap to occur
    private Vector2 fingerDownPos;

    private GUITexture  Gui = null;								// Joystick graphic
    private Rect        DefaultRect;								// Default position / extents of the joystick graphic
    private Vector2     GuiTouchOffset;						// Offset to apply to touch input
    private Vector2     GuiCenter;							// Center of joystick

#if !UNITY_IPHONE && !UNITY_ANDROID

    void Awake () 
    {
	    gameObject.active = false;	
    }

    #else

        void Start()
        {
            // Cache this component at startup instead of looking up every frame	
            Gui = GetComponent<GUITexture>();

            // Store the default rect for the gui, so we can snap back to it
            DefaultRect = Gui.pixelInset;

            DefaultRect.x += transform.position.x * Screen.width;// + gui.pixelInset.x; // -  Screen.width * 0.5;
            DefaultRect.y += transform.position.y * Screen.height;// - Screen.height * 0.5;

            transform.position = Vector3.zero;

            if (TouchPad)
            {
                // If a texture has been assigned, then use the rect ferom the gui as our touchZone
                if (Gui.texture)
                    TouchZone = DefaultRect;
            }
            else
            {
                // This is an offset for touch input to match with the top left
                // corner of the GUI
                GuiTouchOffset.x = DefaultRect.width * 0.5f;
                GuiTouchOffset.y = DefaultRect.height * 0.5f;

                // Cache the center of the GUI, since it doesn't change
                GuiCenter.x = DefaultRect.x + GuiTouchOffset.x;
                GuiCenter.y = DefaultRect.y + GuiTouchOffset.y;
            }
        }

        void Disable()
        {
            gameObject.active = false;
            EnumeratedJoysticks = false;
        }

        void ResetJoystick()
        {
            // Release the finger control and set the joystick back to the default position
            Gui.pixelInset = DefaultRect;
            lastFingerId = -1;
            Position = Vector2.zero;
            fingerDownPos = Vector2.zero;

            if (TouchPad)
            {
                Color Col = Gui.color;
                Col.a = 0.025f;
                Gui.color = Col;
            }
        }

        bool IsFingerDown()
        {
            return (lastFingerId != -1);
        }

        void LatchedFinger(int fingerId)
        {
            // If another joystick has latched this finger, then we must release it
            if (lastFingerId == fingerId)
                ResetJoystick();
        }

        void Update() {	
	    if (!EnumeratedJoysticks) {
		    // Collect all joysticks in the game, so we can relay finger latching messages
		    GJoysticks = FindObjectOfType (typeof(Joystick)) as Joystick;
		    EnumeratedJoysticks = true;
	    }	
		
	    var count = Input.touchCount;
	
	    // Adjust the tap time window while it still available
	    if (tapTimeWindow > 0)
		    tapTimeWindow -= Time.deltaTime;
	    else
		    TapCount = 0;
	
	    if (count == 0) {
		    ResetJoystick ();
	    }
	    else {
		    for (int i = 0; i < count; i++) {
			    Touch touch = Input.GetTouch (i);			
			    Vector2 guiTouchPos = touch.position - GuiTouchOffset;
	
			    bool shouldLatchFinger = false;
			    if (TouchPad) {				
				    if (TouchZone.Contains (touch.position))
					    shouldLatchFinger = true;
			    }
			    else if (Gui.HitTest (touch.position)) {
				    shouldLatchFinger = true;
			    }
	
			    // Latch the finger if this is a new touch
			    if (shouldLatchFinger && (lastFingerId == -1 || lastFingerId != touch.fingerId)) {
				
				    if (TouchPad) 
                    {
                        Color Col   = Gui.color;
                        Col.a       = 0.15f;
                        Gui.color   = Col;
					
					    lastFingerId = touch.fingerId;
					    fingerDownPos = touch.position;
				    }
				
				    lastFingerId = touch.fingerId;
				
				    // Accumulate taps if it is within the time window
				    if (tapTimeWindow > 0) {
					    TapCount++;
				    }
				    else {
					    TapCount = 1;
					    tapTimeWindow = TapTimeDelta;
				    }
											
				    // Tell other joysticks we've latched this finger
                    if (GJoysticks != null && GJoysticks != this)
                        GJoysticks.LatchedFinger(touch.fingerId);					
			    }				
	
			    if (lastFingerId == touch.fingerId) {
				    // Override the tap count with what the iPhone SDK reports if it is greater
				    // This is a workaround, since the iPhone SDK does not currently track taps
				    // for multiple touches
				    if (touch.tapCount > TapCount)
					    TapCount = touch.tapCount;
				
				    if (TouchPad) {	
					    // For a touchpad, let's just set the position directly based on distance from initial touchdown
					    Position.x = Mathf.Clamp ((touch.position.x - fingerDownPos.x) / (TouchZone.width / 2), -1, 1);
					    Position.y = Mathf.Clamp ((touch.position.y - fingerDownPos.y) / (TouchZone.height / 2), -1, 1);
				    }
				    else {					
					    // Change the location of the joystick graphic to match where the touch is
					    Position.x = (touch.position.x - GuiCenter.x) / GuiTouchOffset.x;
					    Position.y = (touch.position.y - GuiCenter.y) / GuiTouchOffset.y;
				    }
				
				    if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
					    ResetJoystick ();
			    }			
		    }
	    }
	
	    // Calculate the length. This involves a squareroot operation,
	    // so it's slightly expensive. We re-use this length for multiple
	    // things below to avoid doing the square-root more than one.
	    float length = Position.magnitude;
	
	
	    if (length < DeadZone) {
		    // If the length of the vector is smaller than the deadZone radius,
		    // set the position to the origin.
		    Position = Vector2.zero;
	    }
	    else {
		    if (length > 1) {
			    // Normalize the vector if its length was greater than 1.
			    // Use the already calculated length instead of using Normalize().
			    Position = Position / length;
		    }
		    else if (Normalize) {
			    // Normalize the vector and multiply it with the length adjusted
			    // to compensate for the deadZone radius.
			    // This prevents the position from snapping from zero to the deadZone radius.
			    Position = Position / length * Mathf.InverseLerp (length, DeadZone, 1);
		    }
	    }
	
	    if (!TouchPad) 
        {
		    // Change the location of the joystick graphic to match the position
            Rect NewRect = Gui.pixelInset;
		    NewRect.x = (Position.x - 1) * GuiTouchOffset.x + GuiCenter.x;
		    NewRect.y = (Position.y - 1) * GuiTouchOffset.y + GuiCenter.y;
            Gui.pixelInset = NewRect;
	    }
    }

#endif

}
