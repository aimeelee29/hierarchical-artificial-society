using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO; //file management

public class WealthDistributionAnalysis : MonoBehaviour
{
    public void CreateWealthFile(int updateCounter)
    {
        // Create new instance of the wealth class
        WealthDistribution wealthDist = new WealthDistribution();
        // Assign the updatecount
        wealthDist.fixedUpdateCount = updateCounter;

        // Add wealth to wealth list
        foreach (Agent agent in Agent.LiveAgents)
        {
            //print("sug " + agent.Sugar);
            //print("spi " + agent.Spice);
            wealthDist.AddtoWealth(agent.Sugar + agent.Spice);
        }

        SaveXML(wealthDist, updateCounter);
    }
    
    [Serializable]
    public class WealthDistribution
    {
        // Keeps track of what update number this is for.
        public int fixedUpdateCount;
        // List which keeps track of number of agents with that wealth
        // index of list will be wealth, and value will be count
        //List<int> wealthDist = new List<int>(); // change to array
        int[] wealthDist = new int[400];

        public void AddtoWealth(int w)
        {
            //print(w);
            //print(wealthDist[w]);
            if (w > 399)
                ++wealthDist[399];
            else
                ++wealthDist[w];
        }


    }

    public void SaveXML(WealthDistribution wealthDist, int updatecounter)
    {
        XmlSerializer save = new XmlSerializer(typeof(WealthDistribution));
        FileStream path = new FileStream(Application.dataPath + "/XMLFiles/WealthDistribution" + updatecounter + ".xml", FileMode.Create);
        save.Serialize(path, wealthDist);
        path.Close();
    }
}
