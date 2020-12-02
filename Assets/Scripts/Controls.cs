using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls {

    public string axisX, axisY, Jump, Defense, Items, BasicAttack, SpecialAttack, FinalAttack;
	
    public Controls() { }

    /// <summary>
    /// Loads controls depending on player.
    /// </summary>
    /// <param name="player"></param>
    public void loadControls(int player)
    {
        switch (player)
        {
            case 0: loadPlayer1Controls(); break;
            case 1: loadPlayer2Controls(); break;
            case 2: loadPlayer3Controls(); break;
            case 3: loadPlayer4Controls(); break;
        }
    }

    private void loadPlayer4Controls()
    {
        if (Input.GetJoystickNames().Length > 3)
        {
            axisX = "HorizontalP4";
            axisY = "VerticalP4";
            Jump = "JumpP4";
            Defense = "DefenseP4";
            Items = "ItemsP4";
            BasicAttack = "BasicAttackP4";
            SpecialAttack = "SpecialAttackP4";
            FinalAttack = "FinalAttackP4";
        }
        else
        {
            axisX = "Horizontal";
            axisY = "Vertical";
            Jump = "Jump";
            Defense = "Defense";
            Items = "Items";
            BasicAttack = "BasicAttack";
            SpecialAttack = "SpecialAttack";
            FinalAttack = "FinalAttack";
        }
    }

    private void loadPlayer3Controls()
    {
        if (Input.GetJoystickNames().Length > 2)
        {
            axisX = "HorizontalP3";
            axisY = "VerticalP3";
            Jump = "JumpP3";
            Defense = "DefenseP3";
            Items = "ItemsP3";
            BasicAttack = "BasicAttackP3";
            SpecialAttack = "SpecialAttackP3";
            FinalAttack = "FinalAttackP3";
        }
        else
        {
            axisX = "Horizontal";
            axisY = "Vertical";
            Jump = "Jump";
            Defense = "Defense";
            Items = "Items";
            BasicAttack = "BasicAttack";
            SpecialAttack = "SpecialAttack";
            FinalAttack = "FinalAttack";
        }
    }

    private void loadPlayer2Controls()
    {
        if (Input.GetJoystickNames().Length > 1)
        {
            axisX = "HorizontalP2";
            axisY = "VerticalP2";
            Jump = "JumpP2";
            Defense = "DefenseP2";
            Items = "ItemsP2";
            BasicAttack = "BasicAttackP2";
            SpecialAttack = "SpecialAttackP2";
            FinalAttack = "FinalAttackP2";
        }
        else
        {
            axisX = "Horizontal";
            axisY = "Vertical";
            Jump = "Jump";
            Defense = "Defense";
            Items = "Items";
            BasicAttack = "BasicAttack";
            SpecialAttack = "SpecialAttack";
            FinalAttack = "FinalAttack";
        }
    }

    private void loadPlayer1Controls()
    {
        if (Input.GetJoystickNames().Length > 0)
        {
            axisX = "HorizontalP1";
            axisY = "VerticalP1";
            Jump = "JumpP1";
            Defense = "DefenseP1";
            Items = "ItemsP1";
            BasicAttack = "BasicAttackP1";
            SpecialAttack = "SpecialAttackP1";
            FinalAttack = "FinalAttackP1";
        }
        else
        {
            axisX = "Horizontal";
            axisY = "Vertical";
            Jump = "Jump";
            Defense = "Defense";
            Items = "Items";
            BasicAttack = "BasicAttack";
            SpecialAttack = "SpecialAttack";
            FinalAttack = "FinalAttack";
        }
    }
}
