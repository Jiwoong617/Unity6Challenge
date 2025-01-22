using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Player player;
    public EnemyBase nowBoss;
    [SerializeField] BossData[] bossData = new BossData[3];
    [SerializeField] private int round = 0;

    private Vector3 spawnPos = new Vector3(9, -2.67f, 0);

    [Header("CutScene")]
    [SerializeField] GameObject CutSceneUI;
    [SerializeField] TextMeshProUGUI CustSceneBossName;
    [SerializeField] Image BossSprite;
    public bool isCutScenePlaying = false;

    [Header("Item UI")]
    [SerializeField] ItemManager itemManager;
    public bool isSelectingFinish = false;

    [Header("Hp UI")]
    [SerializeField] Sprite HpOn, HpOff;
    [SerializeField] Image[] PlayerHpImg;
    [SerializeField] Image[] PlayerEnergyImg;
    [SerializeField] Image BossHpBar;
    [SerializeField] TextMeshProUGUI BossNameText;

    [Header("GameOver UI")]
    [SerializeField] GameObject GameFailedUI;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        Invoke(nameof(RoundStart), 2.5f);
    }

    private void RoundStart()
    {
        GameInitProcess();

        //보스 생성
        nowBoss = Instantiate(bossData[round].BossObj, spawnPos, Quaternion.identity);
        nowBoss.Init(bossData[round].BossHp, bossData[round].BossSpeed);

        //컷씬 및 UI
        StartCoroutine(CutSceneCo());
    }

    public void OnRoundEnd()
    {
        List<Projectile> proj = FindObjectsByType<Projectile>(FindObjectsSortMode.None).ToList();
        while (proj.Count > 0)
        {
            Destroy(proj[0]);
            proj.RemoveAt(0);
        }

        StartCoroutine(RoundEnd());
    }

    private IEnumerator RoundEnd()
    {
        round++;
        if(round > 2)
        {
            //모든 스테이지 클리어
            OnGameClear();
            yield break;
        }

        isCutScenePlaying = true;
        itemManager.gameObject.SetActive(true);
        yield return new WaitUntil(() => isSelectingFinish);
        BossNameText.transform.parent.gameObject.SetActive(false);

        //화면 전환
        yield return StartCoroutine(SceneLoader.instance.Fade(true));
        yield return StartCoroutine(SceneLoader.instance.Fade(false));

        isSelectingFinish = false;
        isCutScenePlaying = false;
        RoundStart();
    }

    private void OnGameClear()
    {
        SceneLoader.instance.LoadScene(SceneType.EndScene);
    }

    public void OnGameFailed()
    {
        Destroy(nowBoss.gameObject);
        nowBoss = null;
        StartCoroutine(CameraZoom(false));
    }

    private IEnumerator CameraZoom(bool isClear)
    {
        float t = 0f;
        Vector3 originPos = Camera.main.transform.position;
        while(t < 1.5f)
        {
            t+= Time.deltaTime;
            Camera.main.transform.position = Vector3.Lerp(originPos, new Vector3(player.transform.position.x, player.transform.position.y, -10f), t / 1.5f);
            Camera.main.orthographicSize = Mathf.Lerp(5f, 2f, t / 1.5f);
            yield return null;
        }
        if (!isClear) GameFailedUI.SetActive(true);
    }

    private void GameInitProcess()
    {
        isCutScenePlaying = false;
        Camera.main.transform.position = new Vector3(0, 0, -10f);
        Camera.main.orthographicSize = 5f;
        player.transform.position = new Vector3(-7f, -2.8f, 0);
        BossNameText.transform.parent.gameObject.SetActive(false);
    }

    private IEnumerator CutSceneCo()
    {
        isCutScenePlaying = true;
        CutSceneUI.SetActive(true);
        CustSceneBossName.text = bossData[round].BossName;
        BossSprite.sprite = bossData[round].BossSprite;

        yield return new WaitForSeconds(2f);

        BossNameText.transform.parent.gameObject.SetActive(true);
        BossNameText.text = bossData[round].BossName;
        ChangeBossHpUI(bossData[round].BossHp, bossData[round].BossHp);

        isCutScenePlaying = false;
        CutSceneUI.SetActive(false);
    }

    #region Hp UI

    public void ChangeBossHpUI(int val, int maxVal)
    {
        if (val <= 0)
        {
            BossHpBar.fillAmount = 0f;
            return;
        }

        BossHpBar.fillAmount = (float)val / (float)maxVal;
    }

    public void ChangePlayerHpUI(int hp)
    {
        for(int i = 0;i < PlayerHpImg.Length; i++)
        {
            if (i < hp) PlayerHpImg[i].sprite = HpOn;
            else PlayerHpImg[i].sprite = HpOff;
        }
    }

    public void ChangePlayerEnergyUI(int energy)
    {
        for (int i = 0; i < PlayerEnergyImg.Length; i++)
        {
            if (i < energy) PlayerEnergyImg[i].color = Color.white;
            else PlayerEnergyImg[i].color = Color.black;
        }
    }
    #endregion

    #region GameOverUI
    public void ResetRoundBtn()
    {
        StartCoroutine(ResetRoundCo());
    }

    IEnumerator ResetRoundCo()
    {
        GameFailedUI.SetActive(false);
        yield return StartCoroutine(SceneLoader.instance.Fade(true));
        GameInitProcess();
        player.Init();
        yield return StartCoroutine(SceneLoader.instance.Fade(false));

        round = 0;
        RoundStart();
    }

    public void BackToMainBtn()
    {
        GameFailedUI.SetActive(false);
        SceneLoader.instance.LoadScene(SceneType.StartScene);
    }
    #endregion
}
