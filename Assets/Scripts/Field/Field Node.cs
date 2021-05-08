using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FieldNode : MonoBehaviour
{
    public List<FieldNode> Connections = new List<FieldNode>();
    private GameObject tile;


    //gizmo shit
    public float Gizmo_Size = 0.5f;
    public bool Gizmo_Selected = false;


    // Start is called before the first frame update
    void Start()
    {
        tile = TileManager.GetTile(TileManager.TileType.Default);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Connect(FieldNode node)
    {
        if (IsConnected(node))
            return;

        Connections.Add(node);
    }

    public void Disconnect(FieldNode node)
    {
        if (!IsConnected(node))
            return;

        Connections.Remove(node);
    }

    public void FlipConnection(FieldNode node)
    {
        bool thisConnection = Connections.Contains(node);
        bool thatConnection = node.IsConnected(this);

        if (!thisConnection && !thatConnection)
            return;

        if (thisConnection && thatConnection)
        {
            node.Disconnect(this);
        }
        else if (thisConnection)
        {
            Disconnect(node);
            node.Connect(this);
        }
        else if (thatConnection)
        {
            Connect(node);
            node.Disconnect(this);
        }

    }

    public bool IsConnected(FieldNode node)
    {
        return Connections.Contains(node);
    }


    public void Move(Vector3 relative)
    {
        transform.position += relative;
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void DrawArrow(Vector3 from, Vector3 to, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 10.0f)
    {
        Gizmos.color = color;
        Vector3 direction = to - from;
        Gizmos.DrawRay(from, direction);

        Vector3 right = Quaternion.Euler(0, 0, arrowHeadAngle) * -direction;
        Vector3 left = Quaternion.Euler(0, 0, -arrowHeadAngle) * -direction;
        Gizmos.DrawRay(to, right * arrowHeadLength);
        Gizmos.DrawRay(to, left * arrowHeadLength);
    }

    void OnDrawGizmos()
    {
        if (Gizmo_Selected)
            Gizmos.color = Color.yellow;
        else
            Gizmos.color = Color.white;
        Gizmos.DrawSphere(transform.position, Gizmo_Size);

        for (int i = 0; i < Connections.Count; i++)
        {
            var connection = Connections[i];
            var color = Color.Lerp(Color.blue, Color.red, (float)i / Connections.Count);
            DrawArrow(transform.position, connection.transform.position, color);
        }
    }
}
