using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class MapCreator : EditorWindow
{
    private Transform parent = null;

    private bool paintMode = false;
    private bool ParentSelect = false;


    private List<FieldNode> Selected = new List<FieldNode>();

    // Start is called before the first frame update
    [MenuItem("CRPG/Node Map Editor")]
    private static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(MapCreator));
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
    private void OnSceneGUI(SceneView sceneView)
    {
        Ray guiRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        Vector3 mousePosition = guiRay.origin - guiRay.direction * (guiRay.origin.z / guiRay.direction.z);

        if (paintMode && parent)
        {
            PaintMode(mousePosition);
        }
        else
        {
            ResetSelection();
        }
    }

    void PaintMode(Vector3 Position)
    {
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
        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
        {
            //ConnectNode(mousePosition);
            FieldNode node = FindNode(Position);
            if (node)
            {
                if (!Event.current.control)
                {
                    ResetSelection();
                }
                SelectNode(node);
            }
            else
            {
                ResetSelection();
            }
        }
    }

    void CreateNode(Vector3 Position)
    {
        // Create the prefab instance while keeping the prefab link
        GameObject obj = new GameObject();
        obj.AddComponent<FieldNode>();
        obj.transform.position = Position;
        obj.transform.parent = parent;

        // Allow the use of Undo (Ctrl+Z, Ctrl+Y).
        Undo.RegisterCreatedObjectUndo(obj, "Create Node");
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

    void SelectNode(FieldNode select)
    {
        if (Selected.Contains(select))
            return;

        select.Gizmo_Selected = true;
        Selected.Add(select);
        Repaint();
    }

    void DeselectNode(FieldNode select)
    {
        if (!Selected.Contains(select))
            return;

        select.Gizmo_Selected = false;
        Selected.Remove(select);
        Repaint();
    }

    void ResetSelection()
    {
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

        Selected[0].Connect(Selected[1]);
    }

    void FlipSelected()
    {
        if (Selected.Count != 2)
            return;

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
