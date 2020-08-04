using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TradeQtyGraph : MonoBehaviour
{
    //area we will plot points on
    private RectTransform plotArea;
    // Array to store points on graph
    // Enables us to reallocate memory after 100
    public static Circle[] circleList = new Circle[100];
    // Can set this to the circle prefab from inspector
    [SerializeField] private GameObject circlePrefab = null;

    void Awake()
    {
        plotArea = transform.Find("Trade Qty Plot Area").GetComponent<RectTransform>();
    }

    private Circle Circle(Vector2 anchor)
    {
        GameObject point = GameObject.Instantiate(circlePrefab);
        point.transform.SetParent(plotArea, false);
        Circle circComponent = point.GetComponent<Circle>();
        circComponent.pointRectTransform.anchoredPosition = anchor;
        return circComponent;
    }

    public void CreateGraph(List<double> graphPoints, int i)
    {
        float graphHeight = plotArea.sizeDelta.y;
        float yMax = 2f;
        float xSize = 10f;

        float x = (i % 100) * xSize;
        float y = (float)(graphPoints[i] / yMax) * graphHeight;

        if(i < 100)
            circleList[i] = Circle(new Vector2(x, y));
        else
        {
            circleList[i % 100].pointRectTransform.anchoredPosition = new Vector2(x, y);
        }
    }
}
