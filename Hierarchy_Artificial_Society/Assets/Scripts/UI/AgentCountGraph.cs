using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AgentCountGraph : MonoBehaviour
{
    //area we will plot points on
    private RectTransform plotArea;
    // Array to store points on graph
    // Enables us to reallocate memory after 100
    public static GameObject[] circleList = new GameObject[100];
    // Can set this to the circle prefab from inspector
    [SerializeField] private GameObject circlePrefab = null;

    void Awake()
    {
        plotArea = transform.Find("Agent Count Plot Area").GetComponent<RectTransform>();
    }

    private GameObject Circle(Vector2 anchor)
    {
        GameObject point = GameObject.Instantiate(circlePrefab); ;
        point.transform.SetParent(plotArea, false);
        RectTransform pointRectTransform = point.GetComponent<RectTransform>();
        pointRectTransform.anchoredPosition = anchor;
        return point;
    }

    public void CreateGraph(List<int> graphPoints, int i)
    {
        float graphHeight = plotArea.sizeDelta.y;
        float yMax = 1000f;
        float xSize = 10f;

        float x = (i % 100) * xSize;       
        float y = (graphPoints[i] / yMax) * graphHeight;
        
        if (i < 100)
            circleList[i] = Circle(new Vector2(x, y));
        else
        {
            circleList[i % 100].GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
        }
    }
}