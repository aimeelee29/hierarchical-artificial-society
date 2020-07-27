using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocialMobilityAnalysis : MonoBehaviour
{
    public SocialMobilityList socialMobiltyListClass;

    // Start is called before the first frame update
    void Start()
    {
        // Create instance of class SocialRankChangeList
        socialMobiltyListClass = new SocialMobilityList();
    }

    public class SocialMobilityList
    {
        public List<SocialRankChange> socialMobilityList = new List<SocialRankChange>();
    }

    public void CreateMobilityFile(int counter)
    {

    }
}
