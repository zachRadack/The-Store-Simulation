using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CategoryAssociationManagerMap
{

    // Maps tile positions to category IDs
    private Dictionary<Vector2Int, int> shelfToCategoryMap; 

    // Maps category IDs to shelf positions
    private Dictionary<int, List<Vector2Int>> categoryToShelvesMap; 
    
     // Maps category IDs to associated category IDs
    private Dictionary<int, HashSet<int>> categoryAssociations;

    private DatabaseConnectionManager dbConnectionManager;
    private ShelfService _shelfService;
    private ProductCategoryService _productCategoryService;
    public MainShelvingManager _MainShelvingManager;

    public void CategoryAssociationManager()
    {
        shelfToCategoryMap = new Dictionary<Vector2Int, int>();
        categoryToShelvesMap = new Dictionary<int, List<Vector2Int>>();
        categoryAssociations = new Dictionary<int, HashSet<int>>();
    }

    public void Start(){
        DatabaseConnectionManager dbConnectionManager = new DatabaseConnectionManager();
        _shelfService = new ShelfService(dbConnectionManager);
        _productCategoryService = new ProductCategoryService(dbConnectionManager);
    }

    // Call this at the start to initialize the shelf-category associations
    public void AnalyzeStoreLayout(Tilemap tilemap)
    {
        foreach (var position in tilemap.cellBounds.allPositionsWithin){
            var localPlace = new Vector3Int(position.x, position.y, position.z);
            bool isItShelf = _MainShelvingManager.isThereAShelf(localPlace);
            if (isItShelf){
                //tile.setName();
                shelfToCategoryMap.Add(new Vector2Int(localPlace.x, localPlace.y),_MainShelvingManager.getShelfId(localPlace));
            }

        }
        
    }

    // Generates category associations based on shelf proximity and category hierarchy
    public void GenerateCategoryAssociations()
    {
        foreach (KeyValuePair<Vector2Int, int> shelf in shelfToCategoryMap){
            int shelfID = shelf.Value;
            HashSet<int> shelfCategoryIds = getShelfCategory(shelfID);
            foreach (int categoryID in shelfCategoryIds){
                if (!categoryAssociations.ContainsKey(categoryID)){
                    categoryAssociations.Add(categoryID, new HashSet<int>());
                }
                foreach (int otherCategoryID in shelfCategoryIds){
                    if (categoryID != otherCategoryID){
                        categoryAssociations[categoryID].Add(otherCategoryID);
                    }
                }
            }
        }
    }

    // gets all associated categories for a given category
    private HashSet<int> getShelfCategory(int shelfID){
        List<int> shelfProductIds = _shelfService.getProductIdsOnShelf(shelfID);
        HashSet<int> shelfCategoryIds = new HashSet<int>();
        foreach (int productID in shelfProductIds){
            shelfCategoryIds.UnionWith(_productCategoryService.GetAllCategoriesForProduct(productID));
            
        }
        return shelfCategoryIds;
    }

    // Gets hierarchical associations for visualization or other purposes
    public Dictionary<int, HashSet<int>> GetCategoryAssociations()
    {
        return categoryAssociations;
    }
}
