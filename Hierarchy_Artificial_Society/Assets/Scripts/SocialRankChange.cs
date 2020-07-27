using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocialRankChange
{
    // public Agent agent;
    public bool isChild;
    public int beginningRank;
    public int endingRank;
    public int numberChanges;
    public int age;

    // Constructor
    public SocialRankChange(bool b, int ranka, int rankb, int change, int a)
    {
        isChild = b;
        beginningRank = ranka;
        endingRank = rankb;
        numberChanges = change;
        age = a;
    }
}
