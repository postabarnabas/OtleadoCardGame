using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject personVsPersonButton;
    public GameObject personVsAIButton;
    public void StartPersonVsPerson()
    {
        GameSettings.playWithAI=false;
        SceneManager.LoadScene("Game");
    }

    public void StartPersonVsAI()
    {
        GameSettings.playWithAI=true;
        SceneManager.LoadScene("Game");
    }
}