using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomGen : MonoBehaviour
{

    [SerializeField] private int seed = 0;

    void Awake()
    {
        UnityEngine.Random.InitState(seed);
    }

}
