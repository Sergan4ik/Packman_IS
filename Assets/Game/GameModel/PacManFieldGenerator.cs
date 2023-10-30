using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public static class PacManFieldGenerator
{
    public static void GenerateField(PackmanGameModel model, int fieldSize, float obstacleDensity, int foodCount, int ghostCount)
    {
        //generate field using perlin noise
        
        model.field = new List<List<FieldType>>();
        for (var i = 0; i < fieldSize; i++)
        {
            model.field.Add(new List<FieldType>());
            for (var j = 0; j < fieldSize; j++)
            {
                float xOff = Random.Range(0f, 1f) * Random.Range(0, 10000);
                float yOff = Random.Range(0f, 1f) * Random.Range(0, 10000);
                float blockTileNoise = Mathf.PerlinNoise(xOff + i * 1000, yOff + j * 1000);
                
                if (blockTileNoise < obstacleDensity)
                    model.field[i].Add(FieldType.Obstacle);
                else
                    model.field[i].Add(FieldType.Free);
            }
        }
        
        //place food
        for (var i = 0; i < foodCount; i++)
        {
            var x = Random.Range(0, model.field.Count);
            var y = Random.Range(0, model.field[x].Count);
            if (model.field[x][y] == FieldType.Free)
            {
                model.field[x][y] = FieldType.Food;
            }
            else
            {
                i--;
            }
        }
        
        model.packman = new Packman()
        {
            position = new Vector2Int(-1,-1)
        };

        //place packman
        var tries = 0;
        while (tries < 20 && model.packman.position == -Vector2Int.one)
        {
            //place packman
            var x = Random.Range(0, model.field.Count);
            var y = Random.Range(0, model.field[x].Count);
            if (model.field[x][y] == FieldType.Free)
            {
                model.packman.position = new Vector2Int(x,y);
                model.packman.speed = 2;
                break;
            }

            tries++;
        }

        if (tries >= 20)
            GenerateField(model, fieldSize, obstacleDensity, foodCount, ghostCount);
        
        //generate ghosts
        model.ghosts = new List<Ghost>();
        
        for (var i = 0; i < 4; i++)
        {
            var ghost = new Ghost()
            {
                position = new Vector2Int(-1,-1)
            };
            
            tries = 0;
            while (tries < 20 && ghost.position == -Vector2Int.one)
            {
                //place packman
                var x = Random.Range(0, model.field.Count);
                var y = Random.Range(0, model.field[x].Count);
                if (model.field[x][y] == FieldType.Free)
                {
                    ghost.position = new Vector2Int(x,y);
                    ghost.speed = 1;
                    break;
                }

                tries++;
            }
            
            model.ghosts.Add(ghost);
        }
    }
}