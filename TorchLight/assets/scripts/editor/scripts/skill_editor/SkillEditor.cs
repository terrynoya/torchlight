using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

class FNodeInput
{
    public string Name;
    public FNode Owner = null;
    public FNodeOutput Output = null;
    public FNodeInput(string InName)
    {
        Name = InName;
    }

    public Rect GetRect()
    {
        return Owner.GetInputRect(this);
    }
}

class FNodeOutput
{
    public string Name;
    public FNode Owner = null;
    public FNodeInput Input = null;
    public FNodeOutput(string InName)
    {
        Name = InName;
    }

    public Rect GetRect()
    {
        return Owner.GetOutputRect();
    }
}

class FNode
{
    public static float NODE_ITEM_HEIGHT = 10.0f;
    public static float NODE_TITLE_HEIGHT = 15.0f;
    public static float NODE_WINDOW_WIDTH = 200.0f;
    public static float NODE_INPUT_WIDTH = 10.0f;
    public static float NODE_TEXTURE_WIDTH = 64.0f;

    private static int  GWindowID   = 0;
    private static int  GSelectedID = -1;
    private int         WindowID    = 0;

    public string           Comment     = "";
    public Rect             WindowRect;

    public List<FNodeInput> NodeInputs  = new List<FNodeInput>();
    public FNodeOutput      NodeOutput  = null;

    public FSkillInfo       SkillInfo   = new FSkillInfo();

    public bool             bHasOutput  = true;
    public bool             bCanDrag    = true;
    private bool            bDeleteMe   = false;

    public string Title
    {
        get
        {
            return SkillInfo.Name;
        }

        set
        {
            SkillInfo.Name = value;
        }
    }

    public static void UnSelected()
    {
        GSelectedID = -1;
    }

    public bool IsSelected()
    {
        return GSelectedID == WindowID;
    }

    public bool NeedDelete()
    {
        return bDeleteMe;
    }

    public FNode(string InTitle, float X, float Y, bool HasOutput = true)
    {
        bHasOutput  = HasOutput;
        WindowID    = GWindowID++;
        Title       = InTitle;
        WindowRect  = new Rect(X, Y, NODE_WINDOW_WIDTH, NODE_WINDOW_WIDTH);

        int InputNum = Random.Range(1, 2);
        for (int i = 0; i < InputNum; i++)
        {
            AddInput(new FNodeInput("Input_" + i));
        }

        if (HasOutput)
            AddOutput(new FNodeOutput("Output"));
    }

    public bool Contants(Vector2 Position)
    {
        return WindowRect.Contains(Position);
    }

    public void AddInput(FNodeInput Input)
    {
        Input.Owner = this;
        NodeInputs.Add(Input);

        ComputeNodeHeight();
    }

    public void Reflesh()
    {
        ComputeNodeHeight();
    }

    public void RemoveInput(int Index)
    {
        FNodeInput Input = GetInput(Index);
        if (Input != null)
        {
            if (Input.Output != null)
            {
                Input.Output.Input = null;
                Input.Output = null;
            }
            NodeInputs.Remove(Input);
        }

        ComputeNodeHeight();
    }

    public void AddOutput(FNodeOutput Output)
    {
        Output.Owner = this;
        NodeOutput = Output;
    }

    public FNodeInput GetInput(int Index)
    {
        return NodeInputs[Index];
    }

    public FNodeOutput GetOutput()
    {
        return NodeOutput;
    }

    public Rect GetInputRect(FNodeInput Input)
    {
        for (int i = 0; i < NodeInputs.Count; i++)
        {
            if (NodeInputs[i] == Input)
                return GetInputRect(i);
        }

        return new Rect();
    }

    public Rect GetInputRect(int Index)
    {
        float X = WindowRect.x, Y = WindowRect.y;
        float PosY = Y + NODE_TITLE_HEIGHT + 7.0f;
        return new Rect(X + NODE_WINDOW_WIDTH, PosY + Index * 21.0f, NODE_INPUT_WIDTH, NODE_INPUT_WIDTH);
    }

    public Rect GetOutputRect()
    {
        if (!bHasOutput)
            return new Rect(0, 0, 0, 0);

        float X = WindowRect.x, Y = WindowRect.y;
        float PosY = Y + NODE_TITLE_HEIGHT + 7.0f;
        return new Rect(X - NODE_INPUT_WIDTH - 1, PosY, NODE_INPUT_WIDTH, NODE_INPUT_WIDTH);
    }

    public int GetInputIndex(Vector2 Position)
    {
        for (int i = 0; i < NodeInputs.Count; i++)
        {
            if (GetInputRect(i).Contains(Position))
                return i;
        }
        return -1;
    }

    public int GetOutputIndex(Vector2 Position)
    {
        if (!bHasOutput) return -1;

        if (GetOutputRect().Contains(Position))
            return 0;
        return -1;
    }

    public void BreakLinks()
    {
        if (NodeOutput != null && NodeOutput.Input != null)
            NodeOutput.Input.Output = null;
    }

    public void DrawGUI(Vector2 CenterOffset)
    {
        if (GSelectedID == WindowID)
            GUI.color = Color.green;

        GUI.skin.window.onNormal.background = GUI.skin.window.normal.background;

        WindowRect.x += CenterOffset.x;
        WindowRect.y += CenterOffset.y;
        WindowRect = GUILayout.Window(WindowID, WindowRect, DoWindowDraw, Title, GUI.skin.window);

        DrawInputOutputs();

        GUI.color = Color.white;
    }

    public void DrawInputOutputs()
    {
        GUI.color = Color.white;
        GUI.skin.box.alignment = TextAnchor.MiddleCenter;
        for (int i = 0; i < NodeInputs.Count; i++)
        {
            GUILayout.BeginArea(GetInputRect(i));
            GUILayout.Box("o", GUI.skin.box, GUILayout.Width(NODE_INPUT_WIDTH), GUILayout.Height(NODE_INPUT_WIDTH));
            GUILayout.EndArea();
        }

        if (NodeOutput != null)
        {
            GUILayout.BeginArea(GetOutputRect());
            GUILayout.Box("", GUILayout.Width(NODE_INPUT_WIDTH), GUILayout.Height(NODE_INPUT_WIDTH));
            GUILayout.EndArea();
        }

        GUILayout.BeginArea(new Rect(WindowRect.x, WindowRect.y + WindowRect.height, WindowRect.width * 2, 100));
        GUILayout.Label(Comment);
        GUILayout.EndArea();
    }

    void DoWindowDraw(int WindowID)
    {
        GUILayout.BeginHorizontal();
        {
            GUI.skin.label.alignment = TextAnchor.MiddleLeft;
            if (NodeOutput != null) GUILayout.Label(NodeOutput.Name, GUI.skin.label);
            GUILayout.BeginVertical();
            {
                GUI.skin.label.alignment = TextAnchor.MiddleRight;
                foreach (FNodeInput Input in NodeInputs)
                {
                    GUILayout.Label(Input.Name, GUI.skin.label);
                    GUILayout.Space(5.0f);
                }
            }
            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();
        GUI.skin.label.alignment = TextAnchor.MiddleLeft;

        Rect TexRect = new Rect((WindowRect.width - NODE_TEXTURE_WIDTH) / 2, 
                                (WindowRect.height - NODE_TEXTURE_WIDTH + NODE_TITLE_HEIGHT) / 2, 
                                 NODE_TEXTURE_WIDTH, NODE_TEXTURE_WIDTH);

        ESkillInfoInspector.DrawTexture(SkillInfo, TexRect);

        // Here we peocess mouse event
        ProcessMouseEvent();

        if (bCanDrag)
            GUI.DragWindow();
    }

    void ComputeNodeHeight()
    {
        float Height = NODE_ITEM_HEIGHT * (NodeInputs.Count + 1);
        if (NodeInputs.Count == 0)
            Height += NODE_ITEM_HEIGHT;

        if (SkillInfo.HasIcon())
        {
            Height += NODE_TEXTURE_WIDTH;
        }

        WindowRect.height = Height;
    }

    void ProcessMouseEvent()
    {
        MouseEvent Event = MouseEvent.GetMouseEvent();
        if (Event.Type == EventType.MouseDown)
        {
            if (Event.IsLeftButton())
            {
                GSelectedID = WindowID;
            }

            if (Event.IsRightButton())
            {
                EditorGUIUtil.PopupMenu("Delete Node", Event.MousePos, DeleteNodeHandle);
            }
        }
    }

    void DeleteNodeHandle()
    {
        bDeleteMe = true;
    }
}

class LinkNodeInfo
{
    public int Index = -1;
    public bool bInput = true;
    public FNode NodeWindow = null;

    public FNodeInput GetInput()
    {
        return NodeWindow.GetInput(Index);
    }

    public FNodeOutput GetOutput()
    {
        return NodeWindow.GetOutput();
    }

    public LinkNodeInfo(FNode InNode, bool IsInput, int InIndex)
    {
        NodeWindow = InNode;
        bInput = IsInput;
        Index = InIndex;
    }

    public Rect GetRect()
    {
        if (bInput) return NodeWindow.GetInputRect(Index);
        return NodeWindow.GetOutputRect();
    }

    public void Link(LinkNodeInfo NodeInfo)
    {
        if (bInput)
        {
            if (GetInput().Owner == NodeInfo.GetOutput().Owner)
                return;

            GetInput().Output = NodeInfo.GetOutput();
            NodeInfo.GetOutput().Input = GetInput();
        }
        else
        {
            if (NodeInfo.GetInput().Owner == GetOutput().Owner)
                return;

            NodeInfo.GetInput().Output = GetOutput();
            GetOutput().Input = NodeInfo.GetInput();
        }
    }

    public void BreakLink()
    {
        if (bInput)
            GetInput().Output = null;
        else
            GetOutput().Input.Output = null;
    }
}

public class SkillEditor : EditorWindow 
{
    public static float PROPERTY_WINDOW_WIDTH = 200.0f;

    [MenuItem("TorchLight/Editor/SkillEditor")]
    static void ShowEditor()
    {
        SkillEditor EditorWindow = CreateInstance<SkillEditor>();
        EditorWindow.title = "Skill Editor";
        EditorWindow.Show();
    }

    List<FNode>     Nodes           = new List<FNode>();
    LinkNodeInfo    StartNode       = null;
    LinkNodeInfo    EndNode         = null;
    LinkNodeInfo    BreakLinkNode   = null;

    Vector2         CenterOffset    = Vector2.zero;

    void OnGUI() 
    {
        if (Nodes.Count == 0)
        {
            // The position of the window    
            Nodes.Add(new FNode("NodeA", 100, 100, false));
            Nodes.Add(new FNode("NodeB", 400, 100));
            Nodes.Add(new FNode("NodeC", 400, 100));
        }

        DrawNodeCanvas();
    }

    void DrawNodeCanvas()
    {
        BeginWindows();
        {
            foreach (FNode Node in Nodes)
                Node.DrawGUI(CenterOffset);
            DrawPropertyWindow();
            DrawMiniMap();
        }
        EndWindows();

        ProcessMouseEvent();
        ProcessNodeMovement();
        ProcessLinkNoude();

        DeleteThatNode();
        DrawLinkLines();
    }

    static int PropertyWindowWidth = 200;
    void DrawMiniMap()
    {
        if (bCanMoveNodes)
        {
            float WindowWidth   = position.width - PropertyWindowWidth;
            float WindowHeight  = position.height;

            float MiniMapWidth = 200;
            float MiniMapHeight = MiniMapWidth * (WindowHeight / WindowWidth);
            float CenterX = WindowWidth / 2;
            float CenterY = WindowHeight / 2;
            GUILayout.BeginArea(new Rect(CenterX - MiniMapWidth / 2, CenterY - MiniMapHeight / 2, MiniMapWidth, MiniMapHeight));
            GUILayout.Box("", GUILayout.Width(MiniMapWidth), GUILayout.Height(MiniMapHeight));
            GUILayout.EndArea();

            foreach (FNode Node in Nodes)
            {
                Rect WinRect = Node.WindowRect;
                float RelatedX = (WinRect.x - CenterX) * (MiniMapWidth / WindowWidth);
                float RelatedY = (WinRect.y - CenterY) * (MiniMapHeight / WindowHeight);
                float RelatedWidth = WinRect.width * MiniMapWidth / WindowWidth;
                float relatedHeight = RelatedWidth * (WinRect.height / WinRect.width);

                GUILayout.BeginArea(new Rect(WindowWidth / 2 + RelatedX, RelatedY + WindowHeight / 2, RelatedWidth, relatedHeight));
                GUILayout.Box("", GUILayout.Width(RelatedWidth), GUILayout.Height(relatedHeight));
                GUILayout.EndArea();
            }
        }
    }

    bool IsInWorkingSpace(Vector2 MousePosition)
    {
        Rect WorkSpace  = position;
        WorkSpace.width -= PropertyWindowWidth;
        MousePosition.x += WorkSpace.x;
        MousePosition.y += WorkSpace.y;
        if (WorkSpace.Contains(MousePosition))
            return true;
        return false;
    }

    void DrawPropertyWindow()
    {
        int PropertyWindowID = 10000;
        Rect Position = new Rect(position.width - PropertyWindowWidth - 2, 0, PropertyWindowWidth, position.height);
        GUILayout.Window(PropertyWindowID, Position, DoDrawPropertyWindow, "Property");
        GUI.BringWindowToFront(PropertyWindowID);
    }

    FNode GetSelectedNode()
    {
        foreach (FNode Node in Nodes)
        {
            if (Node.IsSelected())
                return Node;
        }
        return null;
    }

    void DoDrawPropertyWindow(int WindowID)
    {
        bool ButtonPress = false;
        FNode Node = GetSelectedNode();

        if (ESkillInfoInspector.DoDrawImageSet(PROPERTY_WINDOW_WIDTH, position.height))
            return;

        GUILayout.Space(5);
        if (Node != null)
        {
            PressAddInput(ButtonPress, Node);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Skill", GUILayout.Width(74));
            Node.Title = GUILayout.TextField(Node.Title, GUILayout.Width(115));
            GUILayout.EndHorizontal();

            if (ESkillInfoInspector.DoDrawPropertyWindow(Node.SkillInfo))
                Node.Reflesh();

            GUILayout.Space(10);
            GUILayout.Label("Inputs");
            ButtonPress = GUILayout.Button("Add Input", GUILayout.Width(195));
            PressAddInput(ButtonPress, Node);
            for (int i = 0; i < Node.NodeInputs.Count; i++)
            {
                FNodeInput Input = Node.NodeInputs[i];

                GUILayout.BeginHorizontal();
                GUILayout.Label("Input", GUILayout.Width(50));
                Input.Name  = GUILayout.TextField(Input.Name, GUILayout.Width(118));
                ButtonPress = GUILayout.Button("X", GUILayout.Width(20), GUILayout.Height(17));
                PressDeleteInput(ButtonPress, Node, i);
                GUILayout.EndHorizontal();
            }

            GUILayout.Label("Comment");
            Node.Comment = GUILayout.TextArea(Node.Comment, GUILayout.Width(195));
        }
    }

    void PressDeleteInput(bool Press, FNode Node, int Index)
    {
        if (Press)
        {
            Node.RemoveInput(Index);
        }
    }

    void PressAddInput(bool Press, FNode Node)
    {
        if (Press)
        {
            Node.AddInput(new FNodeInput("New Input"));
        }
    }

    bool CanPopup()
    {
        return StartNode == null;
    }

    void DeleteThatNode()
    {
        foreach (FNode Node in Nodes)
        {
            if (Node.NeedDelete())
            {
                Node.BreakLinks();
                Nodes.Remove(Node);
                return;
            }
        }
    }

    LinkNodeInfo GetLinkNodeInfo(Vector2 MousePosition)
    {
        LinkNodeInfo LinkNode = null;
        foreach (FNode Node in Nodes)
        {
            int Handle = Node.GetInputIndex(MousePosition);
            if (Handle != -1)
            {
                LinkNode = new LinkNodeInfo(Node, true, Handle);
                break;
            }

            Handle = Node.GetOutputIndex(MousePosition);
            if (Handle != -1)
            {
                LinkNode = new LinkNodeInfo(Node, false, Handle);
                break;
            }
        }
        return LinkNode;
    }

    void DrawLinkLines()
    {
        foreach (FNode Node in Nodes)
        {
            foreach (FNodeInput Input in Node.NodeInputs)
            {
                if (Input.Output != null)
                {
                    LineDrawing.DrawCurveFromTo(Input.GetRect(), Input.Output.GetRect(), Color.black, 2);
                }
            }
        }

        if (StartNode != null && EndNode == null)
        {
            MouseEvent Event = MouseEvent.GetMouseEvent();
            Rect MousePos = new Rect(Event.MousePos.x, Event.MousePos.y, 4, 4);

            if (StartNode.bInput)
                LineDrawing.DrawCurveFromTo(StartNode.GetInput().GetRect(), MousePos, Color.black, 2);
            else
                LineDrawing.DrawCurveFromTo(MousePos, StartNode.GetOutput().GetRect(), Color.black, 2);

            Repaint();
        }
    }

    void ProcessMouseEvent()
    {
        MouseEvent Event = MouseEvent.GetMouseEvent();
        if (Event.Type == EventType.MouseDown)
        {
            if (Event.IsRightButton() && CanPopup())
            {
                CurMousePosition = Event.MousePos;

                LinkNodeInfo NodeInfo = GetLinkNodeInfo(Event.MousePos);
                if (NodeInfo != null && NodeInfo.bInput)
                {
                    BreakLinkNode = NodeInfo;
                    EditorGUIUtil.PopupMenu("Break Link", Event.MousePos, BreakNodeLinkhandle);
                }
                else
                {
                    EditorGUIUtil.PopupMenu("Create New Node", Event.MousePos, CreateNewNodeHandle);
                }
            }

            if (Event.IsLeftButton() && IsInWorkingSpace(Event.MousePos))
            {
                FNode.UnSelected();
                Repaint();
            }
        }
    }

    bool bCanMoveNodes = false;
    Vector2 LastMousePosition = Vector2.zero;
    void ProcessNodeMovement()
    {
        MouseEvent Event = MouseEvent.GetMouseEvent();
        if (Event.IsMiddleButton())
        {
            if (Event.Type == EventType.MouseDown)
            {
                bCanMoveNodes = true;
                LastMousePosition = Event.MousePos;
            }

            if (Event.Type == EventType.MouseUp)
            {
                bCanMoveNodes = false;
                CenterOffset = Vector2.zero;
                return;
            }
        }

        if (bCanMoveNodes)
        {
            CenterOffset        = Event.MousePos - LastMousePosition;
            LastMousePosition   = Event.MousePos;

            Repaint();
        }
    }

    void ProcessLinkNoude()
    {
        MouseEvent Event = MouseEvent.GetMouseEvent();

        if (Event.IsLeftButton())
        {
            if (Event.Type == EventType.MouseDown)
            {
                StartNode = GetLinkNodeInfo(Event.MousePos);
            }

            if (Event.Type == EventType.MouseUp)
            {
                if (StartNode != null)
                {
                    EndNode = GetLinkNodeInfo(Event.MousePos);
                    if (EndNode == null)
                    {
                        StartNode   = null;
                        EndNode     = null;
                    }
                }
            }

            if (StartNode != null && EndNode != null)
            {
                if (!(StartNode.bInput && EndNode.bInput ||
                    !StartNode.bInput && !EndNode.bInput))
                {
                    StartNode.Link(EndNode);
                }

                StartNode   = null;
                EndNode     = null;
            }
        }
    }

    Vector2 CurMousePosition = Vector2.zero;
    void CreateNewNodeHandle()
    {
        char Chars = (char)((int)'A' + (Nodes.Count + 1));
        string Title = "Skill" + Chars;
        Nodes.Add(new FNode(Title, CurMousePosition.x, CurMousePosition.y));
    }

    void BreakNodeLinkhandle()
    {
        if (BreakLinkNode != null)
        {
            BreakLinkNode.BreakLink();
            BreakLinkNode = null;
        }
    }
}
