using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : AICharacterStates
{
    private readonly CharacterAI enemyAI;
    public AttackState(CharacterAI characterAI)
    {
        enemyAI = characterAI;
    }
    public void detectDistance()
    {
        float distanceSqr = (enemyAI.my.transform.position - enemyAI.target.transform.position).sqrMagnitude;
        //Si la distancia entre el target y la IA es superior a 10 y no esta atacando o siendo golpeada pasara al estado follow
        if (distanceSqr > 10 + enemyAI.augmentArea/*variable de la IA Boss para augmentar su rango*/  && enemyAI.state != Utilities.state.Attacking && enemyAI.state != Utilities.state.Hit)
        {
            enemyAI.currentState = enemyAI.followState;
        }
        if (distanceSqr < 10 + enemyAI.augmentArea && enemyAI.state != Utilities.state.Attacking && enemyAI.state != Utilities.state.Hit) {
            if (enemyAI.target.state == Utilities.state.Attacking)
            {
                int num = enemyAI.RandomNum();
                if (num < 40)
                {
                    if (enemyAI.augmentArea <= 0)
                    {
                        enemyAI.currentState = enemyAI.evadeState;
                    }
                }
            }
        }
    }

    public void UpdateState()
    {
        if (enemyAI.target == enemyAI.transform.gameObject.GetComponent<Entity>()) { }
        else
        {
            Attack();
            if (enemyAI.isGrounded)
            {
                enemyAI.totalJumps = enemyAI.maxJumps;
            }
        }
    }
    void Attack()
    {
        detectDistance();
        if (enemyAI.state != Utilities.state.Attacking && enemyAI.state != Utilities.state.Hit)
        {
            if (enemyAI.my.transform.position.x <= enemyAI.location.x - 1)
            {
                enemyAI.movementX = 0.6f;
            }
            if (enemyAI.my.transform.position.x >= enemyAI.location.x + 1)
            {
                enemyAI.movementX = -0.6f;
            }
            if (enemyAI.my.isGrounded || enemyAI.my.isOnPlatform)
            {
                //IA puede atacar y el target esta en angulo que le permiten atacar a su lado realiza estos ataques
                if ( enemyAI.attack &&  enemyAI.GetComponent<Rigidbody2D>().velocity.x <= 3.5f && enemyAI.direction.y < 0.4+enemyAI.augmentAngl && enemyAI.direction.y > -0.4 - enemyAI.augmentAngl)
                {
                    enemyAI.state = Utilities.state.Attacking;
                    int num = enemyAI.RandomNum();
                    if (num >= 60)
                    {
                        enemyAI.my.attacks.startAttack(Utilities.attackType.BasicFloorSide);
                    }
                    else if (num >= 30 && num < 60)
                    {
                        enemyAI.my.attacks.startAttack(Utilities.attackType.SpecialSide);
                    }
                    else if (num >= 0 && num < 30)
                    {
                        enemyAI.my.attacks.startAttack(Utilities.attackType.SpecialDown);
                    }

                     enemyAI.attack = false;
                    enemyAI.startDelay();
                }
                //Si el angulo esta por encima de ella utilizara los ataques hacia arriba
                else if ( enemyAI.attack &&  enemyAI.direction.y > 0.3 && enemyAI.direction.x < 0.4 + enemyAI.augmentAngl && enemyAI.direction.x > -0.4 - enemyAI.augmentAngl)
                {
                    enemyAI.state = Utilities.state.Attacking;
                    int num = enemyAI.RandomNum();
                    if (num >= 40)
                    {
                        enemyAI.my.attacks.startAttack(Utilities.attackType.BasicFloorUp);
                    }
                    else if (num >= 0 && num < 40)
                    {
                        enemyAI.my.attacks.startAttack(Utilities.attackType.SpecialUp);
                    }
                    
                     enemyAI.attack = false;
                    enemyAI.startDelay();
                }

            }
            else
            {
                //Esta en el aire y el target esta a su lado utiliza estos ataques
                if ( enemyAI.attack &&  enemyAI.direction.y < 0.4 && enemyAI.direction.y > -0.4 - enemyAI.augmentAngl)
                {
                    enemyAI.state = Utilities.state.Attacking;
                    int num = enemyAI.RandomNum();
                    if (num >= 50)
                    {
                        enemyAI.my.attacks.startAttack(Utilities.attackType.BasicAirSide);
                    }
                    else if (num >= 25 && num < 50)
                    {
                        enemyAI.my.attacks.startAttack(Utilities.attackType.SpecialSide);
                    }
                    else if (num >= 0 && num < 25)
                    {
                        enemyAI.my.attacks.startAttack(Utilities.attackType.SpecialDown);
                    }
                    
                     enemyAI.attack = false;
                    enemyAI.startDelay();
                }
                //esta en el aire y por encima de ella 
                else if ( enemyAI.attack &&  enemyAI.direction.y > 0.3 && enemyAI.direction.x < 0.4 + enemyAI.augmentAngl && enemyAI.direction.x > -0.4 - enemyAI.augmentAngl)
                {
                    enemyAI.state = Utilities.state.Attacking;
                    int num = enemyAI.RandomNum();
                    if (num >= 30)
                    {
                        enemyAI.my.attacks.startAttack(Utilities.attackType.BasicAirUp);
                    }
                    else if (num >= 0 && num < 30)
                    {
                        enemyAI.my.attacks.startAttack(Utilities.attackType.SpecialUp);
                    }
                    
                     enemyAI.attack = false;
                    enemyAI.startDelay();
                }
                //Esta por debajo de ella
                else if (enemyAI.attack)
                {
                    enemyAI.state = Utilities.state.Attacking;
                    enemyAI.my.attacks.startAttack(Utilities.attackType.BasicAirDown);
                    
                     enemyAI.attack = false;
                    enemyAI.startDelay();
                }
            }
            if (enemyAI.immune) enemyAI.becomeHittable();
        }
        enemyAI.checkFacingDirection();
    }
}
