using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnWealthButtonClick : MonoBehaviour
{
    private WealthDistributionAnalysis wealthDistAnalysis;

    void Start()
    {
        wealthDistAnalysis = GameObject.Find("Analysis: Wealth Distribution").GetComponent<WealthDistributionAnalysis>();
    }

    public void CallSaveXML()
    {
        wealthDistAnalysis.SaveXML();
    }
}
