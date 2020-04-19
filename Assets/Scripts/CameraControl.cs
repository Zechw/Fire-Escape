using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public GameObject gameToWatch;

    // Start is called before the first frame update
    void Start()
    {
        Vector2Int boardSize = gameToWatch.GetComponent<GameBoard>().gameController.GetSize();
        Grid targetGrid = gameToWatch.GetComponent<Grid>();
        if (targetGrid.cellLayout == GridLayout.CellLayout.Rectangle)
        {
            Vector2 center = boardSize / 2;
            transform.position = new Vector3(center.x, center.y, transform.position.z);
        }
        else if (targetGrid.cellLayout == GridLayout.CellLayout.Isometric)
        {
            //TODO
            Debug.LogError("Implement ISO camera centering");
        }
        else
        {
            Debug.LogError("Camera doesn't know this cell layout");
        }

        //TODO also zoom
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
