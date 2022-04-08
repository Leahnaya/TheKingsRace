using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KingAbility
{

    public int CooldownTimer;
    private int CooldownLength;
    private int FramestoSeconds = 0;
    private float currenttime = 0f;
    private bool Avaliable = true;
    private GameObject Button;

    private int EnergyCost;

    public KingAbility() {
        CooldownLength = 3;
        CooldownTimer = CooldownLength;
        EnergyCost = 20;
    }

    public KingAbility(int cooldown, int energy) {
        CooldownLength = cooldown;
        CooldownTimer = CooldownLength;
        EnergyCost = energy;
    }

    public void Cooldown() {
        if (Avaliable == false) {
            FramestoSeconds++;
            if (FramestoSeconds == 50) {
                FramestoSeconds = 0;
                CooldownTimer--;
                if (CooldownTimer == 0) {
                    CooldownTimer = CooldownLength;
                    Avaliable = true;
                }
            }
            if (Avaliable == false) {
                Button.GetComponent<Image>().fillAmount = currenttime / CooldownLength * 1.0f;
                currenttime += Time.deltaTime;
            }
            else {
                currenttime = 0;
            }
        }
    }

    public void KingButton(GameObject button) {
        Button = button;
    }

    public void UseItem() {
        Avaliable = false;
    }

    public bool IsAvaliable() {
        return Avaliable;
    }

    public int Energy() {
        return EnergyCost;
    }

}
