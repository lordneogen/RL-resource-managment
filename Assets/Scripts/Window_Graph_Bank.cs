using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;
using TMPro;

public class Window_Graph_Bank : MonoBehaviour {

    [SerializeField] private Sprite circleSprite;
    private RectTransform graphContainer;
    private List<GameObject> gameObjectList;  // Список для хранения всех созданных объектов
    [SerializeField]
    private TextMeshProUGUI textMeshmax;
    [SerializeField]
    private TextMeshProUGUI textMeshmin;
    [SerializeField]
    private TextMeshProUGUI textMeshmidl;
    public string val;

    private void Awake() {
        graphContainer = transform.Find("graphContainer").GetComponent<RectTransform>();
        gameObjectList = new List<GameObject>();  // Инициализация списка
    }

    private GameObject CreateCircle(Vector2 anchoredPosition) {
        GameObject gameObject = new GameObject("circle", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().sprite = circleSprite;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(11, 11);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        
        // Добавляем созданный объект в список
        gameObjectList.Add(gameObject);
        
        return gameObject;
    }

    public void ShowGraph(List<float> valueList) {
        
        valueList = valueList.GetRange(Mathf.Max(0, valueList.Count - 100), Mathf.Min(100, valueList.Count));
        // Удаление всех ранее созданных объектов
        foreach (GameObject go in gameObjectList) {
            Destroy(go);
        }
        gameObjectList.Clear();  // Очищаем список после удаления объектов

        float graphHeight = graphContainer.sizeDelta.y;
        float graphWidth = graphContainer.sizeDelta.x;

        // Вычисление максимального и минимального значений
        float yMaximum = Mathf.Max(valueList.ToArray());
        float yMinimum = Mathf.Min(valueList.ToArray());

        // Настройка текстов
        textMeshmax.SetText(yMaximum.ToString("F2")); // Точное значение с 2 знаками после запятой
        textMeshmin.SetText(yMinimum.ToString("F2")); // Точное значение с 2 знаками после запятой
        textMeshmidl.SetText(((yMaximum + yMinimum) / 2f).ToString("F2")); // Среднее значение

        float yRange = yMaximum - yMinimum;
        float yPadding = yRange * 0.1f;  // Отступ сверху и снизу
        yMaximum += yPadding;
        yMinimum -= yPadding;

        // Вычисляем расстояние между точками по оси X
        float xSize = graphWidth / Mathf.Max(1, valueList.Count - 1);

        GameObject lastCircleGameObject = null;
        for (int i = 0; i < valueList.Count; i++) {
            float xPosition = i * xSize; // Равномерное распределение точек по ширине графика
            float yPosition = ((valueList[i] - yMinimum) / (yMaximum - yMinimum)) * graphHeight;

            GameObject circleGameObject = CreateCircle(new Vector2(xPosition, yPosition));
            if (lastCircleGameObject != null) {
                CreateDotConnection(lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition, circleGameObject.GetComponent<RectTransform>().anchoredPosition);
            }
            lastCircleGameObject = circleGameObject;
        }
    }



    private void CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB) {
        GameObject gameObject = new GameObject("dotConnection", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().color = new Color(1, 1, 1, .5f);
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        Vector2 dir = (dotPositionB - dotPositionA).normalized;
        float distance = Vector2.Distance(dotPositionA, dotPositionB);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.sizeDelta = new Vector2(distance, 3f);
        rectTransform.anchoredPosition = dotPositionA + dir * distance * .5f;
        rectTransform.localEulerAngles = new Vector3(0, 0, UtilsClass.GetAngleFromVectorFloat(dir));

        // Добавляем созданный объект в список
        gameObjectList.Add(gameObject);
    }

    private void Start()
    {
        if (val == "Error")
        {
            Generator.Instance.agentWithGenerator.ErrorGraph = this;
        }
        else
        {
            Generator.Instance.agentWithGenerator.EnergyGraph = this;
        }
    }
}
