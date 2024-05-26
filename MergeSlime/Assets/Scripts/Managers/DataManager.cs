using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

public class DataManager : Singleton<DataManager>
{
    [Title("���� ����")]
    public Coin coin;

    [Title("��������Ʈ")]
    public SlimeSprite[] slimeSprites;
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
    public int coin;

    public void GainCoin(int amount)
    {
        coin += amount;
        UIManager.Instance.moneyUI.SetMoney(coin);
    }

    public bool LoseCoin(int amount)
    {
        if (coin < amount)
            return false;

        coin -= amount;
        UIManager.Instance.moneyUI.SetMoney(coin);
        return true;
    }
}

public enum State { Idle = 0, Pick, Merge }