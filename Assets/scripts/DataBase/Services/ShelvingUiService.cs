using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;

public class ShelvingUiService : MonoBehaviour
{
    private DatabaseConnectionManager _dbConnectionManager;

    public ShelvingUiService(DatabaseConnectionManager dbConnectionManager)
    {
        _dbConnectionManager = dbConnectionManager;
    }

    public List<List<float>> getShelvesBackgroundData(int shelfId)
    {
        List<List<float>> shelfDimensionsList = new List<List<float>>();

        using (IDbConnection dbConnection = _dbConnectionManager.CreateConnection())
        {
            dbConnection.Open();
            Debug.Log("Getting shelf background data");
            // Get all shelves
            using (IDbCommand dbCmd = dbConnection.CreateCommand())
            {
                dbCmd.CommandText = "SELECT Xanchor, Yanchor, Xsize, Ysize FROM ShelvingUi WHERE ShelfID = @ShelfID";
                dbCmd.Parameters.Add(new SqliteParameter("@ShelfID", shelfId));
                using (IDataReader reader = dbCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        List<float> shelfDimensions = new List<float>();
                        shelfDimensions.Add(reader.GetFloat(reader.GetOrdinal("Xanchor")));
                        shelfDimensions.Add(reader.GetFloat(reader.GetOrdinal("Yanchor")));
                        shelfDimensions.Add(reader.GetFloat(reader.GetOrdinal("Xsize")));
                        //shelfDimensions.Add(reader.GetFloat(reader.GetOrdinal("Ysize")));

                        if (reader.IsDBNull(reader.GetOrdinal("Ysize")))
                        {
                            shelfDimensions.Add(-425f);
                        }
                        else
                        {
                            shelfDimensions.Add(reader.GetFloat(reader.GetOrdinal("Ysize")));
                            Debug.Log("Ysize is not null");
                        }
                        shelfDimensionsList.Add(shelfDimensions);
                    }
                }
            }
            dbConnection.Close();
        }
        return shelfDimensionsList;
    }

}
