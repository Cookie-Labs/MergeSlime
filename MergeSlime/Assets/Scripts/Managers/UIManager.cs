using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;
using System;

public class UIManager : Singleton<UIManager>
{
    [Title("����", "��� ������ ����ϴ� UI")]
    public MoneyUI moneyUI;
    public GameObject raycastPannel;

    [Title("�ΰ���")]
    public RectTransform bgCanvas;

    private void Start()
    {
        CameraBound camBound = SpawnManager.Instance.camBound;
        bgCanvas.sizeDelta = new Vector2(camBound.Width, camBound.Height);

        SetUI();
    }

    private void SetUI()
    {
        moneyUI.SetUI();
    }
}

[Serializable]
public class MoneyUI
{
    public TextMeshProUGUI coinTxt;
    public TextMeshProUGUI priceTxt;

    public void SetUI()
    {
        DataManager dataManager = DataManager.Instance;

        SetMoney(dataManager.coin.amount);
        SetPrice(dataManager.spawnPrice);
    }

    public void SetMoney(int amount)
    {
        coinTxt.text = $"<sprite=0>{amount}";
    }

    public void SetPrice(int price)
    {
        priceTxt.text = $"<sprite=0>{price}";
    }
}
