using System;
using UnityEngine;

public class GameSession : MonoBehaviour
{
    public void Awake()
    {
        PackmanGameModel model = new PackmanGameModel(0.5f, 10);
        DrawField(model);
    }

    private void DrawField(PackmanGameModel model)
    {
        GameObject obstacle = Resources.Load<GameObject>("Obstacle");
        GameObject free = Resources.Load<GameObject>("Free");

        GameObject parent = new GameObject()
        {
            name = "FieldView"
        };
        
        for (var i = 0; i < model.field.Count; i++)
        {
            for (var j = 0; j < model.field[i].Count; j++)
            {
                if (model.field[i][j] == FieldType.Obstacle)
                {
                    var spawned = Instantiate(obstacle, new Vector3(i, j, 0), Quaternion.identity, parent.transform);
                    spawned.name = $"[{i};{j}]";
                }
                else if (model.field[i][j] == FieldType.Free)
                {
                    var spawned = Instantiate(free, new Vector3(i, j, 0), Quaternion.identity, parent.transform);
                    spawned.name = $"[{i};{j}]";
                }
            }
        }
    }
}
