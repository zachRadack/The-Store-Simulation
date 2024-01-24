using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;

public class ProductService
{
    private DatabaseConnectionManager _dbConnectionManager;

    public ProductService(DatabaseConnectionManager dbConnectionManager)
    {
        _dbConnectionManager = dbConnectionManager;
    }

    public void CreateTable()
    {
        using (IDbConnection dbConnection = _dbConnectionManager.CreateConnection())
        {
            dbConnection.Open();
            Debug.Log("Creating Product-related tables: " + dbConnection);
            using (IDbCommand dbCmd = dbConnection.CreateCommand())
            {
                string sqlQuery = @"
                    CREATE TABLE IF NOT EXISTS ProductCategories (ProductID INTEGER, CategoryID INTEGER);
                    CREATE TABLE IF NOT EXISTS Products (ProductID INTEGER PRIMARY KEY,ProductName TEXT NOT NULL,ProductDescription TEXT);
                    CREATE TABLE IF NOT EXISTS Categories (CategoryID INTEGER PRIMARY KEY AUTOINCREMENT, CategoryName TEXT);";
                dbCmd.CommandText = sqlQuery;
                dbCmd.ExecuteScalar();
            }
            dbConnection.Close();
        }
    }

    public void AddProduct(string productName)
    {
        using (IDbConnection dbConnection = _dbConnectionManager.CreateConnection())
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

    public void DeleteProduct(int productId)
    {
        using (IDbConnection dbConnection = _dbConnectionManager.CreateConnection())
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
}
