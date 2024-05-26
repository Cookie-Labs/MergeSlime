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

    [Title("�ΰ���")]
    public RectTransform bgCanvas;

    private void Start()
    {
        CameraBound camBound = SpawnManager.Instance.camBound;
        bgCanvas.sizeDelta = new Vector2(camBound.Width, camBound.Height);
    }
}

[Serializable]
public class MoneyUI
{
    public TextMeshProUGUI text;
    public void SetMoney(int amount)
    {
        text.text = $"<sprite=0>{amount}";
    }
}
