using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;

public class ProductCategoryQueryService : MonoBehaviour
{
    private DatabaseConnectionManager _dbConnectionManager;

    public ProductCategoryQueryService(DatabaseConnectionManager dbConnectionManager)
    {
        _dbConnectionManager = dbConnectionManager;
    }

        //<summary>
    /// Class <c>GetProductsForCategory</c> gets all the products for a given category
    /// </summary>
    public List<string> GetProductsForCategory(int categoryId)
    {
        List<string> products = new List<string>();

        using (IDbConnection dbConnection = _dbConnectionManager.CreateConnection())
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

}
