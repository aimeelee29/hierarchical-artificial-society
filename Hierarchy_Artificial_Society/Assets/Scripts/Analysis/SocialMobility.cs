using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Class used as the data type for analysis on social mobility - how do agents move across the social ranks
 */

public class SocialMobility
{
    // public Agent agent;
    public bool isChild;
    public int beginningRank;
    public int endingRank;
    public int numberChanges;
    public int age;

    // Constructor
    public SocialMobility(int ranka, int rankb, int change, int a)
    {
        beginningRank = ranka;
        endingRank = rankb;
        numberChanges = change;
        age = a;
    }

    public SocialMobility()
    {
    }
}
