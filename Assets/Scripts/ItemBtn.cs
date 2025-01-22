using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemBtn : MonoBehaviour
{
    [SerializeField] Image img;
    [SerializeField] TextMeshProUGUI itemName;
    [SerializeField] TextMeshProUGUI itemFunc;
    [SerializeField] TextMeshProUGUI itemDes;
    [SerializeField] ItemType itemType;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnBtnClick);
    }

    public void Init(Item itemData)
    {
        img.sprite = itemData.itemSprite;
        itemName.text = itemData.itemName;
        itemFunc.text = itemData.itemFunc;
        itemDes.text = itemData.itemDescription;
        this.itemType = itemData.itemType;
    }

    private void OnBtnClick()
    {
        switch(itemType)
        {
            case ItemType.Hp:
                GameManager.instance.player.Hp = GameManager.instance.player.MaxHp;
                break;
            case ItemType.Speed:
                GameManager.instance.player.speed *= 1.3f;
                break;
            case ItemType.Jump:
                GameManager.instance.player.jumpForce *= 1.2f;
                break;
            case ItemType.Parry:
                GameManager.instance.player.ParryCooltime -= 1f;
                break;
            case ItemType.Dodge:
                GameManager.instance.player.dodgeDistance += 1f;
                break;
        }

        GameManager.instance.isSelectingFinish = true;
        GetComponentInParent<Animator>().SetTrigger("Close");
    }
}
