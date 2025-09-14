using UnityEngine;
using UnityEngine.Events;

public class MenuManager : MonoBehaviour
{
    public string startSceneName;
    public string CreditsSceneName;
    public string menuSceneName;

    public UnityEvent resumeEvent;

    public void ButtonMenu()
    {
        SceneLoaderManager.getInstance().LoadScene(menuSceneName);
    }

    public void ButtonResume()
    {
        resumeEvent.Invoke();
    }

    public void ButtonStart()
    {
        SceneLoaderManager.getInstance().LoadScene(startSceneName);
    }

    public void ButtonOptions()
    {

    }

    public void ButtonCredits()
    {
        SceneLoaderManager.getInstance().LoadScene(CreditsSceneName);
    }

    public void SetLanguage(string language)
    {
        I18n.Trans().SetLanguage(language);
    }


    public void ButtonQuit()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

}
