using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
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
    
    public int getMaxShelfX(){
        return maxShelfX;
    }
    public int getMaxShelfY(){
        return maxShelfY;
    }

    public ShelfGrid getGrid(int x, int y){
        ShelfGridKey key = new ShelfGridKey(new Vector3Int(x,y,0));
        if (ShelfGridDictionary.ContainsKey(key))
        {
            return ShelfGridDictionary[key];
        }
        else
        {
            return null;
        }
    }
}
