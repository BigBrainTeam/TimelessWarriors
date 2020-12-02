using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for Player controlled Characters.
/// </summary>
public class Player: Character {

    //Inputs
    float runInput = 0.5f;
    float walkInput = 0.1f;
    /// <summary>
    /// Variable to check if Buttons is being pressed more than one frame.
    /// </summary>
    private bool isBeingPressedX, isBeingPressedY, isBeingPressedTriggers, isBeingPressedItem, isBeingPressedJump;
    //Timers
    public Controls controls;

   


    protected override void initializeComponents()
    {
        base.initializeComponents();
        controls = new Controls();
        controls.loadControls(player);
    }

    //Update//
    protected override void entityUpdate()
    {
        //checkInputs();
        movementX = Input.GetAxis(controls.axisX);
        movementY = Input.GetAxisRaw(controls.axisY);

        checkTimers();
        base.entityUpdate();
        checkIdentifier();
    }
    /// <summary>
    /// Disable/enable player identifier.
    /// </summary>
    private void checkIdentifier()
    {
        if(Input.GetButtonDown("SelectP"+(player+1)))identifier.enabled = !identifier.enabled;
    }
    /// <summary>
    /// Checks input timers for delayed actions.
    /// </summary>
    protected override void checkTimers()
    {
        base.checkTimers();
        checkJumpTimer();
        checkDropTimer();
    }

    public void defaultHorizontalInput()
    {
        if (movementX != 0)
        {
            if (!isBeingPressedX) isBeingPressedX = true;
        }
        else isBeingPressedX = false;
    }
    public void defaultVerticalInput()
    {
        if (movementY != 0)
        {
            if (!isBeingPressedY) isBeingPressedY = true;
        }
        else isBeingPressedY = false;
    }

    //Idle//
    protected override void idleUpdate()
    {
        checkIdleInputs();
        //checkBlockingFacingDirection();
    }

    /// <summary>
    /// Check every possible Input from Idle state and acts accordingly.
    /// </summary>
    protected void checkIdleInputs()
    {
        defaultHorizontalInput();
        checkIdleAttackInput();
        checkIdleVerticalInput();   
        checkIdleJumpInput();
        checkIdleDefenseInput();
        checkItemsInput();
        if (rbd.velocity.x != 0 && state != Utilities.state.Ultimate) state = Utilities.state.Moving;
        if (rbd.velocity.y != 0 && state != Utilities.state.Attacking && state != Utilities.state.Ultimate)
        {
            state = Utilities.state.Air;
        }
    }

    protected void checkIdleVerticalInput()
    {
        if (movementY != 0)
        {
            if (!isBeingPressedY)
            {
                if(isOnPlatform && !isIgnoringPlatforms && movementY<0)
                {
                    dropTimer = 2;
                }
            }
            isBeingPressedY = true;
        }
        else isBeingPressedY = false;
    }

    protected void checkIdleAttackInput()
    {
        if(!hasItem)
        {
            if (Input.GetButtonDown(controls.BasicAttack))
            {
                if (movementY > 0) attacks.startAttack(Utilities.attackType.BasicFloorUp);
                else attacks.startAttack(Utilities.attackType.BasicFloorSide);
                state = Utilities.state.Attacking;
            }
            else
        if (Input.GetButtonDown(controls.SpecialAttack))
            {
                if (movementY > 0) attacks.startAttack(Utilities.attackType.SpecialUp);
                else if(movementY < 0) attacks.startAttack(Utilities.attackType.SpecialDown);
                else attacks.startAttack(Utilities.attackType.SpecialSide);
                state = Utilities.state.Attacking;
            }
            else
        if (Input.GetButtonDown(controls.FinalAttack) && energy.CurrentVal == energy.MaxVal)
            {
                state = Utilities.state.Ultimate;
                HurtBox.enabled = false;
                attacks.startAttack(Utilities.attackType.Final);
            }
            if (state == Utilities.state.Attacking)
            {
                if (immune) becomeHittable();
                //if (jumpTimer > 0) jumpTimer = 0;
            }
        }  
    }
    protected void checkIdleDefenseInput()
    {
        defenseInput = Input.GetAxis(controls.Defense);
        if (defenseInput != 0)
        {
            if (!isBeingPressedTriggers) // ButtonDown
            {
                spotDodge();
            }
            isBeingPressedTriggers = true;
        }
        else isBeingPressedTriggers = false;
    }
    protected void checkIdleJumpInput()
    {
        //Jump Button Check
        if (Input.GetButtonDown(controls.Jump) && state != Utilities.state.Attacking)
        {
            if (!isBeingPressedJump)
            {
                if (totalJumps > 0)
                {
                    state = Utilities.state.Air;
                    jumpTimer = 0.2f;
                }                
            }
            isBeingPressedJump = true;
        }
        else isBeingPressedJump = false;
    }


    //Moving//
    protected override void movingUpdate()
    {
        checkMovingInputs();
        checkFacingDirection();
    }
    /// <summary>
    /// Check every possible Input from Moving state and acts accordingly.
    /// </summary>
    protected void checkMovingInputs()
    {
        defaultHorizontalInput();
        checkIdleAttackInput();
        //checkMovingAttackInput();
        checkIdleVerticalInput();
        checkIdleJumpInput();
        checkMovingDefenseInput();     
        checkItemsInput();
        if (rbd.velocity.x == 0 && state != Utilities.state.Attacking && state != Utilities.state.Throwing && state != Utilities.state.Ultimate)
        {
            state = Utilities.state.Idle;
        }
        if(rbd.velocity.y != 0 && state != Utilities.state.Attacking && state != Utilities.state.Ultimate)
        {
            state = Utilities.state.Air;
        }
    }
    protected void checkMovingAttackInput()
    {
        if(!hasItem)
        {
            if (Input.GetButtonDown(controls.BasicAttack))
            {
                state = Utilities.state.Attacking;
                if(Mathf.Abs(movementX) == 1) attacks.startAttack(Utilities.attackType.BasicFloorMoving);
                else
                {
                    if (movementY > 0) attacks.startAttack(Utilities.attackType.BasicFloorUp);
                    else attacks.startAttack(Utilities.attackType.BasicFloorSide);
                }
            }
            else
       if (Input.GetButtonDown(controls.SpecialAttack))
            {
                state = Utilities.state.Attacking;
                if (movementY > 0) attacks.startAttack(Utilities.attackType.SpecialUp);
                else attacks.startAttack(Utilities.attackType.SpecialSide);
            }
            else
       if (Input.GetButtonDown(controls.FinalAttack) && energy.CurrentVal == energy.MaxVal)
            {
                state = Utilities.state.Attacking;
                attacks.startAttack(Utilities.attackType.Final);
            }
            if (state == Utilities.state.Attacking && immune) becomeHittable();
        }     
    }
    protected void checkMovingDefenseInput()
    {
        defenseInput = Input.GetAxis(controls.Defense);
        if (defenseInput != 0)
        {
            if (!isBeingPressedTriggers) // ButtonDown
            {
                if (movementX > 0) dodge(Utilities.direction.Right);
                else dodge(Utilities.direction.Left);       
            }
            isBeingPressedTriggers = true;
        }
        else isBeingPressedTriggers = false;
    }


    //Air//
    protected override void airUpdate()
    {
        checkAirInputs();
        checkAirFallingState();
        checkFacingDirection();
    }
    /// <summary>
    /// Check every possible Input from Air state and acts accordingly.
    /// </summary>
    protected void checkAirInputs()
    {  
        defaultHorizontalInput();
        checkAirVerticalInput();
        checkAirAttackInput();
        checkAirDefenseInput();
        checkAirJumpInput();
        checkItemsInput();      
    }
    protected void checkAirVerticalInput()
    {
        if (movementY != 0)
        {
            if (!isBeingPressedY)
            {
                //Fast Fall
                if (movementY < 0 && rbd.velocity.y <= 10) if (!fastFalling) dropTimer = 2;
                isBeingPressedY = true;
            }
            if (movementY < 0)
            {
                if (!isIgnoringPlatforms) StartCoroutine(ignorePlatforms(true));
            }
        }
        else
        {
            isBeingPressedY = false;
            if (isIgnoringPlatforms) StartCoroutine(ignorePlatforms(false));
        }
    }
    protected void checkAirAttackInput()
    {
        if (!hasItem && state != Utilities.state.AirDodging)
        {
            if (Input.GetButtonDown(controls.BasicAttack))
            {
                state = Utilities.state.Attacking;
                if (movementY != 0)
                {
                    if (movementY > 0) attacks.startAttack(Utilities.attackType.BasicAirUp);
                    else attacks.startAttack(Utilities.attackType.BasicAirDown);
                }
                else attacks.startAttack(Utilities.attackType.BasicAirSide);
            }
            else
        if (Input.GetButtonDown(controls.SpecialAttack))
            {
                if (movementY > 0 && canSpecialUp)
                {
                    state = Utilities.state.Attacking;
                    attacks.startAttack(Utilities.attackType.SpecialUp);
                }
                else if (movementY < 0)
                {
                    state = Utilities.state.Attacking;
                    attacks.startAttack(Utilities.attackType.SpecialDown);
                }
                else if (movementY < 0.5 && movementY > -0.5) {
                    state = Utilities.state.Attacking;
                    attacks.startAttack(Utilities.attackType.SpecialSide);
                } 
            }
            else
        if (Input.GetButtonDown(controls.FinalAttack) && energy.CurrentVal == energy.MaxVal)
            {
                state = Utilities.state.Ultimate;
                HurtBox.enabled = false;
                attacks.startAttack(Utilities.attackType.Final);
            }
            if (state == Utilities.state.Attacking)
            {
                if (immune) becomeHittable();
                //if (jumpTimer > 0) jumpTimer = 0;
            }

        }        
    }
    protected void checkAirDefenseInput()
    {
        defenseInput = Input.GetAxis(controls.Defense);
        if (defenseInput != 0)
        {
            if (!isBeingPressedTriggers) // ButtonDown
            {
                if(state != Utilities.state.Attacking && canDodge)
                {
                    if (movementX != 0) //Side-dash
                    {
                        if (movementX > 0) dodge(Utilities.direction.Right);
                        else dodge(Utilities.direction.Left);
                    }
                    else
                    {
                        if (movementY != 0)
                        {
                            if (movementY > 0) dodge(Utilities.direction.Up);
                            else dodge(Utilities.direction.Down);
                        }
                        else spotDodge();
                    }
                }              
            }
            isBeingPressedTriggers = true;
        }
        else isBeingPressedTriggers = false;
    }
    protected void checkAirJumpInput()
    {
        //Jump Button Check
        if (Input.GetButtonDown(controls.Jump) && state != Utilities.state.Attacking)
        {
            if (!isBeingPressedJump)
            {
                if (totalJumps > 0)
                {
                    if (totalJumps == 1) startJump(Utilities.jumpType.Double);
                    else startJump(Utilities.jumpType.Normal);
                }             
            }
            isBeingPressedJump = true;
        }
        else isBeingPressedJump = false;   
    }

    //WaveDash
    /*protected override void wavedashUpdate()
    {
        wavedashHorizontalInput();
        defaultVerticalInput();
        checkIdleAttackInput();
        if (Mathf.Abs(rbd.velocity.x) <= movementSpeed / 2 && state != Utilities.state.Attacking) stopDodge();
    }

    private void wavedashHorizontalInput()
    {
        if ((movementX < 0 && rbd.velocity.x > 0) || (movementX > 0 && rbd.velocity.x < 0)) stopDodge();
    }*/


    //Edge//
    protected override void edgeUpdate()
    {
        checkEdgeInputs();
    }
    /// <summary>
    /// Check every possible Input from Edge state and acts accordingly.
    /// </summary>
    protected void checkEdgeInputs()
    {
        checkEdgeHorizontalInput();
        checkEdgeVerticalInput();
        checkEdgeJumpInput();
    }
    protected void checkEdgeJumpInput()
    {
        //Jump Button Check
        if (Input.GetButtonDown(controls.Jump))
        {
            if (!isBeingPressedJump)
            {
                if (totalJumps > 0 && boxCollider.enabled) {
                    startJump(Utilities.jumpType.Normal);
                }             
            }
            isBeingPressedJump = true;
        }
        else isBeingPressedJump = false;
    }
    protected void checkEdgeVerticalInput()
    {
        if (movementY != 0)
        {
            if (!isBeingPressedY)//buttonDown events
            {
                //if movement goes up, edge climb. else let go down.
                if (boxCollider.enabled)
                {
                    if (movementY > 0) edgeClimb();
                    else letGo(Utilities.direction.Down);
                }                       
                isBeingPressedY = true;
            }
        }
        else isBeingPressedY = false;
    }
    protected void checkEdgeHorizontalInput()
    {
        if (movementX != 0)
        {
            if (!isBeingPressedX)//buttonDown events
            {
                if (boxCollider.enabled)
                {
                    //first we check where the Character is facing
                    if (transform.localScale.x > 0) // hes facing right
                    {
                        //if he wants to move right, he climbs. else he lets go left.
                        if (movementX > 0) edgeClimb();
                        else letGo(Utilities.direction.Left);
                    }
                    else //hes facing left
                    {
                        if (movementX < 0) edgeClimb();
                        else letGo(Utilities.direction.Right);
                    }
                }           
                isBeingPressedX = true;
            }
        }
        else isBeingPressedX = false;
    }

    protected override void dodgingUpdate()
    {
        defaultHorizontalInput();
        defaultVerticalInput();
    }
    protected override void airDodgingUpdate()
    {
        defaultHorizontalInput();
        defaultVerticalInput();
       /* if (isGrounded)
        {
         //   if (isOnPlatform) transform.position = new Vector2(transform.position.x, (closerPlatform.transform.position.y + closerPlatform.bounds.size.y)+boxCollider.bounds.size.y);
            rbd.gravityScale = gravityAmount;
            state = Utilities.state.WaveDashing;
            rbd.AddForce(Vector2.right * rbd.velocity.y, ForceMode2D.Impulse);
        }*/
    }


    //WallSliding//
    protected override void wallslidingUpdate()
    {
        checkWallSlidingFallingState();
        checkWallSlidingInputs();       
    }
    /// <summary>
    /// Check every possible Input from WallSliding state and acts accordingly.
    /// </summary>
    protected void checkWallSlidingInputs()
    {
        checkWallSlidingJumpInput();
        checkWallSlidingHorizontalInput();
        checkWallSlidingVerticalInput();
    }
    protected void checkWallSlidingVerticalInput()
    {
        if (movementY != 0)
        {
            if (!isBeingPressedY)
            {
                //Fast Fall
                if (movementY < 0) if (!fastFalling) dropTimer = 2;
                isBeingPressedY = true;
            }
        }
        else isBeingPressedY = false;
    }
    protected void checkWallSlidingHorizontalInput()
    {
        if (movementX != 0)
        {
            if(facingDirection == Utilities.direction.Left && movementX > 0) leaveWall();     
            else if(facingDirection == Utilities.direction.Right && movementX < 0) leaveWall();
            isBeingPressedX = true;
        }
        else isBeingPressedX = false;
    }
    protected void checkWallSlidingJumpInput()
    {
        if (Input.GetButtonDown(controls.Jump))
        {
            if (fastFalling) stopFastFalling();
            wallJump();
            totalJumps--;
        }
    }


    //Attacking
    protected override void attackingUpdate()
    {
        checkAttackingInputs();
    }
    /// <summary>
    /// Check every possible Input from Attacking state and acts accordingly.
    /// </summary>
    protected void checkAttackingInputs()
    {
        if (Input.GetButtonUp(controls.BasicAttack) && charging)
        {
            stopCharging();
        }
        if (charging)
        {
            chargeAmount+= Time.deltaTime;
            if (chargeAmount >= 2f)
            {
                chargeAmount = 2f;
                stopCharging();
            }
        }
        
    }
    /// <summary>
    /// checks if current attack is being charged.
    /// </summary>
    protected override void checkChargingAttack()
    {
        if (Input.GetButton(controls.BasicAttack))
        {
            if (attacks.currentAttack == Utilities.attackType.BasicFloorUp) anim.Play("UpCharging");
            else anim.Play("SideCharging");
            charging = true;
        }
        else charging = false;
    }


    protected override void throwingUpdate()
    {
        checkThrowingInputs();
    }
    protected void checkThrowingInputs()
    {
        if (Input.GetButtonUp(controls.Items) && charging)
        {
            anim.Play("Throw", 0, 0.3f);
            charging = false;
        }
        if (charging)
        {
            chargeAmount += Time.deltaTime;
            if (chargeAmount >= 2f)
            {
                chargeAmount = 2f;
                anim.Play("Throw", 0, 0.3f);
                charging = false;
            }
        }
    }
    public override void checkChargeThrowing()
    {
        if (Input.GetButton(controls.Items))
        {
            anim.Play("ThrowCharging");
            charging = true;
            if(state != Utilities.state.Air)
            {
                rbd.velocity = new Vector2(0, rbd.velocity.y);
            }           
        }
        else charging = false;
    }


    //Mechanics//
    protected override void throwItem()
    {
        getAxisDirection();
        base.throwItem();
    }
    protected override void smallJump()
    {
        base.smallJump();
        jumpTimer = 0;
    }
    protected void checkItemsInput()
    {
        if (Input.GetButtonDown(controls.Items))
        {
            if (!isBeingPressedItem)
            {
                if (hasItem)
                {
                    throwDirection = getAxisDirection();
                    useItem();
                }
                else pickupItem();
            }            
            isBeingPressedItem = true;
        }
        else isBeingPressedItem = false;
    }

    /// <summary>
    /// Returns the axis direction the Player has input.
    /// </summary>
    /// <returns></returns>
    protected Utilities.direction getAxisDirection()
    {
        if (movementY != 0)
        {
            if(movementY>0)
            {
                if(movementX!=0)
                {
                    if (movementX > 0) return Utilities.direction.UpRight;
                    else if (movementX<0) return Utilities.direction.UpLeft;
                }
                else return Utilities.direction.Up;
            }
            else
            {
                if (movementX != 0)
                {
                    if (movementX > 0) return Utilities.direction.DownRight;
                    else if(movementX < 0) return Utilities.direction.DownLeft;
                }
                else return Utilities.direction.Down;
            }
        }
        else
        {
            if (movementX > 0) return Utilities.direction.Right;
            else if (movementX < 0) return Utilities.direction.Left;
        }

        if (transform.localScale.x < 0) return Utilities.direction.Left;
        else return Utilities.direction.Right;
    }

    //Timers//
    /// <summary>
    /// Checks the timer for Jumping. Allows small and normal jump.
    /// </summary>
    protected void checkJumpTimer()
    {
        //If character is going to jump, the timer will be > 0
        if (jumpTimer > 0)
        {
            //If the button is realeased early, he will do a short jump.
            if (Input.GetButtonUp(controls.Jump) && jumpTimer > 0.15f)
            {
                startJump(Utilities.jumpType.Small);
                jumpTimer = 0;
                return;
            }
            else
            {
                jumpTimer -= Time.deltaTime;
                //if timer decreased enough, normal jump will be executed.
                if (jumpTimer < 0.15f)
                {
                    startJump(Utilities.jumpType.Normal);
                    jumpTimer = 0;
                }
            }
        }
    }
    /// <summary>
    /// Checks timer for platform drops and fastfalls.
    /// </summary>
    protected void checkDropTimer()
    {
        if (dropTimer > 0)
        {
            if (movementY >= 0)
            {
                dropTimer = 0;
                return;
            }
            else
            {           
                if (dropTimer < 1.91f)
                {
                    if (state == Utilities.state.Air) startFastFalling();
                    else if (isOnPlatform) platformDropdown();
                    dropTimer = 0;
                }
            }
            dropTimer -= Time.deltaTime;
        }
    }
    /// <summary>
    /// Controls character movement from inputs.
    /// </summary>
    protected override void characterMovement()
    {
            if ((state == Utilities.state.Air && wallJumpTimer == 0f) || state == Utilities.state.Moving || state == Utilities.state.Idle)
            {
                if (Mathf.Abs(movementX) > walkInput)
                {
                    if (Mathf.Abs(movementX) > runInput)
                    {
                        if (movementX > 0) movementX = 1;
                        else movementX = -1;
                    }
                    else
                    {
                        if (movementX > 0) movementX = 0.5f;
                        else movementX = -0.5f;
                    }
                    
                }
            //increase velocity of character directly depending of input.
            rbd.velocity = new Vector2(movementX * movementSpeed, rbd.velocity.y);
        }
       // else if (state == Utilities.state.WaveDashing) rbd.velocity = new Vector2(rbd.velocity.x * 0.99f, rbd.velocity.y);  
    }
}
