using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentProfile
{
    public int sugarMetabolism;
    public int spiceMetabolism;
    public int visionHarvest;
    public int visionNeighbour;
    public int lifespan;
    public int dominance;
    public int influence;
    public int age;
    public Vector2Int cellPosition;

    // Constructor
    public AgentProfile(int m1, int m2, int v1, int v2, int life, int dom, int inf, int a, Vector2Int cp)
    {
        sugarMetabolism = m1;
        spiceMetabolism = m2;
        visionHarvest = v1;
        visionNeighbour = v2;
        lifespan = life;
        dominance = dom;
        influence = inf;
        age = a;
        cellPosition = cp;
    }

    public AgentProfile()
    {
    }
}
