using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelfGrid
{
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
