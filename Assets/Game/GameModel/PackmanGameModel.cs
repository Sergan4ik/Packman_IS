using System.Collections.Generic;
using UnityEngine;

public class PackmanGameModel
{
    //here is defined state of game
    public Packman packman;
    public Ghost[] ghosts;
    public int score;
    public GameState state = GameState.NotStarted;
    public List<List<FieldType>> field;

    public void StartGame(Vector2Int packmanStart)
    {
        packman = new Packman()
        {
            position = packmanStart
        };
        
        state = GameState.Started;
    }
    
    public void CalculateGhostDirection()
    {
        
    }
    
    //public 
}

public enum GameState
{
    NotStarted,
    Started,
    Finished
}

public enum FieldType
{
    Free,
    Obstacle
}