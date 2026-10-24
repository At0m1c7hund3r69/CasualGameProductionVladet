using UnityEngine;
using TMPro;

public class VillegersLeft : MonoBehaviour
{
    public static VillegersLeft Instance;
    [SerializeField] private TMP_Text villagersLeftText;
    private int villagerAmount = 1;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    private void Start()
    {
        UpdateVillagerUI();
    }

    public void NextTurn(int amount)
    {
        villagerAmount += amount;
        UpdateVillagerUI();
    }

    private void UpdateVillagerUI()
    {
        villagersLeftText.text = "" +villagerAmount;
    }
}
