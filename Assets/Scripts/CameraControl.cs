using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public GameObject gameToWatch;
    public float zoomFactor = 0.7f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        GameController gameController =  gameToWatch.GetComponent<GameBoard>().gameController;
        if (gameController == null)
        {
            return;
        }

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

        gameObject.GetComponent<Camera>().orthographicSize = Mathf.Max(boardSize.x, boardSize.y) * zoomFactor;

        //TODO ease to new target
    }
}
