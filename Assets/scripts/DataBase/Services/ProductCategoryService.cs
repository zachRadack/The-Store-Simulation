using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System;

public class ProductCategoryService
{
    private DatabaseConnectionManager _dbConnectionManager;

    public ProductCategoryService(DatabaseConnectionManager dbConnectionManager)
    {
        _dbConnectionManager = dbConnectionManager;
    }

    public void AddProductToCategory(int productId, int categoryId)
    {
        using (IDbConnection dbConnection = _dbConnectionManager.CreateConnection())
        {
            dbConnection.Open();
            using (IDbCommand dbCmd = dbConnection.CreateCommand())
            {
                string sqlQuery = "INSERT INTO ProductCategories (ProductID, CategoryID) VALUES (@ProductID, @CategoryID)";
                dbCmd.CommandText = sqlQuery;

                IDbDataParameter productParam = dbCmd.CreateParameter();
                productParam.ParameterName = "@ProductID";
                productParam.Value = productId;
                dbCmd.Parameters.Add(productParam);

                IDbDataParameter categoryParam = dbCmd.CreateParameter();
                categoryParam.ParameterName = "@CategoryID";
                categoryParam.Value = categoryId;
                dbCmd.Parameters.Add(categoryParam);

                dbCmd.ExecuteNonQuery();
            }
            dbConnection.Close();
        }
    }
    public HashSet<int> GetAllCategoriesForProduct(int productId)
    {
        HashSet<int> categories = new HashSet<int>();
        using (IDbConnection dbConnection = _dbConnectionManager.CreateConnection())
        {
            dbConnection.Open();
            using (IDbCommand dbCmd = dbConnection.CreateCommand())
            {
                string sqlQuery = @"
                WITH RECURSIVE CategoryTree AS (
                    SELECT CategoryID 
                    FROM ProductCategories 
                    WHERE ProductID = @ProductID
                    UNION ALL
                    SELECT c.ParentCategoryID 
                    FROM Categories c
                    INNER JOIN CategoryTree ct ON ct.CategoryID = c.CategoryID
                    WHERE c.ParentCategoryID IS NOT NULL
                )
                SELECT CategoryID FROM CategoryTree";

                dbCmd.CommandText = sqlQuery;
                IDbDataParameter productParam = dbCmd.CreateParameter();
                productParam.ParameterName = "@ProductID";
                productParam.Value = productId;
                dbCmd.Parameters.Add(productParam);

                using (IDataReader reader = dbCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        categories.Add(Convert.ToInt32(reader["CategoryID"]));
                    }
                }
            }
            dbConnection.Close();
        }
        return categories;
    }

}
