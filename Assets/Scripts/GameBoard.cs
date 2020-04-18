using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameBoard : MonoBehaviour
{
    struct GameSpace
    {

    }

    private int width;
    private int height;
   //private int[,];



    private GridLayout gridLayout;

    public Tile floorTile;
    public Tile burntFloorTile;
    public Tile fireTile;
    public Tile toonTile;
    public Tile exitTile;

    private FloorLayer floorLayer;
    private FireLayer fireLayer;
    private ToonLayer toonLayer;
    private ExitLayer exitLayer;

    public SpriteLayerBase[] spriteLayers;
    //FIXME; has duplicate references; indidual & in list. Pick one implementation?
  

    private void Start()
    {
        gridLayout = gameObject.GetComponent<GridLayout>();

        //TODO cleaner, less coupled
        floorLayer = gameObject.GetComponentInChildren<FloorLayer>();
        fireLayer = gameObject.GetComponentInChildren<FireLayer>();
        toonLayer = gameObject.GetComponentInChildren<ToonLayer>();
        exitLayer = gameObject.GetComponentInChildren<ExitLayer>();

        spriteLayers = gameObject.GetComponentsInChildren<SpriteLayerBase>();
    }

    public void registerClick(Vector3 mousePosition)
    {
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector3Int cellPosition = gridLayout.WorldToCell(worldPoint);
        Debug.Log(cellPosition);
        foreach (SpriteLayerBase layer in spriteLayers)
        {
            layer.DebugLoc(cellPosition);
        }
        
    }
}
