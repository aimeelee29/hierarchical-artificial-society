using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    //The following GameObjects are assigned in the KnowWorld() function. 
    //They represent the world/environment. The agent will need to communicate with these.
    private GameObject objWorld;
    private GameObject objGrid;
    private World world;
    private GridLayout gridLayout;
    public Vector2Int cellPosition;

    //initial sugar and spice endowments. Used for reproduction
    public int sugarInit;
    public int spiceInit;

    // sugar and spice accumulations
    public int sugar;
    public int spice;

    //Metabolisms - how much sugar and spice the agent 'burns off' each time step
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
    public bool isAlive;

    //attributes for reproduction
    public int childBearingBegins;
    public int childBearingEnds;
    public string sex;

    //hierarchy attributes
    public int dominance;
    //others go here

    /*

    // need the following to go into a utility class

    //Ability to toggle things on and off
    public bool replacement;
    public bool reproduction;
    public bool varingPrefs;

    */


    /* the following is for testing
             * 
            //how many time steps until death by lack of sugar
            double t1 = (double)sugar / sugarMetabolism;
            print("steps until death from sug = " + (double)sugar / sugarMetabolism);
            //how many time steps until death by lack of spice
            double t2 = (double)spice / spiceMetabolism;
            print("steps until death from spice = " + (double)spice / spiceMetabolism);

            //if > 1 then spice more important (since steps until death from sugar would be greater). <1 sugar more important.
            print(t1 / t2);

     */

    void Awake()
    {
        KnowWorld();
    }

    // Start is called before the first frame update
    //could this be moved to awake
    void Start()
    {
        

        
        //check its working ok
        //print(cellPosition);
        //print(world.worldArray[cellPosition.x, cellPosition.y].curSugar);
        
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
        cellPosition = new Vector2Int(gridLayout.WorldToCell(transform.position).x, gridLayout.WorldToCell(transform.position).y);

        /*
        print("agent cell position =" + cellPosition);
        print("agent transform position = " + transform.position);
        */
    }

    //Agent will die if it reaches its lifespan or runs out of either sugar or spice
    public void Death()
    {
        if (isAlive && (age == lifespan || sugar <= 0 || spice <= 0))
        {
            isAlive = false;
            Destroy(gameObject);
        }
    }

    //Used to determine which site would produce most benefit to agent
    public double Welfare(int su, int sp)
    {
        return Math.Pow(sugar + su, (double)sugarMetabolism / (sugarMetabolism + spiceMetabolism)) * Math.Pow(spice, (double)spiceMetabolism / (sugarMetabolism + spiceMetabolism));

    }

    //Agent will need to find sugar/spice to 'eat' from surroundings.
    public void Eat()
    {


    }



    private bool Fertile()
    {
        return (childBearingBegins <= age && childBearingEnds > age && sugar >= sugarInit && spice >= spiceInit);
    }

            /*
    private void Reproduce()
    {
        if (reproduction)
        {

        }
    }
    */
}
