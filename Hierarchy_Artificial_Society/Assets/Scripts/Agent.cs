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
    //Need access to scriptable object Toggle to enable/disable certain things
    private Toggle toggle;
    //Access to Agent Factory to be able to call the method which creates an agent GameObject (for reproduction)
    //actually no, because spawns in random position. but now repitition of code?
    //private AgentFactory factory = GameObject.Find("Agent Factory").GetComponent<AgentFactory>();

    //initial sugar and spice endowments. Used for reproduction
    public int sugarInit;
    public int spiceInit;

    // sugar and spice accumulations
    public int sugar;
    public int spice;

    //Metabolisms - how much sugar and spice the agent 'burns off' each time step
    public int sugarMetabolism;
    public int spiceMetabolism;

    //How far they can 'see' to eat sugar/spice (in number of cells)
    public int vision;

    //Agent's lifespan (in time steps), age and flag for whether they are alive
    public int lifespan;
    public int age;
    public bool isAlive;

    //attributes for reproduction
    public int childBearingBegins;
    public int childBearingEnds;
    private string sex;

    //hierarchy attributes
    private int dominance;
    private int influence;
    //others go here - will use wealth, vision, influence

    void Awake()
    {
        KnowWorld();
    }

    // Update is called once per frame
    void Update()
    {
        //update agent's age
        ++age;

        //decrease sugar & spice through metabolism
        sugar -= sugarMetabolism;
        spice -= spiceMetabolism;

        //check for death
        Death();
        
        if (isAlive)
        {
            //Look around and harvest food
            Harvest();
            //reproduce
            ReproductionProcess();
        }
    }

    /*
     * 
     * GETTERS AND SETTERS
     * 
     */
    public void SetSex(string s)
    {
        sex = s;
    }

    public string GetSex()
    {
        return sex;
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

    // Agent will need to find sugar/spice to 'eat' from surroundings.
    public void Harvest()
    {
        // variables to store info on best location - initially set to current cell position values
        // used for LookAround and eat
        Vector2Int pos = cellPosition;
        double maxWelfare = world.worldArray[cellPosition.x, cellPosition.y].Welfare(sugar, spice, sugarMetabolism, spiceMetabolism);

        //variables to keep track of how to iterate loop (to cope with agents situated at edges)
        int temp;
        int leftover;

        // LOOK NORTH
        // i.e. must increment y value of array (up)

        //if vision pushes you over the grid boundary to the north
        if (cellPosition.y + vision > world.GetRows() - 1)
        {
            temp = world.GetRows() - 1;
            leftover = cellPosition.y + vision - world.GetRows();
        }
        else
        {
            temp = cellPosition.y + vision;
            leftover = 0;
        }


        for (int i = cellPosition.y+1; i <= temp; ++i)
        {
            //if location isn't already ane at location for another agent
            if (world.worldArray[cellPosition.x, i].GetOccupied() == false)
            {
                //if current cell will produce highest welfare so far
                double curWelfare = world.worldArray[cellPosition.x, i].Welfare(sugar, spice, sugarMetabolism, spiceMetabolism);
                if (curWelfare > maxWelfare)
                {
                    pos = new Vector2Int(cellPosition.x, i);
                    maxWelfare = curWelfare;
                }
            }
        }
        if (leftover >0)
        {
            //iterate over
            for (int i = 0; i <= leftover; ++i)
            {
                //if location isn't already ane at location for another agent
                if (world.worldArray[cellPosition.x, i].GetOccupied() == false)
                {
                    //if current cell will produce highest welfare so far
                    double curWelfare = world.worldArray[cellPosition.x, i].Welfare(sugar, spice, sugarMetabolism, spiceMetabolism);
                    if (curWelfare > maxWelfare)
                    {
                        pos = new Vector2Int(cellPosition.x, i);
                        maxWelfare = curWelfare;
                    }
                }
            }
        }

        // LOOK SOUTH
        // i.e. must increment y value of array (down)

        // if vision pushes you over the grid boundary to the south
        if (cellPosition.y - vision < 0)
        {
            temp = 0;
            leftover = vision - cellPosition.y;
        }
        else
        {
            temp = cellPosition.y - vision;
            leftover = 0;
        }

        for (int i = cellPosition.y - 1; i >= 0; --i)
        {
            //if location isn't already ane at location for another agent
            if (world.worldArray[cellPosition.x, i].GetOccupied() == false)
            {
                // if current cell will produce highest welfare so far
                double curWelfare = world.worldArray[cellPosition.x, i].Welfare(sugar, spice, sugarMetabolism, spiceMetabolism);
                if (curWelfare > maxWelfare)
                {
                    pos = new Vector2Int(cellPosition.x, i);
                    maxWelfare = curWelfare;
                }
            }
        }

        if (leftover > 0)
        {
            // iterate over
            for (int i = world.GetRows()-1; i >= leftover; --i)
            {
                //if location isn't already ane at location for another agent
                if (world.worldArray[cellPosition.x, i].GetOccupied() == false)
                {
                    // if current cell will produce highest welfare so far
                    double curWelfare = world.worldArray[cellPosition.x, i].Welfare(sugar, spice, sugarMetabolism, spiceMetabolism);
                    if (curWelfare > maxWelfare)
                    {
                        pos = new Vector2Int(cellPosition.x, i);
                        maxWelfare = curWelfare;
                    }
                }
            }
        }

        // LOOK EAST
        // i.e. must increment x value of array (up)

        //if vision pushes you over the grid boundary to the east
        if (cellPosition.x + vision > world.GetCols() - 1)
        {
            temp = world.GetCols() - 1;
            leftover = cellPosition.x + vision - world.GetCols();
        }
        else
        {
            temp = cellPosition.x + vision;
            leftover = 0;
        }


        for (int i = cellPosition.x + 1; i <= temp; ++i)
        {
            //if location isn't already ane at location for another agent
            if (world.worldArray[i, cellPosition.y].GetOccupied() == false)
            {
                //if current cell will produce highest welfare so far
                double curWelfare = world.worldArray[i, cellPosition.y].Welfare(sugar, spice, sugarMetabolism, spiceMetabolism);
                if (curWelfare > maxWelfare)
                {
                    pos = new Vector2Int(i, cellPosition.y);
                    maxWelfare = curWelfare;
                }
            }
        }
        if (leftover > 0)
        {
            //iterate over
            for (int i = 0; i <= leftover; ++i)
            {
                //if location isn't already ane at location for another agent
                if (world.worldArray[i, cellPosition.y].GetOccupied() == false)
                {
                    //if current cell will produce highest welfare so far
                    double curWelfare = world.worldArray[i, cellPosition.y].Welfare(sugar, spice, sugarMetabolism, spiceMetabolism);
                    if (curWelfare > maxWelfare)
                    {
                        pos = new Vector2Int(i, cellPosition.y);
                        maxWelfare = curWelfare;
                    }
                }
            }
        }

        // LOOK WEST i.e. must increment x value of array (down)

        // if vision pushes you over the grid boundary to the west
        if (cellPosition.x - vision < 0)
        {
            temp = 0;
            leftover = vision - cellPosition.x;
        }
        else
        {
            temp = cellPosition.x - vision;
            leftover = 0;
        }

        for (int i = cellPosition.x - 1; i >= 0; --i)
        {
            //if location isn't already ane at location for another agent
            if (world.worldArray[i, cellPosition.y].GetOccupied() == false)
            {
                // if current cell will produce highest welfare so far
                double curWelfare = world.worldArray[i, cellPosition.y].Welfare(sugar, spice, sugarMetabolism, spiceMetabolism);
                if (curWelfare > maxWelfare)
                {
                    pos = new Vector2Int(i, cellPosition.y);
                    maxWelfare = curWelfare;
                }
            }
        }

        if (leftover > 0)
        {
            // iterate over
            for (int i = world.GetCols()-1; i >= leftover; --i)
            {
                //if location isn't already ane at location for another agent
                if (world.worldArray[i, cellPosition.y].GetOccupied() == false)
                {
                    // if current cell will produce highest welfare so far
                    double curWelfare = world.worldArray[i, cellPosition.y].Welfare(sugar, spice, sugarMetabolism, spiceMetabolism);
                    if (curWelfare > maxWelfare)
                    {
                        pos = new Vector2Int(i, cellPosition.y);
                        maxWelfare = curWelfare;
                    }
                }
            }
        }

        //set agent as harvesting that cell
        world.worldArray[pos.x, pos.y].SetOccupied(true);
        //agent harvests as much as possible from cell
        sugar += world.worldArray[pos.x, pos.y].DepleteSugar();
        spice += world.worldArray[pos.x, pos.y].DepleteSpice();
    }

    /* 
     * 
     * METHODS USED FOR AGENT REPRODUCTION 
     *
     */

    //Method which calls all relevant functions for reproduction
    //Only calls them when reproduction from toggle is set to true
    private void ReproductionProcess()
    {
        // if (toggle.reproduction)
        // {
        Vector2Int currentEmpty = world.CheckEmptyCell(cellPosition.x, cellPosition.y);
        List<Agent> potentialPartners = FindFertileNeighbours();
            foreach (Agent partner in potentialPartners)
            {
                Vector2Int partnerEmpty = world.CheckEmptyCell(partner.cellPosition.x, partner.cellPosition.y);
            
                //if either current agent or neighbour has an empty neighbouring cell
                if (currentEmpty.x != -1 || partnerEmpty.x !=-1)
                {
                    //then reproduce
                    //creates gameobject for child agent
                    GameObject agentObj = CreateAgent.CreateAgentObject();
                    //sets position for child on grid
                    CreateAgent.GeneratePosition(agentObj, currentEmpty, partnerEmpty);
                    //sets Agent component values
                    CreateAgent.CreateAgentComponent(agentObj, this, partner);

                //agent reproduction was too much so for now have break in here, so it doesn't go through all
                break;
                }
            }
       // }
    }

    // Returns true if agent is currently fertile
    private bool Fertile()
    {
        return (childBearingBegins <= age && childBearingEnds > age && sugar >= sugarInit && spice >= spiceInit);
    }

    private List<Agent> FindFertileNeighbours()
    {
        //Generate array of colliders within radius (set to vision)
        Collider2D[] colliderList = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), vision);

        //Create empty List into which fertile agents of different sex will go
        List<Agent> fertileAgentList = new List<Agent>();

        //stores sex of current agent (since 'this' will not work in the for each loop)
        string sex = this.GetSex();

        //goes through each collider within radius
        foreach (Collider2D neighbour in colliderList)
        {
            if (neighbour.tag != "Agent")
                continue;
            Agent agent = neighbour.gameObject.GetComponent<Agent>();

            //Agent agent = (Agent)GetComponent(typeof(Agent));
            //print("agent = " + agent.GetSex());
            //print("this = " + sex);
            //print(String.Equals(agent.GetSex(), sex) == false);


            // check for agent tag (as overlapcircleall will also catch collider for tilemap) and makes sure agent is fertile and of different sex
            // the sex check also rules out an agent mating with itself
            /*
            if (
            //neighbour.tag == "Agent" && neighbour.gameObject.GetComponent<Agent>().Fertile() == true 
            && neighbour.gameObject.GetComponent<Agent>().GetSex() != this.GetSex())
            */

            if (agent.Fertile() == true && String.Equals(agent.GetSex(), this.GetSex()) == false)
            {
                //adds to list
                //fertileAgentList.Add(neighbour.gameObject.GetComponent<Agent>());
                fertileAgentList.Add(agent);
            }
        }
        return fertileAgentList;
    }
}
