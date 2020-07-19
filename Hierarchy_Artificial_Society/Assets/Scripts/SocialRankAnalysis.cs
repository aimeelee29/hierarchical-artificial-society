using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO; //file management

public class SocialRankAnalysis : MonoBehaviour
{
    public void CreateRankFile(int updateCounter)
    {
        // Create new instance of the social rank class
        SocialRank rankClass = new SocialRank();


        // Add wealth to wealth list
        foreach (Agent agent in Agent.LiveAgents)
        {
            rankClass.AddtoRank(agent.SocialRank);
        }

        SaveXML(rankClass, updateCounter);
    }

    [Serializable]
    public class SocialRank
    {
        // List which keeps track of number of agents with that wealth
        // index of list will be wealth, and value will be count
        //List<int> wealthDist = new List<int>(); // change to array
        private int[] rankDist = new int[7];

        public int[] RankDist { get => rankDist; set => rankDist = value; }

        public void AddtoRank(int w)
        {
           //print(w);
            ++rankDist[w - 1];
        }
    }

    public void SaveXML(SocialRank rankClass, int updatecounter)
    {
        XmlSerializer save = new XmlSerializer(typeof(SocialRank));
        FileStream path = new FileStream(Application.dataPath + "/XMLFiles/WealthDistribution" + updatecounter + ".xml", FileMode.Create);
        save.Serialize(path, rankClass);
        path.Close();
    }
}
