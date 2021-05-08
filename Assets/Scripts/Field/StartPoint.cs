using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPoint : MonoBehaviour
{
    private FieldNode node;


    // Start is called before the first frame update
    void Start()
    {
        node = GetComponent<FieldNode>();
    }

    FieldNode GetNode()
    {
        return node;
    }
}
