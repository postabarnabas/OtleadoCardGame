using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void StartHumanVsHuman()
    {
        GameSettings.playWithAI=false;
        SceneManager.LoadScene("Game");
    }

    public void StartHumanVsAI()
    {
        GameSettings.playWithAI=true;
        SceneManager.LoadScene("Game");
    }
}