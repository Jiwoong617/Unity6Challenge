using UnityEngine;

public class StartBtn : MonoBehaviour
{
    bool isLoad = false;
    private void Start()
    {
        isLoad = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(!isLoad)
        {
            SceneLoader.instance.LoadScene(SceneType.GameScene);
            isLoad =true;
        }
    }
}
