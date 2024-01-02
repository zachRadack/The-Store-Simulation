using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
[CreateAssetMenu(fileName = "ShelvingScriptTile", menuName = "2D/Tiles/ShelvingScriptTile")]

public class ShelvingScript : ScriptableObject
    {
        //determines how max length of the shelf
        private int maxShelfX = 32;
        //determines how many shelves are on the actual shelf
        public int maxShelfY =5;
        // Add custom properties for a shelf
        public int inventoryCount;
        public bool isDirty;
        public string shelfType;
        
        public struct ShelfGrid {
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
            public void setItemID(int itemID, int itemIDPiece){
                this.itemID = itemID;
                this.itemIDPiece = itemIDPiece;
            }
            public override string ToString()
            {
                return this.x + "," + this.y;
            }
        }

        public void fillWithAir(){
            for (int i = 0; i < maxShelfY; i++)
            {
                for (int j = 0; j < maxShelfX; j++)
                {
                    ShelfGrid ShelvingGrid = new ShelfGrid(j, i);
                    
                }
            }
        }
        
        public override string ToString()
            {
                return "This shelf has " + inventoryCount + " items in stock.";
            }
}
