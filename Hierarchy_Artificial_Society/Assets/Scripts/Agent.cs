using System.Collections;
using System.Collections.Generic;
//using System.Security.Cryptography;
//using System.Security.Policy;
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
    private int sugar;
    private int spice;

    //Position - not sure I need this
    private int x;
    private int y;

    // Start is called before the first frame update
    void Start()
    {
        KnowWorld();
        //check its working ok
        Vector3Int cellPosition = gridLayout.WorldToCell(transform.position);
        print(cellPosition);
        print(world.worldArray[0, 0]);
        print(world.worldArray[cellPosition.x, cellPosition.y].curSugar);
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
