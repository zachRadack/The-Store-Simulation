using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;

public class MainShelvingManager : MonoBehaviour
{



    [SerializeField]
    public class ShelvingData
    {
        public Vector3Int position;
        public int inventoryCount;
        public bool isDirty;
        public string shelfType;
        public Dictionary<ShelvingScript.ShelfGrid, string> ShelvingGrid;
        public int maxShelfY;
    }

    [SerializeField]
    public List<ShelvingScript> shelves = new List<ShelvingScript>();

    public Tilemap tilemap;
    private Dictionary<TileBase, ShelvingScript> ShelvingScriptsDictionary;
    public string directoryPath;
    public string filePath;

    void Awake()
    {
        
        string directoryPath = Application.persistentDataPath + "/saveData";
        string filePath = directoryPath + "/shelves.json";
        fileChecker(directoryPath, filePath);

        ShelvingScriptsDictionary = new Dictionary<TileBase, ShelvingScript>();


        foreach(var ShelvingScriptData in shelves){
            foreach (var tile in ShelvingScriptData.tiless){
                Debug.Log(ShelvingScriptData);
                //tile.setName();
                ShelvingScriptsDictionary.Add(tile,ShelvingScriptData);
                
            }
        }


        //LoadAllShelvesData(filePath);
        //Debug.Log("ShelvingScriptsDictionary: " + ShelvingScriptsDictionary);
        //SaveAllShelvesData(filePath);
    }

    void fileChecker(string directoryPath, string filePath)
    {
            Debug.Log("directoryPath:" + directoryPath);

        // Check if the directory exists, create it if not
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // Check if the file exists
        if (!File.Exists(filePath))
        {
            // The file does not exist, so create a new file
            File.WriteAllText(filePath, "{}"); // Creates a file with an empty JSON object

            //Debug.Log("Created a new file at: " + filePath);
        }
        else
        {
            // The file exists
            //Debug.Log("File already exists at: " + filePath);
        }

    }




    public bool isThereAShelf(Vector3Int position){
        TileBase clickedTile = tilemap.GetTile(position);
        if (ShelvingScriptsDictionary.ContainsKey(clickedTile))
        {
            return true;
        }
        return false;
    }

}
