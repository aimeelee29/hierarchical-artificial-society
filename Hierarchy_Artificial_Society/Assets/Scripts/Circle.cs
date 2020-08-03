using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Circle : MonoBehaviour
{

    //image of points to plot
    public Sprite circleSprite;

    void Awake()
    {
        GetComponent<Image>().sprite = circleSprite;
        RectTransform pointRectTransform = GetComponent<RectTransform>();
        //size of each point
        pointRectTransform.sizeDelta = new Vector2Int(10, 10);
        //lower left corner of plot area
        Vector2Int origin = new Vector2Int(0, 0);
        pointRectTransform.anchorMin = origin;
        pointRectTransform.anchorMax = origin;
    }
}
