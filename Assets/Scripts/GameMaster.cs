﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controlls the menus & flow of the levls
public class GameMaster : MonoBehaviour
{
    public Grid[] levels;
    private int levelI = 0;

    private GameBoard gameBoard;

    // UI components //FIXME cleaner mgmt... #ludumCrunch
    public GameObject winScreen;
    public GameObject loseScreen;
    public GameObject restart;
    public GameObject endOfGame;
    public GameObject mainMenu;
    public GameObject loading;


    private 

    // Start is called before the first frame update
    void Start()
    {
        gameBoard = gameObject.GetComponentInChildren<GameBoard>();
        loading.SetActive(false);
        mainMenu.SetActive(true);
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
        gameBoard.LoadTilemaps(levels[levelI]);
        winScreen.SetActive(false);
        loseScreen.SetActive(false);
        restart.SetActive(true); //FIXME PassTurn button is child'ed to save me from toggling it too. group to single game obj
    }

    public void SkipTurn()
    {
        gameBoard.gameController.SkipTurn();
        gameBoard.AdvanceState(); //overcoupled
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
