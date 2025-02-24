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
            {
                selectedCategories.Add(category);
            }
        }
        else
        {
            selectedCategories.Remove(category);

            // Disable subcategory parent when no categories are selected
            if (selectedCategories.Count == 0)
            {
                subcategoryParent.gameObject.SetActive(false);
            }
        }

        UpdateSubcategories();
    }

    void UpdateSubcategories()
    {
        // Enable subcategory parent if at least one main category is selected
        subcategoryParent.gameObject.SetActive(selectedCategories.Count > 0);

        // Add new subcategories for selected categories
        foreach (string sub in productManager.subcategories)
        {
            if (!subcategoryToggles.ContainsKey(sub))
            {
                GameObject toggleObj = Instantiate(filterTogglePrefab, subcategoryParent);
                toggleObj.GetComponentInChildren<Text>().text = sub;
                Toggle toggle = toggleObj.GetComponent<Toggle>();
                toggle.onValueChanged.AddListener((isOn) => OnSubcategoryToggled(sub, isOn));
                subcategoryToggles[sub] = toggle;
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

    void ResetFilters()
    {
        // Clear selected categories and subcategories
        selectedCategories.Clear();
        productManager.selectedSubcategories.Clear();

        // Uncheck and disable all subcategory toggles
        foreach (var toggle in subcategoryToggles.Values)
        {
            if (toggle != null)
            {
                toggle.isOn = false;  // Uncheck the toggle
            }
        }

        subcategoryParent.gameObject.SetActive(false);

        // Uncheck all category toggles
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