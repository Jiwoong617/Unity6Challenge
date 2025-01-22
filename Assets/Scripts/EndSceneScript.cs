using UnityEngine;
using UnityEngine.Playables;

public class EndSceneScript : MonoBehaviour
{
    bool isLoading = true;
    [SerializeField] PlayableDirector timeline;

    void Start()
    {
        Invoke(nameof(StartTimeLine), 1f);
        Invoke(nameof(TimelineFin), (float)timeline.duration);
        isLoading = true;
    }

    private void Update()
    {
        ReturnStartScene();
    }

    private void StartTimeLine()
    {
        timeline.Play();
    }

    private void ReturnStartScene()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!isLoading)
            {
                SceneLoader.instance.LoadScene(SceneType.StartScene);
                isLoading = true;
            }
        }
    }

    private void TimelineFin() => isLoading = false;
}
