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

    public void SaveAllShelvesData(Dictionary<ShelfKey, ShelvingData> shelvingScriptsDictionary)
    {
        foreach (KeyValuePair<ShelfKey, ShelvingData> pair in shelvingScriptsDictionary)
        {
            SaveShelfData(pair.Key, pair.Value);
        }
    }

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
        Debug.Log("ShelvingScriptsDictionary: " + shelvingScriptsDictionary.Count);
        return shelvingScriptsDictionary;
    }

    public void DebugAddPlaceholderShelfToAllShelves()
    {
        List<int> shelfIds = GetAllShelfIds();
        foreach (int shelfId in shelfIds)
        {
            AddPlaceholderShelf(shelfId);
        }
    }

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
}
