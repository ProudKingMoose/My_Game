using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinBattlesQuestStep : QuestStep
{
    private int battlesWon = 0;
    private int battlesToWin = 1;

    private void OnEnable()
    {
        GameManager.instance.OnBattleWon += BattlesWon;
    }

    private void OnDisable()
    {
        GameManager.instance.OnBattleWon -= BattlesWon;
    }

    private void BattlesWon()
    {
        if (battlesWon < battlesToWin)
        {
            battlesWon++;
        }

        if (battlesWon >= battlesToWin)
        {
            FinishQuestStep();
        }
    }
}
