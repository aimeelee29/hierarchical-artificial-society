using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WealthDistributionList
{
    // List which keeps track of number of agents with that wealth
    // index of list will be wealth, and value will be count
    //List<int> wealthDist = new List<int>(); // change to array
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
