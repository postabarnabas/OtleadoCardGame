using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject personVsPersonButton;
    public GameObject personVsAIButton;
    public GameObject rulesPanel;
    public GameObject rulesbutton;
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
    public void ShowRules()
    {
        rulesPanel.SetActive(true);
        personVsAIButton.SetActive(false);
        personVsPersonButton.SetActive(false);
        rulesbutton.SetActive(false);
    }
    public void HideRules() 
    { 
    rulesPanel.SetActive(false);
        personVsAIButton.SetActive(true);
        personVsPersonButton.SetActive(true);
        rulesbutton.SetActive(true);
    }
}