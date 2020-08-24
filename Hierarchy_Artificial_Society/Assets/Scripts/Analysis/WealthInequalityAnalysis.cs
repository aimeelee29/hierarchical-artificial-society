using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO; //file management

public class WealthInequalityAnalysis : MonoBehaviour
{
    public void SaveXML(int updateCounter, WealthInequalityList wealthInequalityListClass, WealthInequalityIndividualList wealthInequalityIndListClass)
    {
        XmlSerializer save = new XmlSerializer(typeof(WealthInequalityList));
        FileStream path = new FileStream(Application.dataPath + "/XMLFiles/WealthInequality" + updateCounter + ".xml", FileMode.Append);
        save.Serialize(path, wealthInequalityListClass);
        path.Close();

        XmlSerializer saveInd = new XmlSerializer(typeof(WealthInequalityIndividualList));
        FileStream pathInd = new FileStream(Application.dataPath + "/XMLFiles/WealthInequalityIndividual" + updateCounter + ".xml", FileMode.Create);
        saveInd.Serialize(pathInd, wealthInequalityIndListClass);
        pathInd.Close();
    }
}
