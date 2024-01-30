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
    private InventoryService _inventoryService;
    // Other services as needed

    public DatabaseTestService(DatabaseConnectionManager dbConnectionManager)
    {
        _categoryService = new CategoryService(dbConnectionManager);
        _productService = new ProductService(dbConnectionManager);
        _productCategoryService = new ProductCategoryService(dbConnectionManager);
        _tableManager = new TableManager(dbConnectionManager);
        _inventoryService = new InventoryService(dbConnectionManager);
        // Initialize other services as needed
    }

    public void TestDatabaseFunctions()
    {
        // Clear tables
        _tableManager.ClearTable("ProductCategories");
        _tableManager.ClearTable("Products");
        _tableManager.ClearTable("Categories");
        _tableManager.ClearTable("Inventory");
        

        // Add categories
        Debug.Log("Adding categories");
        // _categoryService.AddCategory("Snacks");
        // _categoryService.AddCategory("Beverages");
        // _categoryService.AddCategory("Desserts");
        // _categoryService.AddCategory("JunkFood");

        _categoryService.AddCategory(1,"Store Items");
        _categoryService.AddCategory("Food",1,2);
        _categoryService.AddCategory("Snacks",2,3);
        _categoryService.AddCategory("Junk Food",2,4);
        _categoryService.AddCategory("Chips",3,5);

        // Add products
        Debug.Log("Adding products");
        _productService.AddProduct("Dorito Chips");
        _productService.AddProduct("Spicy Dorito Chips");

        int StoreItemsCategoryId = 1;
        int FoodCategoryId = 2;
        int SnacksCategoryId = 3;
        int JunkFoodCategoryId = 4;
        int ChipsCategoryId = 5;

        // Associate products with categories
        Debug.Log("Associating products with categories");
        _productCategoryService.AddProductToCategory(1, ChipsCategoryId);
        _productCategoryService.AddProductToCategory(1, JunkFoodCategoryId);
        _productCategoryService.AddProductToCategory(2, ChipsCategoryId);
        _productCategoryService.AddProductToCategory(2, JunkFoodCategoryId);

        // Add shelves logic here
        // TODO: Add shelves logic here
        Debug.Log("Adding shelves");

        // Add inventory logic here
        _inventoryService.addInventoryItem(1,9);
        _inventoryService.addInventoryItem(2,19);
        _inventoryService.addInventoryItem(1,9);
        _inventoryService.addInventoryItem(2,20);
        _inventoryService.addInventoryItem(2,21);
        _inventoryService.addInventoryItem(2,22);


        HashSet<int> categoryIds = _productCategoryService.GetAllCategoriesForProduct(1);
        Debug.Log("Categories for product 1:" + categoryIds.Count);
        foreach (int categoryId in categoryIds)
        {
            Debug.Log(_categoryService.getNameOfCategories(categoryId));
        }
    }

}

