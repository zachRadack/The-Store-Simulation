using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;




public class ShelfManager : MonoBehaviour
{



    [System.Serializable]
    public class ShelfData
    {
        public Vector3Int position;
        public int inventoryCount;
        public bool isDirty;
        public string shelfType;
        public Dictionary<shelfTile.ShelfGrid, string> ShelvingGrid;
        public int maxShelfY;
    }

    [System.Serializable]
    public class AllShelvesData
    {
        public List<ShelfData> shelves = new List<ShelfData>();
    }

    public Tilemap tilemap;
    private Dictionary<Vector3Int, shelfTile> shelfTiles = new Dictionary<Vector3Int, shelfTile>();
    public string directoryPath;
    public string filePath;

    void Start()
    {
        
        string directoryPath = Application.persistentDataPath + "/saveData";
        string filePath = directoryPath + "/shelves.json";
        fileChecker(directoryPath, filePath);
        InitializeShelves();
        //LoadAllShelvesData(filePath);
        Debug.Log("shelfTiles: " + shelfTiles);

        SaveAllShelvesData();
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

    void InitializeShelves()
    {
        foreach (var position in tilemap.cellBounds.allPositionsWithin)
        {
            
            var localPlace = new Vector3Int(position.x, position.y, position.z);
            
            if (tilemap.HasTile(localPlace)){
                //Debug.Log("position: " + position);
                var tile = tilemap.GetTile<shelfTile>(localPlace);
                Debug.Log("tile: " + tile.isDirty);
                if (tile != null)
                {
                    shelfTiles[localPlace] = tile;
                    //Debug.Log("shelfTiles position: " + localPlace);
                    shelfTiles[localPlace].fillWithAir();
                    shelfTiles[localPlace].setName(localPlace.ToString());
                    //Debug.Log("shelfTilesgrid: " + shelfTiles[localPlace].ShelvingGrid);
                    //Debug.Log("shelfTilesgrid: " + localPlace);
                    //Debug.Log("Key: "+shelfTiles.Keys);
                    //foreach(var key in shelfTiles[localPlace].ShelvingGrid.Keys)
                    //{
                        //Debug.Log("Keys: " + shelfTiles[localPlace] +" inside: " + shelfTiles[localPlace].ShelvingGrid[key]);
                    //}
                } 
            }
        }

        
    }

    // Load data for all shelves
    void LoadAllShelvesData(string filePath)
    {
        if (System.IO.File.Exists(filePath))
        {
            string jsonData = System.IO.File.ReadAllText(filePath);
            AllShelvesData allShelvesData = JsonUtility.FromJson<AllShelvesData>(jsonData);
            foreach (var shelfData in allShelvesData.shelves)
            {
                if (shelfTiles.TryGetValue(shelfData.position, out shelfTile shelf))
                {
                    // Update the shelf tile with data
                    shelf.inventoryCount = shelfData.inventoryCount;
                    shelf.isDirty = shelfData.isDirty;
                    shelf.shelfType = shelfData.shelfType;
                }
            }
        }
    }

    // Save data for all shelves
    public void SaveAllShelvesData()
    {
        AllShelvesData allShelvesDataDic = new AllShelvesData();
        string directoryPath = Application.persistentDataPath + "/saveData";
        string filePath = directoryPath + "/shelves.json";
        Debug.Log("shelfTiles:" + shelfTiles);
        foreach (var kvp in shelfTiles)
        {
            ShelfData shelfData = new ShelfData
            {
                position = kvp.Key,
                inventoryCount = kvp.Value.inventoryCount,
                isDirty = kvp.Value.isDirty,
                shelfType = kvp.Value.shelfType,
                maxShelfY = kvp.Value.maxShelfY,

            };
            allShelvesDataDic.shelves.Add(shelfData);
        }
        //Debug.Log("allShelvesDataDic: " + allShelvesDataDic);

        Debug.Log("filePath: " + filePath);
        string jsonData = JsonUtility.ToJson(allShelvesDataDic, true);
        System.IO.File.WriteAllText(filePath, jsonData);
    }

    public shelfTile GetShelfTile(Vector3Int position)
    {
        if (shelfTiles.TryGetValue(position, out shelfTile shelf))
        {
            return shelf;
        }
        return null;
    }
    
    public bool isThereAShelf(Vector3Int position)
    {
        //Debug.Log("position: " + position);

        foreach(var key in shelfTiles.Keys)
        {
            Debug.Log("Keys: " + key);
        }
        if (shelfTiles.ContainsKey(position))
        {
            return true;
        }
        return false;
    }
}

