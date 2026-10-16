using UnityEngine;
using UnityEngine.EventSystems;

public class SwitchMenu : MonoBehaviour
{
    public GameObject NewMenu;
    public GameObject PrevouisMenu;

    public GameObject FirstSelectedButton;

    public bool nextMenu;

    private EventSystem eventSystem;

    void Start() //Sets the Pause Panel to inactive on start
    {
        NewMenu.SetActive(false);
    }


    public void MenuNext()
    {
        if (nextMenu) return;

        NewMenu.SetActive(true);

        if (PrevouisMenu != null)
        {
            PrevouisMenu.SetActive(false);
        }
    }


    public void MenuBack()
    {
        if (nextMenu) return;

        NewMenu.SetActive(false);

        if (PrevouisMenu != null)
        {
            PrevouisMenu.SetActive(true);
        }
    }
}
