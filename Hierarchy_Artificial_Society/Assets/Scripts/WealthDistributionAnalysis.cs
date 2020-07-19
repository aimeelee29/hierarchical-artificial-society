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
        WealthDistribution wealthDistClass = new WealthDistribution();

        // Variable to hold agent's wealth
        int wealth;
        // Variable to hold max wealth - feeds into social rank
        int maxWealth = 0;

        // Add wealth to wealth list
        foreach (Agent agent in Agent.LiveAgents)
        {
            //print("sug " + agent.Sugar);
            //print("spi " + agent.Spice);
            wealth = agent.Sugar + agent.Spice;
            wealthDistClass.AddtoWealth(wealth);
            if (wealth > maxWealth)
                maxWealth = wealth;
        }

        SaveXML(wealthDistClass, updateCounter);

        // Update the agent static variable for max wealth
        Agent.MaxWealth = maxWealth;
    }
    
    [Serializable]
    public class WealthDistribution
    {
        // List which keeps track of number of agents with that wealth
        // index of list will be wealth, and value will be count
        //List<int> wealthDist = new List<int>(); // change to array
        private int[] wealthDist = new int[400];

        public int[] WealthDist { get => wealthDist; set => wealthDist = value; }

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

    public void SaveXML(WealthDistribution wealthDistClass, int updatecounter)
    {
        XmlSerializer save = new XmlSerializer(typeof(WealthDistribution));
        FileStream path = new FileStream(Application.dataPath + "/XMLFiles/WealthDistribution" + updatecounter + ".xml", FileMode.Create);
        save.Serialize(path, wealthDistClass);
        path.Close();
    }
}
