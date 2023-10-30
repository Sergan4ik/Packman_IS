using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public static class PacManFieldGenerator
{
    public static List<List<FieldType>> GenerateField(int size, double obstacleDensity, int foodCount)
    {
        if (obstacleDensity < 0 || obstacleDensity > 1)
            throw new ArgumentException("Obstacle density should be between 0 and 1");

        if (size < 3 || foodCount < 0 || foodCount > size * size)
            throw new ArgumentException("Invalid size or food count");

        var field = new List<List<FieldType>>();

        for (int i = 0; i < size; i++)
        {
            field.Add(new List<FieldType>());
            for (int j = 0; j < size; j++)
            {
                if (obstacleDensity == 1 || (obstacleDensity > 0 && Random.value <= obstacleDensity))
                {
                    field[i].Add(FieldType.Obstacle);
                }
                else if (foodCount > 0 && Random.value <= (double)foodCount / (size * size))
                {
                    field[i].Add(FieldType.Food);
                    foodCount--;
                }
                else
                {
                    field[i].Add(FieldType.Free);
                }
            }
        }

        return field;
    }
}