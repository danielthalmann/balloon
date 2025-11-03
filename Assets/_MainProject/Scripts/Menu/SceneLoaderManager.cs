using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoaderManager : MonoBehaviour
{

    public Image fadeImage;
    public Color fadeColor;
    public float duration;

    public static SceneLoaderManager instance;

    string currentSceneName = null;

    public delegate void OnStartLoading();
    public static OnStartLoading onStartLoading;

    public delegate void OnFinishLoading();
    public static OnFinishLoading onFinishLoading;

    public delegate void OnCompletedLoading();
    public static OnCompletedLoading onCompletedLoading;

    private AsyncOperation async;

    public static SceneLoaderManager getInstance()
    {
        if (instance == null)
        {
            instance = FindAnyObjectByType<SceneLoaderManager>();
        }

        return instance;
    }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one Scene Loader Manager in the scene.");
        }

        instance = this;

    }

    public void LoadScene(string sceneName)
    {
        currentSceneName = sceneName;
        onStartLoading?.Invoke();
        StartCoroutine(FadeLoadingScreen(2, true));
    }

    // Start is called before the first frame update
    void Start()
    {

        if(fadeImage == null)
        {
            Canvas canvas;
            //Canvas canvas = FindAnyObjectByType<Canvas>();
            //if (!canvas)
            //{
            //Debug.LogWarning("Fail load cursor. The scene contain nothing canvas");
                GameObject gameObjectCanvas = new GameObject();
                canvas = gameObjectCanvas.AddComponent<Canvas>();
                gameObjectCanvas.AddComponent<CanvasScaler>();
                gameObjectCanvas.name = "Canvas";

                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.worldCamera = Camera.main;
                canvas.sortingOrder = 100;
            //}
            GameObject gameObject = new GameObject();

            fadeImage = gameObject.AddComponent<Image>();
            gameObject.name = "Splash screen";
            gameObject.transform.SetParent(canvas.transform);
            AdjustPanelToScreen(gameObject, 0);
        }


        onStartLoading?.Invoke();
        StartCoroutine(FadeLoadingScreen(2));
    }

    IEnumerator FadeLoadingScreen(float duration, bool fadeOut = false)
    {
        Color currentColor = fadeColor;
        float startValue = currentColor.a;
        float time = 0;
        fadeImage.gameObject.SetActive(true);

        while (time < duration)
        {
            if (fadeOut)
                currentColor.a = Mathf.Lerp(startValue, 1, time / duration);
            else
                currentColor.a = 1f - Mathf.Lerp(startValue, 1, time / duration);

            time += Time.deltaTime;
            fadeImage.color = currentColor;

            yield return null;
        }
        
        if (fadeOut)
        {
            onFinishLoading?.Invoke();
            async = SceneManager.LoadSceneAsync(currentSceneName);
            async.completed += Completed;
        } else
        {
            onFinishLoading?.Invoke();
            fadeImage.gameObject.SetActive(false);
        }
    }

    private void Completed(AsyncOperation async)
    {
        onCompletedLoading?.Invoke();
    }

    public string GetCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

    // Adjust the panel to fill the screen, allowing for optional padding
    private void AdjustPanelToScreen(GameObject panel, float paddingPercentage = 0)
    {
        RectTransform panelRect = panel.GetComponent<RectTransform>();

        if (panelRect != null)
        {
            // Calculate screen size considering padding
            float screenWidth = Screen.width * (1f - paddingPercentage);
            float screenHeight = Screen.height * (1f - paddingPercentage);

            // Apply screen size to the panel's rect transform
            panelRect.sizeDelta = new Vector2(screenWidth, screenHeight);

            Vector2 halfScreen = new Vector2(Screen.width / 2, Screen.height / 2);
            panelRect.anchoredPosition = new Vector2(0, 0);
            panelRect.position = halfScreen;

        }
        else
        {
            Debug.LogError("Panel's RectTransform is missing.");
        }
    }

}
