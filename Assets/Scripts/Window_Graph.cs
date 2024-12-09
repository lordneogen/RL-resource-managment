using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;
using TMPro;

public class Window_Graph : MonoBehaviour {

    [SerializeField] private Sprite circleSprite;
    private RectTransform graphContainer;
    private List<GameObject> gameObjectList;  // Список для хранения всех созданных объектов
    [SerializeField]
    private TextMeshProUGUI textMeshmax;
    [SerializeField]
    private TextMeshProUGUI textMeshmin;
    [SerializeField]
    private TextMeshProUGUI textMeshmidl;
    [SerializeField]
    private TextMeshProUGUI textMeshInfo; 

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
        
        valueList = valueList.GetRange(Mathf.Max(valueList.Count - 10, 0), valueList.Count - Mathf.Max(valueList.Count - 10, 0));
        
        // Удаление всех ранее созданных объектов
        foreach (GameObject go in gameObjectList) {
            Destroy(go);
        }
        gameObjectList.Clear();  // Очищаем список после удаления объектов

        float graphHeight = graphContainer.sizeDelta.y;

        // Вычисление максимального и минимального значений
        float yMaximum = (int)Mathf.Max(valueList.ToArray());
        float yMinimum = (int)Mathf.Min(valueList.ToArray());
        
        textMeshmax.SetText(((int)yMaximum).ToString());
        textMeshmin.SetText(((int)yMinimum).ToString());
        textMeshmidl.SetText(((int)((-yMinimum+yMaximum)/2)).ToString());
        
        float yRange = yMaximum - yMinimum;
        float yPadding = yRange * 0.1f;  // 10% от диапазона для отступа сверху и снизу
        yMaximum += yPadding;
        yMinimum -= yPadding;

        float xSize = 50f;

        GameObject lastCircleGameObject = null;
        for (int i = 0; i < valueList.Count; i++) {
            float xPosition = xSize + i * xSize;
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

    public void showInfo(string info)
    {
        textMeshInfo.SetText(info);
    }

    private void Start()
    {
        Generator.Instance.agentWithGenerator.BuildingGraph = this;
    }
}
