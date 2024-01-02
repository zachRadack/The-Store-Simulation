using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;

public class MainShelvingManager : MonoBehaviour
{


    [SerializeField]
    public struct ShelfKey
    {
        public Vector3Int Position;

        public ShelfKey(Vector3Int position)
        {
            this.Position = position;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ShelfKey)){
                return false;
            }

            ShelfKey other = (ShelfKey)obj;
            return this.Position.Equals(other.Position);
        }

        public override int GetHashCode()
        {
            return this.Position.GetHashCode();
        }
    }


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
    public List<ShelvingData> shelves = new List<ShelvingData>();

    [SerializeField]
    public Dictionary<ShelfKey, ShelvingData> ShelvingScriptsDictionary;

    public Tilemap tilemap;
    
    public string directoryPath;
    public string filePath;

    void Awake()
    {
        
        string directoryPath = Application.persistentDataPath + "/saveData";
        string filePath = directoryPath + "/shelves.json";
        fileChecker(directoryPath, filePath);

        ShelvingScriptsDictionary = new Dictionary<ShelfKey, ShelvingData>();
        InitializeShelves();

        


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

    void InitializeShelves(){
        foreach (var position in tilemap.cellBounds.allPositionsWithin){
            var localPlace = new Vector3Int(position.x, position.y, position.z);
            if (tilemap.HasTile(localPlace)){
                //tile.setName();
                ShelvingScriptsDictionary.Add(new ShelfKey(localPlace),new ShelvingData());
                }
        }
    }
        
    


    public bool isThereAShelf(Vector3Int position){
        var localPlace = new Vector3Int(position.x, position.y, position.z);
        if (ShelvingScriptsDictionary.ContainsKey(new ShelfKey(localPlace)))
        {
            return true;
        }
        return false;
    }

}
