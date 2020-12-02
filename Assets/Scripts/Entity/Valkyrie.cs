using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Special controller of Valkyrie.
/// </summary>
public class Valkyrie : Speciality{

    int jumps;
    Image wingImage;
    public int state;
    [SerializeField]
    int ultMaxTime;
    int ultTimer;
    enum ultimateState { START, FLY, ATTACK, END}
    Vector3 upPosition, direction;

    protected override void initComponents()
    {
        base.initComponents();
        jumps = 3;
        wingImage = instanceUI.transform.GetChild(1).GetComponent<Image>();
        state = (int)ultimateState.START;
        ultTimer = ultMaxTime;
        updateChargesUI();
    }

    public override void ultimateUpdate()
    {
       if (state == (int)ultimateState.FLY) flyState();
    }

    private void flyState()
    {
        if (user is Player) {
            Player pl = (Player)user;
            if (Input.GetButtonDown(pl.controls.FinalAttack))
            {
                state = (int)ultimateState.ATTACK;
                pl.attacks.startAttack(Utilities.attackType.Final);              
            }
            if (pl.movementY < 0) pl.StartCoroutine(pl.ignorePlatforms(true));
            else if(pl.movementY >= 0) pl.StartCoroutine(pl.ignorePlatforms(false));
        }
        user.checkFacingDirection(); 
    }

    public void endCharge()
    {
        state = (int)ultimateState.FLY;
        StartCoroutine(ultimateTime());
    }

    private IEnumerator ultimateTime()
    {
        while(ultTimer > 0)
        {
            ultTimer--;
            yield return new WaitForSeconds(1);
        }
        user.RigidBody.velocity = new Vector2(0, 0);
        if (state == (int)ultimateState.ATTACK) stopUltimateAttack();
        state = (int)ultimateState.END;
        user.Animator.Play("Ultimate_End");
    }

    private void FixedUpdate()
    {
        if(state == (int)ultimateState.FLY)
            user.RigidBody.velocity = new Vector2(user.movementX * user.movementSpeed, user.movementY * user.movementSpeed);
    }

    public void stopUltimateAttack()
    {
        state = (int)ultimateState.FLY;
        user.attacks.attacks[(int)user.attacks.currentAttack].resetHittedEntities();
        Destroy(user.attackEffect);
    }

    public override void endUltimate()
    {
        base.endUltimate();
        StopAllCoroutines();
        user.RigidBody.gravityScale = user.gravityAmount;
        ultTimer = ultMaxTime;
        if (user.isGrounded) user.state = Utilities.state.Idle;
        else user.state = Utilities.state.Air;
        state = (int)ultimateState.START;
    }

    void updateChargesUI()
    {
        switch (jumps)
        {
            case 0: wingImage.fillAmount = 0; break;
            case 1: wingImage.fillAmount = 0.33f; break;
            case 2: wingImage.fillAmount = 0.66f; break;
            case 3: wingImage.fillAmount = 1; break;
        }
    }

    public override void onJump()
    {
        if (jumps > 0) jumps--;
        updateChargesUI();
    }
    public override void resetJump(int _jumps)
    {
        jumps = _jumps;
        updateChargesUI();
    }
}
