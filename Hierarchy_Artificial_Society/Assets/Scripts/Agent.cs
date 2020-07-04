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
    private Vector2Int cellPosition;
    //Need access to scriptable object Toggle to enable/disable certain things
    [SerializeField] private static Toggle toggle;

    // initial sugar and spice endowments. Used for reproduction
    public int sugarInit;
    public int spiceInit;

    // sugar and spice accumulations
    public int sugar;
    public int spice;

    // Metabolisms - how much sugar and spice the agent 'burns off' each time step
    public int sugarMetabolism;
    public int spiceMetabolism;

    // Time until death by sugar and spice. Needed for trading.
    private double timeUntilSugarDeath;
    private double timeUntilSpiceDeath;
    // marginal rate of substitution(MRS). An agent's MRS of spice for sugar is the amount of spice the agent considers to be as valuable as 
    // one unit of sugar, that is, the value of sugar in units of spice . 
    private double MRS;

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
    //List of agents that current agent has mated with for that time step.
    List<Agent> agentReproductionList;
    //List of children
    List<Agent> agentChildList;

    //hierarchy attributes
    private int dominance;
    private int influence;
    //others go here - will use wealth, vision
    private int hierarchyScore;

    void Awake()
    {
        KnowWorld();
        //Reference to SO Toggle so we can turn various things on and off in the model
        toggle = Resources.Load<Toggle>("ScriptableObjects/Toggle");
        //empty list of agent's children
        agentChildList = new List<Agent>();
    }

    void Start()
    {
        //time until death for each commodity
        timeUntilSugarDeath = sugar / sugarMetabolism;
        timeUntilSpiceDeath = spice / spiceMetabolism;
        if (timeUntilSugarDeath != 0)
        {
            MRS = timeUntilSpiceDeath / timeUntilSugarDeath;
        }
        else
            MRS = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //update agent's age
        ++age;

        //decrease sugar & spice through metabolism
        sugar -= sugarMetabolism;
        spice -= spiceMetabolism;

        //time until death for each commodity
        timeUntilSugarDeath = sugar / sugarMetabolism;
        timeUntilSpiceDeath = spice / spiceMetabolism;
        if (timeUntilSugarDeath != 0)
        {
            MRS = timeUntilSpiceDeath / timeUntilSugarDeath;
        }
        else
            MRS = 0;
        
        //check for death
        Death();
        
        if (isAlive)
        {
            //Look around and harvest food
            Harvest();
            //reproduce - only if selected in toggle SO
            if (toggle.GetReproduction())
            {
                ReproductionProcess();
            }
            //trade - only if selected in toggle SO
            if (toggle.GetTrade())
            {
                Trade();
            }  
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

    public double GetMRS()
    {
        return MRS;
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


    private List<Agent> FindNeighbours()
    {
        //Generate array of colliders within radius (set to vision)
        Collider2D[] colliderList = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), vision);

        //Create empty List into which fertile agents of different sex will go
        List<Agent> neighbourAgentList = new List<Agent>();

        //goes through each collider within radius
        foreach (Collider2D neighbour in colliderList)
        {
            // if collider is not attached to an agent then skip that one (as overlapcircleall will also catch collider for tilemap) 
            if (neighbour.tag != "Agent")
                continue;

            //get agent from object and add to list
            Agent agent = neighbour.gameObject.GetComponent<Agent>();
            neighbourAgentList.Add(agent);
        }
        return neighbourAgentList;
    }

    /*
     * 
     * METHODS USED FOR TRADING
     * 
     */

    private void Trade()
    {
        //first get neighbours
        List<Agent> neighbourAgentList = FindNeighbours();
        
        //for every neighbour
        foreach (Agent neighbour in neighbourAgentList)
        {
            // Get MRS of neighbour
            double neighbourMRS = neighbour.GetMRS();
            // If MRSA = MRSB then no trade. Continue skips the loop
            if (this.GetMRS() == neighbourMRS)
                continue;
            //print(this.GetMRS() + "neighbour = " + neighbourMRS);
            // otherwise 
            // calculate price (geometric mean of the two MRSs)
            double price = Math.Sqrt(this.GetMRS() * neighbourMRS);

            // vars for how many sugar units are traded for spice units (and vice versa)
            int sugarUnits;
            int spiceUnits;

            // If price(p) > 1, p units of spice are exchanged for 1 unit of sugar.
            if (price > 1)
            {
                //print("price>1");
                sugarUnits = 1;
                spiceUnits = (int)price;
            }
            // If p < 1, then 1 unit of spice is exchanged for p units of sugar
            else
            {
                //print("price <1");
                sugarUnits = (int)(1/price);
                spiceUnits = 1;
            }

            double currentWelfareA = this.Welfare(0, 0);
            double currentWelfareB = neighbour.Welfare(0, 0);

            // If MRSA > MRSB then agent A buys sugar, sells spice (A considers sugar to be relatively more valuable than agent B)
            if (this.GetMRS() > neighbourMRS)
            {
                // If this trade will:
                // (a) make both agents better off(increases the welfare of both agents), and
                // (b) not cause the agents' MRSs to cross over one another, then the trade is made and return to start, else end.

                double potentialWelfareA = this.Welfare(sugarUnits, -spiceUnits);
                double potentialWelfareB = neighbour.Welfare(-sugarUnits, spiceUnits);

                if (neighbour.timeUntilSugarDeath - sugarUnits > 0)
                {
                    double potentialMRSA = (timeUntilSpiceDeath - spiceUnits) / (timeUntilSugarDeath + sugarUnits);
                    double potentialMRSB = (neighbour.timeUntilSpiceDeath + spiceUnits) / (neighbour.timeUntilSugarDeath - sugarUnits);

                    if (potentialWelfareA > currentWelfareA && potentialWelfareB > currentWelfareB &&
                        potentialMRSA >= potentialMRSB)
                    {
                        this.sugar += sugarUnits;
                        neighbour.sugar -= sugarUnits;
                        this.spice -= spiceUnits;
                        neighbour.spice += spiceUnits;
                        /*
                        print("traded");
                        print("sugar units = " + sugarUnits);
                        print("spice units = " + spiceUnits);
                        */
                        
                    }
                }
            }
            // Else MRSA < MRSB
            else
            {
                double potentialWelfareA = this.Welfare(-sugarUnits, +spiceUnits);
                double potentialWelfareB = neighbour.Welfare(+sugarUnits, -spiceUnits);

                if (timeUntilSugarDeath - sugarUnits > 0)
                {
                    double potentialMRSA = (timeUntilSpiceDeath + spiceUnits) / (timeUntilSugarDeath - sugarUnits);
                    double potentialMRSB = (neighbour.timeUntilSpiceDeath - spiceUnits) / (neighbour.timeUntilSugarDeath + sugarUnits);

                    if (potentialWelfareA > currentWelfareA && potentialWelfareB > currentWelfareB &&
                        potentialMRSA >= potentialMRSB)
                    {
                        this.sugar -= sugarUnits;
                        neighbour.sugar += sugarUnits;
                        this.spice += spiceUnits;
                        neighbour.spice -= spiceUnits;
                        /*
                        print("traded");
                        print("sugar units = " + sugarUnits);
                        print("spice units = " + spiceUnits);
                        */
                    }
                }
            }
        }
    }

    private double Welfare(int x, int y)
    {
        return Math.Pow(x + sugar, (double)sugarMetabolism / (sugarMetabolism + spiceMetabolism)) * Math.Pow(y + spice, (double)spiceMetabolism / (sugarMetabolism + spiceMetabolism));
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
        // checks if there is an empty cell adjacent to current agent's cell
        Vector2Int currentEmpty = world.CheckEmptyCell(cellPosition.x, cellPosition.y);
       
        // produces list of potential partners (fertile agents of different sex) 
        List<Agent> potentialPartners = FindFertileNeighbours();

        //refreshes list of agents mated with
        agentReproductionList = new List<Agent>();

        //for each potential partner
        foreach (Agent partner in potentialPartners)
        {
            // go through list of agents that current agent has mated with and ensure that they haven't mated before
            // probably don't really need this first if statement
            if (agentReproductionList.Contains(partner))
            {
                print("1 already mated");
                continue; //skips to next iteration of loop
            }

            if (partner.agentReproductionList != null && agentReproductionList.Contains(this))
            {
                print("2 already mated"); //why is it never picking this up?
                continue; //skips to next iteration of loop
            }

            if (agentChildList.Contains(partner))
            {
                //print("child");
                continue; //skips to next iteration of loop
            }
            
            // checks if there is an empty cell adjacent to the potential partner agent's cell
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
                Agent childCom = CreateAgent.CreateAgentComponent(agentObj, this, partner);
                //adds partner to list of agents mated with
                agentReproductionList.Add(partner);
                //adds child to list of children
                agentChildList.Add(childCom);

                //agent reproduction was too much so for now have break in here, so it doesn't go through all
                break;
            }
        }
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
            // if collider is not attached to an agent then skip that one (as overlapcircleall will also catch collider for tilemap) 
            if (neighbour.tag != "Agent")
                continue;

            Agent agent = neighbour.gameObject.GetComponent<Agent>();

            //  makes sure agent is fertile and of different sex
            // the sex check also rules out an agent mating with itself
            if (agent.Fertile() == true && String.Equals(agent.GetSex(), this.GetSex()) == false)
            {
                //adds to list
                fertileAgentList.Add(agent);
            }
        }
        return fertileAgentList;
    }
}


//trading

//The ratio of the spice to sugar quantities exchanged is simply the price. This price must, of necessity , fall in the range [MRSA , MRSB]. 
//(this is what I would need to change if rules were unfair).
//While all prices within the feasible range are " agreeable " to the agents, not all prices appear to be equally "fair." 
//Prices near either end of the range would seem to be a better deal for one of the agents, particularly when the price range is very large. 

//special care must be taken to avoid infinite loops in which a pair of agents alternates between being buyers and sellers of the same 
//resource upon successive application of the trade rule.This is accomplished by forbidding the MRSs to cross over one another.

//When an agent following M moves to a new location it has from 0 to 4 (von Neumann) neighbors. 
//It interacts through T exactly once with each of its neighbors, selected in random order.
