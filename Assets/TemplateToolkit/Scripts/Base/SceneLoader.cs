using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    [Header("UI Load Settings")]
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private UnityEngine.UI.Slider loadingBar;
    [SerializeField] private TMPro.TextMeshProUGUI loadingText;

    [Header("Params")]
    [SerializeField] private float minLoadTime = 0.5f;

    private bool isLoading = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(string sceneName)
    {
        if (!isLoading)
        {
            StartCoroutine(LoadSceneAsync(sceneName));
        }
    }

    public void LoadScene(int sceneIndex)
    {
        if (!isLoading)
        {
            StartCoroutine(LoadSceneAsync(sceneIndex));
        }
    }

    public void ReloadCurrentScene()
    {
        LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadNextScene()
    {
        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            LoadScene(nextIndex);
        }
        else
        {
            Debug.LogWarning("Следующей сцены не существует!");
        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        isLoading = true;
        float startTime = Time.time;

        if (loadingScreen != null)
            loadingScreen.SetActive(true);
        if (loadingBar != null)
            loadingBar.gameObject.SetActive(true);
        if (loadingText != null)
            loadingText.gameObject.SetActive(true);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            UpdateLoadingUI(progress);

            if (operation.progress >= 0.9f && Time.time - startTime >= minLoadTime)
            {
                operation.allowSceneActivation = true;
            }

            yield return null;
        }

        if (loadingScreen != null)
            loadingScreen.SetActive(false);
        if (loadingBar != null)
            loadingBar.gameObject.SetActive(false);
        if (loadingText != null)
            loadingText.gameObject.SetActive(false);

        isLoading = false;
    }

    private IEnumerator LoadSceneAsync(int sceneIndex)
    {
        isLoading = true;
        float startTime = Time.time;

        if (loadingScreen != null)
            loadingScreen.SetActive(true);
        if (loadingBar != null)
            loadingBar.gameObject.SetActive(true);
        if (loadingText != null)
            loadingText.gameObject.SetActive(true);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            UpdateLoadingUI(progress);

            if (operation.progress >= 0.9f && Time.time - startTime >= minLoadTime)
            {
                operation.allowSceneActivation = true;
            }

            yield return null;
        }

        if (loadingScreen != null)
            loadingScreen.SetActive(false);
        if (loadingBar != null)
            loadingBar.gameObject.SetActive(false);
        if (loadingText != null)
            loadingText.gameObject.SetActive(false);

        isLoading = false;
    }

    private void UpdateLoadingUI(float progress)
    {
        if (loadingBar != null)
            loadingBar.value = progress;

        if (loadingText != null)
            loadingText.text = $"Загрузка... {Mathf.RoundToInt(progress * 100)}%";
    }
}

public class SceneLoadButton : MonoBehaviour
{
    [SerializeField] private string sceneName;
    [SerializeField] private int sceneIndex = -1;

    public void LoadScene()
    {
        if (SceneLoader.Instance != null)
        {
            if (sceneIndex >= 0)
                SceneLoader.Instance.LoadScene(sceneIndex);
            else if (!string.IsNullOrEmpty(sceneName))
                SceneLoader.Instance.LoadScene(sceneName);
        }
    }
}