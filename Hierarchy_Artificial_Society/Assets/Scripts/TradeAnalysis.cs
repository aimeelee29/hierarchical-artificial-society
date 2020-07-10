using System;
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
    // total price 
    private double price;
    // total number of trades
    private int quantity;

    //instance of AvPriceList class which holds list of average prices- needs to be its own class for serialisation
    AvPrice avPriceClass;
    Quantity quantityClass;

    [Serializable]
    public class AvPrice
    {
        //List with average trade price for each time step
        public List<double> avPriceList = new List<double>();
    }

    [Serializable]
    public class Quantity
    {
        //List with average trade price for each time step
        public List<int> quantityList = new List<int>();
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
        quantityClass = new Quantity();
    }

    // Script execution order - set to run after Manager class
    void FixedUpdate()
    {
        // add new entry in lists with total price and quantity for that update
        avPriceClass.avPriceList.Add(price/ quantity);
        quantityClass.quantityList.Add(quantity);

        //for graph display
        //specify start and end points for display (since we can only display 100 points at a time)
        int start = (avPriceClass.avPriceList.Count / 100) * 100;
        int end = start + 100;
        graphScriptPrice.CreateGraph(avPriceClass.avPriceList, start, end);
        graphScriptQty.CreateGraph(quantityClass.quantityList, start, end);

        // then reset price and qty
        price = 0;
        quantity = 0;
    }

    public void AddToPrice(double p)
    {
        price += p;
    }

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

        save = new XmlSerializer(typeof(Quantity));
        path = new FileStream(Application.dataPath + "/XMLFiles/TradeQuantity.xml", FileMode.Create);
        save.Serialize(path, quantityClass);
        path.Close();
    }
}
