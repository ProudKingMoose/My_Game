using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;

[System.Serializable]
public class LevelSystem
{
    public int currentLV = 0;
    public int currentXP = 0;
    public int MaxXP;
    public int MaxLV = 99;
    public Action OnLevelUp;
    public LevelSystem(Action onLevelUp)
    {
        MaxXP = GetXPForLevel(MaxLV);
        OnLevelUp = onLevelUp;
    }

    public int GetXPForLevel(int level)
    {
        if (level > MaxLV)
            return 0;
        int firstPass = 0;
        int secondPass = 0;
        for (int i = 1; i < level; i++)
        {
            firstPass += (int)Math.Floor(i + (300.0f * Math.Pow(2.0, i / 7.0f)));
            secondPass = firstPass / 4;
        }
        if (secondPass > MaxXP && MaxXP != 0)
            return MaxXP;
        if (secondPass < 0)
            return MaxXP;

        return secondPass;
    }

    public int GetLevelForXP(int XP)
    {
        if (XP > MaxXP)
            return MaxXP;

        int firstPass = 0;
        int secondPass = 0;
        for (int i = 1; i < MaxLV; i++)
        {
            firstPass += (int)Math.Floor(i + (300.0f * Math.Pow(2.0, i / 7.0f)));
            secondPass = firstPass / 4;
            if (secondPass > XP)
                return i;
        }
        if (secondPass < XP)
            return MaxLV;

        return 0;
    }

    public bool AddXP(int amountOfXP)
    {
        Debug.Log("Activated");
        if (amountOfXP + currentXP < 0 || currentXP > MaxXP)
        {
            if (currentXP > MaxXP)
                currentXP = MaxXP;
            return false;
        }
        int oldLV = GetLevelForXP(currentXP);
        currentXP += amountOfXP;
        Debug.Log(currentXP);
        if (oldLV < GetLevelForXP(currentXP))
        {
            if (currentLV < GetLevelForXP(currentXP))
            {
                currentLV = GetLevelForXP(currentXP);
                if (OnLevelUp != null)
                    OnLevelUp.Invoke();
                return true;
            }
        }
        return false;
    }

}
