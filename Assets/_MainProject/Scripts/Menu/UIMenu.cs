using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class UIMenu : MenuManager

{
    private UIDocument document;
    [SerializeField]
    private Camera cam;

    [SerializeField]
    private Transform targetPosition;
    private Transform sourcePosition;

    private bool toMap;
    private bool animate;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sourcePosition = cam.transform;
        toMap = false;
        animate = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(animate)
        {

        }
        
    }

    private void OnEnable()
    {

        document = GetComponent<UIDocument>();

        document.rootVisualElement.Q<Button>("ButtonStart").clicked += StartGame;
        document.rootVisualElement.Q<Button>("ButtonQuit").clicked += QuitGame;

        StartCoroutine(WaitAndFinish(2.0f));

    }

    private IEnumerator WaitAndFinish(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        document.rootVisualElement.Focus();
        document.rootVisualElement.Q<Button>("ButtonStart").Focus();

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
