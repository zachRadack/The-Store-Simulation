using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;

public class CategoryService
{
    private DatabaseConnectionManager _dbConnectionManager;

    public CategoryService(DatabaseConnectionManager dbConnectionManager)
    {
        _dbConnectionManager = dbConnectionManager;
    }

    public void AddCategory(string categoryName)
    {
        using (IDbConnection dbConnection = _dbConnectionManager.CreateConnection())
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

    public void DeleteCategory(int categoryId)
    {
        using (IDbConnection dbConnection = _dbConnectionManager.CreateConnection())
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

    public List<string> GetCategoriesForProduct(int productId)
    {
        List<string> categories = new List<string>();
        using (IDbConnection dbConnection = _dbConnectionManager.CreateConnection())
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

}
