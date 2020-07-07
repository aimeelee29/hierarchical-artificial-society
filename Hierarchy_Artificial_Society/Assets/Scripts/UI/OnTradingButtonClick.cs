using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/*
 * 
 * Class which saves trading data to XML on button click
 * 
 */

public class OnTradingButtonClick : MonoBehaviour
{
    private TradeAnalysis tradeAnalysis;

    void Awake()
    {
        tradeAnalysis = GameObject.Find("Analysis: Trading").GetComponent<TradeAnalysis>();
    }

    public void CallSaveXML()
    {
        tradeAnalysis.SaveXML();
    }
}
