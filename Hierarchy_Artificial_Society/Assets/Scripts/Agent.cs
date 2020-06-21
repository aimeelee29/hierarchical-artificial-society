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
    private Vector3Int cellPosition;

    // sugar and spice accumulations
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
    public bool isAlive;

    //hierarchy attributes
    public int dominance;
    //others go here


    //only needs to be defined once since no movement
    //Collider2D[] listColliders = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), vision);
        

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


// Start is called before the first frame update
void Start()
    {
        KnowWorld();

        /*
        //check its working ok
        Vector3Int cellPosition = gridLayout.WorldToCell(transform.position);
        print(cellPosition);
        print(world.worldArray[cellPosition.x, cellPosition.y].curSugar);
        */

        Eat();
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
        cellPosition = gridLayout.WorldToCell(transform.position);

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
            //print("died");
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
        //thought about circle cast but this is more like throwing a frisbee in one direction
        //try overlapcircleall

        //for unoccupied points, consider one producing maximum welfare

        //Below not working. Only ever seems to be picking up one collider (tilemap collider as a whole instead of separate tiles)
        /*
        Collider2D[] listColliders = Physics2D.OverlapBoxAll(new Vector2(transform.position.x, transform.position.y), new Vector2(vision,vision), 0);
        print(listColliders.Length);       
        foreach (var collider in listColliders)
        {
            print(collider.name); 
            print(collider.transform.position);
        }
        */

        //try circle cast. Also doesn't work
        /*
        RaycastHit2D[] hits = Physics2D.CircleCastAll(new Vector2(transform.position.x, transform.position.y), vision, new Vector2(0,1));
        print(hits.Length);
        foreach (var raycastHit2D in hits)
        {
            print(raycastHit2D.transform.position);
        }
        */

    }
}
