using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TileManager : MonoBehaviour
{
    private static TileManager _instance;
    public static TileManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<TileManager>();

                if (_instance == null)
                {
                    GameObject container = new GameObject("TileManager");
                    _instance = container.AddComponent<TileManager>();
                }
            }

            return _instance;
        }
    }

    [SerializeField]
    private GameObject DefaultTile = null;


    public enum TileType
    {
        Default,
    }

    public GameObject GetTilePrefab(TileType type)
    {
        GameObject tile = DefaultTile;
        switch (type)
        {
            case TileType.Default:
                tile = DefaultTile;
                break;
        }

        return tile;
    }

    public GameObject GetTile(TileType type)
    {
        GameObject tile = DefaultTile;
        switch (type)
        {
            case TileType.Default:
                tile = DefaultTile;
                break;
        }

        return Instantiate(tile);
    }


}
