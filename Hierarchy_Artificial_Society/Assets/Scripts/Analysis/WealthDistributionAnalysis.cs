using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO; //file management

public class WealthDistributionAnalysis : MonoBehaviour
{   
    public void SaveXML(int updatecounter, WealthDistributionList wealthDistListClass)
    {
        XmlSerializer save = new XmlSerializer(typeof(WealthDistributionList));
        FileStream path = new FileStream(Application.dataPath + "/XMLFiles/WealthDistribution" + updatecounter + ".xml", FileMode.Create);
        save.Serialize(path, wealthDistListClass);
        path.Close();
    }
}
