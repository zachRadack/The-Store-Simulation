using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;

public class ProductCategoryService : MonoBehaviour
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

}
