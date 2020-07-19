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
        pointRectTransform.sizeDelta = new Vector2(10, 10);
        //lower left corner of plot area
        pointRectTransform.anchorMin = new Vector2(0, 0);
        pointRectTransform.anchorMax = new Vector2(0, 0);
    }
}
