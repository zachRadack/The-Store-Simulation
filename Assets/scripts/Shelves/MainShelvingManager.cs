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
    
    
    public TriggerUI uiTrigger;

    private ShelvingUiService _shelvingUiService;
    private ShelfService _shelfService;
    void Awake()
    {
        ShelvingScriptsDictionary = new Dictionary<ShelfKey, ShelvingData>();
        DatabaseConnectionManager dbConnectionManager = new DatabaseConnectionManager();

        _shelvingUiService = new ShelvingUiService(dbConnectionManager);
        _shelfService = new ShelfService(dbConnectionManager);
        //PrintShelvingScriptsDictionary();
        loadDB();
        //SaveAllShelvesData(filePath);
    }


    /// <summary>
    /// Class <c>loadDB</c> Loads the database and then prints the shelf tiles
    /// </summary>
    void loadDB()
    {
        // TODO: Add a default db checker so we will load in a default db if there is no db
        loadAllShelvesFromDataBase();
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
        

    // <summary>
    /// Class <c>addShelfToDatabaseDebug</c> adds a generic placeholder shelf to the database for every single shelve
    /// </summary>
    public void addShelfToDatabaseDebug(){
        _shelfService.DebugAddPlaceholderShelfToAllShelves();
    }


    //<summary>
    /// Class <c>isThereAShelf</c> detects if there is a shelve at a given position
    ///</summary>
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
            List<List<float>> shelfDimensionsList = _shelvingUiService.getShelvesBackgroundData(shelfId);
            //Debug.Log("shelfDimensionsList: " + shelfDimensionsList);
            //for(int i = 0; i < shelfDimensionsList.Count; i++){
            //    for(int j = 0; j < shelfDimensionsList[i].Count; j++){
            //        Debug.Log("shelfDimensionsList[i][j]: " + shelfDimensionsList[i][j]);
            //    }
            //}
            uiTrigger.OnPointerClickShelfUI(shelfDimensionsList);
            //Debug.Log("key " + shelfId);
            return ShelvingScriptsDictionary[key].gridToString();
        }
        return "no shelf";
    }

    public void SaveAllShelvesData()
    {
        _shelfService.SaveAllShelvesData(ShelvingScriptsDictionary);
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


    void loadAllShelvesFromDataBase(){
        ShelvingScriptsDictionary = _shelfService.LoadAllShelvesData();
    }


    private void printShelves(){
        foreach (KeyValuePair<ShelfKey, ShelvingData> kvp in ShelvingScriptsDictionary)
        {
            //Debug.Log("Key: " + kvp.Key + " Value: " + kvp.Value);
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


}
