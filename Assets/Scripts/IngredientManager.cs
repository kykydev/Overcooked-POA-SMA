using UnityEngine;

public class IngredientManager : MonoBehaviour
{
    /// --- Attributes ---
    public Ingredient Tomato;
    public Ingredient Salad;
    public Ingredient Bread;
    public Ingredient Steak;

    /// --- Methods ---
    void Awake()
    {
        Tomato = new Ingredient("Tomato");
        Salad = new Ingredient("Salad");
        Bread = new Ingredient("Bread");
        Steak = new Ingredient("Steak");
    }
}

