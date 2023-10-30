using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PackmanGameModel
{
    public readonly float tickTime = 0.5f;
    //here is defined state of game
    public Packman packman;
    public List<Ghost> ghosts;
    public int score;
    public GameState state = GameState.NotStarted;
    public List<List<FieldType>> field;
    public float elapsedTime = 0;
    public int tick = 0;
    
    private float fruitRadius = 0.1f;

    public PackmanGameModel(float tickTime, int fieldSize)
    {
        this.tickTime = tickTime;
        
        PacManFieldGenerator.GenerateField(this, fieldSize, 0.2f, 5, 2);
    }
    
    public void StartGame()
    {
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
        if (state != GameState.Started) return;
        packman.direction = direction;
    }

    public void Tick()
    {
        tick++;
        CalculateNextGhostMove();
        ProcessMovement(tickTime);

        CheckEndGame();
    }

    private IEnumerable<Vector2Int> GetCellInRadius(Vector2Int center, int radius)
    {
        for (var i = -radius; i <= radius; i++)
        {
            for (var j = -radius; j <= radius; j++)
            {
                var x = center.x + i;
                var y = center.y + j;
                if (IsPositionNotValid(new Vector2Int(x,y))) continue;
                yield return new Vector2Int(x,y);
            }
        }
    }
    
    private void CheckEndGame()
    {
        foreach (var cell in GetCellInRadius(Vector2Int.FloorToInt(packman.position), 1))
        {
            if (field[cell.x][cell.y] == FieldType.Food && Vector2.SqrMagnitude(packman.position - (Vector2)cell) <
                (fruitRadius + packman.radius) * (fruitRadius + packman.radius))
            {
                field[cell.x][cell.y] = FieldType.Free;
                score++;    
            }
        }

        foreach (var ghost in ghosts)
        {
            if (Vector2.SqrMagnitude(ghost.position - packman.position) < (ghost.radius + packman.radius) * (ghost.radius + packman.radius))
            {
                state = GameState.Finished;
            }
        }
    }

    private void ProcessMovement(float dt)
    {
        //check if pacman can move
        var nextPackmanPosition = packman.position + (Vector2)packman.direction * (packman.speed * dt);
        if (CanGoToCell(Vector2Int.RoundToInt(nextPackmanPosition), packman.radius))
        {
            packman.position = nextPackmanPosition;
        }
        else
        {
            packman.direction = Vector2Int.zero;
        }
        
        //check if ghosts can move
        foreach (var ghost in ghosts)
        {
            var nextGhostPosition = ghost.position + (Vector2)ghost.direction * (ghost.speed * dt);
            if (CanGoToCell(Vector2Int.RoundToInt(nextGhostPosition), ghost.radius))
            {
                ghost.position = nextGhostPosition;
            }
            else
            {
                ghost.direction = Vector2Int.zero;
            }
        }
    }

    public bool CanGoToCell(Vector2Int pos, float radius = 1)
    {
        if (IsPositionNotValid(pos)) return false;
        
        foreach (var cell in GetCellInRadius(pos, 1))
        {
            if (field[cell.x][cell.y] == FieldType.Obstacle && Vector2.SqrMagnitude(pos - cell) < (0.5f + radius) * (0.5f +radius))
                return false;
        }

        return true;
    }

    private bool IsPositionNotValid(Vector2Int pos)
    {
        return pos.x < 0 || pos.x >= field.Count || pos.y < 0 || pos.y >= field[0].Count;
    }

    public void CalculateNextGhostMove()
    {
        // var roundedPackX = Mathf.RoundToInt(packman.position.x);
        // var roundedPackY = Mathf.RoundToInt(packman.position.x);
        
        foreach (var ghost in ghosts)
        {
            // var roundedX = Mathf.RoundToInt(ghost.position.x);
            // var roundedY = Mathf.RoundToInt(ghost.position.y);

            List<Vector2Int> path = AStarSearch(Vector2Int.RoundToInt(ghost.position), Vector2Int.RoundToInt(packman.position));
            
            if (path.Count > 1)
            {
                Vector2Int nextPosition = path[1];
                ghost.direction = CalculateDirection(Vector2Int.RoundToInt(ghost.position), nextPosition);
            }
        }
    }

    private List<Vector2Int> AStarSearch(Vector2Int start, Vector2Int goal)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        Dictionary<Vector2Int, float> gScore = new Dictionary<Vector2Int, float>();
        Dictionary<Vector2Int, float> fScore = new Dictionary<Vector2Int, float>();
        HashSet<Vector2Int> openSet = new HashSet<Vector2Int>();
        HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>();

        gScore[start] = 0;
        fScore[start] = start.ManhattanDistanceTo(goal);
        openSet.Add(start);

        while (openSet.Count > 0)
        {
            Vector2Int current = GetLowestFScore(openSet, fScore);
            if (current == goal)
            {
                path = ReconstructPath(cameFrom, current);
                return path;
            }

            openSet.Remove(current);
            closedSet.Add(current);

            foreach (var neighbor in GetNeighbors(current))
            {
                if (closedSet.Contains(neighbor))
                    continue;

                float tentativeGScore = gScore[current] + current.ManhattanDistanceTo(neighbor);

                if (!openSet.Contains(neighbor) || tentativeGScore < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = gScore[neighbor] + neighbor.ManhattanDistanceTo(goal);

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        return path; // Return an empty path if no path is found
    }

    private Vector2Int GetLowestFScore(HashSet<Vector2Int> openSet, Dictionary<Vector2Int, float> fScore)
    {
        // Find and return the node in openSet with the lowest fScore
        Vector2Int lowestNode = openSet.First();
        foreach (var node in openSet)
        {
            if (fScore.ContainsKey(node) && fScore[node] < fScore[lowestNode])
            {
                lowestNode = node;
            }
        }
        return lowestNode;
    }

    private List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int current)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        path.Add(current);
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Insert(0, current);
        }
        return path;
    }

    private IEnumerable<Vector2Int> GetNeighbors(Vector2Int current)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();
        Vector2Int[] directions = new Vector2Int[]
        {
            Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left ,
            //diagonals
            Vector2Int.up + Vector2Int.right, Vector2Int.up + Vector2Int.left,
            Vector2Int.down + Vector2Int.right, Vector2Int.down + Vector2Int.left
        };
        foreach (var direction in directions)
        {
            Vector2Int neighbor = current + direction;
            if (!IsPositionNotValid(neighbor) && field[neighbor.x][neighbor.y] != FieldType.Obstacle)
            {
                neighbors.Add(neighbor);
            }
        }
        return neighbors;
    }

    private Vector2Int CalculateDirection(Vector2Int current, Vector2Int next)
    {
        if (next.x > current.x)
            return Vector2Int.right;
        else if (next.x < current.x)
            return Vector2Int.left;
        else if (next.y > current.y)
            return Vector2Int.up;
        else
            return Vector2Int.down;
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