using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO; //file management

public class HarvestAnalysis : MonoBehaviour
{
    // Variables that get updated each time step
    private int totalSugar;
    private int totalSpice;
    //Classes
    HarvestSugarList harvestSugarListClass;
    HarvestSpiceList harvestSpiceListClass;

    // Start is called before the first frame update
    void Start()
    {
        //create instances of classes
        harvestSugarListClass = new HarvestSugarList();
        harvestSpiceListClass = new HarvestSpiceList();
    }

    void FixedUpdate()
    {
        // Add to lists
        harvestSugarListClass.harvestSugarList.Add(totalSugar);
        harvestSpiceListClass.harvestSpiceList.Add(totalSpice);
        // reset totals
        totalSugar = 0;
        totalSpice = 0;
        // Save XML
        SaveXML();
    }

    public void AddSugar(int sug) 
    {
        totalSugar += sug;
    }
    public void AddSpice(int spi)
    {
        totalSpice += spi;
    }

    public void SaveXML()
    {
        XmlSerializer save = new XmlSerializer(typeof(HarvestSugarList));
        FileStream path = new FileStream(Application.dataPath + "/XMLFiles/HarvestSugarQty.xml", FileMode.Create);
        save.Serialize(path, harvestSugarListClass);
        path.Close();

        save = new XmlSerializer(typeof(HarvestSpiceList));
        path = new FileStream(Application.dataPath + "/XMLFiles/HarvestSpiceQty.xml", FileMode.Create);
        save.Serialize(path, harvestSpiceListClass);
        path.Close();
    }

}
