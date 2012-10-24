using UnityEngine;
using UnityEditor;
using System.Collections;

public class MouseEvent
{
    public EventType    Type        = EventType.Ignore;
    public Vector2      MousePos    = Vector2.zero;
    public int          Button      = -1;

    public bool IsLeftButton()      { return Button == 0; }
    public bool IsRightButton()     { return Button == 1; }
    public bool IsMiddleButton()    { return Button == 2; }

    public static MouseEvent GetMouseEvent()
    {
        Event Evt           = Event.current;
        MouseEvent NewEvent = new MouseEvent();
        NewEvent.Button     = Evt.button;
        NewEvent.Type       = Evt.type;
        NewEvent.MousePos   = Evt.mousePosition;
        return NewEvent;
    }
}

public class EditorGUIUtil {

    static float LastUpdataTime = -1f;
    public static float DeltaTime()
    {
        if (LastUpdataTime < 0.0f)
            LastUpdataTime = (float)EditorApplication.timeSinceStartup;

        float CurTime = (float)EditorApplication.timeSinceStartup;
        float Delta = CurTime - LastUpdataTime;
        LastUpdataTime = CurTime;

        return Delta;
    }

    public static void PopupMenu(string Description, Vector2 MousePosition, GenericMenu.MenuFunction Function)
    {
        GenericMenu ToolsMenu = new GenericMenu();
        ToolsMenu.AddItem(new GUIContent(Description), false, Function);
        ToolsMenu.DropDown(new Rect(MousePosition.x, MousePosition.y, 0, 0));
        EditorGUIUtility.ExitGUI();
    }

    public static bool OpenFile(string Description, string Ext, ref string OutPath)
    {
        string Path = EditorUtility.OpenFilePanel(Description, TorchLightConfig.TorchLightAssetFolder, Ext);
        if (Path.Length > 0)
        {
            OutPath = Path;
            return true;
        }
        return false;
    }

    public static bool OpenFolder(string Description, ref string OutPath)
    {
        string Path = EditorUtility.OpenFolderPanel(Description, TorchLightConfig.TorchLightAssetFolder, "");
        if (Path.Length > 0)
        {
            OutPath = Path;
            return true;
        }
        return false;
    }

}
