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
        CreateTable();
        AddDataToTable(1, "Hello");
        ReadTable();
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
                string sqlQuery = "SELECT * FROM example_table"; // Replace with your table name
                dbCmd.CommandText = sqlQuery;

                using (IDataReader reader = dbCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Assuming you have two columns: Column1 (integer) and Column2 (text)
                        int column1 = reader.GetInt32(0); // Column1
                        string column2 = reader.GetString(1); // Column2

                        // Debug.Log or process the data as needed
                        Debug.Log("id: " + column1 + ", value: " + column2);
                    }
                    reader.Close();
                }
            }
            dbConnection.Close();
        }
    }

    public void AddDataToTable(int column1Value, string column2Value)
    {
        using (IDbConnection dbConnection = new SqliteConnection(connectionString))
        {
            dbConnection.Open();

            using (IDbCommand dbCmd = dbConnection.CreateCommand())
            {
                string sqlQuery = "INSERT INTO example_table (id, value) VALUES (@id, @value)";
                dbCmd.CommandText = sqlQuery;

                // Create parameters and add values
                IDbDataParameter param1 = dbCmd.CreateParameter();
                param1.ParameterName = "@id";
                param1.Value = column1Value;
                dbCmd.Parameters.Add(param1);

                IDbDataParameter param2 = dbCmd.CreateParameter();
                param2.ParameterName = "@value";
                param2.Value = column2Value;
                dbCmd.Parameters.Add(param2);

                // Execute the command
                dbCmd.ExecuteNonQuery();
            }

            dbConnection.Close();
        }

        Debug.Log("Data added to table: Column1 = " + column1Value + ", Column2 = " + column2Value);
    }

}

