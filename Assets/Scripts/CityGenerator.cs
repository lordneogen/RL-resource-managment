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
    private List<GameObject> buildings = new List<GameObject>();
    
    

    private void Awake()
    {
        CITY_GRID = new int[GRID_SIZE, GRID_SIZE];
    }

    private void Start()
    {
        GenerateCity();
        Generator.Instance.cityGenerator = this;
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
        var buildings = FindObjectsOfType<BasicBuilding>();
        foreach (var element in buildings)
        {
            element.OnBuild();
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

        // Instantiate дома в центре области
        // Vector3 housePosition = new Vector3(x * distanceBetweenBuildings, 0, y * distanceBetweenBuildings);
        // Instantiate(housePrefab, housePosition, Quaternion.identity);
        Vector3 housePosition = new Vector3(x * distanceBetweenBuildings, 0, y * distanceBetweenBuildings)+offserVector;
        GameObject GameObject=Instantiate(housePrefab, housePosition, Quaternion.identity,parent:transform);
        this.buildings.Append(GameObject);
        var buildings = FindObjectsOfType<BasicBuilding>();
        foreach (var element in buildings)
        {
            element.OnBuild();
        }
    }

    // Генерация домов
    private void GenerateHouses()
    {
        List<(int width, int height, GameObject prefab)> houseTypes = new List<(int, int, GameObject)>
        {
            (1, 1, housePrefab1x1),
            (2, 2, housePrefab2x2),
            (3, 2, housePrefab3x2),
            (3, 3, housePrefab3x3)
        };

        Random random = new Random();

        for (int x = 0; x < GRID_SIZE; x++)
        {
            for (int y = 0; y < GRID_SIZE; y++)
            {
                if (CITY_GRID[x, y] == 0) // Пустое место
                {
                    var (width, height, prefab) = houseTypes[random.Next(houseTypes.Count)];
                    if (CanPlaceHouse(x, y, width, height))
                    {
                        PlaceHouse(x, y, width, height, prefab);
                    }
                }
            }
        }
        var buildings = FindObjectsOfType<BasicBuilding>();
        foreach (var element in buildings)
        {
            element.OnBuild();
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
                    Vector3 roadPosition = new Vector3(x * distanceBetweenBuildings, 0, y * distanceBetweenBuildings)+offserVector;
                    GameObject GameObject=Instantiate(roadPrefab, roadPosition, Quaternion.identity,parent:transform);
                    this.buildings.Append(GameObject);
                }
            }
        }
    }

    // Добавление нового дома рядом с дорогой
    public void AddNewHouseNearRoad()
    {
        List<(int width, int height, GameObject prefab)> houseTypes = new List<(int, int, GameObject)>
        {
            (1, 1, housePrefab1x1),
            (2, 2, housePrefab2x2),
            (3, 2, housePrefab3x2),
            (3, 3, housePrefab3x3)
        };

        Random random = new Random();

        for (int attempt = 0; attempt < 1000; attempt++)
        {
            // Поиск дороги
            int roadX = random.Next(0, GRID_SIZE);
            int roadY = random.Next(0, GRID_SIZE);

            if (CITY_GRID[roadX, roadY] == 1) // Дорога найдена
            {
                // Попробуем разместить дом рядом с дорогой
                int houseX = roadX + (random.NextDouble() < 0.5 ? -1 : 1);
                int houseY = roadY + (random.NextDouble() < 0.5 ? -1 : 1);

                if (houseX >= 0 && houseX < GRID_SIZE && houseY >= 0 && houseY < GRID_SIZE)
                {
                    var (width, height, prefab) = houseTypes[random.Next(houseTypes.Count)];
                    if (CanPlaceHouse(houseX, houseY, width, height))
                    {
                        PlaceHouse(houseX, houseY, width, height, prefab);
                        break;
                    }
                }
            }
        }
    }
}
