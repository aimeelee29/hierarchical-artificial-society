using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TradePriceGraph : MonoBehaviour
{
    //area we will plot points on
    private RectTransform plotArea;
    //image of points to plot
    public Sprite circleSprite;
    // Array to store points on graph
    // Enables us to reallocate memory after 100
    public static GameObject[] circleList = new GameObject[100];

    void Awake()
    {
        plotArea = transform.Find("Trade Price Plot Area").GetComponent<RectTransform>();
    }

    private GameObject Circle(Vector2 anchor)
    {
        GameObject point = new GameObject("circle", typeof(Image));
        point.transform.SetParent(plotArea, false);
        point.GetComponent<Image>().sprite = circleSprite;
        RectTransform pointRectTransform = point.GetComponent<RectTransform>();
        pointRectTransform.anchoredPosition = anchor;
        //size of each point
        pointRectTransform.sizeDelta = new Vector2(10, 10);
        //lower left corner of plot area
        pointRectTransform.anchorMin = new Vector2(0, 0);
        pointRectTransform.anchorMax = new Vector2(0, 0);
        return point;
    }

    public void CreateGraph(List<double> graphPoints, int i)
    {
        float graphHeight = plotArea.sizeDelta.y;
        float yMax = 2f;
        float xSize = 10f;

        float x = (i % 100) * xSize;
        float y = (float)(graphPoints[i] / yMax) * graphHeight;

        if (i < 100)
            circleList[i] = Circle(new Vector2(x, y));
        else
        {
            circleList[i % 100].GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
        }
    }
}

