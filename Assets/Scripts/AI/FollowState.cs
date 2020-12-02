using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowState : AICharacterStates
{
    private readonly CharacterAI enemyAI;

    public FollowState(CharacterAI characterAI)
    {
        enemyAI = characterAI;
    }
    public void detectDistance()
    {
            float distanceSqr = (enemyAI.my.transform.position - enemyAI.target.transform.position).sqrMagnitude;
            //Si la distancia entre el target y la IA es inferior a 10 y no esta siendo golpeada si target no esta atacandola pasara al estado de ataque
            if (distanceSqr < 10 + enemyAI.augmentArea && enemyAI.state != Utilities.state.Hit)
            {
            //Si el target la esta atacando puede entrar en el estado de esquivar 
            if (enemyAI.target.state == Utilities.state.Attacking)
            {
                int num = enemyAI.RandomNum();
                if (num < 40)
                {
                    if(enemyAI.augmentArea <= 0)
                    enemyAI.currentState = enemyAI.evadeState;
                }
                else { enemyAI.currentState = enemyAI.attackState; }
            }
            else { enemyAI.currentState = enemyAI.attackState; }
            }
    }
    public void UpdateState()
    {
        if (enemyAI.target == enemyAI.transform.gameObject.GetComponent<Entity>()) { }
        else
        {
            Follow();
            if (enemyAI.isGrounded)
            {
                enemyAI.totalJumps = enemyAI.maxJumps;
            }
        }
    }
    void Follow()
    {
        enemyAI.checkFacingDirection();
        enemyAI.movementX = 0;
        if (enemyAI.state != Utilities.state.Attacking && enemyAI.state != Utilities.state.Hit)
        {
            //Si la posicion es inferior a el restpoint(punto en el centro del mapa) por la parte inferior y laterales
            if ((enemyAI.target.transform.position.y >= enemyAI.restpoint.transform.position.y - 7 - enemyAI.augmentArea) && (enemyAI.restpoint.transform.position.x <= enemyAI.target.transform.position.x + 26 && enemyAI.restpoint.transform.position.x >= enemyAI.target.transform.position.x - 26))
            {
                //Hace que la IA siempre este mirando en direccion al jugador(target) variando el valor de movementX
                if (enemyAI.my.transform.position.x <= enemyAI.location.x - 1)
                {
                    enemyAI.movementX = 0.6f;
                }
                if (enemyAI.my.transform.position.x >= enemyAI.location.x + 1)
                {
                    enemyAI.movementX = -0.6f;
                }
                //Comprobamos que la IA no se encuentre en estados en los que no deberia moverse
                if ((enemyAI.state == Utilities.state.Air && enemyAI.wallJumpTimer == 0f && enemyAI.state != Utilities.state.Hit) || enemyAI.state == Utilities.state.Moving || enemyAI.state == Utilities.state.Idle && enemyAI.state != Utilities.state.Hit)
                {
                    //Dependiendo de la direccion en la que mire el jugador iniciamos el movimiento hacia el target
                    if (Mathf.Abs(enemyAI.movementX) > 0.1f)
                    {
                        if (Mathf.Abs(enemyAI.movementX) > 0.5f)
                        {
                            if (enemyAI.movementX > 0) enemyAI.movementX = 1;
                            else enemyAI.movementX = -1;
                        }
                        else
                        {
                            if (enemyAI.movementX > 0) enemyAI.movementX = 0.5f;
                            else enemyAI.movementX = -0.5f;
                        }
                        enemyAI.my.GetComponent<Rigidbody2D>().velocity = new Vector2(enemyAI.movementX * enemyAI.movementSpeed, enemyAI.my.RigidBody.velocity.y);
                    }
                    else enemyAI.my.GetComponent<Rigidbody2D>().velocity = new Vector2(enemyAI.my.RigidBody.velocity.x * 0.9f, enemyAI.my.RigidBody.velocity.y);
                }
                else enemyAI.my.GetComponent<Rigidbody2D>().velocity = new Vector2(enemyAI.my.RigidBody.velocity.x * 0.9f, enemyAI.my.RigidBody.velocity.y);

                //Hacemos la comprovacion del numero de saltos restantes y si el target esta por encima nuestro procedemos a realizar el salto para alcanzarlo
                if (enemyAI.my.RigidBody.velocity.y <= 0 && enemyAI.my.totalJumps > 0)
                {
                    enemyAI.salto = true;
                }
                if ((enemyAI.my.transform.position.y <= enemyAI.location.y - 1 && enemyAI.salto) || (enemyAI.my.transform.position.y <= enemyAI.restpoint.transform.position.y - 5 - enemyAI.augmentArea&& enemyAI.salto))
                {
                    enemyAI.startJump(Utilities.jumpType.Normal);
                    enemyAI.salto = false;
                }
                //En caso que el target este por debajo nuestro y una plataforma atravesable se encuentre entre nosotros procederemos a atravesarla
                if (enemyAI.target.transform.position.y -1 < enemyAI.my.transform.position.y && (enemyAI.target.transform.position.x <= enemyAI.my.transform.position.x + 2 && enemyAI.target.transform.position.x >= enemyAI.my.transform.position.x - 2))
                {
                    enemyAI.my.platformDropdown();
                }
                else enemyAI.my.restorePlatform();
            }
            else
            {
                if (enemyAI.my.RigidBody.velocity.y <= 0 && enemyAI.my.totalJumps > 0)
                {
                    enemyAI.salto = true;
                }
                //Si nos encontramos por debajo del restpoint saltaremos para no caer del mapa
                if ((enemyAI.my.transform.position.y <= enemyAI.location.y - 1 && enemyAI.salto) || (enemyAI.my.transform.position.y <= enemyAI.restpoint.transform.position.y - 5 - enemyAI.augmentArea && enemyAI.salto))
                {
                    enemyAI.startJump(Utilities.jumpType.Normal);
                    enemyAI.salto = false;
                }
                //Durante el periodo en el que no tengamos target porque ha muerto nos moveremos de izquierda a derecha al rededor del centro del mapa o restpoint
                if (enemyAI.my.transform.position.x < enemyAI.restpoint.transform.position.x - 6 - enemyAI.augmentArea)
                {
                    enemyAI.my.GetComponent<Rigidbody2D>().velocity = new Vector2(1 * enemyAI.movementSpeed, enemyAI.my.RigidBody.velocity.y);
                    enemyAI.movementX = 0.6f;
                }
                else if (enemyAI.my.transform.position.x > enemyAI.restpoint.transform.position.x + 6 + enemyAI.augmentArea)
                {
                    enemyAI.my.GetComponent<Rigidbody2D>().velocity = new Vector2(-1 * enemyAI.movementSpeed, enemyAI.my.RigidBody.velocity.y);
                    enemyAI.movementX = -0.6f;
                }
            }
        }
        detectDistance();
    }
}
