using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class PlayerShooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public PlayerInventory inventory;
    public Animator animator;

    public GameObject selectVFXPrefab;
    public GameObject floatingTextPrefab; // Prefab hiển thị text (tương tự floatingDamagePrefab)

    private List<string> selectedIngredients = new List<string>();
    private string currentBulletType = "mixed"; // Đạn mặc định

    private bool isMixing = false; // Trạng thái đang chọn nguyên liệu
    public int firstStrikeDamage = 10;
    public int secondStrikeDamage = 15;
    public int thirdStrikeDamage = 20;

    public float comboWindow = 0.5f; // Thời gian cho phép combo
    private bool isAttacking = false; // Kiểm tra trạng thái tấn công
    private int currentComboStep = 0; // Bước hiện tại trong combo
    private float[] lastStrikeTimes = new float[3]; // Lưu thời gian nhấn J cho từng đòn đánh

    public float meleeRange = 2f; // Phạm vi đánh gần (Raycast)
    public LayerMask enemyLayer; // Layer để phát hiện kẻ địch

    void Update()
    {
        if (inventory == null)
        {
            Debug.LogError("inventory chưa được gán trong Inspector!");
            return;
        }

        // Nhấn F để bắt đầu chọn nguyên liệu hoặc chốt công thức
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!isMixing)
            {
                isMixing = true;
                selectedIngredients.Clear();
                Debug.Log("Bắt đầu chọn nguyên liệu...");
            }
            else
            {
                isMixing = false;
                currentBulletType = MixSelectedIngredients();
                SpawnSelectVFX();
                Debug.Log($"Công thức hoàn thành! Đạn mới: {currentBulletType}");
            }
        }
        void SpawnSelectVFX()
        {
            if (selectVFXPrefab != null)
            {
                GameObject vfx = Instantiate(selectVFXPrefab, transform.position, Quaternion.identity);
                Destroy(vfx, 1.5f);
            }

            // Hiển thị loại đạn trên đầu player
            if (floatingTextPrefab != null)
            {
                Vector3 textPos = transform.position + new Vector3(0, 1f, 0); // Trên đầu
                GameObject textObj = Instantiate(floatingTextPrefab, textPos, Quaternion.identity);
                FloatingText floatingText = textObj.GetComponent<FloatingText>();
                if (floatingText != null)
                {
                    floatingText.SetText(currentBulletType);
                }
                else
                {
                    Debug.LogWarning("floatingTextPrefab không có FloatingText component!");
                }
                Debug.Log($"Hiển thị loại đạn: {currentBulletType}");
            }
            else
            {
                Debug.LogWarning("floatingTextPrefab chưa được gán!");
            }
        }
        // Nếu đang chọn nguyên liệu, cho phép bấm số 1-5 để chọn
        if (isMixing)
        {
            for (int i = 0; i < 5; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    string ingredient = inventory.GetIngredientBySlot(i);
                    if (!string.IsNullOrEmpty(ingredient) && selectedIngredients.Count < 2) // Chỉ tối đa 2 nguyên liệu
                    {
                        selectedIngredients.Add(ingredient);
                        Debug.Log($"Đã chọn: {ingredient}");
                    }
                }
            }
        }

        // Nhấn X để reset về đạn mặc định
        if (Input.GetKeyDown(KeyCode.X))
        {
            currentBulletType = "mixed";
            selectedIngredients.Clear();
            isMixing = false;
            Debug.Log("Reset về đạn mặc định: mixed");
        }

        // Nhấn Chuột Trái để bắn
        if (Input.GetKeyDown(KeyCode.K) && !isAttacking && !isMixing)
        {
            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }
            Shoot(currentBulletType);
        }

        // Nhấn J để bắt đầu hoặc tiếp tục combo đòn đánh gần
        if (Input.GetKeyDown(KeyCode.J) && !isMixing)
        {
            if (!isAttacking)
            {
                // Bắt đầu combo
                currentComboStep = 1;
                isAttacking = true;
                lastStrikeTimes[0] = Time.time; // Ghi lại thời gian bắt đầu combo (đòn đánh đầu tiên)
                animator.SetTrigger("MeleeAttack1");
                Debug.Log("Bắt đầu đòn đánh đầu tiên.");
            }
            else
            {
                // Kiểm tra thời gian giữa các đòn đánh
                if (Time.time - lastStrikeTimes[Mathf.Clamp(currentComboStep - 1, 0, 2)] < comboWindow)
                {
                    // Tiếp tục combo nếu thời gian đủ điều kiện
                    lastStrikeTimes[Mathf.Clamp(currentComboStep, 0, 2)] = Time.time; // Ghi lại thời gian đòn đánh tiếp theo
                    ProceedToNextCombo();
                }
                else
                {
                    // Kết thúc combo nếu quá lâu không nhấn J
                    EndAttack();
                }
            }
        }
    }

    void ProceedToNextCombo()
    {
        // Tiến tới bước tiếp theo trong combo
        currentComboStep = Mathf.Clamp(currentComboStep + 1, 1, 3); // Đảm bảo currentComboStep nằm trong phạm vi 1 đến 3

        // Thực hiện animation cho mỗi bước combo
        switch (currentComboStep)
        {
            case 2:
                animator.SetTrigger("MeleeAttack2");
                break;
            case 3:
                animator.SetTrigger("MeleeAttack3");
                break;
            default:
                EndAttack();
                break;
        }
    }

    public void OnMeleeAttack1Finished()
    {
        if (Time.time - lastStrikeTimes[0] < comboWindow)
        {
            // Tiếp tục combo đòn đánh thứ 2
            ProceedToNextCombo();
        }
        else
        {
            isAttacking = false;
            EndAttack();
        }
    }

    public void OnMeleeAttack2Finished()
    {
        if (Time.time - lastStrikeTimes[1] < comboWindow)
        {
            // Tiếp tục combo đòn đánh thứ 3
            ProceedToNextCombo();
        }
        else
        {
            isAttacking = false;
            EndAttack();
        }
    }

    public void OnMeleeAttack3Finished()
    {
        EndAttack();
    }

    void EndAttack()
    {
        // Kết thúc combo
        currentComboStep = 0;
        isAttacking = false;
        Debug.Log("Đòn đánh kết thúc.");
    }

    void Shoot(string bulletType)
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.bulletType = bulletType;

        // Xác định hướng nhân vật (hướng quay mặt)
        float direction = transform.localScale.x > 0 ? 1f : -1f;

        // Nếu đạn có Rigidbody2D thì di chuyển bằng velocity
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            float bulletSpeed = 10f;
            rb.velocity = new Vector2(direction * bulletSpeed, 0f);
        }

        // Cập nhật hướng đạn bằng cách lật sprite
        Vector3 scale = bullet.transform.localScale;
        scale.x = Mathf.Abs(scale.x) * direction; // Đảm bảo hướng đúng
        bullet.transform.localScale = scale;

        Debug.Log($"Bắn {bulletType} theo hướng {direction}");
    }

    public void PerformMeleeAttack()
    {
        // Kiểm tra hướng của người chơi
        float playerDirection = transform.localScale.x > 0 ? 1f : -1f;

        // Raycast để phát hiện kẻ địch
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right * playerDirection, meleeRange, enemyLayer);

        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                Enemy enemy = hit.collider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    int damage = 0;
                    switch (currentComboStep)
                    {
                        case 1:
                            damage = firstStrikeDamage;
                            break;
                        case 2:
                            damage = secondStrikeDamage;
                            break;
                        case 3:
                            damage = thirdStrikeDamage;
                            break;
                    }

                    if (damage > 0)
                    {
                        enemy.TakeDamage(damage, "melee", true);
                        Debug.Log($"Đánh trúng kẻ địch và gây {damage} sát thương melee (Combo: {currentComboStep})!");
                    }
                }
            }
        }

        Debug.DrawRay(transform.position, transform.right * playerDirection * meleeRange, Color.red, 0.1f);
    }

    string MixSelectedIngredients()
    {
        selectedIngredients.Sort(); // Đảm bảo thứ tự chính xác

        if (selectedIngredients.Count == 2)
        {
            if (selectedIngredients.Contains("robus") && selectedIngredients.Contains("condensed milk"))
                return "browncoffe";

            if (selectedIngredients.Contains("robus") && selectedIngredients.Contains("ara"))
                return "mixed";

            if (selectedIngredients.FindAll(i => i == "robus").Count == 2)
                return "supperdark";

            if (selectedIngredients.Contains("ara") && selectedIngredients.Contains("orange"))
                return "orange";
        }

        if (selectedIngredients.Count == 3)
        {
            if (selectedIngredients.Contains("robus") &&
                selectedIngredients.Contains("condensed milk") &&
                selectedIngredients.Contains("milk"))
                return "whitecoffe";
        }

        return "mixed"; // Đạn mặc định nếu không khớp công thức
    }
}