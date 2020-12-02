using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvadeState : AICharacterStates
{
    private readonly CharacterAI enemyAI;
    int direction = 0;
    public EvadeState(CharacterAI characterAI)
    {
        enemyAI = characterAI;
    }
    public void detectDistance() {
        float distanceSqr = (enemyAI.my.transform.position - enemyAI.target.transform.position).sqrMagnitude;
        if (distanceSqr > 10 && enemyAI.state != Utilities.state.Hit)
        {
            enemyAI.currentState = enemyAI.followState;
        }
        if (distanceSqr < 10 && enemyAI.state != Utilities.state.Dodging && enemyAI.state != Utilities.state.Blocking)
        {
            if (enemyAI.target.state != Utilities.state.Attacking)
            {
                    enemyAI.currentState = enemyAI.attackState;
            }
        }
    }

    public void UpdateState()
    {
        Evade();
        if (enemyAI.isGrounded)
        {
            enemyAI.totalJumps = enemyAI.maxJumps;
        }
    }
    void Evade()
    {
        detectDistance();
        if (enemyAI.state != Utilities.state.Dodging && enemyAI.state != Utilities.state.Blocking)
        {
            if (enemyAI.my.transform.position.x <= enemyAI.location.x - 1)
            {
                enemyAI.movementX = 0.6f;
            }
            if (enemyAI.my.transform.position.x >= enemyAI.location.x + 1)
            {
                enemyAI.movementX = -0.6f;
            }
            //En caso que la ataquen desde al lado, cumpla el random y este en movimiento la IA evadera el ataque
            if (enemyAI.dodge && enemyAI.GetComponent<Rigidbody2D>().velocity.x >= 3.5f && enemyAI.direction.y < 0.3 && enemyAI.direction.y > -0.3)
            {
                int num = enemyAI.RandomNum();
                if (num >= 50)
                {
                    if (direction == 1)
                    {
                        enemyAI.my.dodge(Utilities.direction.Right);
                        enemyAI.state = Utilities.state.Dodging;
                    }
                    if (direction == 2) { enemyAI.my.dodge(Utilities.direction.Left); enemyAI.state = Utilities.state.Dodging; }
                    enemyAI.dodge = false;
                    enemyAI.startDelay();
                }
            }
            //En caso de que este quieta y sobrepase el random lo bloqueara
            else if (enemyAI.dodge && enemyAI.GetComponent<Rigidbody2D>().velocity.x <= 3.5f && enemyAI.direction.y < 0.3 && enemyAI.direction.y > -0.3)
            {
                int num = enemyAI.RandomNum();
                if (num >= 50)
                {
                    enemyAI.state = Utilities.state.Blocking;
                    enemyAI.my.spotDodge();
                    enemyAI.dodge = false;
                    enemyAI.startblockDelay();
                }
            }
        }
        enemyAI.checkFacingDirection();
    }
}
