using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO; //file management

public class WealthDistributionAnalysis : MonoBehaviour
{

    // Counter to keep track of number of fixedupdates
    private int updateCounter;

    private WealthDistribution wealthDist;

    void FixedUpdate()
    {
        // Incremenent Counter
        ++updateCounter;

        //Create new class every ten updates to report wealth distribution
        if (updateCounter % 10 == 1)
        {
            wealthDist = new WealthDistribution(updateCounter);
            // Add wealth to wealth list
            foreach (Agent agent in Agent.LiveAgents)
            {
                wealthDist.AddtoWealth(agent.Sugar + agent.Spice);
            }

            SaveXML();
        }
    }
    
    [Serializable]
    public class WealthDistribution
    {
        // Keeps track of what update number this is for.
        public int fixedUpdateCount;
        // List which keeps track of number of agents with that wealth
        // index of list will be wealth, and value will be count
        List<int> wealthDist = new List<int>();

        //Constructor
        public WealthDistribution(int i)
        {
            fixedUpdateCount = i;
        }

        public void AddtoWealth(int w)
        {
            ++wealthDist[w];
        }


    }

    public void SaveXML()
    {
        XmlSerializer save = new XmlSerializer(typeof(WealthDistribution));
        FileStream path = new FileStream(Application.dataPath + "/XMLFiles/WealthDistribution" + updateCounter + ".xml", FileMode.Create);
        save.Serialize(path, wealthDist);
        path.Close();
    }
}
