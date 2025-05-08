using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private Dictionary<string, int> ingredients = new Dictionary<string, int>();
    public event Action OnInventoryChanged; // 🔥 Sự kiện thay đổi kho đồ

    void Start()
    {
        AddIngredient("robus");
        AddIngredient("ara");
    }

    public void AddIngredient(string ingredientName)
    {
        if (ingredients.ContainsKey(ingredientName))
            ingredients[ingredientName]++;
        else
            ingredients[ingredientName] = 1;

        Debug.Log($"Nhặt {ingredientName}. Tổng: {ingredients[ingredientName]}");

        OnInventoryChanged?.Invoke(); // 🔥 Gọi sự kiện để cập nhật UI
    }

    public Dictionary<string, int> GetInventory()
    {
        return new Dictionary<string, int>(ingredients);
    }

    public void UseIngredients(params string[] usedIngredients)
    {
        foreach (string ingredient in usedIngredients)
        {
            if (ingredients.ContainsKey(ingredient))
            {
                ingredients[ingredient]--;
                if (ingredients[ingredient] <= 0) ingredients.Remove(ingredient);
            }
        }

        OnInventoryChanged?.Invoke(); // 🔥 Cập nhật UI ngay sau khi sử dụng nguyên liệu
    }

    public string MixIngredients()
    {
        if (HasIngredients("robus", "condensed milk", "milk"))
        {
            UseIngredients("robus", "condensed milk", "milk");
            return "whitecoffe";
        }
        else if (HasIngredients("robus", "condensed milk"))
        {
            UseIngredients("robus", "condensed milk");
            return "browncoffe";
        }
        else if (HasIngredients("robus", "ara"))
        {
            UseIngredients("robus", "ara");
            return "mixed";
        }
        else if (HasIngredients("robus", "robus"))
        {
            UseIngredients("robus", "robus");
            return "supperdark";
        }

        Debug.Log("Không đủ nguyên liệu để mix!");
        return null;
    }

    private bool HasIngredients(params string[] requiredIngredients)
    {
        foreach (string ingredient in requiredIngredients)
        {
            if (!ingredients.ContainsKey(ingredient) || ingredients[ingredient] <= 0)
                return false;
        }
        return true;
    }
    public string GetIngredientBySlot(int slot)
    {
        List<string> keys = new List<string>(ingredients.Keys);
        return (slot >= 0 && slot < keys.Count) ? keys[slot] : null;
    }
}
