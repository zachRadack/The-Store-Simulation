using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "ShelfTile", menuName = "2D/Tiles/ShelfTile")]

public class shelfTile : Tile
{

    // ShelfGrid is used to determien where a product on the shelf is
    // x refers to the x on the shelf, while y refers to the shelf its on.
    [System.Serializable]
    public struct ShelfGrid {
        public int x;
        public int y;


        public ShelfGrid(int x, int y) {
            this.x = x;
            this.y = y;
        }
        public override string ToString()
        {
            return this.x + "," + this.y;
        }
    }

    //determines how max length of the shelf
    private int maxShelfX = 32;
    //determines how many shelves are on the actual shelf
    public int maxShelfY =5;
    // Add custom properties for a shelf
    public int inventoryCount;
    public bool isDirty;
    public string shelfType;

    public Dictionary<ShelfGrid, string> ShelvingGrid = new Dictionary<ShelfGrid, string>();

    private string name;
    

    // You can also add methods specific to the shelf
    public void Restock(int amount)
    {
        inventoryCount += amount;
    }

    public void Clean()
    {
        isDirty = false;
    }

    //main objective of this is to make sure when booting up that ShelfGrid is populated with non null values.

    public void fillWithAir(){
        for (int i = 0; i < maxShelfY; i++)
        {
            for (int j = 0; j < maxShelfX; j++)
            {
                ShelfGrid shelfGrid = new ShelfGrid(j, i);
                ShelvingGrid.Add(shelfGrid, "air");
            }
        }
    }
    

    public void setName(string name){
        this.name = name;
    }
    public override string ToString()
    {
        return this.name;
    }
}

