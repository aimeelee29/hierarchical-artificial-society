using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Toggle : ScriptableObject
{
    //Ability to toggle things on and off
    //persistent warning error when private and serializable so change these to public
    [SerializeField] private bool reproduction;
    [SerializeField] private bool varyingPrefs; // TBC: if I will include this
    [SerializeField] private bool hierarchy;
    [SerializeField] private bool trade;

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
}
