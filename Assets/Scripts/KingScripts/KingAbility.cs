using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingAbility
{

    public int CooldownTimer;
    private int CooldownLength;
    private int FramestoSeconds = 0;
    private bool Avaliable = true;

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
        }
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
