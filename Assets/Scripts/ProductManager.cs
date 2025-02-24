using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using TMPro;

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
    public Button nextButton; 
    public Button prevButton; 
    public ScrollRect scrollRect; 

    [Header("Filter Settings")]
    public List<string> categories = new List<string> { "Watches", "Cloths", "Jewelry" };
    public List<string> subcategories = new List<string> { "Male", "Female", "Kids" };
    public List<string> selectedSubcategories = new List<string>(); 

    [Header("Product Detail Panel")]
    public GameObject productDetailPanel;
    public TMP_Text detailProductName;
    public TMP_Text detailProductDescription;
    public TMP_Text detailProductPrice;

    private ProductData productData;
    private List<Product> currentProducts = new List<Product>();
    private List<GameObject> productPool = new List<GameObject>();
    private HashSet<int> selectedProducts = new HashSet<int>();

    private int maxVisibleProducts = 16; // Number of products per page
    private int currentPage = 0;
    private List<Product> filteredProducts = new List<Product>(); // Store filtered list


    void Start()
    {
        LoadProducts();
        InitializePool(maxVisibleProducts);
        ApplyFilters(categories, subcategories); // Apply filters on start
        nextButton.onClick.AddListener(NextPage);
        prevButton.onClick.AddListener(PrevPage);
        prevButton.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(false);
        scrollRect.onValueChanged.AddListener(OnScrollValueChanged);
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

    public void UpdateProducts(int page)
    {
        foreach (GameObject obj in productPool) obj.SetActive(false);

        int startIdx = page * maxVisibleProducts;
        int endIdx = Mathf.Min(startIdx + maxVisibleProducts, filteredProducts.Count);

        for (int i = 0; i < maxVisibleProducts; i++)
        {
            if (startIdx + i < endIdx)
            {
                GameObject obj = GetPooledObject();
                ProductItem item = obj.GetComponent<ProductItem>();
                item.Initialize(filteredProducts[startIdx + i], this);
                obj.SetActive(true);
            }
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
        filteredProducts = productData.products.FindAll(p =>
            (categories.Count == 0 || categories.Contains(p.category)) &&
            (subcategories.Count == 0 || subcategories.Contains(p.subcategory)));

        currentPage = 0; // Reset to first page after filtering
        UpdateProducts(currentPage);
        nextButton.gameObject.SetActive(filteredProducts.Count > maxVisibleProducts);
        prevButton.gameObject.SetActive(false);
        scrollRect.verticalNormalizedPosition = 1f;
    }

    public void ResetFilters()
    {
        filteredProducts = productData.products;
        currentPage = 0;
        UpdateProducts(currentPage);
    }

    void NextPage()
    {
        if ((currentPage + 1) * maxVisibleProducts < filteredProducts.Count)
        {
            currentPage++;
            UpdateProducts(currentPage);
            prevButton.gameObject.SetActive(true);
        }
        if ((currentPage + 1) * maxVisibleProducts >= filteredProducts.Count)
        {
            nextButton.gameObject.SetActive(false);
        }

        // Scroll to top when next page is clicked
        scrollRect.verticalNormalizedPosition = 1f;
    }

    void PrevPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            UpdateProducts(currentPage);
            nextButton.gameObject.SetActive(true);
        }
        if (currentPage == 0)
        {
            prevButton.gameObject.SetActive(false);
        }
        scrollRect.verticalNormalizedPosition = 1f;
    }

    void OnScrollValueChanged(Vector2 scrollPosition)
    {
        nextButton.gameObject.SetActive(scrollPosition.y <= 0.01f && (currentPage + 1) * maxVisibleProducts < filteredProducts.Count);
        prevButton.gameObject.SetActive(scrollPosition.y <= 0.01f && currentPage != 0);

    }

    public void ShowProductDetails(Product product)
    {
        productDetailPanel.SetActive(true);
        detailProductName.text = product.name;
        detailProductDescription.text = product.description;
        detailProductPrice.text = "$" + product.price.ToString("F2");

        productParent.gameObject.SetActive(false); // Hide product list
    }

    public void CloseProductDetails()
    {
        productDetailPanel.SetActive(false);
        productParent.gameObject.SetActive(true); // Show product list again
    }

}
