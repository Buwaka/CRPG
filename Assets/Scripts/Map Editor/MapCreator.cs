using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEditor.EditorTools;
using UnityEditor.Tilemaps;
using UnityEngine.SceneManagement;

//[EditorTool("CRPG Tool")]
//public class MapCreatorTool : EditorTool
//{
//    private int lastDragHandleID = 0;

//    public Rect SelectionBox = Rect.zero;

//    public bool Repaint = false;

//    void OnWillBeDeactivated()
//    {
//        EditorTools.RestorePreviousPersistentTool();

//        //WERK VERDER AAN TOOL!
//    }

//    public override void OnToolGUI(EditorWindow window)
//    {
//        //Ray guiRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
//        //Vector3 mousePosition = guiRay.origin - guiRay.direction * (guiRay.origin.z / guiRay.direction.z);

//        //int id = GUIUtility.GetControlID(FocusType.Passive);
//        //lastDragHandleID = id;

//        //Vector3 screenPosition = Handles.matrix.MultiplyPoint(mousePosition);
//        //Matrix4x4 cachedMatrix = Handles.matrix;



//        //Event e = Event.current;

//        //if (e.type == EventType.Layout)
//        //    HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));

//        ////if (e.type == EventType.Layout)
//        ////{
//        ////    id = GUIUtility.GetControlID(FocusType.Passive);
//        ////    HandleUtility.AddDefaultControl(id);
//        ////}

//        //Debug.Log(Event.current.type);

//        //switch (Event.current.GetTypeForControl(id))
//        //{

//        //}

//        if (SelectionBox != Rect.zero)
//        {
//            Handles.DrawWireCube(SelectionBox.center, SelectionBox.size);
//        }

//        if (Repaint)
//        {
//            window.Repaint();
//            Repaint = false;
//        }

//    }
//}

public class MapCreator : EditorWindow
{
    private Transform parent = null;

    private bool paintMode = false;
    private bool ParentSelect = false;
    private bool dragBox = false;
    private bool dragNodes = false;
    private Vector2 dragBoxPosition = Vector2.zero;

    //private MapCreatorTool MapTool = null;


    private List<FieldNode> Selected = new List<FieldNode>();

    // Start is called before the first frame update
    [MenuItem("CRPG/Node Map Editor")]
    private static void ShowWindow()
    {
        EditorWindow editorWindow = EditorWindow.GetWindow(typeof(MapCreator));
        editorWindow.autoRepaintOnSceneChange = true;
        editorWindow.Show();
    }

    // Called to draw the MapEditor windows.
    private void OnGUI()
    {
        if (ParentSelect)
        {
            if (Event.current.type == EventType.Layout && Event.current.button == 0)
            {
                var objs = Selection.GetTransforms(SelectionMode.Editable);

                if (objs.Length > 0)
                {
                    parent = objs[0];
                    ParentSelect = false;
                }
            }
        }

        paintMode = GUILayout.Toggle(paintMode && parent, "Paint Mode", "Button", GUILayout.Height(30f));

        //if (paintMode && !MapTool)
        //{
        //    MapTool = CreateInstance<MapCreatorTool>();
        //    EditorTools.SetActiveTool(MapTool);
        //}
        //if (!paintMode && MapTool)
        //{
        //    DestroyImmediate(MapTool);
        //    MapTool = null;
        //}

        GUILayout.Label("Parent");
        EditorGUILayout.BeginHorizontal();
        ParentSelect = GUILayout.Toggle(ParentSelect, "Select Parent", "Button");

        if (GUILayout.Button("Reset Parent"))
            parent = null;

        string pname = parent ? parent.name : "No Parent Selected";
        GUILayout.TextArea(pname);
        EditorGUILayout.EndHorizontal();

        if (parent)
        {
            GUILayout.Label("Actions");
            EditorGUILayout.BeginHorizontal();


            EditorGUI.BeginDisabledGroup(Selected.Count < 2);
            if (GUILayout.Button("Connect"))
                ConnectSelectedAll();
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(Selected.Count != 2);
            if (GUILayout.Button("Connect One Way"))
                ConnectSelectedOne();
            if (GUILayout.Button("Flip Connection"))
                FlipSelected();
            EditorGUI.EndDisabledGroup();




            EditorGUILayout.EndHorizontal();
        }

    }

    // Does the rendering of the map editor in the scene view.
    private Vector3 mousePosition = Vector3.zero;
    private void OnSceneGUI(SceneView sceneView)
    {
        Ray guiRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        Vector3 newMousePosition = guiRay.origin - guiRay.direction * (guiRay.origin.z / guiRay.direction.z);
        Vector3 mouseDelta = newMousePosition - mousePosition;
        mousePosition = newMousePosition;


        if (paintMode && parent)
        {
            PaintMode(mousePosition, mouseDelta);
        }
        else
        {
            ResetSelection();
        }

        sceneView.Repaint();
    }

    void PaintMode(Vector3 Position, Vector3 delta)
    {
        //to make sure we get all the events
        if (Event.current.type == EventType.Layout)
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));

        //right mouse button - create & reset
        if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
        {
            if (Selected.Count > 0)
            {
                ResetSelection(); 

            }
            else
            {
                CreateNode(Position);
            }

        }

        //left mouse key - select
        else if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
        {
            //ConnectNode(mousePosition);
            dragBox = true;
            dragBoxPosition = Position;

            FieldNode node = FindNode(Position);
            if (node)
            {
                if (node.Gizmo_Selected)
                {
                    dragBox = false;
                    dragNodes = true;
                }
                else
                {
                    if (!Event.current.control)
                    {
                        ResetSelection();
                    }
                    SelectNode(node);
                }

            }
            else if(!Event.current.control)
            {
                ResetSelection();
            }
        }
        else if (Event.current.type == EventType.MouseUp)
        {
            dragBox = false;
            dragNodes = false;
            Repaint();
        }

        if (dragNodes)
        {
            MoveNodes(Selected.ToArray(), delta);
        }

        ////Box Select
        //if (Event.current.type == EventType.MouseDrag)
        //{
        //    if (!dragBox)
        //    {
        //        dragBox = true;
        //        dragBoxPosition = Event.current.mousePosition;

        //        Rect boxRect = new Rect(dragBoxPosition.x, dragBoxPosition.y, Event.current.mousePosition.x - dragBoxPosition.x, Event.current.mousePosition.y - dragBoxPosition.y);
        //        var nodes = FindNode(boxRect);
        //        SelectNode(nodes);
        //    }

        //}

        if (dragBox)
        {
            Ray guiRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            Vector3 temppos = guiRay.origin - guiRay.direction * (guiRay.origin.z / guiRay.direction.z);

            var p2 = new Vector2(temppos.x - dragBoxPosition.x, temppos.y - dragBoxPosition.y);

            Rect boxRect = Rect.zero;

            //if (dragBoxPosition.x + p2.x <= 0 || dragBoxPosition.y + p2.y <= 0)
            //{
            //    boxRect = new Rect(temppos.x, temppos.y, dragBoxPosition.x - temppos.x, dragBoxPosition.y - temppos.y);
            //}
            //else
            //{
            //    boxRect = new Rect(dragBoxPosition.x, dragBoxPosition.y, p2.x, p2.y);
            //}

            boxRect = new Rect(dragBoxPosition.x, dragBoxPosition.y, p2.x, p2.y);

            Handles.DrawWireCube(boxRect.center, boxRect.size);

            SelectNode(FindNode(boxRect));
        }


    }

    void CreateNode(Vector3 Position)
    {
        GameObject obj;

        GameObject prefab = TileManager.Instance.GetTilePrefab(TileManager.TileType.Default);

        if (prefab)
            obj = Instantiate(prefab);
        else
            obj = new GameObject();

        obj.AddComponent<FieldNode>();
        obj.transform.position = Position;
        obj.transform.parent = parent;

        // Allow the use of Undo (Ctrl+Z, Ctrl+Y).
        Undo.RegisterCreatedObjectUndo(obj, "Create Node");
    }

    FieldNode[] FindNode(Rect Box)
    {
        List<FieldNode> BoxedNodes = new List<FieldNode>();
        var children = parent.GetComponentsInChildren<FieldNode>();
        for (int i = 0; i < children.Length; i++)
        {
            var child = children[i];
            if (Box.Contains(child.GetPosition(), true))
            {
                BoxedNodes.Add(child);
            }
        }

        return BoxedNodes.ToArray();
    }

    FieldNode FindNode(Vector3 Position)
    {
        var children = parent.GetComponentsInChildren<FieldNode>();
        for (int i = 0; i < children.Length; i++)
        {
            var child = children[i];
            if (Mathf.Abs((Position - child.transform.position).magnitude) < child.Gizmo_Size)
            {
                return child;
            }
        }

        return null;
    }

    void MoveNodes(FieldNode[] nodes, Vector2 delta)
    {
        if (nodes.Length == 0)
            return;

        Undo.RegisterCompleteObjectUndo(nodes, "Move Nodes");

        foreach (var node in nodes)
        {
            node.Move(delta);
        }
    }

    void SelectNode(FieldNode select)
    {
        if (Selected.Contains(select))
            return;

        Undo.RegisterCompleteObjectUndo(select, "Select Node");

        select.Gizmo_Selected = true;
        Selected.Add(select);
        Repaint();
    }

    void SelectNode(FieldNode[] selectionList)
    {
        if (selectionList.Length == 0)
            return;

        Undo.RegisterCompleteObjectUndo(selectionList, " Select Node");

        for (int i = 0; i < selectionList.Length; i++)
        {
            FieldNode select = selectionList[i];

            if (Selected.Contains(select))
                continue;

            select.Gizmo_Selected = true;
            Selected.Add(select);
        }

        Repaint();
    }

    void DeselectNode(FieldNode select)
    {
        if (!Selected.Contains(select))
            return;

        Undo.RegisterCompleteObjectUndo(select, "Deselect Node");

        select.Gizmo_Selected = false;
        Selected.Remove(select);
        Repaint();
    }

    void DeselectNode(FieldNode[] selectionList)
    {
        if (selectionList.Length == 0)
            return;

        Undo.RegisterCompleteObjectUndo(selectionList, " Reset Selection");

        for (int i = 0; i < selectionList.Length; i++)
        {
            FieldNode select = selectionList[i];

            if (!Selected.Contains(select))
                continue;

            select.Gizmo_Selected = false;
            Selected.Remove(select);
        }

        Repaint();
    }

    void ResetSelection()
    {
        if (Selected.Count == 0)
            return;

        Undo.RegisterCompleteObjectUndo(Selected.ToArray(), " Reset Selection");

        foreach (var select in Selected)
        {
            select.Gizmo_Selected = false;
        }
        Selected.Clear();
        Repaint();
    }

    void ConnectSelectedAll()
    {
        if (Selected.Count < 2)
            return;

        Undo.RegisterCompleteObjectUndo(Selected.ToArray(), " Connect All Selected Nodes");

        for (int i = 0; i < Selected.Count; i++)
        {
            var select = Selected[i];
            for (int j = 0; j < Selected.Count; j++)
            {
                var subselect = Selected[j];
                if (select != subselect)
                {
                    select.Connect(subselect);
                }
            }
        }
    }

    void ConnectSelectedOne()
    {
        if (Selected.Count != 2)
            return;

        Undo.RegisterCompleteObjectUndo(Selected.ToArray(), " Connect All Selected Nodes - Oneway");

        Selected[0].Connect(Selected[1]);
    }

    void FlipSelected()
    {
        if (Selected.Count != 2)
            return;

        Undo.RegisterCompleteObjectUndo(Selected.ToArray(), " Flip connection");

        Selected[0].FlipConnection(Selected[1]);
    }

    void OnFocus()
    {
        SceneView.duringSceneGui -= this.OnSceneGUI; // Just in case
        SceneView.duringSceneGui += this.OnSceneGUI;
    }

    void OnDestroy()
    {
        SceneView.duringSceneGui -= this.OnSceneGUI;
    }
}
