using UnityEngine;
using UnityEngine.UIElements;

public class UIMenu : MenuManager

{
    private UIDocument document;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {

        document = GetComponent<UIDocument>();

        document.rootVisualElement.Q<Button>("ButtonStart").clicked += StartGame;
        document.rootVisualElement.Q<Button>("ButtonQuit").clicked += QuitGame;

    }

    void StartGame()
    {
        ButtonStart();
    }

    void ReadyGame()
    {

    }

    void QuitGame()
    {
        ButtonQuit();
    }
}
