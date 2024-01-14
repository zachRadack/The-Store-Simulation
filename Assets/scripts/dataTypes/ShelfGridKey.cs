using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelfGridKey
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