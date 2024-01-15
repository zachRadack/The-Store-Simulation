using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;

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
        //CreateTable();
        //AddDataToTable(1, "Hello");
    }
    private void CreateTable()
    {
        // Open a connection to the database
        using (IDbConnection dbConnection = new SqliteConnection(connectionString))
        {
            dbConnection.Open();

            // Create a command to execute
            using (IDbCommand dbCmd = dbConnection.CreateCommand())
            {
                // SQL query to create a new table
                string sqlQuery = "CREATE TABLE IF NOT EXISTS example_table (id INTEGER PRIMARY KEY, value TEXT)";
                dbCmd.CommandText = sqlQuery;
                
                // Execute the command
                dbCmd.ExecuteScalar();

                
            }
            dbConnection.Close();
        }

        Debug.Log("Database and table created at: " + connectionString);
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

}

