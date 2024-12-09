using System;
using System.Collections.Generic;
using System.Linq;
using CityBuilder;
using UnityEngine;
using Random = System.Random;

public class CityGenerator : MonoBehaviour
{
    public GameObject housePrefab1x1;
    public GameObject housePrefab2x2;
    public GameObject housePrefab3x2;
    public GameObject housePrefab3x3;
    public GameObject roadPrefab;

    public Vector3 offserVector;
    public int GRID_SIZE = 20;
    public float distanceBetweenBuildings = 10f;
    public int roadInterval = 5;

    private int[,] CITY_GRID;
    public List<GameObject> buildings_new = new List<GameObject>();
    public int currentHouseCount = 0; // Количество домов на поле

    private void Awake()
    {
        CITY_GRID = new int[GRID_SIZE, GRID_SIZE];
    }

    private void Start()
    {
        if (!Generator.Instance.agentWithGenerator.IsTraining)
        {
            GenerateCity();
        }
        Generator.Instance.agentWithGenerator.CityGenerator = this;
    }

    // Генерация города
    private void GenerateCity()
    {
        CreateRoads();
        GenerateHouses();
        DrawGrid();
        var buildings = FindObjectsOfType<BasicBuilding>();
        foreach (var element in buildings)
        {
            element.OnBuild();
        }
    }

    // Создание дорог
    private void CreateRoads()
    {
        for (int i = 0; i < GRID_SIZE; i++)
        {
            for (int j = 0; j < GRID_SIZE; j++)
            {
                if (i % roadInterval == 0 || j % roadInterval == 0)
                {
                    CITY_GRID[i, j] = 1; // Дорога
                }
            }
        }
    }

    // Проверка, можно ли разместить дом
    private bool CanPlaceHouse(int x, int y, int width, int height)
    {
        if (x + width > GRID_SIZE || y + height > GRID_SIZE)
            return false;

        for (int i = x; i < x + width; i++)
        {
            for (int j = y; j < y + height; j++)
            {
                if (CITY_GRID[i, j] != 0) // Занято дорогой или домом
                    return false;
            }
        }
        return true;
    }

    // Размещение дома
    private void PlaceHouse(int x, int y, int width, int height, GameObject housePrefab)
    {
        for (int i = x; i < x + width; i++)
        {
            for (int j = y; j < y + height; j++)
            {
                CITY_GRID[i, j] = 2; // Занято домом
            }
        }
        Vector3 housePosition = new Vector3(x * distanceBetweenBuildings, 0, y * distanceBetweenBuildings) + offserVector;
        GameObject building = Instantiate(housePrefab, housePosition, Quaternion.identity, parent: transform);
        Generator.Instance.agentWithGenerator.Buildings.Add(building.GetComponent<Build>());
        buildings_new.Add(building);
        currentHouseCount++; // Увеличиваем количество домов
    }

    // Генерация домов
    private void GenerateHouses()
    {
        List<(int width, int height, GameObject prefab, float probability)> houseTypes = new List<(int, int, GameObject, float)>
        {
            (1, 1, housePrefab1x1, 0.99f),
            (2, 2, housePrefab2x2, 0.7f),
            (3, 2, housePrefab3x2, 0.5f),
            (3, 3, housePrefab3x3, 0.3f)
        };

        Random random = new Random();

        for (int x = 0; x < GRID_SIZE; x++)
        {
            for (int y = 0; y < GRID_SIZE; y++)
            {
                if (CITY_GRID[x, y] == 0) // Пустое место
                {
                    foreach (var (width, height, prefab, probability) in houseTypes.OrderByDescending(h => h.probability))
                    {
                        if (random.NextDouble() < probability && CanPlaceHouse(x, y, width, height))
                        {
                            PlaceHouse(x, y, width, height, prefab);
                            break;
                        }
                    }
                }
            }
        }
    }

    // Отрисовка дорог
    private void DrawGrid()
    {
        for (int x = 0; x < GRID_SIZE; x++)
        {
            for (int y = 0; y < GRID_SIZE; y++)
            {
                if (CITY_GRID[x, y] == 1) // Если это дорога
                {
                    Vector3 roadPosition = new Vector3(x * distanceBetweenBuildings, 0, y * distanceBetweenBuildings) + offserVector;
                    GameObject road = Instantiate(roadPrefab, roadPosition, Quaternion.identity, parent: transform);
                    buildings_new.Add(road);
                }
            }
        }
    }

    public void ResetGrid()
    {
        CITY_GRID = new int[GRID_SIZE, GRID_SIZE];
        foreach (var build in buildings_new)
        {
            Destroy(build);
        }
        buildings_new.Clear();
        currentHouseCount = 0; // Сброс количества домов
        Start();
    }

    // Добавление нового дома рядом с дорогой
    public void AddNewHouseNearRoad()
    {
        List<(int width, int height, GameObject prefab, float probability)> houseTypes = new List<(int, int, GameObject, float)>
        {
            (1, 1, housePrefab1x1, 0.5f),
            (2, 2, housePrefab2x2, 0.1f),
            (3, 2, housePrefab3x2, 0.1f),
            (3, 3, housePrefab3x3, 0.1f)
        };

        Random random = new Random();

        for (int attempt = 0; attempt < 1000; attempt++)
        {
            int roadX = random.Next(0, GRID_SIZE);
            int roadY = random.Next(0, GRID_SIZE);

            if (CITY_GRID[roadX, roadY] == 1) // Дорога найдена
            {
                int houseX = roadX + (random.NextDouble() < 0.5 ? -1 : 1);
                int houseY = roadY + (random.NextDouble() < 0.5 ? -1 : 1);

                if (houseX >= 0 && houseX < GRID_SIZE && houseY >= 0 && houseY < GRID_SIZE)
                {
                    foreach (var (width, height, prefab, probability) in houseTypes.OrderByDescending(h => h.probability))
                    {
                        if (random.NextDouble() < probability && CanPlaceHouse(houseX, houseY, width, height))
                        {
                            PlaceHouse(houseX, houseY, width, height, prefab);
                            break;
                        }
                    }
                }
            }
        }
    }
}
