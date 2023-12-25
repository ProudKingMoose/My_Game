using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroTeamButton : MonoBehaviour
{
    public GameObject theButton;
    public string HeroName;

    public void ActivateDisactivateHero()
    {
        GameObject.Find("Canvas").GetComponent<PauseMenu>().AddRemoveHero(theButton, HeroName);
    }
    public void ActivateDisactivateHeroDisplay()
    {
        GameObject.Find("Canvas").GetComponent<PauseMenu>().HoverDisplayHeroes(HeroName);
    }
    public void ActivateDisactivateStatView()
    {
        GameObject.Find("Canvas").GetComponent<PauseMenu>().HoverDisplayHeroStats(HeroName);
    }
    public void CurrentHerogear()
    {
        GameObject.Find("Canvas").GetComponent<PauseMenu>().HeroCurrentGear(HeroName);
    }
}
