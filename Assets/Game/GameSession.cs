using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameSession : MonoBehaviour
{
    private PackmanGameModel _model;
    private List<GhostView> ghostViews = new List<GhostView>();
    private PackmanView packman;
    private int shownTick = -1;
    
    public void Awake()
    {
    }

    public void StartNewGame()
    {
        ClearField();
        
        _model = new PackmanGameModel(1f / 30f, 10);
        DrawField(_model);
        _model.StartGame();
    }

    public void Update()
    {
        if (_model?.state == GameState.Finished)
        {
            Debug.Log($"Game finished with score {_model.score}");
            StartNewGame();
        }
        
        if (Input.GetKeyDown(KeyCode.N))
        {
            StartNewGame();
        }
        
        if (_model == null || _model.state != GameState.Started) return;
        
        PollInput();
        _model.TickUnity(Time.deltaTime);
        DrawField(_model);
    }

    public void PollInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            _model.ProcessPackmanInput(Vector2Int.up);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            _model.ProcessPackmanInput(Vector2Int.down);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            _model.ProcessPackmanInput(Vector2Int.left);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            _model.ProcessPackmanInput(Vector2Int.right);
        }
    }

    private void ClearField()
    {
        GameObject parent;
        if (transform.childCount == 0 || transform.GetChild(0).name != "FieldView")
        {
            parent = new GameObject()
            {
                name = "FieldView"
            };
            parent.transform.parent = this.transform;
        }
        else
        {
            parent = transform.GetChild(0).gameObject;
        }
        
        foreach (Transform child in parent.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void DrawField(PackmanGameModel model)
    {
        //TODO if will be lags, pool previous field
        if (shownTick == model.tick) return;
        
        GameObject obstacle = Resources.Load<GameObject>("Obstacle");
        GameObject free = Resources.Load<GameObject>("Free");
        GameObject packmanPrefab = Resources.Load<GameObject>("Packman");
        GameObject ghostPrefab = Resources.Load<GameObject>("Ghost");


        GameObject parent;
        if (transform.childCount == 0 || transform.GetChild(0).name != "FieldView")
        {
            parent = new GameObject()
            {
                name = "FieldView"
            };
            parent.transform.parent = this.transform;
        }
        else
        {
            parent = transform.GetChild(0).gameObject;
        }
        
        foreach (Transform child in parent.transform)
        {
            if (!child.TryGetComponent<GhostView>(out var ghost_) && !child.TryGetComponent<PackmanView>(out var packman_))
                Destroy(child.gameObject);
        }

        foreach (var ghostPos in model.ghosts)
        {
            if (ghostViews.Any(g => g.model == ghostPos) == false)
            {
                var spawnedGhost = Instantiate(ghostPrefab,
                    new Vector3(ghostPos.position.x, ghostPos.position.y, -0.1f), Quaternion.identity,
                    parent.transform);
                spawnedGhost.name = $"[Ghost]";
                ghostViews.Add(spawnedGhost.GetComponent<GhostView>());
                ghostViews.Last().model = ghostPos;
            }

            ghostViews.Find(g => g.model == ghostPos).transform.position =
                new Vector3(ghostPos.position.x, ghostPos.position.y, -0.1f);
        }



        if (this.packman == null)
        {
            var spawnedPac = Instantiate(packmanPrefab,
                new Vector3(model.packman.position.x, model.packman.position.y, -0.1f), Quaternion.identity,
                parent.transform);
            spawnedPac.name = $"[Packman]";
            this.packman = spawnedPac.GetComponent<PackmanView>();
            this.packman.model = model.packman;
        }
        else
        {
            this.packman.transform.position = new Vector3(model.packman.position.x, model.packman.position.y, -0.1f);
        }
        
        for (var i = 0; i < model.field.Count; i++)
        {
            for (var j = 0; j < model.field[i].Count; j++)
            {
                if (model.field[i][j] == FieldType.Obstacle)
                {
                    var spawned = Instantiate(obstacle, new Vector3(i , j, 0), Quaternion.identity, parent.transform);
                    spawned.name = $"[{i};{j}]";
                }
                
                var spawnedFree = Instantiate(free, new Vector3(i, j, 0.1f), Quaternion.identity, parent.transform);
                spawnedFree.name = $"[{i};{j}]";
            }
        }
    }
}