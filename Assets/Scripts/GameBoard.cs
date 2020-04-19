using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameBoard : MonoBehaviour
{
    public Tile unburntfloorTile;
    public Tile burntFloorTile;
    public Tile fireTile;
    public Tile toonTile;
    public Tile toonAwayTile;
    public Tile exitTile;
    public Tile unknownTile;

    public GameController gameController;

    private FloorLayer floorLayer;
    private FireLayer fireLayer;
    private ToonLayer toonLayer;
    private ExitLayer exitLayer;

    public SpriteLayerBase[] spriteLayers;
    //FIXME; has duplicate references; indidual & in list. Pick one implementation?

    private GridLayout gridLayout;

    private void Awake()
    {
        gridLayout = gameObject.GetComponent<GridLayout>();

        //TODO cleaner, less coupled
        floorLayer = gameObject.GetComponentInChildren<FloorLayer>();
        fireLayer = gameObject.GetComponentInChildren<FireLayer>();
        toonLayer = gameObject.GetComponentInChildren<ToonLayer>();
        exitLayer = gameObject.GetComponentInChildren<ExitLayer>();

        spriteLayers = gameObject.GetComponentsInChildren<SpriteLayerBase>();

        /*
        //load tilemaps into gameBoard
        BoundsInt boardBounds = floorLayer.tilemap.cellBounds;
        Debug.Log(boardBounds);
        gameSpaces = new Tile[boardBounds.size.x, boardBounds.size.y];

        //TODO: efficency - this gets a lot of empy space
        foreach (Vector3Int position in boardBounds.allPositionsWithin)
        {
            TileBase tile = floorLayer.tilemap.GetTile( position );
            if (tile != null)
            {
                floorLayer.tilemap.SetTile(position, unknownTile);
            }
        }
        */

        gameController = new GameController(6, 6);
    }

    private void Start()
    {
        RenderTiles();
    }

    // place tiles in desired positions, for rendering
    private void RenderTiles()
    {
        //clear
        foreach (SpriteLayerBase l in spriteLayers)
        {
            l.tilemap.ClearAllTiles();
        }
        //floor
        Vector2Int boardSize = gameController.GetSize();
        for (int x = 0; x < boardSize.x; x++)
        {
            for (int y = 0; y < boardSize.y; y++)
            {
                floorLayer.tilemap.SetTile(
                    new Vector3Int(x, y, 0),
                    gameController.gameSpaces[x, y]? burntFloorTile : unburntfloorTile
                );
            }
        }
        //toons
        foreach (Vector3Int toonLoc in gameController.toons)
        {
            toonLayer.tilemap.SetTile(toonLoc, toonTile);
        }
        //fires
        foreach (Vector3Int fireLoc in gameController.fires)
        {
            fireLayer.tilemap.SetTile(fireLoc, fireTile);
        }
        //exits
        foreach (Vector3Int exitLoc in gameController.exits)
        {
            exitLayer.tilemap.SetTile(exitLoc, exitTile);
        }

    }

    private void OnMouseDown()
    {
        //only a colider on the floor layer; so clicks are relative to grid loc of mouse
        // - could probably implement on it's own; not coupled to floor collider
        // - if raycast, could save the collider & rigidbody
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = gridLayout.WorldToCell(mousePosition);

        if (gameController.MakePlay(new Vector2Int(cellPosition.x, cellPosition.y)))
        {
            RenderTiles();
        }
    }
}
