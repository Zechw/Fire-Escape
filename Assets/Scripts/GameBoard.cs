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

        LoadTilemaps();

    }

    private void LoadTilemaps()
    {        //TODO: efficency - this gets a lot of empy space
        //load tilemaps into gameBoard
        BoundsInt boardBounds = floorLayer.tilemap.cellBounds;
        Debug.Log(boardBounds);

        GameController.GameState gameState; 
        gameState.gameSpaces = new GameController.FloorTypes[boardBounds.size.x, boardBounds.size.y];
        gameState.toons = new List<Vector2Int>();
        gameState.obstacles = new List<Vector2Int>();
        gameState.fires = new List<Vector2Int>();
        gameState.exits = new List<Vector2Int>();
   
        Vector3Int offset = boardBounds.position * -1;


        foreach (Vector3Int tilemapPosition in boardBounds.allPositionsWithin)
        {
            List<TileBase> sprites = new List<TileBase>();
            foreach (SpriteLayerBase layer in spriteLayers)
            {
                TileBase tileFromMap = layer.tilemap.GetTile(tilemapPosition);
                if (tileFromMap != null)
                {
                    sprites.Add(tileFromMap);
                }

            }

            //offset position to 0,0 base & drop z
            Vector2Int loc = (Vector2Int)(offset + tilemapPosition);

            //parse floor
            GameController.FloorTypes floor = GameController.FloorTypes.None;
            if (sprites.Remove(unburntfloorTile))
            {
                floor = GameController.FloorTypes.Normal;
            }
            else if (sprites.Remove(burntFloorTile))
            {
                floor = GameController.FloorTypes.Burned;
            }
            gameState.gameSpaces[loc.x, loc.y] = floor;

            //parse toons
            if (sprites.Remove(toonTile))
            {
                gameState.toons.Add(loc);
            }

            //parse fire
            if (sprites.Remove(fireTile))
            {
                gameState.fires.Add(loc);
            }

            //TODO parse obstacles
            //parse exits
            if (sprites.Remove(exitTile))
            {
                gameState.exits.Add(loc);
            }
        }

        gameController = new GameController(gameState);
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
                Tile tile;
                switch (gameController.gameSpaces[x,y])
                {
                    case GameController.FloorTypes.None:
                        tile = unknownTile;
                        //TODO continue
                        break;
                    case GameController.FloorTypes.Normal:
                        tile = unburntfloorTile;
                        break;
                    case GameController.FloorTypes.Burned:
                        tile = burntFloorTile; //TODO rename burned
                        break;
                    default:
                        tile = unknownTile;
                        break;
                }
                floorLayer.tilemap.SetTile(new Vector3Int(x, y, 0),tile);
            }
        }
        //toons
        foreach (Vector3Int toonLoc in gameController.toons)
        {
            toonLayer.tilemap.SetTile(toonLoc, toonTile);
        }
        //TODO obstacles
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
