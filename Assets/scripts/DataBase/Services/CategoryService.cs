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

    public void AddCategory( int CategoryID,string categoryName){
        using (IDbConnection dbConnection = _dbConnectionManager.CreateConnection())
        {
            dbConnection.Open();
            using (IDbCommand dbCmd = dbConnection.CreateCommand())
            {
                string sqlQuery = "INSERT INTO Categories (CategoryName, CategoryID) VALUES (@CategoryName, @CategoryID)";
                dbCmd.CommandText = sqlQuery;

                IDbDataParameter categoryNameParam = dbCmd.CreateParameter();
                categoryNameParam.ParameterName = "@CategoryName";
                categoryNameParam.Value = categoryName;
                dbCmd.Parameters.Add(categoryNameParam);

                IDbDataParameter categoryIDParam = dbCmd.CreateParameter();
                categoryIDParam.ParameterName = "@CategoryID";
                categoryIDParam.Value = CategoryID;
                dbCmd.Parameters.Add(categoryIDParam);

                dbCmd.ExecuteNonQuery();
            }
            dbConnection.Close();
        }
    }


    public void AddCategory(string categoryName, int ParentCategoryID, int CategoryID){
        using (IDbConnection dbConnection = _dbConnectionManager.CreateConnection())
        {
            dbConnection.Open();
            using (IDbCommand dbCmd = dbConnection.CreateCommand())
            {
                string sqlQuery = "INSERT INTO Categories (CategoryName, ParentCategoryID, CategoryID) VALUES (@CategoryName, @ParentCategoryID, @CategoryID)";
                dbCmd.CommandText = sqlQuery;

                IDbDataParameter categoryNameParam = dbCmd.CreateParameter();
                categoryNameParam.ParameterName = "@CategoryName";
                categoryNameParam.Value = categoryName;
                dbCmd.Parameters.Add(categoryNameParam);

                IDbDataParameter parentCategoryIDParam = dbCmd.CreateParameter();
                parentCategoryIDParam.ParameterName = "@ParentCategoryID";
                parentCategoryIDParam.Value = ParentCategoryID;
                dbCmd.Parameters.Add(parentCategoryIDParam);

                IDbDataParameter categoryIDParam = dbCmd.CreateParameter();
                categoryIDParam.ParameterName = "@CategoryID";
                categoryIDParam.Value = CategoryID;
                dbCmd.Parameters.Add(categoryIDParam);

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

    public string getNameOfCategories(int categoryId){
        string categoryName = "";
        using(IDbConnection dbConnection = _dbConnectionManager.CreateConnection()){
            dbConnection.Open();
            using(IDbCommand dbCmd = dbConnection.CreateCommand()){
                string sqlQuery = "SELECT CategoryName FROM Categories WHERE CategoryID = @CategoryID";
                dbCmd.CommandText = sqlQuery;
                IDbDataParameter param = dbCmd.CreateParameter();
                param.ParameterName = "@CategoryID";
                param.Value = categoryId;
                dbCmd.Parameters.Add(param);

                using(IDataReader reader = dbCmd.ExecuteReader()){
                    while(reader.Read()){
                        categoryName = reader.GetString(0);
                    }
                }
            }
            dbConnection.Close();
        }
        return categoryName;
    }

    // this gets all the categories and returns them in a list
    public List<int> getAllCategories(){
        List<int> categoryIDs = new List<int>();
        using(IDbConnection dbConnection = _dbConnectionManager.CreateConnection()){
            dbConnection.Open();
            using(IDbCommand dbCmd = dbConnection.CreateCommand()){
                string sqlQuery = "SELECT CategoryID FROM Categories";
                dbCmd.CommandText = sqlQuery;
                using(IDataReader reader = dbCmd.ExecuteReader()){
                    while(reader.Read()){
                        categoryIDs.Add(reader.GetInt32(0));
                    }
                }
            }
        }
        return categoryIDs;
    }
}
