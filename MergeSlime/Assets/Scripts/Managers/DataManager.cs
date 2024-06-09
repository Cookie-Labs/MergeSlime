using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using System.Linq;

public class DataManager : Singleton<DataManager>
{
    [Title("���� ����")]
    public Coin coin;
    public Luck luck;
    public int spawnPrice;
    public string lastConnect;
    public Upgrade[] upgrades;

    [Title("���̵� ����", "���̵� ���� ���� ����")]
    public int MAX_MINING_CYCLE; // �ִ� ���̴� ��ٿ�(���� 1�� ��)
    public float LEVEL_PER_MINING_REDUCE_COOLDOWN; // ���� �� ��ٿ� ���ҷ�
    public float LEVEL_PER_MINING_INCREASE_AMOUNT; // ���� �� ���̴� ������
    public float SPECIAL_MINING_COOLDOWN_INCEASE_AMOUNT; // ����� ������ ��ٿ� ���ҷ��� ������� (~�� / ex. 2��, 2.5�� ��)
    public int MAX_CONNECT_MINUTES; // �ִ� ���� ���� �ð�
    public float MAX_LUCK; // �ִ� �� (0.7 -> 70%)

    [Title("�ߺ� ����", "���� �� �ڵ� ó��")]
    [ReadOnly] public int SLIME_LENGTH;
    [ReadOnly] public int SLIME_S_LENGTH;

    [Title("��Ÿ ����")]
    public float SLIME_SCALE;
    public int DEFAULT_COIN; public int DEFAULT_SPAWNPRICE;

    [Title("������ ����")]
    public SlimeData[] slimeDatas;
    public SlimeData[] slimeDatas_S;

    protected override void Awake()
    {
        base.Awake();

        ES3Manager.Instance.LoadAll();

        SetData();
    }

    private void SetData()
    {
        SLIME_LENGTH = slimeDatas.Length;
        SLIME_S_LENGTH = slimeDatas_S.Length;

        ConnectReward();
        luck.UpdateLevel();
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

    public SlimeData Find_SlimeData(int id)
    {
        SlimeData slimeData = Array.Find(slimeDatas_S, slime => slime.ID == id);
        if (slimeData != null)
        {
            return slimeData;
        }
        return Array.Find(slimeDatas, slime => slime.ID == id);
    }
    public ref SlimeData Find_SlimeData_Ref(int id)
    {
        int index = Array.FindIndex(slimeDatas_S, slime => slime.ID == id);
        if (index != -1)
        {
            return ref slimeDatas_S[index];
        }

        index = Array.FindIndex(slimeDatas, slime => slime.ID == id);
        if (index != -1)
        {
            return ref slimeDatas[index];
        }

        throw new KeyNotFoundException($"SlimeData with ID {id} not found.");
    }
    public SlimeData Find_SlimeData_level(int level, bool isSpecial)
    {
        if (isSpecial)
            return Array.Find(slimeDatas_S, slime => slime.level == level);
        else
            return Array.Find(slimeDatas, slime => slime.level == level);
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

    [Title("���̵� ����")]
    public int cost;
    public int amount;
    public int costIncrease;
    public int amountIncrease;

    public void UpLevel()
    {
        level++;
        SetCost();
        SetAmount();
        ES3Manager.Instance.Save(SaveType.Upgrades);
    }

    public void SetCost()
    {
        cost += costIncrease * level;
    }
    public void SetAmount()
    {
        amount += amountIncrease * level;
    }

    public int NextAmountIncrease()
    {
        return amountIncrease * (level + 1);
    }
}

[Serializable]
public struct Luck
{
    public int level; // ã�� ������ ��

    public void UpdateLevel()
    {
        level = 0;

        DataManager dataManager = DataManager.Instance;

        foreach (SlimeData data in dataManager.slimeDatas)
            if (data.isCollect)
                level++;
        foreach (SlimeData data in dataManager.slimeDatas_S)
            if (data.isCollect)
                level++;
    }

    public float GetAmount()
    {
        DataManager dataManager = DataManager.Instance;
        return Mathf.Floor(dataManager.MAX_LUCK / (dataManager.SLIME_LENGTH + dataManager.SLIME_S_LENGTH) * level * 100f) / 100f;
    }
}

[Serializable]
public class SlimeData
{
    [Title("���� ����", "ID ���� ���� ����")]
    public int ID;
    public int level;
    public string name;
    public string explain;
    [ReadOnly] public bool isSpecial;
    public Color color;
    public SlimeSprite sprite;

    [Title("���� ����")]
    public bool isCollect;
    public int spawnCount;

    public void FindCollect()
    {
        if (isCollect)
            return;

        UIManager uiManager = UIManager.Instance;

        isCollect = true;
        DataManager.Instance.luck.UpdateLevel();
        uiManager.collectionPannel.SetPannel_NewCollection(level, isSpecial);
        uiManager.collectionUI.UpdateUI(ID);
    }

    public void IncreaseSpawnCount()
    {
        spawnCount++;
    }
    public void DecreaseSpawnCount()
    {
        spawnCount--;
    }

    public int GetMiningAmount()
    {
        DataManager dataManager = DataManager.Instance;

        float baseAmount = isSpecial ? Mathf.Pow(level + dataManager.SLIME_LENGTH, dataManager.LEVEL_PER_MINING_INCREASE_AMOUNT) : 
            Mathf.Pow(level, dataManager.LEVEL_PER_MINING_INCREASE_AMOUNT);
        return Mathf.RoundToInt(baseAmount + baseAmount * dataManager.upgrades[1].amount / 100);
    }
    public float GetMiningCool()
    {
        DataManager dataManager = DataManager.Instance;

        float reduceAmount = isSpecial ? dataManager.LEVEL_PER_MINING_REDUCE_COOLDOWN * dataManager.SPECIAL_MINING_COOLDOWN_INCEASE_AMOUNT
            : dataManager.LEVEL_PER_MINING_REDUCE_COOLDOWN;

        return dataManager.MAX_MINING_CYCLE - reduceAmount * level;
    }
}

public enum State { Idle = 0, Pick, Merge }
public enum Face { Cute, Idle, Surprise }
public enum SaveType { Coin, SpawnPrice, LastConnect, Upgrades, SlimeData, SlimeData_S}