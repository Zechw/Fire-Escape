using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FloorLayer : SpriteLayerBase
{
    //TODO: don't couple mouse control down into here.
    private void OnMouseDown()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        gameBoard.registerClick(Input.mousePosition);
    }
}
