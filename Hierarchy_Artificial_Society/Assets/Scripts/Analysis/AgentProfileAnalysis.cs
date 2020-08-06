using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO; //file management

public class AgentProfileAnalysis : MonoBehaviour
{
    public void SaveXML(int updatecounter, AgentProfileList agentProfileListClass)
    {
        XmlSerializer save = new XmlSerializer(typeof(AgentProfileList));
        FileStream path = new FileStream(Application.dataPath + "/XMLFiles/AgentProfile" + updatecounter + ".xml", FileMode.Create);
        save.Serialize(path, agentProfileListClass);
        path.Close();
    }
}
