using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;

public class TableManager
{
    private DatabaseConnectionManager _dbConnectionManager;

    public TableManager(DatabaseConnectionManager dbConnectionManager)
    {
        _dbConnectionManager = dbConnectionManager;
    }
    public void CreateTable()
    {
        using (IDbConnection dbConnection = _dbConnectionManager.CreateConnection())
        {
            dbConnection.Open();
            Debug.Log("Creating table: " + dbConnection);
            using (IDbCommand dbCmd = dbConnection.CreateCommand())
            {
                string sqlQuery = "CREATE TABLE IF NOT EXISTS ProductCategories (ProductID INTEGER, CategoryID INTEGER);" +
                    "CREATE TABLE IF NOT EXISTS Products (ProductID INTEGER PRIMARY KEY,ProductName TEXT NOT NULL,ProductDescription TEXT);" +
                    "CREATE TABLE IF NOT EXISTS Categories (CategoryID INTEGER PRIMARY KEY AUTOINCREMENT, CategoryName TEXT);" +
                    "CREATE TABLE IF NOT EXISTS Shelves (ShelfID INTEGER PRIMARY KEY,PosX INTEGER NOT NULL,PosY INTEGER NOT NULL,PosZ INTEGER NOT NULL,MaxShelfY INTEGER NOT NULL,InventoryCount INTEGER NOT NULL,IsDirty BOOLEAN NOT NULL,ShelfType TEXT);" +
                    "CREATE TABLE IF NOT EXISTS Inventory (InventoryID INTEGER PRIMARY KEY,ProductID INTEGER NOT NULL,ShelfID INTEGER,PositionX INTEGER,PositionY INTEGER,PositionZ INTEGER,IsGoBack BOOLEAN DEFAULT FALSE,FOREIGN KEY (ProductID) REFERENCES Products(ProductID),FOREIGN KEY (ShelfID) REFERENCES Shelves(ShelfID));" +
                    "CREATE TABLE IF NOT EXISTS ShelvingUi (ShelvingUiId INTEGER NOT NULL UNIQUE,Xanchor REAL NOT NULL,Yanchor REAL NOT NULL,Xsize REAL,Ysize REAL,ShelfID INTEGER NOT NULL,FOREIGN KEY(ShelfID) REFERENCES Shelves(ShelfID),PRIMARY KEY(ShelvingUiId));";
                dbCmd.CommandText = sqlQuery;
                dbCmd.ExecuteScalar();
            }
            dbConnection.Close();
        }
    }

    public void ClearTable(string tableName)
    {
        using (IDbConnection dbConnection = _dbConnectionManager.CreateConnection())
        {
            dbConnection.Open();
            Debug.Log("Clearing table: " + tableName);
            using (IDbCommand dbCmd = dbConnection.CreateCommand())
            {
                string sqlQuery = $"DELETE FROM {tableName}";
                dbCmd.CommandText = sqlQuery;
                dbCmd.ExecuteNonQuery();
            }
            dbConnection.Close();
        }
    }

}
