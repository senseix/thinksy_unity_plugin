using UnityEngine;
using System.Collections;

/// <summary>
/// HP & MP Manager
/// </summary>
public class HpManager : MonoBehaviour {
    // HP bar, MP bar
    public UISlider hpBar, mpBar;

    // Max State 
    public int hpMax = 100;
    public int mpMax = 100;

	// Current State
    int hp = 100;
    int mp = 100;

    // Init HP State
    public void InitHp()
    {
        SetHp(hpMax);
    }

    // Init MP State
    public void InitMp()
    {
        SetHp(mpMax);
    }

    // Set Damage on HP State
    public void DoDamageHp(int point)
    {
        SetHp(hp - point);
    }

    // Set Recover on HP State
    public void DoSaveHp(int point)
    {
        SetHp(hp + point);
    }

    // Set Recover on MP State
    public void DoSaveMp(int point)
    {
        SetMp(mp + point);
    }

    // Set HP State
    public void SetHp(int point)
    {
        hp = Mathf.Clamp(point, 0, hpMax);
        if (hpBar)
            hpBar.value = (float)hp / (float)hpMax;
    }

    // Set MP State
    public void SetMp(int point)
    {
        mp = Mathf.Clamp(point, 0, mpMax);
        if (mpBar)
            mpBar.value = (float)mp / (float)mpMax;
    }

}
