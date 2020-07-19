using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Toggle : ScriptableObject
{
    // Ability to toggle things on and off
    // You set these in the inspector
    // Persistent warning error when so I have set to false in script (but will be overriden by inspector)
    [SerializeField] private bool reproduction = false;
    //[SerializeField] private bool varyingPrefs; // TBC: if I will include this
    [SerializeField] private bool hierarchy = false;
    [SerializeField] private bool trade = false;
    [SerializeField] private bool restrictNeighbour = false;
    [SerializeField] private bool biasTrade = false;

    /*
     * 
     * Getters
     * 
     */

    public bool GetReproduction()
    {
        return reproduction;
    }

    public bool GetHierarchy()
    {
        return hierarchy;
    }

    public bool GetTrade()
    {
        return trade;
    }

    public bool GetRestrictNeighbour()
    {
        return restrictNeighbour;
    }

    public bool GetBiasTrade()
    {
        return biasTrade;
    }
}
