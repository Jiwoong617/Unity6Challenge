using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{
    [SerializeField] Item[] itemData;
    [SerializeField] ItemBtn[] items;

    void OnEnable()
    {
        SetRandItem();
    }

    private void SetRandItem()
    {
        int n = 3;
        List<int> randList = new List<int>();
        for(int i = 0; i<n;)
        {
            int rand = Random.Range(0, itemData.Length);
            if(!randList.Contains(rand))
            {
                i++;
                randList.Add(rand);
            }
        }

        for (int i = 0; i<n; i++)
            items[i].Init(itemData[randList[i]]);
    }

    public void CloseAnim() => gameObject.SetActive(false);
}
