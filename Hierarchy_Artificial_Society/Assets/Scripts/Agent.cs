using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    //The following GameObjects are assigned in the Start() function. 
    //They represent the world/environment. The agent will need to communicate with these.
    private GameObject objWorld;
    private GameObject objGrid;
    private World world;
    GridLayout gridLayout;

    // How much sugar and spice the agent is 'carrying'
    public int sugar;
    public int spice;

    //Metbolisms - how much sugar and spice the agent 'burns off' each time step
    public int sugarMetabolism;
    public int spiceMetabolism;

    //Position - not sure I need this now
    private int x;
    private int y;

    //How far they can 'see' to eat sugar/spice (in number of cells)
    public int vision;

    //Agent's lifespan (in time steps), age and flag for whether they are alive
    public int lifespan;
    public int age;
    public Boolean isAlive;

    //hierarchy attributes
    public int dominance;
    //others go here


    // Start is called before the first frame update
    void Start()
    {
        KnowWorld();

        /*
        //check its working ok
        Vector3Int cellPosition = gridLayout.WorldToCell(transform.position);
        print(cellPosition);
        print(world.worldArray[0, 0]);
        print(world.worldArray[cellPosition.x, cellPosition.y].curSugar);
        */

    }

    // Update is called once per frame
    void Update()
    {
        //update agent's age
        ++age;

        //check for death
        Death();

        //decrease sugar & spice through metabolism
        sugar -= sugarMetabolism;
        spice -= spiceMetabolism;
        
    }

    //this method enables the Agent to communicate with its surroundings
    public void KnowWorld()
    {
        //Assign GameObjects through Find. 
        objWorld = GameObject.Find("World");
        objGrid = GameObject.Find("Grid");
        //Need access to the script attached to World GameObject
        world = objWorld.GetComponent<World>();
        //Need access to the GridLayout component to be able to convert World location to cell location
        gridLayout = objGrid.GetComponent<GridLayout>();
    }

    //Agent will die if it reaches its lifespan or runs out of either sugar or spice
    public void Death()
    {
        if (isAlive && (age == lifespan || sugar <= 0 || spice <= 0))
        {
            isAlive = false;
            //Destroy(this.GameObject);
        }
    }

    //Agent will need to find sugar/spice to 'eat' from surroundings.
    public void Eat()
    {
        //vision

    }
}
