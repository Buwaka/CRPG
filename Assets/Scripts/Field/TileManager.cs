using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//public static ComboCreator _instance;
//public static ComboCreator Instance
//{
//get
//{
//    if (_instance == null)
//    {
//        _instance = GameObject.FindObjectOfType<ComboCreator>();

//        if (_instance == null)
//        {
//            GameObject container = new GameObject("ComboCreator");
//            _instance = container.AddComponent<ComboCreator>();
//        }
//    }

//    return _instance;
//}
//}

public class TileManager : MonoBehaviour
{
    private static GameObject DefaultTile;
    private static GameObject ReachableTile;

    public enum TileType
    {
        Default,
    reachable
    }


    public static GameObject GetTile(TileType type)
    {
        GameObject tile = DefaultTile;
        switch (type)
        {
                case TileType.Default:
                    tile = DefaultTile;
                    break;
            case TileType.reachable:
                tile = ReachableTile;
                break;
        }

        return Instantiate(tile);
    }


}
