using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public FieldMap StartFieldMap;

    private FieldNode CurrentNode = null;
    private List<GameObject> Highlights = new List<GameObject>();

    [SerializeField]
    private GameObject ReachableHighlight = null;
    [SerializeField]
    private GameObject CurrentPositionHighlight = null;

    // Start is called before the first frame update
    void Start()
    {
        if (StartFieldMap)
        {
            CurrentNode = StartFieldMap.GetEntryPoint();
            if (CurrentNode)
            {
                SetPosition(CurrentNode.GetPosition());
            }


            Reset();
        }
    }

    void Reset()
    {
        ResetHighlights();

        HighlightReachableNodes();
        HighlightPosition();
    }

    void HighlightPosition()
    {
        if (!CurrentPositionHighlight)
            return;

        var hl = Instantiate(CurrentPositionHighlight);
        hl.transform.position = transform.position;

        Highlights.Add(hl);
    }

    void HighlightReachableNodes()
    {
        if (!CurrentNode)
            return;
        if (!ReachableHighlight)
            return;

        var connections = CurrentNode.GetConnections();

        foreach (var node in connections)
        {
            var hl = Instantiate(ReachableHighlight);
            hl.transform.position = node.transform.position;

            Highlights.Add(hl);
        }
    }

    void ResetHighlights()
    {
        for (int i = 0; i < Highlights.Count; i++)
        {
            Destroy(Highlights[i]);
        }
        Highlights.Clear();
    }

    void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    void MoveNode(FieldNode node)
    {
        CurrentNode = node;

        SetPosition(node.GetPosition());

        Reset();
    }

    void CheckInput()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Camera.main.nearClipPlane;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePos);

            var connections = CurrentNode.GetConnections();

            foreach (var node in connections)
            {
                float distance = Vector2.Distance(worldPosition, node.GetPosition());

                //replace with a proper size instead of the gizmo size
                if (distance < node.Gizmo_Size)
                {
                    MoveNode(node);
                    return;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
    }
}
