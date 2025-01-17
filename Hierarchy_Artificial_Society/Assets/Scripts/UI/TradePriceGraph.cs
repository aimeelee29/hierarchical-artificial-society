﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TradePriceGraph : MonoBehaviour
{
    //area we will plot points on
    private RectTransform plotArea;
    //image of points to plot
    //public Sprite circleSprite;
    // Array to store points on graph
    // Enables us to reallocate memory after 100
    public static Circle[] circleList = new Circle[100];
    // Can set this to the circle prefab from inspector
    [SerializeField] private GameObject circlePrefab = null;

    void Awake()
    {
        plotArea = transform.Find("Trade Price Plot Area").GetComponent<RectTransform>();
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
        float yMax = 3f;
        float xSize = 10f;

        Vector2 circLocation = new Vector2();

        float x = (i % 100) * xSize;
        float y = (float)(graphPoints[i] / yMax) * graphHeight;

        circLocation.Set(x, y);

        if (i < 100)
            circleList[i] = Circle(new Vector2(x, y));
        else
        {
            circleList[i % 100].pointRectTransform.anchoredPosition = new Vector2(x, y);
        }
    }
}

