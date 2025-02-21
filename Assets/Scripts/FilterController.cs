using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class FilterController : MonoBehaviour
{
    [Header("UI Components")]
    public GameObject filterPanel;
    public Transform categoryParent;
    public Transform subcategoryParent;
    public GameObject filterTogglePrefab;

    [Header("References")]
    public ProductManager productManager;

    private List<string> selectedCategories = new List<string>();
    private Dictionary<string, Toggle> subcategoryToggles = new Dictionary<string, Toggle>();

    void Start()
    {
        InitializeCategories();
        filterPanel.SetActive(false);
    }

    void InitializeCategories()
    {
        foreach (string category in productManager.categories)
        {
            GameObject toggleObj = Instantiate(filterTogglePrefab, categoryParent);
            toggleObj.GetComponentInChildren<Text>().text = category;
            Toggle toggle = toggleObj.GetComponent<Toggle>();
            toggle.onValueChanged.AddListener((isOn) => OnCategoryToggled(category, isOn));
        }
    }

    void OnCategoryToggled(string category, bool isOn)
    {
        if (isOn)
        {
            if (!selectedCategories.Contains(category))
                selectedCategories.Add(category);
        }
        else
        {
            selectedCategories.Remove(category);
        }

        UpdateSubcategories();
    }

    void UpdateSubcategories()
    {
        // Clear only subcategories that are no longer relevant
        List<string> subcategoriesToRemove = new List<string>();
        foreach (var subcategoryKey in subcategoryToggles.Keys)
        {
            if (!IsSubcategoryRelevant(subcategoryKey))
            {
                Destroy(subcategoryToggles[subcategoryKey].gameObject);
                subcategoriesToRemove.Add(subcategoryKey);
            }
        }

        // Remove irrelevant subcategories from the dictionary
        foreach (string subcategoryKey in subcategoriesToRemove)
        {
            subcategoryToggles.Remove(subcategoryKey);
        }

        // Add new subcategories for selected categories
        foreach (string sub in productManager.subcategories)
        {
            string subcategoryKey = sub; // Use subcategory name as the key
            if (!subcategoryToggles.ContainsKey(subcategoryKey))
            {
                GameObject toggleObj = Instantiate(filterTogglePrefab, subcategoryParent);
                toggleObj.GetComponentInChildren<Text>().text = sub;
                Toggle toggle = toggleObj.GetComponent<Toggle>();
                toggle.onValueChanged.AddListener((isOn) => OnSubcategoryToggled(sub, isOn));
                subcategoryToggles[subcategoryKey] = toggle;
            }
        }
    }

    bool IsSubcategoryRelevant(string subcategoryKey)
    {
        // Subcategories are relevant if at least one category is selected
        return selectedCategories.Count > 0;
    }

    void OnSubcategoryToggled(string subcategory, bool isOn)
    {
        if (isOn)
        {
            if (!productManager.selectedSubcategories.Contains(subcategory))
                productManager.selectedSubcategories.Add(subcategory);
        }
        else
        {
            productManager.selectedSubcategories.Remove(subcategory);
        }
    }

    public void ApplyFilters()
    {
        productManager.ApplyFilters(selectedCategories, productManager.selectedSubcategories);
    }

    public void ResetFilters()
    {
        // Clear selected categories and subcategories
        selectedCategories.Clear();
        productManager.selectedSubcategories.Clear();

        // Destroy all subcategory toggle GameObjects
        foreach (var toggle in subcategoryToggles.Values)
        {
            if (toggle != null && toggle.gameObject != null)
            {
                Destroy(toggle.gameObject);
            }
        }

        // Clear the subcategoryToggles dictionary
        subcategoryToggles.Clear();

        // Force an immediate UI update
        Canvas.ForceUpdateCanvases();

        // Reset all category toggles
        foreach (Transform child in categoryParent)
        {
            Toggle toggle = child.GetComponent<Toggle>();
            if (toggle != null)
            {
                toggle.isOn = false;
            }
        }

        // Reset the product list
        productManager.ResetFilters();
    }

    public void ToggleFilterPanel() => filterPanel.SetActive(!filterPanel.activeSelf);
}