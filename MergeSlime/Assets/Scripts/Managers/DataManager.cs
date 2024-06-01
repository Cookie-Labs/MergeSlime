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
    public Upgrade[] upgradeLv;

    [Title("���̵� ����", "���̵� ���� ���� ����")]
    public int MINING_CYCLE;

    [Title("�ߺ� ����", "���� �� �ڵ� ó��")]
    [ReadOnly] public int SLIME_LENGTH;
    [ReadOnly] public int SLIME_S_LENGTH;

    [Title("��Ÿ ����")]
    public float SLIME_SCALE;

    [Title("��������Ʈ")]
    public SlimeSprite[] slimeSprites;
    public SlimeSprite[] specialSlimeSprites;

    protected override void Awake()
    {
        base.Awake();

        SetData();
    }

    private void SetData()
    {
        SLIME_LENGTH = slimeSprites.Length;
        SLIME_S_LENGTH = specialSlimeSprites.Length;
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

    public Sprite FindFace(Face face)
    {
        return faceSprites[(int)face];
    }
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

[Serializable]
public struct Upgrade
{
    public int level, levelLimit;
    public int cost;
    public float amount, amountIncrease;

    public void UpLevel()
    {
        DataManager dataManager = DataManager.Instance;

        if (level >= levelLimit || !dataManager.coin.LoseCoin(cost))
            return;

        level++;
        cost += Math.Max(1, 2 * level);
        SetAmount();
    }

    public void SetAmount()
    {
        amount = amountIncrease * level;
    }
}

public enum State { Idle = 0, Pick, Merge }
public enum Face { Cute, Idle, Surprise }