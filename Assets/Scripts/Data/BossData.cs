using UnityEngine;

[CreateAssetMenu(fileName = "BossData", menuName = "Scriptable Objects/BossData")]
public class BossData : ScriptableObject
{
    public string BossName;
    public int BossHp;
    public float BossSpeed;
    public Sprite BossSprite;

    public EnemyBase BossObj;
}
