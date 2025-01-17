﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO; //file management

//originally scriptable object but they don't have lateupdate
public class TradeAnalysis : MonoBehaviour
{ 
    //Want to display on graphs
    private GameObject graphPrice;
    private GameObject graphQty;
    private TradePriceGraph graphScriptPrice;
    private TradeQtyGraph graphScriptQty;

    // Variables that get updated each time step
    // Total price 
    private double price;
    // Total units traded (sugar)
    private int units;
    // Total units traded (spice);
    private int spiUnits;
    // Total number of trades
    private int quantity;

    //instance of AvPriceList class which holds list of average prices and average units class - needs to be its own class for serialisation
    AvPrice avPriceClass;
    AvUnits avUnitsClass;
    TotalUnits totUnitsClass;
    AvSpiceUnits avSpiceUnitsClass;
    TotalSpiceUnits totSpiceUnitsClass;

    [Serializable]
    public class AvPrice
    {
        //List with average trade price for each time step
        public List<double> avPriceList = new List<double>();
    }

    [Serializable]
    public class AvUnits
    {
        //List with average sugar units traded for each time step
        public List<double> avUnitsList = new List<double>();
    }

    [Serializable]
    public class AvSpiceUnits
    {
        //List with average spice units for each time step
        public List<double> avUnitsList = new List<double>();
    }

    [Serializable]
    public class TotalUnits
    {
        //List with average trade price for each time step
        public List<int> totUnitsList = new List<int>();
    }

    [Serializable]
    public class TotalSpiceUnits
    {
        //List with average trade price for each time step
        public List<int> totUnitsList = new List<int>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //hold reference to graphs
        graphPrice = GameObject.Find("Trade Price Graph");
        graphScriptPrice = graphPrice.GetComponent<TradePriceGraph>();
        graphQty = GameObject.Find("Trade Qty Graph");
        graphScriptQty = graphQty.GetComponent<TradeQtyGraph>();

        //create instances of classes
        avPriceClass = new AvPrice();
        avUnitsClass = new AvUnits();
        totUnitsClass = new TotalUnits();
        avSpiceUnitsClass = new AvSpiceUnits();
        totSpiceUnitsClass = new TotalSpiceUnits();
    }

    // Script execution order - set to run after Manager class
    void FixedUpdate()
    {
        // add new entry in lists with total price and quantity for that update
        if (quantity>0)
        {
            avPriceClass.avPriceList.Add(price / quantity);
            avUnitsClass.avUnitsList.Add((double)units / quantity);
            totUnitsClass.totUnitsList.Add(units);
            avSpiceUnitsClass.avUnitsList.Add((double)spiUnits / quantity);
            totSpiceUnitsClass.totUnitsList.Add(spiUnits);
        }
        else
        {
            avPriceClass.avPriceList.Add(0);
            avUnitsClass.avUnitsList.Add(0);
            avSpiceUnitsClass.avUnitsList.Add(0);
            totUnitsClass.totUnitsList.Add(0);
            totSpiceUnitsClass.totUnitsList.Add(0);
        }

        //for graph display
        //specify start and end points for display (since we can only display 100 points at a time)
        
        int i = avPriceClass.avPriceList.Count - 1;
        graphScriptPrice.CreateGraph(avPriceClass.avPriceList, i);
        graphScriptQty.CreateGraph(avUnitsClass.avUnitsList, i);

        // then reset price , units and qty
        price = 0;
        units = 0;
        spiUnits = 0;
        quantity = 0;

        // Save XML
        SaveXML();
    }

    public void AddToPrice(double p)
    {
        price += p;
    }

    public void AddToUnits(int u)
    {
        units += u;
    }

    public void AddToSpiceUnits(int u)
    {
        spiUnits += u;
    }

    // Need to also keep track of total number of trades to compute averages
    public void IncrementQty()
    {
        quantity += 1;
    }

    public void SaveXML()
    {
        XmlSerializer save = new XmlSerializer(typeof(AvPrice));
        FileStream path = new FileStream(Application.dataPath + "/XMLFiles/TradeAvPrice.xml", FileMode.Create);
        save.Serialize(path, avPriceClass);
        path.Close();

        save = new XmlSerializer(typeof(AvUnits));
        path = new FileStream(Application.dataPath + "/XMLFiles/TradeQuantity.xml", FileMode.Create);
        save.Serialize(path, avUnitsClass);
        path.Close();

        save = new XmlSerializer(typeof(AvSpiceUnits));
        path = new FileStream(Application.dataPath + "/XMLFiles/TradeSpiceQuantity.xml", FileMode.Create);
        save.Serialize(path, avSpiceUnitsClass);
        path.Close();

        save = new XmlSerializer(typeof(TotalUnits));
        path = new FileStream(Application.dataPath + "/XMLFiles/TradeUnitTotal.xml", FileMode.Create);
        save.Serialize(path, totUnitsClass);
        path.Close();

        save = new XmlSerializer(typeof(TotalSpiceUnits));
        path = new FileStream(Application.dataPath + "/XMLFiles/TradeSpiceUnitTotal.xml", FileMode.Create);
        save.Serialize(path, totSpiceUnitsClass);
        path.Close();
    }
}
