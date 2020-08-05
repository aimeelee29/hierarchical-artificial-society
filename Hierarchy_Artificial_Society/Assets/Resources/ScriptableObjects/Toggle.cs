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
    [SerializeField] private bool trade = false;
    [SerializeField] private bool hierarchyRestrictNeighbour = false;
    [SerializeField] private bool hierarchyRestrictNeighbourLowerRank = false;
    [SerializeField] private bool hierarchyBiasTrade = false;
    [SerializeField] private bool hierarchyGreaterVisionHigherRank = false;

    /*
     * 
     * Getters
     * 
     */

    public bool GetReproduction()
    {
        return reproduction;
    }

    public bool GetTrade()
    {
        return trade;
    }

    public bool GetRestrictNeighbour()
    {
        return hierarchyRestrictNeighbour;
    }

    public bool GetRestrictNeighbourLowerRank()
    {
        return hierarchyRestrictNeighbourLowerRank;
    }

    public bool GetBiasTrade()
    {
        return hierarchyBiasTrade;
    }

    public bool GetGreaterVisionHigherRank()
    {
        return hierarchyGreaterVisionHigherRank;
    }
}
