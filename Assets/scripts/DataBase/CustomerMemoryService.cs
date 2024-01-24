using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System;

public class CustomerMemoryService
{
    private DatabaseConnectionManager _dbConnectionManager;

    public CustomerMemoryService(DatabaseConnectionManager dbConnectionManager)
    {
        _dbConnectionManager = dbConnectionManager;
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        using (IDbConnection dbConnection = _dbConnectionManager.CreateConnection())
        {
            dbConnection.Open();

            using (IDbCommand dbCmd = dbConnection.CreateCommand())
            {
                dbCmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Customers (
                    CustomerID INTEGER PRIMARY KEY AUTOINCREMENT,
                    CustomerName TEXT
                );
                CREATE TABLE IF NOT EXISTS CustomerKnownCategories (
                    CustomerID INTEGER,
                    CategoryID INTEGER,
                    FOREIGN KEY (CustomerID) REFERENCES Customers(CustomerID),
                    FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID)
                );
                CREATE TABLE IF NOT EXISTS CustomerKnownProducts (
                    CustomerID INTEGER,
                    ProductID INTEGER,
                    FOREIGN KEY (CustomerID) REFERENCES Customers(CustomerID),
                    FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
                );";
                dbCmd.ExecuteNonQuery();
            }

            dbConnection.Close();
        }
    }
    public void AddCustomer(string customerName)
    {
        using (IDbConnection dbConnection = _dbConnectionManager.CreateConnection())
        {
            dbConnection.Open();
            using (IDbCommand dbCmd = dbConnection.CreateCommand())
            {
                string sqlQuery = "INSERT INTO Customers (CustomerName) VALUES (@CustomerName)";
                dbCmd.CommandText = sqlQuery;

                IDbDataParameter nameParam = dbCmd.CreateParameter();
                nameParam.ParameterName = "@CustomerName";
                nameParam.Value = customerName;
                dbCmd.Parameters.Add(nameParam);

                dbCmd.ExecuteNonQuery();
            }
            dbConnection.Close();
        }
    }

    public void RememberCategory(int customerId, int categoryId)
    {
        ModifyCustomerMemory("CustomerKnownCategories", customerId, categoryId);
    }

    public void RememberProduct(int customerId, int productId)
    {
        ModifyCustomerMemory("CustomerKnownProducts", customerId, productId);
    }

    private void ModifyCustomerMemory(string tableName, int customerId, int itemId)
    {
        using (IDbConnection dbConnection = _dbConnectionManager.CreateConnection())
        {
            dbConnection.Open();
            using (IDbCommand dbCmd = dbConnection.CreateCommand())
            {
                string sqlQuery = $"INSERT INTO {tableName} (CustomerID, ItemID) VALUES (@CustomerID, @ItemID)";
                dbCmd.CommandText = sqlQuery;

                IDbDataParameter customerParam = dbCmd.CreateParameter();
                customerParam.ParameterName = "@CustomerID";
                customerParam.Value = customerId;
                dbCmd.Parameters.Add(customerParam);

                IDbDataParameter itemParam = dbCmd.CreateParameter();
                itemParam.ParameterName = "@ItemID";
                itemParam.Value = itemId;
                dbCmd.Parameters.Add(itemParam);

                dbCmd.ExecuteNonQuery();
            }
            dbConnection.Close();
        }
    }

    public List<int> GetKnownCategoriesForCustomer(int customerId)
    {
        List<int> knownCategories = new List<int>();
        using (IDbConnection dbConnection = _dbConnectionManager.CreateConnection())
        {
            dbConnection.Open();
            using (IDbCommand dbCmd = dbConnection.CreateCommand())
            {
                dbCmd.CommandText = @"
                SELECT CategoryID 
                FROM CustomerKnownCategories 
                WHERE CustomerID = @CustomerID";

                IDbDataParameter customerParam = dbCmd.CreateParameter();
                customerParam.ParameterName = "@CustomerID";
                customerParam.Value = customerId;
                dbCmd.Parameters.Add(customerParam);

                using (IDataReader reader = dbCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        knownCategories.Add(Convert.ToInt32(reader["CategoryID"]));
                    }
                }
            }
            dbConnection.Close();
        }
        return knownCategories;
    }

    public HashSet<int> GetKnownCategoriesForProductByCustomer(int customerId, int productId)
    {
        HashSet<int> knownProductCategories = new HashSet<int>();
        using (IDbConnection dbConnection = _dbConnectionManager.CreateConnection())
        {
            dbConnection.Open();
            using (IDbCommand dbCmd = dbConnection.CreateCommand())
            {
                dbCmd.CommandText = @"
                WITH RECURSIVE CategoryTree AS (
                    SELECT c.CategoryID 
                    FROM Categories c
                    INNER JOIN ProductCategories pc ON pc.CategoryID = c.CategoryID
                    WHERE pc.ProductID = @ProductID
                    UNION
                    SELECT c.ParentCategoryID 
                    FROM Categories c
                    INNER JOIN CategoryTree ct ON ct.CategoryID = c.CategoryID
                )
                SELECT CategoryID 
                FROM CategoryTree 
                WHERE CategoryID IN (SELECT CategoryID FROM CustomerKnownCategories WHERE CustomerID = @CustomerID)";

                IDbDataParameter productParam = dbCmd.CreateParameter();
                productParam.ParameterName = "@ProductID";
                productParam.Value = productId;
                dbCmd.Parameters.Add(productParam);

                IDbDataParameter customerParam = dbCmd.CreateParameter();
                customerParam.ParameterName = "@CustomerID";
                customerParam.Value = customerId;
                dbCmd.Parameters.Add(customerParam);

                using (IDataReader reader = dbCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        knownProductCategories.Add(Convert.ToInt32(reader["CategoryID"]));
                    }
                }
            }
            dbConnection.Close();
        }
        return knownProductCategories;
    }
}
