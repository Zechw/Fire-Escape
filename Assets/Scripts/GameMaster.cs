using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controlls the menus & flow of the levls
public class GameMaster : MonoBehaviour
{
    public Grid[] levels;
    private int levelI = 0;

    private GameBoard renderGrid;

    public GameObject winScreen;
    public GameObject loseScreen;
    public GameObject restart;
    public GameObject endOfGame;


    private 

    // Start is called before the first frame update
    void Start()
    {
        renderGrid = gameObject.GetComponentInChildren<GameBoard>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Success()
    {
        winScreen.SetActive(true);
        restart.SetActive(false);
    }

    public void Failure()
    {
        loseScreen.SetActive(true);
        restart.SetActive(false);
    }

    public void LoadLevel()
    {
        renderGrid.LoadTilemaps(levels[levelI]);
        winScreen.SetActive(false);
        loseScreen.SetActive(false);
        restart.SetActive(true);
    }

    public void NextLevel()
    {
        levelI++;
        if (levelI >= levels.Length)
        {
            endOfGame.SetActive(true);
        } else
        {
            LoadLevel();
        }
    }

    public void Restart()
    {
        endOfGame.SetActive(false);
        levelI = 0;
        LoadLevel();
    }

 



    
}
