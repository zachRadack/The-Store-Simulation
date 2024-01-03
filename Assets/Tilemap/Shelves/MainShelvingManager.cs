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
        //determines how max length of the shelf
        private int maxShelfX = 32;
        //determines how many shelves are on the actual shelf
        public int maxShelfY =5;
        // Add custom properties for a shelf
        public int inventoryCount;
        public bool isDirty;
        public string shelfType;
        [SerializeField]
        private Dictionary<ShelfGridKey, ShelfGrid> ShelfGridDictionary = new Dictionary<ShelfGridKey, ShelfGrid>();
        
        public struct ShelfGridKey
        {
            public Vector3Int Position;

            public ShelfGridKey(Vector3Int position){
                this.Position = position;
            }

            public override bool Equals(object obj)
            {
                if (!(obj is ShelfGridKey)){
                    //Debug.Log("obj is not a ShelfGridKey");
                    return false;
                }

                ShelfGridKey other = (ShelfGridKey)obj;
                return this.Position.Equals(other.Position);
            }

            public override int GetHashCode(){
                return this.Position.GetHashCode();
            }

            public override string ToString(){
                return this.Position.ToString();
            }
        }


        public class ShelfGrid {
            // TODO: Add a detector so rouge goback items will always be put in a valid spot
            public int x;
            public int y;
            public int itemID;
            // itemidpiece is because items can take up multiple coords of a shelf. left to right is how its read.
            public int itemIDPiece;

            // This shows how many of the item is in stock
            public int numberInStock;

            public ShelfGrid(int x, int y) {
                this.x = x;
                this.y = y;
                this.itemID = 0;
                this.itemIDPiece = 0;
                this.numberInStock=0;
            }
            public ShelfGrid(int x, int y, int itemID, int itemIDPiece,int numberInStock) {
                this.x = x;
                this.y = y;
                this.itemID = itemID;
                this.itemIDPiece = itemIDPiece;
                this.numberInStock=numberInStock;
            }
            public void setItemID(int itemID, int itemIDPiece, int numberInStock){
                this.itemID = itemID;
                this.itemIDPiece = itemIDPiece;
                this.numberInStock=numberInStock;
            }
            public override string ToString(){
                return this.x + "," + this.y;
            }
        }

        public void fillWithAir(){
            for (int y = 0; y < maxShelfY; y++){
                for (int x = 0; x < maxShelfX; x++){
                    ShelfGridDictionary.Add(new ShelfGridKey(new Vector3Int(x,y,0)),new ShelfGrid(x, y));
                    
                }
            }
        }
        
        // this overrides the shelfgrid with a new itemID and itemIDPiece
        // TODO: Add a detector so rouge goback items will always be put in a valid spot
        // TODO: Add a overwrite so if so I set an item that crosess over another item it will overwrite it
        public bool setItem(int x, int y, int itemID, int itemIDPiece, int numberInStock){
            if(x + itemIDPiece > maxShelfX || y > maxShelfY){
                Debug.Log("The item is too big for the shelf");
                return false;
            }

            for (int i = 0; i < itemIDPiece; i++){
                ShelfGridKey key = new ShelfGridKey(new Vector3Int(x + i, y, 0));
                ShelfGridKey theKey;
                foreach (var kvp in ShelfGridDictionary){
                    //Debug.Log("kvp: " + kvp.Key);
                    //Debug.Log("key: " + key);
                    if(kvp.Key.Equals(key)){
                        //Debug.Log("key founds!");
                        theKey= kvp.Key;
                    }else{
                        //Debug.Log("key not founds!");
                    }
                }
                if (ShelfGridDictionary.ContainsKey(key)){
                    // If the key exists, modify the existing value
                    ShelfGridDictionary[key].setItemID(itemID, itemIDPiece, numberInStock);
                }
                else{
                    Debug.Log("The key doesn't exist");
                    // TODO: Handle the situation where the key doesn't exist
                    // ADD a handle for the error
                }
            }

            return true;
        }

        
        //this will pull the coords setItemID for every itemIDPiece in the grid and return a list of itemIDs with their coords
        // it will ignore any itemIDPiece that is 0
        public Dictionary<ShelfGridKey,ShelfGrid> getAllGridItems(){
            Dictionary<ShelfGridKey,ShelfGrid> itemsID = new Dictionary<ShelfGridKey,ShelfGrid>();
            //
            foreach (var kvp in ShelfGridDictionary)
            {
                if(kvp.Value.itemID!=0){
                    if(kvp.Value.itemIDPiece==0){
                        itemsID.Add(kvp.Key,kvp.Value);
                    }
                }
            }
            return itemsID;
        }

        public string gridToString(){
            string grid = "";
            for (int y = 0; y < maxShelfY; y++)
            {
                for (int x = 0; x < maxShelfX; x++)
                {
                    ShelfGridKey key = new ShelfGridKey(new Vector3Int(x,y,0));
                    if (ShelfGridDictionary.ContainsKey(key))
                    {
                        grid += ShelfGridDictionary[key].itemID + " ";
                    }
                    else
                    {
                        grid += "0 ";
                    }
                }
                grid += "\n";
            }
            return grid;
        }
        public override string ToString()
            {
                return "This shelf has " + inventoryCount + " items in stock.";
            }
    }

    
    public List<ShelvingData> shelves = new List<ShelvingData>();

    
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
                ShelvingScriptsDictionary[new ShelfKey(localPlace)].fillWithAir();
                }
        }
    }
        
    
    public void testAddItemToShelf(Vector3Int position){
        if (isThereAShelf(position))
        {
            ShelvingData shelf = ShelvingScriptsDictionary[new ShelfKey(position)];
            shelf.setItem(1,1,3,2,3);
            
            Debug.Log(shelf.getAllGridItems());
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

    public string GetShelfTile(Vector3Int position){
        
        if (isThereAShelf(position))
        {

            return ShelvingScriptsDictionary[new ShelfKey(position)].gridToString();
        }
        return "no shelf";
    }

}
