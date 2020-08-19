using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WealthDistributionList
{
    private List<int> wealthDist = new List<int>();

    public List<int> WealthDist { get => wealthDist; set => wealthDist = value; }

    public void AddtoWealth(int w)
    {
        //print(w);
        //print(wealthDist[w]);
        if (w < 0)
        {
            wealthDist.Add(0);
        }   
        else
        {
            wealthDist.Add(w);
        }   
    }
}
