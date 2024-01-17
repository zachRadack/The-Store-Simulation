using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
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
    public Vector3Int GetPosition()
    {
        return this.Position;
    }

    public override string ToString()
    {
        return $"ShelfKey({this.Position.x}, {this.Position.y}, {this.Position.z})";
    }

}