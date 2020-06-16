using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

//Tile represents a single cell in a grid
//Possibly don't need scripted tile anymore

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

    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
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
}

