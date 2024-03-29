using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
public class ShelfService

{
    private DatabaseConnectionManager _dbConnectionManager;

    public ShelfService(DatabaseConnectionManager dbConnectionManager)
    {
        _dbConnectionManager = dbConnectionManager;
    }

    /**
    * Goes through every shelf, sending them all through SaveShelfData
    */
    public void SaveAllShelvesData(Dictionary<ShelfKey, ShelvingData> shelvingScriptsDictionary)
    {
        foreach (KeyValuePair<ShelfKey, ShelvingData> pair in shelvingScriptsDictionary)
        {
            SaveShelfData(pair.Key, pair.Value);
        }
    }

    /**
    * Saves the shelf data to the database
    */
    private void SaveShelfData(ShelfKey key, ShelvingData data)
    {
        using (IDbConnection dbConnection = _dbConnectionManager.CreateConnection())
        {
            dbConnection.Open();

            using (IDbCommand dbCmd = dbConnection.CreateCommand())
            {
                string sqlQuery = @"
                INSERT INTO Shelves (PosX, PosY, PosZ, MaxShelfY, InventoryCount, IsDirty, ShelfType) 
                VALUES (@PosX, @PosY, @PosZ, @MaxShelfY, @InventoryCount, @IsDirty, @ShelfType)
                ON CONFLICT(ShelfID) DO UPDATE SET
                PosX = @PosX, PosY = @PosY, PosZ = @PosZ, MaxShelfY = @MaxShelfY, 
                InventoryCount = @InventoryCount, IsDirty = @IsDirty, ShelfType = @ShelfType;";

                dbCmd.CommandText = sqlQuery;
                dbCmd.Parameters.Add(new SqliteParameter("@PosX", key.Position.x));
                dbCmd.Parameters.Add(new SqliteParameter("@PosY", key.Position.y));
                dbCmd.Parameters.Add(new SqliteParameter("@PosZ", key.Position.z));
                dbCmd.Parameters.Add(new SqliteParameter("@MaxShelfY", data.maxShelfY));
                dbCmd.Parameters.Add(new SqliteParameter("@InventoryCount", data.inventoryCount));
                dbCmd.Parameters.Add(new SqliteParameter("@IsDirty", data.isDirty));
                dbCmd.Parameters.Add(new SqliteParameter("@ShelfType", data.shelfType));

                dbCmd.ExecuteNonQuery();
            }
            dbConnection.Close();
        }
    }

    /**
    * Loads all shelves from the database
    */
    public Dictionary<ShelfKey, ShelvingData> LoadAllShelvesData()
    {
        Dictionary<ShelfKey, ShelvingData> shelvingScriptsDictionary = new Dictionary<ShelfKey, ShelvingData>();
        using (IDbConnection dbConnection = _dbConnectionManager.CreateConnection())
        {
            dbConnection.Open();

            using (IDbCommand dbCmd = dbConnection.CreateCommand())
            {
                dbCmd.CommandText = "SELECT * FROM Shelves";

                using (IDataReader reader = dbCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Vector3Int position = new Vector3Int(
                            reader.GetInt32(reader.GetOrdinal("PosX")),
                            reader.GetInt32(reader.GetOrdinal("PosY")),
                            reader.GetInt32(reader.GetOrdinal("PosZ")));

                        ShelfKey key = new ShelfKey(position);

                        ShelvingData data = new ShelvingData
                        {
                            maxShelfY = reader.GetInt32(reader.GetOrdinal("MaxShelfY")),
                            inventoryCount = reader.GetInt32(reader.GetOrdinal("InventoryCount")),
                            isDirty = reader.GetBoolean(reader.GetOrdinal("IsDirty")),
                            shelfType = reader.GetString(reader.GetOrdinal("ShelfType")),
                            shelfID = reader.GetInt32(reader.GetOrdinal("ShelfID"))
                        };

                        shelvingScriptsDictionary.Add(key, data);
                    }
                }
            }
            dbConnection.Close();
        }
        //Debug.Log("ShelvingScriptsDictionary: " + shelvingScriptsDictionary.Count);
        return shelvingScriptsDictionary;
    }

    /**
    * Debug tool, this adds a placeholder shelf to every shelf in the database
    */
    public void AddPlaceholderShelfToAllShelvesDebug()
    {
        List<int> shelfIds = GetAllShelfIds();
        foreach (int shelfId in shelfIds)
        {
            AddPlaceholderShelf(shelfId);
        }
    }

    /**
    * Debug tool, Adds a placeholder shelf to a single shelf in the database
    */
    void AddPlaceholderShelf(int shelfId)
    {
        // find all shelves, and their shelfid, and then add one shelf that has the an xanchor of 0, and a yanchor of 0, and a xsize of 165f, and a ysize of null
        using (IDbConnection dbConnection = _dbConnectionManager.CreateConnection())
        {
            dbConnection.Open();
            Debug.Log("Adding shelves");
            // Get all shelves
            using (IDbCommand dbCmd = dbConnection.CreateCommand())
            {
                dbCmd.CommandText = "INSERT INTO ShelvingUi (Xanchor, Yanchor, Xsize, Ysize, ShelfID) VALUES (0, 0, -165, null, @ShelfID)";
                dbCmd.Parameters.Add(new SqliteParameter("@ShelfID", shelfId));
                dbCmd.ExecuteNonQuery();
            }
            dbConnection.Close();
        }
    }

    /**
    * This gets every single shelfid in the database
    */
    private List<int> GetAllShelfIds()
    {
        List<int> shelfIds = new List<int>();
        using (IDbConnection dbConnection = _dbConnectionManager.CreateConnection())
        {
            dbConnection.Open();
            Debug.Log("Getting all shelf ids");
            using (IDbCommand dbCmd = dbConnection.CreateCommand())
            {
                dbCmd.CommandText = "SELECT ShelfID FROM Shelves";
                using (IDataReader reader = dbCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        shelfIds.Add(reader.GetInt32(reader.GetOrdinal("ShelfID")));
                    }
                }
            }
            dbConnection.Close();
        }
        return shelfIds;
    }

    /**
    * Returns a string that shows all the inventory on a shelf
    */
    public string printNamesOfInventoryOnShelf(int shelfId)
    {
        Debug.Log("shelfId: " + shelfId);
        string result = "";
        List<int> ProductIds = new List<int>();
        List<string> inventoryNames = new List<string>();
        List<int> inventoryIds = new List<int>();
        using (IDbConnection dbConnection = _dbConnectionManager.CreateConnection())
        {
            dbConnection.Open();
            //Debug.Log("Getting all shelf ids");
            using (IDbCommand dbCmd = dbConnection.CreateCommand())
            {
                dbCmd.CommandText = "SELECT * FROM Inventory WHERE ShelfID = @ShelfID";
                dbCmd.Parameters.Add(new SqliteParameter("@ShelfID", shelfId));
                using (IDataReader reader = dbCmd.ExecuteReader())
                {
                    //Debug.Log("Getting all inventory ids");
                    //Debug.Log("reader: " + reader);
                    while (reader.Read())
                    {
                        ProductIds.Add(reader.GetInt32(reader.GetOrdinal("ProductID")));
                        inventoryIds.Add(reader.GetInt32(reader.GetOrdinal("InventoryID")));
                    }
                }
            }
            dbConnection.Close();
        }

        using (IDbConnection dbConnection = _dbConnectionManager.CreateConnection())
        {
            dbConnection.Open();
            //Debug.Log("Getting all shelf ids");
            using (IDbCommand dbCmd = dbConnection.CreateCommand())
            {
                dbCmd.CommandText = "SELECT * FROM Products WHERE ProductID = @ProductID";
                foreach (int ProductID in ProductIds)
                {
                    dbCmd.Parameters.Add(new SqliteParameter("@ProductID", ProductID));
                    using (IDataReader reader = dbCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            inventoryNames.Add(reader.GetString(reader.GetOrdinal("ProductName")));
                        }
                    }
                }
            }
            dbConnection.Close();
        }
        //Debug.Log("ProductIds: " + ProductIds.Count);
        //Debug.Log("inventoryNames: " + inventoryNames.Count);
        //Debug.Log("inventoryIds: " + inventoryIds.Count);
        // string should return both the inventory id and name
        for (int i = 0; i < ProductIds.Count; i++)
        {
            Debug.Log("ProductIds[i]: " + ProductIds[i]);
            result += ProductIds[i] + " " + inventoryNames[i] + " Inventory ID: " + inventoryIds[i] + "\n";
        }
        //Debug.Log("ttttttttttttt ");
        //Debug.Log(result);
        return result;
    }

    /**
    * getProductIdsOnShelf gets all productids on a shelf
    */
    public List<int> getProductIdsOnShelf(int shelfId)
    {
        List<int> ProductIds = new List<int>();
        using (IDbConnection dbConnection = _dbConnectionManager.CreateConnection())
        {
            dbConnection.Open();
            using (IDbCommand dbCmd = dbConnection.CreateCommand())
            {
                dbCmd.CommandText = "SELECT * FROM Inventory WHERE ShelfID = @ShelfID";
                dbCmd.Parameters.Add(new SqliteParameter("@ShelfID", shelfId));
                using (IDataReader reader = dbCmd.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        ProductIds.Add(reader.GetInt32(reader.GetOrdinal("ProductID")));
                    }
                }
            }
            dbConnection.Close();
        }
        return ProductIds;
    }

    /**
    * findShelvesWithCategory returns a list of shelfids that have a desired categoryid
    */
    public List<int> findShelvesWithCategory(int categoryID)
    {
        List<int> shelfIds = new List<int>();
        //Debug.Log("categoryID: " + categoryID);
        using (IDbConnection dbConnection = _dbConnectionManager.CreateConnection())
        {
            dbConnection.Open();
            //Debug.Log("Getting all shelf ids");
            using (IDbCommand dbCmd = dbConnection.CreateCommand())
            {
                dbCmd.CommandText = @"
                SELECT DISTINCT Shelves.ShelfID 
                FROM Shelves 
                INNER JOIN Inventory ON Shelves.ShelfID = Inventory.ShelfID 
                INNER JOIN Products ON Inventory.ProductID = Products.ProductID 
                INNER JOIN ProductCategories ON Products.ProductID = ProductCategories.ProductID 
                WHERE ProductCategories.CategoryID = @CategoryID";

                dbCmd.Parameters.Add(new SqliteParameter("@CategoryID", categoryID));
                using (IDataReader reader = dbCmd.ExecuteReader())
                {
                    //Debug.Log("Getting all inventory ids");
                    //Debug.Log("reader: " + reader);
                    while (reader.Read())
                    {


                        int shelfIdIndex = reader.GetOrdinal("ShelfID");
                        if (shelfIdIndex >= 0)
                        {
                            shelfIds.Add(reader.GetInt32(shelfIdIndex));
                        }
                        else
                        {
                            Debug.LogWarning("ShelfID column not found");
                        }
                    }
                }
            }
            dbConnection.Close();
        }
        return shelfIds;
    }

    /**
    * getShelfPosition returns the Vector3Int position of a shelf
    */
    public Vector3Int getShelfPosition(int shelfId)
    {
        Vector3Int shelfPosition = new Vector3Int();
        using (IDbConnection dbConnection = _dbConnectionManager.CreateConnection())
        {
            dbConnection.Open();
            //Debug.Log("Getting all shelf ids");
            using (IDbCommand dbCmd = dbConnection.CreateCommand())
            {
                dbCmd.CommandText = "SELECT * FROM Shelves WHERE ShelfID = @ShelfID";
                dbCmd.Parameters.Add(new SqliteParameter("@ShelfID", shelfId));
                using (IDataReader reader = dbCmd.ExecuteReader())
                {
                    //Debug.Log("Getting all inventory ids");
                    //Debug.Log("reader: " + reader);
                    while (reader.Read())
                    {
                        shelfPosition = new Vector3Int(
                            reader.GetInt32(reader.GetOrdinal("PosX")),
                            reader.GetInt32(reader.GetOrdinal("PosY")),
                            reader.GetInt32(reader.GetOrdinal("PosZ")));
                    }
                }
            }
            dbConnection.Close();
        }
        return shelfPosition;
    }

    /**
    * isShelfOfCategory returns true if the shelf at a given position has a desired categoryid
    */
    public bool isShelfOfCategory(Vector3Int position, int categoryID)
    {
        bool result = false;
        using (IDbConnection dbConnection = _dbConnectionManager.CreateConnection())
        {
            dbConnection.Open();
            //Debug.Log("Getting all shelf ids");
            //find the shelf assosicated with the position, and then compare its categoryid to the categoryid passed in
            using (IDbCommand dbCmd = dbConnection.CreateCommand())
            {
                dbCmd.CommandText = @"
                SELECT DISTINCT Shelves.ShelfID
                FROM Shelves
                INNER JOIN Inventory ON Shelves.ShelfID = Inventory.ShelfID
                INNER JOIN Products ON Inventory.ProductID = Products.ProductID
                INNER JOIN ProductCategories ON Products.ProductID = ProductCategories.ProductID
                WHERE Shelves.PosX = @PosX AND Shelves.PosY = @PosY AND Shelves.PosZ = @PosZ AND ProductCategories.CategoryID = @CategoryID";
                dbCmd.Parameters.Add(new SqliteParameter("@PosX", position.x));
                dbCmd.Parameters.Add(new SqliteParameter("@PosY", position.y));
                dbCmd.Parameters.Add(new SqliteParameter("@PosZ", position.z));
                dbCmd.Parameters.Add(new SqliteParameter("@CategoryID", categoryID));
                using (IDataReader reader = dbCmd.ExecuteReader())
                {
                    //Debug.Log("Getting all inventory ids");
                    //Debug.Log("reader: " + reader);
                    while (reader.Read())
                    {
                        result = true;
                    }
                }
            }
            dbConnection.Close();
        }
        return result;
    }
}
