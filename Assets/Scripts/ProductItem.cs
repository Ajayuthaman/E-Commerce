using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProductItem : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text productNameText;
    public Toggle selectionToggle;

    private ProductManager.Product product;
    private ProductManager productManager;

    public void Initialize(ProductManager.Product productData, ProductManager manager)
    {
        product = productData;
        productManager = manager;
        productNameText.text = product.name;
        selectionToggle.onValueChanged.AddListener(OnToggleChanged);
    }

    void OnToggleChanged(bool isSelected)
    {
        productManager.ToggleProductSelection(product.id);
    }
}