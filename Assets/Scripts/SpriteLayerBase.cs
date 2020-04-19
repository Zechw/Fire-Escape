using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class SpriteLayerBase: MonoBehaviour
{
    protected GameBoard gameBoard;
    public Tilemap tilemap;

    protected TileBase[] tiles;

    private void Awake()
    {
        gameBoard = gameObject.GetComponentInParent<GameBoard>();
        tilemap = gameObject.GetComponent<Tilemap>();

    }

    public void DebugLayer()
    {


        TileBase[]  tiles = GetAllTiles();

        
        Debug.Log("----");
        Debug.Log(gameObject.name);
        Debug.Log(tiles.Length);
        foreach (TileBase tile in tiles)
        {
            Debug.Log(tile);
        }
        
        
    }


    public void DebugLoc(Vector3Int loc)
    {
        TileBase tile = tilemap.GetTile(loc);
        if (tile != null)
        {
            Debug.Log(tile);
        }
    }

    private TileBase[] GetAllTiles()
    {
        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] tiles =  tilemap.GetTilesBlock(bounds);
        
        //remove nulls
        List<TileBase> tileList = new List<TileBase>(tiles);
        tileList.RemoveAll(x => x == null);
        tiles = tileList.ToArray();

        return tiles;
    }
}
