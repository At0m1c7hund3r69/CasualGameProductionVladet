using UnityEngine;

public class BookAnimatedMenu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject bookAnimation;
    public GameObject menuOptions;
    public Animator animator;

    public void OpenMenu()
    {
        mainMenu.SetActive(false);

        bookAnimation.SetActive(true);

        menuOptions.SetActive(false);

        animator.Play("MenuReveal", 0, 0f);
    }

    public void RevealOptions()
    {
        menuOptions.SetActive(true);
    }

    public void CloseMenu()
    {
        bookAnimation.SetActive(false);
        mainMenu.SetActive(true);
    }
}
