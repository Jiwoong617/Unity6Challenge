using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader instance;

    [SerializeField]
    private Image FadeUI;
    private SceneType sceneType;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    public void LoadScene(SceneType scene)
    {
        sceneType = scene;
        SceneManager.sceneLoaded += LoadSceneEnd;
        StartCoroutine(Load(scene));

        Global.IsPlayerMoveRight = null;
    }

    private IEnumerator Load(SceneType scene)
    {
        yield return StartCoroutine(Fade(true));

        AsyncOperation op = SceneManager.LoadSceneAsync((int)scene);
        op.allowSceneActivation = true;
    }

    private void LoadSceneEnd(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.buildIndex == (int)sceneType)
        {
            StartCoroutine(Fade(false));
            SceneManager.sceneLoaded -= LoadSceneEnd;
        }
    }

    public IEnumerator Fade(bool isFade)
    {
        FadeUI.gameObject.SetActive(true);
        float timer = 0f;
        while (timer <= 2f)
        {
            yield return null;
            timer += Time.unscaledDeltaTime;
            FadeUI.color = Color.Lerp(isFade ? new Color(0, 0, 0, 0) : new Color(0, 0, 0, 1), isFade ? new Color(0, 0, 0, 1) : new Color(0, 0, 0, 0), timer);
        }

        if(!isFade)
            FadeUI.gameObject.SetActive(false);
    }
}
