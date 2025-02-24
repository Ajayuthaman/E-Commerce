using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProductItem : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text productNameText;
    public TMP_Text price;
    public Button productButton;

    private ProductManager.Product product;
    private ProductManager productManager;

    public void Initialize(ProductManager.Product productData, ProductManager manager)
    {
        product = productData;
        productManager = manager;
        productNameText.text = product.name;
        price.text = $"{product.price}$";


        productButton.onClick.RemoveAllListeners();
        productButton.onClick.AddListener(() => productManager.ShowProductDetails(productData));
    }

}