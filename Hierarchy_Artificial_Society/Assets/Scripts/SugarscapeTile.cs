using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

//Tile represents a single cell in a grid

public class SugarscapeTile : Tile
{

    // Tile image
    public Sprite TileImage;

    public override void GetTileData(Vector3Int location, ITilemap tilemap, ref TileData tileData)
    {
        //base.GetTileData(location, tileMap, ref tileData);
        {
            tileData.sprite = TileImage;
        }

    }

    /*
    public void SetColour(Vector3Int location, ITilemap tilemap, ref TileData tileData)
    {
        tileData.color = Color.red;
    }
    */

    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //TODO: write growback in here. Could be that it grows 1 every 4 time steps, in which case growback would be 0.25 and you would
        //increment a counter. when reaches four (1/growback), you add sugar/spice. 
    }

#if UNITY_EDITOR
// Adds a menu item
    [MenuItem("Assets/Create/SugarscapeTile")]
    public static void CreateSugarscapeTile()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Sugarscape Tile", "New Sugarscape Tile", "Asset", "Save Sugarscape Tile", "Sugarscape Tile");
        if (path == "")
            return;
    AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<SugarscapeTile>(), path);
    }
#endif

    //TODO: Shading based on sugar/spice amount

}

