using UnityEngine;

public class DestroyOBJ : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, 0.5f); // Biến mất sau 5 giây
    }
}
