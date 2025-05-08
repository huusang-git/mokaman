using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerInventoryUI : MonoBehaviour
{
    public PlayerInventory playerInventory;
    public Image[] slotImages;
    public Sprite defaultSprite;
    public Image[] slotFrames;
    public Sprite normalFrame;
    public Sprite selectedFrame;
    private int selectedIndex = -1;
    private Dictionary<string, Sprite> ingredientSprites;

    void Start()
    {
        LoadIngredientSprites();
        playerInventory.OnInventoryChanged += UpdateUI;
        UpdateUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SelectSlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SelectSlot(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SelectSlot(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) SelectSlot(4);
    }

    void OnDestroy()
    {
        playerInventory.OnInventoryChanged -= UpdateUI;
    }

    void LoadIngredientSprites()
    {
        ingredientSprites = new Dictionary<string, Sprite>
        {
            { "robus", Resources.Load<Sprite>("Sprites/robus") },
            { "ara", Resources.Load<Sprite>("Sprites/ara") },
            { "condensed milk", Resources.Load<Sprite>("Sprites/condensed_milk") },
            { "milk", Resources.Load<Sprite>("Sprites/milk") },
            { "orange", Resources.Load<Sprite>("Sprites/orange") }
        };
    }

    public void UpdateUI()
    {
        Dictionary<string, int> inventory = playerInventory.GetInventory();
        List<string> ingredientKeys = new List<string>(inventory.Keys);

        for (int i = 0; i < slotImages.Length; i++)
        {
            if (i < ingredientKeys.Count && ingredientSprites.ContainsKey(ingredientKeys[i]))
            {
                slotImages[i].sprite = ingredientSprites[ingredientKeys[i]];
            }
            else
            {
                slotImages[i].sprite = defaultSprite;
            }
            slotFrames[i].sprite = (i == selectedIndex) ? selectedFrame : normalFrame;
        }
    }

    public void SelectSlot(int index)
    {
        if (index < 0 || index >= slotFrames.Length) return;
        for (int i = 0; i < slotFrames.Length; i++)
        {
            slotFrames[i].sprite = normalFrame;
        }
        selectedIndex = index;
        slotFrames[selectedIndex].sprite = selectedFrame;
    }

    public void SetUIVisible(bool isVisible)
    {
        gameObject.SetActive(isVisible); // Bật/tắt toàn bộ UI inventory
    }
}