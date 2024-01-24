using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;

public class DatabaseTestService
{
    private CategoryService _categoryService;
    private ProductService _productService;
    private ProductCategoryService _productCategoryService;
    private TableManager _tableManager;
    // Other services as needed

    public DatabaseTestService(DatabaseConnectionManager dbConnectionManager)
    {
        _categoryService = new CategoryService(dbConnectionManager);
        _productService = new ProductService(dbConnectionManager);
        _productCategoryService = new ProductCategoryService(dbConnectionManager);
        _tableManager = new TableManager(dbConnectionManager);
        // Initialize other services as needed
    }

    public void TestDatabaseFunctions()
    {
        // Clear tables
        _tableManager.ClearTable("ProductCategories");
        _tableManager.ClearTable("Products");
        _tableManager.ClearTable("Categories");

        // Add categories
        Debug.Log("Adding categories");
        _categoryService.AddCategory("Snacks");
        _categoryService.AddCategory("Beverages");
        _categoryService.AddCategory("Desserts");
        _categoryService.AddCategory("JunkFood");

        // Add products
        Debug.Log("Adding products");
        _productService.AddProduct("Dorito Chips");
        _productService.AddProduct("Coca Cola");

        int doritosId = 1;
        int colaId = 2;
        int snacksCategoryId = 1;
        int beveragesCategoryId = 2;
        int junkFoodCategoryId = 4;

        // Associate products with categories
        _productCategoryService.AddProductToCategory(doritosId, snacksCategoryId);
        _productCategoryService.AddProductToCategory(doritosId, junkFoodCategoryId);
        _productCategoryService.AddProductToCategory(colaId, beveragesCategoryId);
        _productCategoryService.AddProductToCategory(colaId, junkFoodCategoryId);

        // Add shelves logic here
        // TODO: Add shelves logic here
        Debug.Log("Adding shelves");
    }

}

