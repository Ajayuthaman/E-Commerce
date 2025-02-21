using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ProductManager : MonoBehaviour
{
    [System.Serializable]
    public class ProductData
    {
        public List<Product> products;
    }

    [System.Serializable]
    public class Product
    {
        public int id;
        public string name;
        public string category;
        public string subcategory;
        public string description;
        public float price;
    }

    [Header("UI References")]
    public Transform productParent;
    public GameObject productPrefab;

    [Header("Filter Settings")]
    public List<string> categories = new List<string> { "Watches", "Cloths", "Jewelry" };
    public List<string> subcategories = new List<string> { "Male", "Female", "Kids" };

    public List<string> selectedSubcategories = new List<string>(); // Track selected subcategories

    private ProductData productData;
    private List<Product> currentProducts = new List<Product>();
    private List<GameObject> productPool = new List<GameObject>();
    private HashSet<int> selectedProducts = new HashSet<int>();

    void Start()
    {
        LoadProducts();
        InitializePool(20);
        UpdateProducts(productData.products);
    }
        
    void LoadProducts()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("Data/Products");
        if (jsonFile == null)
        {
            Debug.LogError("Failed to load JSON file.");
            return;
        }

        try
        {
            productData = JsonUtility.FromJson<ProductData>(jsonFile.text);
            if (productData == null || productData.products == null)
            {
                Debug.LogError("Failed to parse JSON data.");
                return;
            }
            currentProducts = productData.products;
        }
        catch (System.Exception e)
        {
            Debug.LogError("JSON parse error: " + e.Message);
        }
    }

    void InitializePool(int size)
    {
        for (int i = 0; i < size; i++)
        {
            GameObject obj = Instantiate(productPrefab, productParent);
            obj.SetActive(false);
            productPool.Add(obj);
        }
    }

    GameObject GetPooledObject()
    {
        foreach (GameObject obj in productPool)
        {
            if (!obj.activeInHierarchy) return obj;
        }
        GameObject newObj = Instantiate(productPrefab, productParent);
        productPool.Add(newObj);
        return newObj;
    }

    public void UpdateProducts(List<Product> products)
    {
        foreach (GameObject obj in productPool) obj.SetActive(false);

        for (int i = 0; i < products.Count; i++)
        {
            GameObject obj = GetPooledObject();
            ProductItem item = obj.GetComponent<ProductItem>();
            item.Initialize(products[i], this);
            obj.SetActive(true);
        }
    }

    public void ToggleProductSelection(int productId)
    {
        if (selectedProducts.Contains(productId))
            selectedProducts.Remove(productId);
        else
            selectedProducts.Add(productId);
    }

    public void ApplyFilters(List<string> categories, List<string> subcategories)
    {
        var filtered = productData.products.FindAll(p =>
            (categories.Count == 0 || categories.Contains(p.category)) &&
            (subcategories.Count == 0 || subcategories.Contains(p.subcategory)));

        UpdateProducts(filtered);
    }

    public void ResetFilters()
    {
        UpdateProducts(productData.products);
    }
}