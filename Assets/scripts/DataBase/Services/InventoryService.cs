using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;

public class InventoryService : MonoBehaviour
{
    private DatabaseConnectionManager _dbConnectionManager;

    public InventoryService(DatabaseConnectionManager dbConnectionManager)
    {
        _dbConnectionManager = dbConnectionManager;
    }

    public List<int> GetFrontlineItems(int shelfId)
    {
        List<int> frontlineItems = new List<int>();
        using (IDbConnection dbConnection = _dbConnectionManager.CreateConnection())
        {
            dbConnection.Open();

            using (IDbCommand dbCmd = dbConnection.CreateCommand())
            {
                string sqlQuery = @"
                    SELECT InventoryID
                    FROM Inventory
                    WHERE ShelfID = @ShelfID AND PositionX = (
                        SELECT MIN(PositionX)
                        FROM Inventory
                        WHERE ShelfID = @ShelfID AND PositionY = Inventory.PositionY AND PositionZ = Inventory.PositionZ
                    )";
                dbCmd.CommandText = sqlQuery;

                IDbDataParameter param = dbCmd.CreateParameter();
                param.ParameterName = "@ShelfID";
                param.Value = shelfId;
                dbCmd.Parameters.Add(param);

                using (IDataReader reader = dbCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        frontlineItems.Add(reader.GetInt32(0));
                    }
                }
            }

            dbConnection.Close();
        }

        return frontlineItems;
    }

    public List<List<List<int>>> GetShelfMatrix(int shelfId)
    {
        // Assuming you know the dimensions of the shelf matrix
        int depth = 10; // Replace with actual depth
        int height = 5; // Replace with actual height
        int width = 10; // Replace with actual width

        List<List<List<int>>> shelfMatrix = new List<List<List<int>>>();

        for (int x = 0; x < width; x++)
        {
            List<List<int>> depthLayer = new List<List<int>>();
            for (int y = 0; y < height; y++)
            {
                List<int> row = new List<int>();
                for (int z = 0; z < depth; z++)
                {
                    row.Add(GetInventoryIdAtPosition(shelfId, x, y, z));
                }
                depthLayer.Add(row);
            }
            shelfMatrix.Add(depthLayer);
        }

        return shelfMatrix;
    }

    private int GetInventoryIdAtPosition(int shelfId, int x, int y, int z)
    {
        using (IDbConnection dbConnection = _dbConnectionManager.CreateConnection())
        {
            dbConnection.Open();

            using (IDbCommand dbCmd = dbConnection.CreateCommand())
            {
                string sqlQuery = @"
                    SELECT InventoryID
                    FROM Inventory
                    WHERE ShelfID = @ShelfID AND PositionX = @PositionX AND PositionY = @PositionY AND PositionZ = @PositionZ";
                dbCmd.CommandText = sqlQuery;

                dbCmd.Parameters.Add(new SqliteParameter("@ShelfID", shelfId));
                dbCmd.Parameters.Add(new SqliteParameter("@PositionX", x));
                dbCmd.Parameters.Add(new SqliteParameter("@PositionY", y));
                dbCmd.Parameters.Add(new SqliteParameter("@PositionZ", z));

                object result = dbCmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : -1; // Returns -1 if no item is found
            }
        }
    }
}
