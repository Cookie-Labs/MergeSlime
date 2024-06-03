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
    public string lastConnect;
    public Upgrade[] upgrades;

    [Title("���̵� ����", "���̵� ���� ���� ����")]
    public int MINING_CYCLE;
    public int MAX_CONNECT_MINUTES;

    [Title("�ߺ� ����", "���� �� �ڵ� ó��")]
    [ReadOnly] public int SLIME_LENGTH;
    [ReadOnly] public int SLIME_S_LENGTH;

    [Title("��Ÿ ����")]
    public float SLIME_SCALE;
    public int DEFAULT_COIN; public int DEFAULT_SPAWNPRICE;

    [Title("��������Ʈ")]
    public SlimeSprite[] slimeSprites;
    public SlimeSprite[] specialSlimeSprites;

    protected override void Awake()
    {
        base.Awake();

        ES3Manager.Instance.LoadAll();

        SetData();
    }

    private void SetData()
    {
        SLIME_LENGTH = slimeSprites.Length;
        SLIME_S_LENGTH = specialSlimeSprites.Length;

        ConnectReward();
    }

    private void ConnectReward()
    {
        if (lastConnect == "")
        {
            SetLastConnect();
            return;
        }

        TimeSpan timespan = DateTime.Now - Convert.ToDateTime(lastConnect);

        double proportion = Math.Min(1.0, timespan.TotalMinutes / MAX_CONNECT_MINUTES);

        coin.GainCoin((int)(upgrades[2].amount * proportion));
        SetLastConnect();
    }

    private void SetLastConnect()
    {
        lastConnect = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        ES3Manager.Instance.Save(SaveType.LastConnect);
    }

    public void SetPrice()
    {
        int upPrice = Mathf.RoundToInt(spawnPrice * 1.2f);

        spawnPrice = spawnPrice != upPrice ? upPrice : spawnPrice + 1;
        UIManager.Instance.moneyUI.SetPrice(spawnPrice);
        ES3Manager.Instance.Save(SaveType.SpawnPrice);
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
        ES3Manager.Instance.Save(SaveType.Coin);
    }

    public bool LoseCoin(int _amount)
    {
        if (amount < _amount)
            return false;

        amount -= _amount;
        UIManager.Instance.moneyUI.SetMoney(amount);
        ES3Manager.Instance.Save(SaveType.Coin);
        return true;
    }
}

[Serializable]
public struct Upgrade
{
    [Title("���� ����")]
    public int level;
    public int cost;
    public int amount;

    [Title("���̵� ����")]
    public int levelLimit; // -1�̸� ����
    public int costIncrease;
    public int amountIncrease;

    public void UpLevel()
    {
        DataManager dataManager = DataManager.Instance;

        if ((level >= levelLimit && levelLimit != -1) || !dataManager.coin.LoseCoin(cost))
            return;

        level++;
        cost += Math.Max(1, costIncrease * level);
        SetAmount();
        ES3Manager.Instance.Save(SaveType.Upgrades);
    }

    public void SetAmount()
    {
        amount = amountIncrease * level;
    }
}

public enum State { Idle = 0, Pick, Merge }
public enum Face { Cute, Idle, Surprise }
public enum SaveType { Coin, SpawnPrice, LastConnect, Upgrades, SLIMES }