using UnityEngine;

public class EnemyDrop : MonoBehaviour
{
    public GameObject[] lootItems; // Danh sách vật phẩm có thể rơi ra
    public float dropChance = 0.5f; // Xác suất rơi vật phẩm (50%)

    public void DropLoot()
    {
        if (Random.value < dropChance) // Kiểm tra xác suất rơi
        {
            int randomIndex = Random.Range(0, lootItems.Length);
            Instantiate(lootItems[randomIndex], transform.position, Quaternion.identity);
        }
    }

    private void OnDestroy()
    {
        DropLoot();
    }
}
