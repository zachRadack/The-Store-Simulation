using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using UnityEngine.Tilemaps;



public class DatabaseConnectionManager
{
    private string connectionString;
    private TableManager _TableMan;
    public DatabaseConnectionManager()
    {
        string dbPath = "URI=file:" + Application.persistentDataPath + "/Database/MyDatabase.db";
        connectionString = dbPath;
        Debug.Log("dbPath: " + dbPath);
    }

    public IDbConnection CreateConnection()
    {
        return new SqliteConnection(connectionString);
    }

    public void InitializeConnection()
    {
        string dbPath = "URI=file:" + Application.persistentDataPath + "/Database/MyDatabase.db";
        connectionString = dbPath;
        Debug.Log("dbPath: " + dbPath);
        // Create table
        _TableMan = new TableManager(this);
        _TableMan.CreateTable();
    }
}