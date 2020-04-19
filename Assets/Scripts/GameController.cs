using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameController
{
    public enum FloorTypes { None, Normal, Burned }
    public FloorTypes[,] gameSpaces;
    public List<Vector2Int> toons;
    public List<Vector2Int> obstacles;
    public List<Vector2Int> fires;
    public List<Vector2Int> exits;

    public struct GameState
    {
        public FloorTypes[,] gameSpaces;
        public List<Vector2Int> toons;
        public List<Vector2Int> obstacles;
        public List<Vector2Int> fires;
        public List<Vector2Int> exits;
    }

    public GameController(int width, int height)
    {
        gameSpaces = new FloorTypes[width, height];
        Debug.Log("default spaces");
        Debug.Log(gameSpaces[0, 0]);
    }

    public GameController(GameState state)
    {
        gameSpaces = state.gameSpaces;
        obstacles = state.obstacles;
        toons = state.toons;
        fires = state.fires;
        exits = state.exits;
        BurnFloors();
    }

    //TODO is there strut magic for this?
    public GameState GetState()
    {
        GameState state;
        state.gameSpaces = gameSpaces;
        state.obstacles = obstacles;
        state.toons = toons;
        state.fires = fires;
        state.exits = exits;
        return state;
    }

    public Vector2Int GetSize()
    {
        return new Vector2Int(gameSpaces.GetLength(0), gameSpaces.GetLength(1));
    }

    public bool MakePlay(Vector2Int playLoc)
    {
        Vector2Int gameSize = GetSize();
        if (playLoc.x < 0 || playLoc.y < 0 || playLoc.x > gameSize.x-1 || playLoc.y > gameSize.y-1)
        {
            Debug.Log("Play out of bounds");
            return false;
        }

        //legal move: remove fire if exits, otherwise start fire
        if (!fires.Remove(playLoc))
        {
            if (gameSpaces[playLoc.x, playLoc.y] == FloorTypes.Burned)
            {
                Debug.Log("Already burned"); //TODO surface this to UI
                return false;
            }
            fires.Add(playLoc);
        }

        MoveToons();
        SpreadFires(playLoc);
        CheckBurns();
        CheckExits();

        return true;
    }

    private void MoveToons() {
        for (int toonI = 0; toonI < toons.Count; toonI++)
        {
            Vector2Int toonLoc = toons[toonI];
            Vector2 safetyVector = GetSafetyVector(toonLoc);
            //Debug.DrawRay(new Vector3(toonLoc.x, toonLoc.y, 0), safetyVector, Color.green, 1f);
            MoveToon(toonI, safetyVector);
        }
    }

    private Vector2 GetSafetyVector(Vector2Int toonLoc)
    {
        List<Vector2> closestFireVectors = new List<Vector2>();
        float closest = Mathf.Infinity;
        foreach (Vector2Int fireLoc in fires)
        {
            Vector2 fireVector = fireLoc - toonLoc;
            if (Mathf.Approximately(fireVector.magnitude, closest))
            {
                closestFireVectors.Add(fireVector);
            }
            else if (fireVector.magnitude < closest)
            {
                closest = fireVector.magnitude;
                closestFireVectors.Clear();
                closestFireVectors.Add(fireVector);
            }
        }

        Vector2 safetyVector = Vector2.zero;
        foreach (Vector2 fireVector in closestFireVectors)
        {
            //Debug.DrawRay(new Vector3(toonLoc.x, toonLoc.y, 0), fireVector, Color.red, 1f);
            safetyVector -= fireVector;
        }
        return safetyVector.normalized;
    }

    private void MoveToon(int toonI, Vector2 safetyVector)
    {

        int newX = toons[toonI].x + Mathf.RoundToInt(safetyVector.x);
        int newY = toons[toonI].y + Mathf.RoundToInt(safetyVector.y);


        toons[toonI] = ClampToBoard(new Vector2Int(newX, newY));
        //FIXME check for obstacles & "None" floors.
    }

    private void CheckBurns()
    {
        int removedToonCount = 0;
        foreach (Vector2Int fireLoc in fires)
        {
            removedToonCount += toons.RemoveAll(t => t == fireLoc);
        }

        if (removedToonCount > 0)
        {
            Debug.LogError($"burned {removedToonCount} toons!");
            //FIXME show gameover screen
        }
    }

    private void CheckExits() {
        int exitingToonsCount = 0;
        foreach (Vector2Int exitLoc in exits)
        {
            exitingToonsCount += toons.RemoveAll(t => t == exitLoc);
        }
        if (exitingToonsCount > 0)
        {
            Debug.Log($"Saved {exitingToonsCount} toons!");
            if (toons.Count == 0)
            {
                Debug.LogError("Level complete!");
                //FIXME next level
            }
        }
    }


    private Vector2Int[] cardinalDirections = {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };
    private void SpreadFires(Vector2Int playLoc)
    {
        List<Vector2Int> newFires = new List<Vector2Int>();
        foreach (Vector2Int fireLoc in fires)
        {
            if (fireLoc == playLoc && gameSpaces[playLoc.x, playLoc.y] == FloorTypes.Normal) {
                newFires.Add(playLoc);
                continue;
            }
            foreach (Vector2Int direction in cardinalDirections)
            {
                Vector2Int newLoc = ClampToBoard(fireLoc + direction);
                if (gameSpaces[newLoc.x, newLoc.y] == FloorTypes.Normal && fireLoc != playLoc)
                {
                    newFires.Add(newLoc);
                }
            }
        }
        fires = newFires;
        BurnFloors();
    }
    private void BurnFloors() { 
        foreach (Vector2Int fireLoc in fires)
        {
            gameSpaces[fireLoc.x, fireLoc.y] = FloorTypes.Burned;
        }
    }


    private Vector2Int ClampToBoard(Vector2Int vector)
    {
        int newX = Mathf.Clamp(vector.x, 0, gameSpaces.GetLength(0) - 1);
        int newY = Mathf.Clamp(vector.y, 0, gameSpaces.GetLength(1) - 1);
        return new Vector2Int(newX, newY);
    } 
}
