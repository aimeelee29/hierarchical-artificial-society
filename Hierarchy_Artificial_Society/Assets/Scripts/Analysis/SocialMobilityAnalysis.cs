using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO; //file management

public class SocialMobilityAnalysis : MonoBehaviour
{
    public SocialMobilityList socialMobiltyListClass;

    // Start is called before the first frame update
    void Awake()
    {
        // Create instance of class SocialRankChangeList
        socialMobiltyListClass = new SocialMobilityList();
    }

    [Serializable]
    public class SocialMobilityList
    {
        public List<SocialMobility> socialMobilityList = new List<SocialMobility>();
    }

    public void CreateMobilityFile(int updatecounter)
    {
        XmlSerializer save = new XmlSerializer(typeof(SocialMobilityList));
        FileStream path = new FileStream(Application.dataPath + "/XMLFiles/SocialMobility" + updatecounter + ".xml", FileMode.Create);
        save.Serialize(path, socialMobiltyListClass);
        path.Close();
    }
}
