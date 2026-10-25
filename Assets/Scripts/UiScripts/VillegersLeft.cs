using UnityEngine;
using TMPro;

public class VillegersLeft : MonoBehaviour
{
    public static VillegersLeft Instance;
    [SerializeField] private TMP_Text villagersLeftText;
    public int villagerAmount = 10;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    private void Start()
    {
        UpdateVillagerUI();
    }

    public void Villagers(int amount)
    {
        villagerAmount += amount;
        UpdateVillagerUI();
    }

    private void UpdateVillagerUI()
    {
        villagersLeftText.text = "" + villagerAmount;
    }
}
