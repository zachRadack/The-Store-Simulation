using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;

public class MainShelvingManager : MonoBehaviour
{

    public bool verbose = true;

    
    public List<ShelvingData> shelves = new List<ShelvingData>();
    public Dictionary<ShelfKey, ShelvingData> ShelvingScriptsDictionary;

    public Tilemap tilemap;
    public Tilemap tilemapFloor;
    public TileBase shelfTile;
    public TileBase floorTile;
    
    public string directoryPath;
    public string filePath;
    public DatabaseManager databaseManager;
    public TriggerUI uiTrigger;

    void Awake()
    {
        ShelvingScriptsDictionary = new Dictionary<ShelfKey, ShelvingData>();
        //PrintShelvingScriptsDictionary();
        directoryPath = Application.persistentDataPath + "/saveData";
        filePath = directoryPath + "/shelves.json";
        fileChecker(directoryPath, filePath);

        //InitializeShelves();
        
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

            Debug.Log("Created a new file at: " + filePath);
        }
        else
        {
            // The file exists
            Debug.Log("File already exists at: " + filePath);
            loadAllShelvesFromDataBase();
            
            //databaseManager.LoadAllShelvesData(ShelvingScriptsDictionary);

        }
        printShelves();

    }

    void InitializeShelves(){
        foreach (var position in tilemap.cellBounds.allPositionsWithin){
            var localPlace = new Vector3Int(position.x, position.y, position.z);
            if (tilemap.HasTile(localPlace)){
                //tile.setName();
                ShelvingScriptsDictionary.Add(new ShelfKey(localPlace),new ShelvingData());
                ShelvingScriptsDictionary[new ShelfKey(localPlace)].fillWithAir();
                }
        }
    }
        
    
    public void testAddItemToShelf(Vector3Int position){
        if (isThereAShelf(position))
        {
            ShelvingData shelf = ShelvingScriptsDictionary[new ShelfKey(position)];
            shelf.setItem(1,1,3,2,3);
            if(verbose){
            
                Debug.Log(shelf.getAllGridItems());
            }
        }
    }

    public bool isThereAShelf(Vector3Int position){
        var localPlace = new Vector3Int(position.x, position.y, position.z);
        //Debug.Log("localPlace: " + localPlace);
        if (ShelvingScriptsDictionary.ContainsKey(new ShelfKey(localPlace)))
        {
            //Debug.Log("There is a shelf at " + position);
            return true;
        }
        return false;
    }

    public string GetShelfTile(Vector3Int position){
        
        if (isThereAShelf(position))
        {
            ShelfKey key = new ShelfKey(position);

            int shelfId = 1;
            if (ShelvingScriptsDictionary.TryGetValue(key, out ShelvingData shelvingData))
            {
                // Key exists, and shelvingData is now the ShelvingData object associated with the key
                shelfId = shelvingData.getShelfId();
            }
            List<List<float>> shelfDimensionsList = databaseManager.getShelvesBackgroundData(shelfId);
            Debug.Log("shelfDimensionsList: " + shelfDimensionsList);
            for(int i = 0; i < shelfDimensionsList.Count; i++){
                for(int j = 0; j < shelfDimensionsList[i].Count; j++){
                    Debug.Log("shelfDimensionsList[i][j]: " + shelfDimensionsList[i][j]);
                }
            }
            uiTrigger.OnPointerClickShelfUI(shelfDimensionsList);
            Debug.Log("key " + shelfId);
            return ShelvingScriptsDictionary[key].gridToString();
        }
        return "no shelf";
    }

    public void SaveAllShelvesData()
    {
        databaseManager.SaveAllShelvesData(ShelvingScriptsDictionary);
    }
    
    [System.Serializable]
    private class Serialization<T>
    {
        public List<T> items;
        public Serialization(List<T> items)
        {
            this.items = items;
        }
    }

    public void LoadAllShelvesDataJson(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError("File not found: " + filePath);
            return;
        }

        string json = File.ReadAllText(filePath);
        Serialization<ShelvingDataEntry> deserializedData = JsonUtility.FromJson<Serialization<ShelvingDataEntry>>(json);

        ShelvingScriptsDictionary.Clear();
        foreach (ShelvingDataEntry entry in deserializedData.items)
        {
            ShelvingScriptsDictionary.Add(entry.key, entry.value);
            ShelvingScriptsDictionary[new ShelfKey(entry.key.GetPosition())].fillWithAir();
            //Debug.Log("entry.key.GetPosition(): " + entry.key.GetPosition());
        
        }
       
        //TODO: Add in scene editing to add in the shelves
    }

    void loadAllShelvesFromDataBase(){
        ShelvingScriptsDictionary = databaseManager.LoadAllShelvesData(ShelvingScriptsDictionary);
    }


    private void printShelves(){
        foreach (KeyValuePair<ShelfKey, ShelvingData> kvp in ShelvingScriptsDictionary)
        {
            Debug.Log("Key: " + kvp.Key + " Value: " + kvp.Value);
            PrintShelf(kvp.Value, kvp.Key.GetPosition());
        }
    }

    public void PrintShelvingScriptsDictionary() {
        if (ShelvingScriptsDictionary == null) {
            Debug.LogError("ShelvingScriptsDictionary is null.");
            return;
        }

        Debug.Log("Printing ShelvingScriptsDictionary contents:");
        foreach (KeyValuePair<ShelfKey, ShelvingData> kvp in ShelvingScriptsDictionary) {
            Debug.Log($"Key: {kvp.Key.ToString()} - Value: {kvp.Value.ToString()}");
        }
    }

    //This will print to tilemap a given self at the x y position a shelf tile
    public void PrintShelf(ShelvingData shelf, Vector3Int position){
        tilemap.SetTile(position,shelfTile);
        tilemapFloor.SetTile(position,floorTile);
    }

    public void addShelfToDatabaseDebug(){
        databaseManager.DebugAddPlaceholderShelfToAllShelves();
    }

}
