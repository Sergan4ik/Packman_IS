using System.Collections.Generic;
using UnityEngine;

public class PackmanGameModel
{
    public readonly float tickTime = 0.5f;
    //here is defined state of game
    public Packman packman;
    public Ghost[] ghosts;
    public int score;
    public GameState state = GameState.NotStarted;
    public List<List<FieldType>> field;
    public float elapsedTime = 0;

    public PackmanGameModel(float tickTime, int fieldSize)
    {
        this.tickTime = tickTime;
        field = PacManFieldGenerator.GenerateField(fieldSize, 0.3f, 5);
    }
    
    public void StartGame(Vector2Int packmanStart)
    {
        packman = new Packman()
        {
            position = packmanStart
        };
        
        state = GameState.Started;
    }

    public void TickUnity(float dt)
    {
        elapsedTime += dt;
        while (elapsedTime > tickTime)
        {
            elapsedTime -= tickTime;
            Tick();
        }
    }

    public void ProcessPackmanInput(Vector2Int direction)
    {
        
    }

    private void Tick()
    {
        CalculateGhostsLogic();
    }

    public void CalculateGhostsLogic()
    {
        //Calculate directions
        foreach (var ghost in ghosts)
        {
            CalculateGhostDirection(ghost);
        }
    }

    private void CalculateGhostDirection(Ghost ghost)
    {
        var target = GetNextPositionByAStar(ghost.position, packman.position);
        ghost.direction = target - ghost.position;
    }
    
    public Vector2Int GetNextPositionByAStar(Vector2Int start, Vector2Int target)
    {
        if (field[target.x][target.y] == FieldType.Obstacle) return start;
        // A* algorithm
        
        //this var is used to store nodes that are discovered but not yet evaluated
        var open = new List<Vector2Int>();
        //this var is used to store nodes that are already evaluated
        var closed = new List<Vector2Int>();
        //this var is used to store the node that is the closest to the target
        var cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        //this var is used to store the gScore of each node
        var gScore = new Dictionary<Vector2Int, float>();
        //this var is used to store the fScore of each node
        var fScore = new Dictionary<Vector2Int, float>();
        
        gScore[start] = 0;
        fScore[start] = Vector2Int.Distance(start, target);
        open.Add(start);
        while (open.Count > 0)
        {
            var current = open[0];
            foreach (var node in open)
            {
                if (fScore[node] < fScore[current])
                {
                    current = node;
                }
            }
            if (current == target)
            {
                return ReconstructPath(cameFrom, current);
            }
            open.Remove(current);
            closed.Add(current);
            foreach (var neighbor in GetNeighbors(current))
            {
                if (closed.Contains(neighbor)) continue;
                var tentativeGScore = gScore[current] + Vector2Int.Distance(current, neighbor);
                if (!open.Contains(neighbor))
                {
                    open.Add(neighbor);
                }
                else if (tentativeGScore >= gScore[neighbor])
                {
                    continue;
                }
                cameFrom[neighbor] = current;
                gScore[neighbor] = tentativeGScore;
                fScore[neighbor] = gScore[neighbor] + Vector2Int.Distance(neighbor, target);
            }
        }
        
        //return position if path not found
        return start;
        
    }

    private IEnumerable<Vector2Int> GetNeighbors(Vector2Int current)
    {
        var res = new List<Vector2Int>();
        if (current.x > 0) res.Add(new Vector2Int(current.x - 1, current.y));
        if (current.x < field.Count - 1) res.Add(new Vector2Int(current.x + 1, current.y));
        if (current.y > 0) res.Add(new Vector2Int(current.x, current.y - 1));
        if (current.y < field[0].Count - 1) res.Add(new Vector2Int(current.x, current.y + 1));
        return res;
    }

    private Vector2Int ReconstructPath(Dictionary<Vector2Int,Vector2Int> cameFrom, Vector2Int current)
    {
        if (cameFrom.TryGetValue(current, out var pos))
        {
            var p = ReconstructPath(cameFrom, pos);
            return p;
        }
        return current;
    }
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
    Obstacle,
    Food
}