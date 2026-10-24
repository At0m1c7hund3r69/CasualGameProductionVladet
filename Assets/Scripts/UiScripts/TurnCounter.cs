using UnityEngine;
using TMPro;

public class TurnCounter : MonoBehaviour
{
    public static TurnCounter Instance;
    [SerializeField] private TMP_Text turnText;
    private int currentTurn = 1;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    private void Start()
    {
        UpdateTurnUI();
    }

    public void NextTurn(int amount)
    {
        currentTurn += amount;
        UpdateTurnUI();
    }

    private void UpdateTurnUI()
    {
        turnText.text = "" +currentTurn;
    }
}
