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

    public CategoryReach _categoryReach;

    private ShelvingUiService _shelvingUiService;
    private ShelfService _shelfService;
    void Awake()
    {
        ShelvingScriptsDictionary = new Dictionary<ShelfKey, ShelvingData>();
        DatabaseConnectionManager dbConnectionManager = new DatabaseConnectionManager();

        _shelvingUiService = new ShelvingUiService(dbConnectionManager);
        _shelfService = new ShelfService(dbConnectionManager);
        //PrintShelvingScriptsDictionary();
        dbConnectionManager.InitializeConnection();
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
        _shelfService.AddPlaceholderShelfToAllShelvesDebug();
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

    /**
    * Todo: Figure out why I made this function
    */
    public string GetShelfTile(Vector3Int position){
        // TODO: ALTER THIS TO USE DATABASE

        if (isThereAShelf(position))
        {
            ShelfKey key = new ShelfKey(position);

            int shelfId = 1;
            if (ShelvingScriptsDictionary.TryGetValue(key, out ShelvingData shelvingData))
            {
                // Key exists, and shelvingData is now the ShelvingData object associated with the key
                shelfId = shelvingData.getShelfId();
            }else{
                Debug.LogError("Key doesn't exist.");
            }
            uiTrigger.OnPointerClickShelfUI(shelfId);
            //Debug.Log("key " + shelfId);
            return _shelfService.printNamesOfInventoryOnShelf(shelfId);
        }
        return "no shelf";
    }

    /**
    * This function saves all the shelves in the database
    */
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


    /**
    * This function loads all the shelves from the database
    */
    void loadAllShelvesFromDataBase(){
        ShelvingScriptsDictionary = _shelfService.LoadAllShelvesData();
    }

    /**
    * This function prints all the shelves in the dictionary
    */
    private void printShelves(){
        foreach (KeyValuePair<ShelfKey, ShelvingData> kvp in ShelvingScriptsDictionary)
        {
            //Debug.Log("Key: " + kvp.Key + " Value: " + kvp.Value);
            PrintShelf(kvp.Value, kvp.Key.GetPosition());
        }
    }

    /**
    * This function prints all the shelves in the dictionary in a verbose manner
    */
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

    /**
    * This will print to tilemap a given self at the x y position a shelf tile
    */
    public void PrintShelf(ShelvingData shelf, Vector3Int position){
        tilemap.SetTile(position,shelfTile);
        tilemapFloor.SetTile(position,floorTile);
    }

    /**
    * Gets shelfId from a Vector3Int position
    */
    public int getShelfId(Vector3Int position){
        ShelfKey key = new ShelfKey(position);
        int shelfId = 1;
        if (ShelvingScriptsDictionary.TryGetValue(key, out ShelvingData shelvingData))
        {
            // Key exists, and shelvingData is now the ShelvingData object associated with the key
            shelfId = shelvingData.getShelfId();
        }else{
            Debug.LogError("Key doesn't exist.");
            return -1;
        }
        return shelfId;
    }

    public void categoryReachPrintCategories(int categoryID){
        // find all shelves that have categoryID in them and then visualize them using _categoryReach
        List<int> shelves = _shelfService.findShelvesWithCategory(categoryID);
        Debug.Log("PAAAAAAAAAAAINNNNN" + shelves.Count);
        foreach (int shelf in shelves){
            Vector3Int shelfPosition = _shelfService.getShelfPosition(shelf);
            Debug.Log("shelfPosition: " + shelfPosition);
            
            _categoryReach.AddCategoryReach(shelfPosition, 1f);
        }

        _categoryReach.visualizeAllReach();
    }
    

}
