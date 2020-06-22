using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderTest : MonoBehaviour
{
    private GameObject objWorld;
    private World world;
    private GameObject objGrid;
    private GridLayout gridLayout;

         
        // Start is called before the first frame update
        void Start()
    {
        objWorld = GameObject.Find("World");
        world = objWorld.GetComponent<World>();
        objGrid = GameObject.Find("Grid");
        gridLayout = objGrid.GetComponent<GridLayout>();
        print(gridLayout.WorldToCell(transform.position));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Vector3 position = this.transform.position;
            position.x--;
            this.transform.position = position;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Vector3 position = this.transform.position;
            position.x++;
            this.transform.position = position;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Vector3 position = this.transform.position;
            position.y++;
            this.transform.position = position;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Vector3 position = this.transform.position;
            position.y--;
            this.transform.position = position;
        }
    }
}
