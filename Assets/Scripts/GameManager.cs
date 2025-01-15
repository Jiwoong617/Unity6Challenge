using System.Collections;
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
    [SerializeField] TextMeshProUGUI BossName;
    [SerializeField] Image BossSprite;
    public bool isCutScenePlaying = false;

    [Header("UI")]
    [SerializeField] Image PlayerHpBar;
    [SerializeField] Image BossHpBar;

    private void Awake()
    {
        if (instance == null) instance = this;

        round = 2;
    }

    private void Start()
    {
        RoundStart();
    }
    public void RoundStart()
    {
        //보스 생성
        nowBoss = Instantiate(bossData[round].BossObj, spawnPos, Quaternion.identity);
        nowBoss.Init(bossData[round].BossHp, bossData[round].BossSpeed);
        //컷씬 및 UI
        StartCoroutine(CutSceneCo());
        //게임 시작
    }

    public void RoundEnd()
    {
        round++;
    }

    public IEnumerator CutSceneCo()
    {
        isCutScenePlaying = true;
        CutSceneUI.SetActive(true);
        BossName.text = bossData[round].BossName;
        BossSprite.sprite = bossData[round].BossSprite;

        yield return new WaitForSeconds(2f);

        isCutScenePlaying = false;
        CutSceneUI.SetActive(false);
    }

    private void CameraPosOnCutScene()
    {

    }
}
