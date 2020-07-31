using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO; //file management

public class AgentProfileAnalysis : MonoBehaviour
{
    public AgentProfileList agentProfileListClass;

    // Start is called before the first frame update
    void Awake()
    {
        // Create instance of class SocialRankChangeList
        agentProfileListClass = new AgentProfileList();
    }

    [Serializable]
    public class AgentProfileList
    {
        public List<AgentProfile> agentProfileList = new List<AgentProfile>();
    }

    public void CreateAgentProfileFile(int updatecounter)
    {
        XmlSerializer save = new XmlSerializer(typeof(AgentProfileList));
        FileStream path = new FileStream(Application.dataPath + "/XMLFiles/AgentProfile" + updatecounter + ".xml", FileMode.Create);
        save.Serialize(path, agentProfileListClass);
        path.Close();
    }
}
