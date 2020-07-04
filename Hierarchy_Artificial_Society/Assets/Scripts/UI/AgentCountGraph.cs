using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AgentCountGraph : MonoBehaviour
{
    //area we will plot points on
    private RectTransform plotArea;
    //image of points to plot
    public Sprite circleSprite;

    void Awake()
    {
        plotArea = transform.Find("Agent Count Plot Area").GetComponent<RectTransform>();
    }

    private void Circle(Vector2 anchor)
    {
        GameObject point = new GameObject("circle", typeof(Image));
        point.transform.SetParent(plotArea, false);
        point.GetComponent<Image>().sprite = circleSprite;
        RectTransform pointRectTransform = point.GetComponent<RectTransform>();
        pointRectTransform.anchoredPosition = anchor;
        //size of each point
        pointRectTransform.sizeDelta = new Vector2(10, 10);
        //lower left corner
        pointRectTransform.anchorMin = new Vector2(0, 0);
        pointRectTransform.anchorMax = new Vector2(0, 0);
    }

    public void CreateGraph(List<int> graphPoints, int start, int end)
    {
        float graphHeight = plotArea.sizeDelta.y;
        float yMax = 2000f;
        float xSize = 10f;
        
        for (int i = start; i < graphPoints.Count; ++i)
        {
            float x = (i%100) * xSize;             
            float y = (graphPoints[i] / yMax) * graphHeight;
            Circle(new Vector2(x, y));
        }
    }
}