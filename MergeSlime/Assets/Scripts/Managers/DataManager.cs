using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

public class DataManager : Singleton<DataManager>
{
    [Title("���� ����")]
    public Coin coin;
    public int spawnPrice;

    [Title("���̵� ����", "���̵� ���� ���� ����")]
    public int MAX_MINING;

    [Title("�ߺ� ����", "�ߺ��Ǿ� ó���� �� �ִ� ������ ó����.")]
    public int SLIME_LENGTH;
    public float SLIME_SCALE;

    [Title("��������Ʈ")]
    public SlimeSprite[] slimeSprites;

    protected override void Awake()
    {
        base.Awake();

        SetData();
    }

    private void SetData()
    {
        SLIME_LENGTH = slimeSprites.Length;
    }

    public void SetPrice()
    {
        int upPrice = Mathf.RoundToInt(spawnPrice * 1.2f);

        spawnPrice = spawnPrice != upPrice ? upPrice : spawnPrice + 1;
        UIManager.Instance.moneyUI.SetPrice(spawnPrice);
    }
}

[Serializable]
public struct SlimeSprite
{
    public Sprite bodySprite;
    public Sprite[] faceSprites;
}

[Serializable]
public struct Coin
{
    public int amount;

    public void GainCoin(int _amount)
    {
        amount += _amount;
        UIManager.Instance.moneyUI.SetMoney(amount);
    }

    public bool LoseCoin(int _amount)
    {
        if (amount < _amount)
            return false;

        amount -= _amount;
        UIManager.Instance.moneyUI.SetMoney(amount);
        return true;
    }
}

public enum State { Idle = 0, Pick, Merge }