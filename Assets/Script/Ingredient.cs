using UnityEngine;

public class Ingredient : MonoBehaviour
{
    public string ingredientName; // Tên nguyên liệu (VD: "ara", "milk")

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // Khi Player chạm vào
        {
            PlayerInventory inventory = collision.GetComponent<PlayerInventory>();
            if (inventory != null)
            {
                inventory.AddIngredient(ingredientName);
                Destroy(gameObject); // Xóa nguyên liệu sau khi nhặt
            }
        }
    }
}
