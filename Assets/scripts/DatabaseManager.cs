using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using UnityEngine.Tilemaps;

public class DatabaseManager : MonoBehaviour
{
    private string dbPath;
    private string connectionString;

    void Start()
    {
        dbPath = "URI=file:" + Application.persistentDataPath + "/Database/MyDatabase.db";
        connectionString = dbPath;
        Debug.Log("dbPath: " + dbPath);
        // Create table
        CreateTable();
        
        //AddDataToTable(1, "Hello");
    }
    private void CreateTable()
    {
        using (IDbConnection dbConnection = new SqliteConnection(connectionString))
        {
            dbConnection.Open();
            Debug.Log("Creating table: " + dbConnection);
            using (IDbCommand dbCmd = dbConnection.CreateCommand())
            {
                string sqlQuery = "CREATE TABLE IF NOT EXISTS ProductCategories (ProductID INTEGER, CategoryID INTEGER);"+
                    "CREATE TABLE IF NOT EXISTS Products (ProductID INTEGER PRIMARY KEY,ProductName TEXT NOT NULL,ProductDescription TEXT);" +
                    "CREATE TABLE IF NOT EXISTS Categories (CategoryID INTEGER PRIMARY KEY AUTOINCREMENT, CategoryName TEXT);"+
                    "CREATE TABLE IF NOT EXISTS Shelves (ShelfID INTEGER PRIMARY KEY,PosX INTEGER NOT NULL,PosY INTEGER NOT NULL,PosZ INTEGER NOT NULL,MaxShelfY INTEGER NOT NULL,InventoryCount INTEGER NOT NULL,IsDirty BOOLEAN NOT NULL,ShelfType TEXT);"+
                    "CREATE TABLE IF NOT EXISTS Inventory (InventoryID INTEGER PRIMARY KEY,ProductID INTEGER NOT NULL,ShelfID INTEGER,PositionX INTEGER,PositionY INTEGER,PositionZ INTEGER,IsGoBack BOOLEAN DEFAULT FALSE,FOREIGN KEY (ProductID) REFERENCES Products(ProductID),FOREIGN KEY (ShelfID) REFERENCES Shelves(ShelfID));";
                dbCmd.CommandText = sqlQuery;
                dbCmd.ExecuteScalar();
            }
            dbConnection.Close();
        }
    }

    public void ReadTable()
    {
        using (IDbConnection dbConnection = new SqliteConnection(connectionString))
        {
            dbConnection.Open();
            Debug.Log("Reconnecting: " + dbConnection);
            using (IDbCommand dbCmd = dbConnection.CreateCommand())
            {
                string sqlQuery = "SELECT * FROM ProductCategories"; // Replace with your table name
                dbCmd.CommandText = sqlQuery;

                using (IDataReader reader = dbCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Read the values from the reader
                        int productId = reader.GetInt32(0);
                        int categoryId = reader.GetInt32(1);


                        // Print the value of the received data
                        Debug.Log("ProductID: " + productId + " CategoryID: " + categoryId);
                    }
                    reader.Close();
                }
            }
            dbConnection.Close();
        }
    }

    

    public void AddProduct(string productName)
    {
        using (IDbConnection dbConnection = new SqliteConnection(connectionString))
        {
            dbConnection.Open();

            using (IDbCommand dbCmd = dbConnection.CreateCommand())
            {
                string sqlQuery = "INSERT INTO Products (ProductName) VALUES (@ProductName)";
                dbCmd.CommandText = sqlQuery;

                IDbDataParameter productNameParam = dbCmd.CreateParameter();
                productNameParam.ParameterName = "@ProductName";
                productNameParam.Value = productName;
                dbCmd.Parameters.Add(productNameParam);

                dbCmd.ExecuteNonQuery();
            }

            dbConnection.Close();
        }
    }

    public void AddCategory(string categoryName)
    {
        using (IDbConnection dbConnection = new SqliteConnection(connectionString))
        {
            dbConnection.Open();

            using (IDbCommand dbCmd = dbConnection.CreateCommand())
            {
                string sqlQuery = "INSERT INTO Categories (CategoryName) VALUES (@CategoryName)";
                dbCmd.CommandText = sqlQuery;

                IDbDataParameter categoryNameParam = dbCmd.CreateParameter();
                categoryNameParam.ParameterName = "@CategoryName";
                categoryNameParam.Value = categoryName;
                dbCmd.Parameters.Add(categoryNameParam);

                dbCmd.ExecuteNonQuery();
            }

            dbConnection.Close();
        }
    }

    public void AddProductToCategory(int productId, int categoryId)
    {
        using (IDbConnection dbConnection = new SqliteConnection(connectionString))
        {
            dbConnection.Open();

            using (IDbCommand dbCmd = dbConnection.CreateCommand())
            {
                string sqlQuery = "INSERT INTO ProductCategories (ProductID, CategoryID) VALUES (@ProductID, @CategoryID)";
                dbCmd.CommandText = sqlQuery;

                // Parameters for ProductID and CategoryID
                IDbDataParameter productParam = dbCmd.CreateParameter();
                productParam.ParameterName = "@ProductID";
                productParam.Value = productId;
                dbCmd.Parameters.Add(productParam);

                IDbDataParameter categoryParam = dbCmd.CreateParameter();
                categoryParam.ParameterName = "@CategoryID";
                categoryParam.Value = categoryId;
                dbCmd.Parameters.Add(categoryParam);

                // Execute the command
                dbCmd.ExecuteNonQuery();
            }

            dbConnection.Close();
        }
    }

    public void DeleteCategory(int categoryId)
    {
        using (IDbConnection dbConnection = new SqliteConnection(connectionString))
        {
            dbConnection.Open();

            using (IDbCommand dbCmd = dbConnection.CreateCommand())
            {
                string sqlQuery = "DELETE FROM Categories WHERE CategoryID = @CategoryID";
                dbCmd.CommandText = sqlQuery;

                IDbDataParameter param = dbCmd.CreateParameter();
                param.ParameterName = "@CategoryID";
                param.Value = categoryId;
                dbCmd.Parameters.Add(param);

                dbCmd.ExecuteNonQuery();
            }

            dbConnection.Close();
        }
    }

    public void DeleteProduct(int productId)
    {
        using (IDbConnection dbConnection = new SqliteConnection(connectionString))
        {
            dbConnection.Open();

            using (IDbCommand dbCmd = dbConnection.CreateCommand())
            {
                string sqlQuery = "DELETE FROM Products WHERE ProductID = @ProductID";
                dbCmd.CommandText = sqlQuery;

                IDbDataParameter param = dbCmd.CreateParameter();
                param.ParameterName = "@ProductID";
                param.Value = productId;
                dbCmd.Parameters.Add(param);

                dbCmd.ExecuteNonQuery();
            }

            dbConnection.Close();
        }
    }

    public void ClearTable(string tableName)
    {
        using (IDbConnection dbConnection = new SqliteConnection(connectionString))
        {
            dbConnection.Open();

            using (IDbCommand dbCmd = dbConnection.CreateCommand())
            {
                string sqlQuery = $"DELETE FROM {tableName}";
                dbCmd.CommandText = sqlQuery;
                dbCmd.ExecuteNonQuery();
            }

            dbConnection.Close();
        }
    }

    public List<string> GetCategoriesForProduct(int productId)
    {
        List<string> categories = new List<string>();

        using (IDbConnection dbConnection = new SqliteConnection(connectionString))
        {
            dbConnection.Open();

            using (IDbCommand dbCmd = dbConnection.CreateCommand())
            {
                string sqlQuery = @"
                    SELECT Categories.CategoryName 
                    FROM Categories
                    JOIN ProductCategories ON Categories.CategoryID = ProductCategories.CategoryID
                    WHERE ProductCategories.ProductID = @ProductID";

                dbCmd.CommandText = sqlQuery;
                IDbDataParameter param = dbCmd.CreateParameter();
                param.ParameterName = "@ProductID";
                param.Value = productId;
                dbCmd.Parameters.Add(param);

                using (IDataReader reader = dbCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        categories.Add(reader.GetString(0));
                    }
                }
            }

            dbConnection.Close();
        }

        return categories;
    }

    public List<string> GetProductsForCategory(int categoryId)
    {
        List<string> products = new List<string>();

        using (IDbConnection dbConnection = new SqliteConnection(connectionString))
        {
            dbConnection.Open();

            using (IDbCommand dbCmd = dbConnection.CreateCommand())
            {
                string sqlQuery = @"
                    SELECT Products.ProductName 
                    FROM Products
                    JOIN ProductCategories ON Products.ProductID = ProductCategories.ProductID
                    WHERE ProductCategories.CategoryID = @CategoryID";

                dbCmd.CommandText = sqlQuery;
                IDbDataParameter param = dbCmd.CreateParameter();
                param.ParameterName = "@CategoryID";
                param.Value = categoryId;
                dbCmd.Parameters.Add(param);

                using (IDataReader reader = dbCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        products.Add(reader.GetString(0));
                    }
                }
            }

            dbConnection.Close();
        }

        return products;
    }

    


    public void TestDatabaseFunctions() {
        ClearTable("ProductCategories");
        ClearTable("Products");
        ClearTable("Categories");
        // Add categories
        Debug.Log("Adding categories");
        AddCategory("Snacks");
        AddCategory("Beverages");
        AddCategory("Desserts");
        AddCategory("JunkFood");

        // Add products
        Debug.Log("Adding products");
        AddProduct("Dorito Chips");
        AddProduct("Coca Cola");

        // Assuming the IDs for the categories and products are known or retrieved
        int doritosId = 1; // Replace with actual ID
        int colaId = 2;    // Replace with actual ID
        int snacksCategoryId = 1; // Replace with actual ID
        int beveragesCategoryId = 2; // Replace with actual ID
        int JunkFood = 4;

        // Associate products with categories
        AddProductToCategory(doritosId, snacksCategoryId);
        AddProductToCategory(doritosId, JunkFood);
        AddProductToCategory(colaId, beveragesCategoryId);
        AddProductToCategory(colaId, JunkFood);
    }


    public List<int> GetFrontlineItems(int shelfId)
    {
        List<int> frontlineItems = new List<int>();

        using (IDbConnection dbConnection = new SqliteConnection(connectionString))
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
        using (IDbConnection dbConnection = new SqliteConnection(connectionString))
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

    public void SaveAllShelvesData(Dictionary<ShelfKey, ShelvingData> ShelvingScriptsDictionary)
    {
        foreach (KeyValuePair<ShelfKey, ShelvingData> pair in ShelvingScriptsDictionary)
        {
            SaveShelfData(pair.Key, pair.Value);
        }
    }

    private void SaveShelfData(ShelfKey key, ShelvingData data)
    {
        // SQL command to insert or update shelf data
        // Replace 'dbConnection' with your actual database connection
        using (IDbConnection dbConnection = new SqliteConnection(connectionString))
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


    public Dictionary<ShelfKey, ShelvingData> LoadAllShelvesData(Dictionary<ShelfKey, ShelvingData> ShelvingScriptsDictionary)
    {
        ShelvingScriptsDictionary.Clear();

        using (IDbConnection dbConnection = new SqliteConnection(connectionString))
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
                            shelfType = reader.GetString(reader.GetOrdinal("ShelfType"))
                        };

                        ShelvingScriptsDictionary.Add(key, data);
                    }
                }
            }

            dbConnection.Close();
        }
        return ShelvingScriptsDictionary;
    }



}