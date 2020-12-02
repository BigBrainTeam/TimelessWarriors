using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : Entity {
    
    //Stats//
    [SerializeField]
    GameObject characterBars;
    [SerializeField]
    protected Stat energy;
    [SerializeField]
    protected int myLayer;
    protected Utilities.jumpType jumpType;
    protected float defenseInput, jumpTimer, dropTimer;
    public float movementX, movementY;
    [SerializeField]
    protected float fastFallingSpeed, maxFallingSpeed, groundDodgeSpeed, airDodgeSpeed;
    protected bool nextStepIsSafe, fastFalling, isVisible, lastVisible;
    public bool canDodge, canSpecialUp;
    public int player;
    public bool immune;
    private EdgeController grabbedEdge;
    //public bool isCrouching;

    //Timers//
    public float wallJumpTimer;
    public SpriteRenderer identifier;


    //Start//
    protected override void initializeComponents()
    {
        base.initializeComponents();
        chargeAmount = 0;
        charging = false;
        nextStepIsSafe = true;
        isVisible = true;
        canDodge = true;
        canSpecialUp = true;
        lastVisible = true;
        attacks = GetComponent<AttacksController>();
    }

    //Update
    protected override void entityUpdate()
    {
        base.entityUpdate();
        checkVisibility();
        checkStates();
        setAnimationParameters();
    }

    public void checkVisibility()
    {
        isVisible = RendererExtensions.IsVisibleFrom(sprite, cam);
        if (isVisible && !lastVisible) GameManager.Instance.currentFight.onPlayerInsideCamera(player);
        if (!isVisible && lastVisible) GameManager.Instance.currentFight.onPlayerOutOfCamera(player);
        lastVisible = isVisible; 
    }

    //Timers//
    protected virtual void checkTimers()
    {
        //checkDodgeTimer();
        checkWallJumpTimer();
    }
    /// <summary>
    /// Timer for wall jump, prevents 0 Xmovement.
    /// </summary>
    protected void checkWallJumpTimer()
    {
        if (wallJumpTimer > 0)
        {
            wallJumpTimer -= Time.deltaTime;
            if (wallJumpTimer <= 0)
            {
                wallJumpTimer = 0;
            }
        }
    }
    /// <summary>
    /// Used to check if a character wants to charge the current attack.
    /// </summary>
    protected virtual void checkChargingAttack(){}

    //States//
    /// <summary>
    /// Checks the different states of a Character and works according to them.
    /// </summary>
    /// 
    override protected void checkStates()
    {
        switch (state)
        {           
            case Utilities.state.Idle:
                idleUpdate();
                break;
            case Utilities.state.Moving:
                movingUpdate();
                break;
            case Utilities.state.Air:
                airUpdate();
                break;
            case Utilities.state.Edge:
                edgeUpdate();
                break;
            case Utilities.state.Attacking:
                attackingUpdate();
                break;
            case Utilities.state.WallSliding:
                wallslidingUpdate();
                break;
            case Utilities.state.Throwing:
                throwingUpdate();
                break;
            case Utilities.state.Dodging:
                dodgingUpdate();
                break;
            case Utilities.state.AirDodging:
                airDodgingUpdate();
                break;
            /*case Utilities.state.WaveDashing:
                wavedashUpdate();
                break;*/
            case Utilities.state.Ultimate:
                speciality.ultimateUpdate();
                break;
        }
    }
    //Declaration of Updates for inheritance implementation
    protected virtual void edgeUpdate(){}
    protected virtual void throwingUpdate(){}
    protected virtual void wallslidingUpdate(){}
    virtual protected void airDodgingUpdate(){}
    virtual protected void dodgingUpdate(){}
    protected virtual void wavedashUpdate(){}

    /// <summary>
    /// Checks every movement state of a Character
    /// </summary>
    virtual protected void checkMovementStates()
    {
        //checkFallingStates();
        checkFacingDirection();
        //if (isCrouching && rbd.velocity.x != 0) stopCrouching();
    }
    /// <summary>
    /// Checks the different character falling states.
    /// </summary>
    protected void checkAirFallingState()
    {
        if (rbd.velocity.y < maxFallingSpeed && !fastFalling) rbd.velocity = new Vector2(rbd.velocity.x, maxFallingSpeed);
    }
    protected void checkWallSlidingFallingState()
    {
        if (fastFalling) rbd.velocity = new Vector2(rbd.velocity.x, maxFallingSpeed / 1.2f);
        else rbd.velocity = new Vector2(rbd.velocity.x, maxFallingSpeed / 2.5f);
    }

    /// <summary>
    /// Checks if character is facing right or left.
    /// </summary>
    public void checkFacingDirection()
    {       
        //if movement is positive, the character is moving right, otherwise hes going left.
        if (movementX > 0 && facingDirection == Utilities.direction.Left)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);
            facingDirection = Utilities.direction.Right;
        }
        else if (movementX < 0 && facingDirection == Utilities.direction.Right)
        {
            
            transform.localScale = new Vector3(-(Mathf.Abs(transform.localScale.x)), transform.localScale.y, 1);
            facingDirection = Utilities.direction.Left;
        }
    }
    protected void checkBlockingFacingDirection()
    {
        //if movement is positive, the character is moving right, otherwise hes going left.
        if (movementX > 0 && facingDirection == Utilities.direction.Left)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);
            facingDirection = Utilities.direction.Right;
        }
        else if (movementX < 0 && facingDirection == Utilities.direction.Right)
        {
            transform.localScale = new Vector3(-(Mathf.Abs(transform.localScale.x)), transform.localScale.y, 1);
            facingDirection = Utilities.direction.Left;
        }
    }



    //Animation//
    /// <summary>
    /// Updates the different animation parameters for correct state-animation relation.
    /// </summary>
    protected override void setAnimationParameters()
    {
        base.setAnimationParameters();
        anim.SetBool("safeWalk", nextStepIsSafe);
        if (facingDirection == Utilities.direction.Right) identifier.flipX = false;
        else identifier.flipX = true;
    }
    /// <summary>
    /// Moves the character during the animation of edge climb.
    /// </summary>
    /// <param name="animTime">identifier for frame</param>
    void EdgeUpAnimationMovement(float animTime)
    {
        if (facingDirection == Utilities.direction.Left)
        {
            if (animTime == 0.05f) transform.position = new Vector2(transform.position.x, transform.position.y + 0.5f);
            else if (animTime == 0.15f) transform.position = new Vector2(transform.position.x, transform.position.y + 0.1f);
            else if (animTime == 0.2f) transform.position = new Vector2(transform.position.x, transform.position.y + 0.4f);
            else if (animTime == 0.3f) transform.position = new Vector2(transform.position.x, transform.position.y + 0.3f);
            else if (animTime == 0.4f) transform.position = new Vector2(transform.position.x - 0.2f, transform.position.y + 0.20f);
            else if (animTime == 0.45f) transform.position = new Vector2(transform.position.x - 0.15f, transform.position.y + 0.1f);
            else if (animTime == 0.55f) transform.position = new Vector2(transform.position.x - 0.2f, transform.position.y + 0.25f);
            else if (animTime == 0.65f) transform.position = new Vector2(transform.position.x - 0.34f, transform.position.y + 0.15f);
            else if (animTime == 0.8f) transform.position = new Vector2(transform.position.x - 0.18f, transform.position.y);
            else if (animTime == 1)
            {
                transform.position = new Vector2(transform.position.x, transform.position.y + 0.20f);
                boxCollider.enabled = true;
                rbd.gravityScale = gravityAmount;
                state = Utilities.state.Idle;
                becomeHittable();
            }
        }
        else
        {
            if (animTime == 0.05f) transform.position = new Vector2(transform.position.x, transform.position.y + 0.5f);
            else if (animTime == 0.15f) transform.position = new Vector2(transform.position.x, transform.position.y + 0.1f);
            else if (animTime == 0.2f) transform.position = new Vector2(transform.position.x, transform.position.y + 0.4f);
            else if (animTime == 0.3f) transform.position = new Vector2(transform.position.x, transform.position.y + 0.3f);
            else if (animTime == 0.4f) transform.position = new Vector2(transform.position.x + 0.2f, transform.position.y + 0.20f);
            else if (animTime == 0.45f) transform.position = new Vector2(transform.position.x + 0.15f, transform.position.y + 0.1f);
            else if (animTime == 0.55f) transform.position = new Vector2(transform.position.x + 0.2f, transform.position.y + 0.25f);
            else if (animTime == 0.65f) transform.position = new Vector2(transform.position.x + 0.34f, transform.position.y + 0.15f);
            else if (animTime == 0.8f) transform.position = new Vector2(transform.position.x + 0.18f, transform.position.y);
            else if (animTime == 1)
            {
                transform.position = new Vector2(transform.position.x, transform.position.y + 0.20f);
                boxCollider.enabled = true;
                rbd.gravityScale = gravityAmount;
                state = Utilities.state.Idle;
                becomeHittable();
            }
        }
    }

    //Collision//
    /// <summary>
    /// Check different possible collision with the Character body.
    /// </summary>
    /// <param name="collider">the external collider</param>
    override protected void OnCollisionEnter2D(Collision2D collider)
    {
        base.OnCollisionEnter2D(collider);
        // Touch floor.
        if (collider.gameObject.layer == 8)
        {
            if (collider.transform.position.y < transform.position.y && state != Utilities.state.Ultimate) {
                if (speciality is Valkyrie) speciality.resetJump(maxJumps);
                canSpecialUp = true;
                canDodge = true;
                if (fastFalling) stopFastFalling();
                if (state == Utilities.state.WallSliding) leaveWall();
            }                      
        }
        //Platform
        if(collider.gameObject.layer == 15)
        {           
            isOnPlatform = true;
            if (rbd.velocity.y <= 0 && state != Utilities.state.Ultimate && state != Utilities.state.Attacking)
            {
                if (speciality is Valkyrie) speciality.resetJump(maxJumps);
                if (fastFalling) stopFastFalling();
                canDodge = true;
                canSpecialUp = true;
            }
        }
        //Touch wall
        if(collider.gameObject.layer == 14)
        {
            if(speciality is Valkyrie) speciality.resetJump(maxJumps);
            if (state == Utilities.state.Air)
            {
                grabWall();
            }
            else if(state == Utilities.state.AirDodging)
            {
                stopDodge();
                grabWall();
            }
        }
    }
    /// <summary>
    /// Makes the character immune to hits.
    /// </summary>
    public void becomeImmune()
    {
        immune = true;
        hurtBox.enabled = false;
        Vector4 shaderVect = sprite.material.GetVector("_HSVAAdjust");
        Vector4 newVect = new Vector4(shaderVect.x, shaderVect.y, shaderVect.z, -0.5f);
        sprite.material.SetVector("_HSVAAdjust", newVect);
    }

    override protected void OnCollisionExit2D(Collision2D collider)
    {
        base.OnCollisionExit2D(collider);
        if (collider.gameObject.layer == 14 && state != Utilities.state.Attacking)
        {
            if(state == Utilities.state.WallSliding) leaveWall();
        }
    }


    //Mechanics//
    /// <summary>
    /// Puts the character on Edge Grabbing
    /// </summary>
    /// <param name="pos">the position where character stays</param>
    /// <param name="flip">where character faces</param>
    public void setEdgeGrabbing(Vector2 pos, int flip, EdgeController edge)
    {
        if(state != Utilities.state.Attacking && state != Utilities.state.Throwing && state != Utilities.state.Hit && state != Utilities.state.Ultimate)
        {
            state = Utilities.state.Edge;
            if (flip == 1)
            {
                facingDirection = Utilities.direction.Right;
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);
            }
            else {
                facingDirection = Utilities.direction.Left;
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);

            }
            rbd.velocity = new Vector2(0, 0);
            transform.position = pos;
            if (!immune)
            {
                becomeImmune();
                Invoke("becomeHittable", 2);
            }
            grabbedEdge = edge;
            rbd.gravityScale = 0;
            jumpTimer = 0;
            anim.Play("EdgeGrab");
            totalJumps = maxJumps;
        }
    }
    /// <summary>
    /// Lets go to specified direction
    /// </summary>
    /// <param name="dir">The desired direction</param>
    protected void letGo(Utilities.direction dir)
    {
        rbd.gravityScale = gravityAmount;
        state = Utilities.state.Air;
        grabbedEdge = null;
        becomeHittable();

        if (dir == Utilities.direction.Down) rbd.velocity = new Vector2(rbd.velocity.x, -1);
        else if (dir == Utilities.direction.Right) rbd.velocity = new Vector2(1, rbd.velocity.y);
        else if (dir == Utilities.direction.Left) rbd.velocity = new Vector2(-1, rbd.velocity.y);
    }
    /// <summary>
    /// Starts edge climbing.
    /// </summary>
    protected void edgeClimb()
    {
        grabbedEdge.OnTriggerExit2D(boxCollider);
        boxCollider.enabled = false;
        grabbedEdge = null;
        anim.Play("EdgeUp");
    }
    /// <summary>
    /// Starts a Dodge.
    /// </summary>
    /// <param name="dir"></param>
    public void dodge(Utilities.direction dir)
    {
        becomeImmune();
        rbd.velocity = new Vector2(0, 0);
        jumpTimer = 0f;
        float dashSpeed;
        if (state == Utilities.state.Air)
        {
            canDodge = false;
            rbd.gravityScale = 0f;
            if (fastFalling) stopFastFalling();
            dashSpeed = airDodgeSpeed;
            state = Utilities.state.AirDodging;
        }
        else
        {
            state = Utilities.state.Dodging;
            dashSpeed = groundDodgeSpeed;
        }        
        anim.Play("Dash");
        switch (dir)
        {
            case Utilities.direction.Up:
                {
                    rbd.AddForce(Vector2.up * dashSpeed, ForceMode2D.Impulse);
                }
                break;
            case Utilities.direction.Left:
                {
                    rbd.AddForce(Vector2.left * dashSpeed, ForceMode2D.Impulse);
                }
                break;
            case Utilities.direction.Right:
                {
                    rbd.AddForce(Vector2.right * dashSpeed, ForceMode2D.Impulse);
                }
                break;
            case Utilities.direction.Down:
                {
                    rbd.AddForce(Vector2.down * dashSpeed, ForceMode2D.Impulse);
                }
                break;
        }
    }
    /// <summary>
    /// Stops the dodge animation and recovers basic character functions.
    /// </summary>
    virtual protected void stopDodge()
    {
        becomeHittable();
        rbd.gravityScale = gravityAmount;
        if (isGrounded) state = Utilities.state.Idle;
        else state = Utilities.state.Air;
    }
    /// <summary>
    /// Dodges at the spot where character is.
    /// </summary>
    public void spotDodge() {
        canDodge = false;
        state = Utilities.state.Blocking;
        if (fastFalling) stopFastFalling();
        rbd.velocity = new Vector2(0, 0);
        anim.Play("SpotDodge");
    }
    /// <summary>
    /// Used to ignore the closer platform to the Character.
    /// </summary>
    /// <param name="ignore"></param>
    /// <returns></returns>
    public IEnumerator ignorePlatforms(bool ignore)
    {
        if (ignore)
        {
            if(closerPlatform != null) Physics2D.IgnoreCollision(boxCollider, closerPlatform, true);
            isIgnoringPlatforms = true;
            isOnPlatform = false;
        }
        else
        {
            yield return new WaitForSeconds(0.1f);
            if(closerPlatform != null) Physics2D.IgnoreCollision(boxCollider, closerPlatform, false);
            isIgnoringPlatforms = false;
        }
    }
    /// <summary>
    /// Starts the selected Jump.
    /// </summary>
    /// <param name="type"></param>
    public void startJump(Utilities.jumpType type)
    {
        if(speciality is Valkyrie) speciality.onJump();
        rbd.gravityScale = gravityAmount;
        boxCollider.enabled = true;
        if (type == Utilities.jumpType.Double)
        {
            doubleJump();
        }else
        {
            state = Utilities.state.Air;
            anim.Play("Jump",-1,0f);
            isOnPlatform = false;
            jumpType = type;
        }
        totalJumps--;
    }
    private void doJump()
    {
        if (grabbedEdge != null) {
            becomeHittable();
            grabbedEdge = null;
        } 
        if (jumpType == Utilities.jumpType.Small) smallJump();
        else normalJump();
    }
    /// <summary>
    /// Normal jump force
    /// </summary>
    protected virtual void normalJump()
    {
        rbd.velocity = new Vector2(rbd.velocity.x, 0f);
        rbd.AddForce(new Vector2(0, jumpForce),ForceMode2D.Impulse);
    }
    /// <summary>
    /// Small jump force
    /// </summary>
    protected virtual void smallJump()
    {
        rbd.velocity = new Vector2(rbd.velocity.x, 0f);
        rbd.AddForce(new Vector2(0, jumpForce / 1.3f), ForceMode2D.Impulse);
    }
    /// <summary>
    /// Makes the character double Jump
    /// </summary>
    protected void doubleJump()
    {
        if (fastFalling) stopFastFalling();
        anim.Play("Jump_Flip");
        normalJump();
    }
    /// <summary>
    /// Wall jump force, depending on facing direction
    /// </summary>
    protected void wallJump()
    {
        wallJumpTimer = 0.2f;
        if (fastFalling) stopFastFalling();
        state = Utilities.state.Air;
        anim.Play("Jump");

        rbd.velocity = new Vector2(rbd.velocity.x, 0f);
        if (facingDirection == Utilities.direction.Left) rbd.AddForce(new Vector2(jumpForce/2, jumpForce), ForceMode2D.Impulse);
        else rbd.AddForce(new Vector2(-jumpForce/2, jumpForce), ForceMode2D.Impulse);
    }
    /// <summary>
    /// Grab wall and start sliding.
    /// </summary>
    protected void grabWall()
    {
        state = Utilities.state.WallSliding;
        anim.Play("WallSlide");      
    }
    public Stat Energy
    {
        get { return energy; }
    }
    /// <summary>
    /// Drops out of the wall.
    /// </summary>
    protected void leaveWall()
    {     
        if (fastFalling) stopFastFalling();
        if (isGrounded) state = Utilities.state.Idle;
        else
        {
            anim.Play("Falling");
            state = Utilities.state.Air;
        }
    }
    /// <summary>
    /// Stops fast fall.
    /// </summary>
    protected void stopFastFalling()
    {
        fastFalling = false;
    }
    /// <summary>
    /// Enables fast falling.
    /// </summary>
    protected void startFastFalling()
    {
        fastFalling = true;
        rbd.velocity = new Vector2(rbd.velocity.x, fastFallingSpeed);
    }
    /// <summary>
    /// Changes the amount of energy of a Character
    /// </summary>
    /// <param name="newEnergy">energy amount</param>
    public void setEnergy(float newEnergy)
    {
        energy.CurrentVal = newEnergy;
    }
    /// <summary>
    /// Increases energy.
    /// </summary>
    /// <param name="extraEnergy"></param>
    public void increaseEnergy(float extraEnergy)
    {
        float startEnergy = energy.CurrentVal;
        energy.CurrentVal = energy.CurrentVal + extraEnergy;
        if(energy.CurrentVal == energy.MaxVal && energy.CurrentVal > startEnergy) Instantiate(effects[4], transform, false); // if energy reached 100 this call, spawns particle effect.
    }
    /// <summary>
    /// Does damage to Character.
    /// </summary>
    /// <param name="damage"></param>
    public override void receiveDamage(float damage)
    {
        base.receiveDamage(damage);
        increaseEnergy(damage / 4);
    }

    //Fixed Update//
    /// <summary>
    /// Updates the physics of a Character.
    /// </summary>
    protected override void physUpdate()
    {
        base.physUpdate();
        characterMovement();
    }
    /// <summary>
    /// Make all Physics OverlapCircles and Raycast for correct function of Character.
    /// </summary>
    protected override void doPhysicCasts()
    {
        base.doPhysicCasts();
        if(isGrounded)
        {
            if (facingDirection == Utilities.direction.Right) nextStepIsSafe = Physics2D.Raycast(transform.position, new Vector2(0.3f, -1), 5f, whatIsFloor);
            else nextStepIsSafe = Physics2D.Raycast(transform.position, new Vector2(-0.3f, -1), 5f, whatIsFloor);
        }  
    }
    /// <summary>
    /// Stops charging current attack.
    /// </summary>
    public void stopCharging()
    {
        if (attacks.currentAttack == Utilities.attackType.BasicFloorUp) anim.Play("BasicFloorUp", 0, 0.2f);
        else anim.Play("BasicFloorSide", 0, 0.2f);
        charging = false;
    }
    /// <summary>
    /// Moves the character
    /// </summary>
    protected virtual void characterMovement()
    {
        
            
    }
    /// <summary>
    /// Drop down from current platform.
    /// </summary>
    public void platformDropdown()
    {
        StartCoroutine(ignorePlatforms(true));
        rbd.velocity = new Vector2(rbd.velocity.x, -10);
        isIgnoringPlatforms = true;
        isOnPlatform = false;
        isGrounded = false;
    }
    /// <summary>
    /// Restores collision with Platform.
    /// </summary>
    public void restorePlatform()
    {
        StartCoroutine(ignorePlatforms(false));
    }
    /// <summary>
    /// Makes Character be able to get hit again.
    /// </summary>
    public void becomeHittable()
    {
        if (immune)
        {
            if(state != Utilities.state.Ultimate) hurtBox.enabled = true;
            Vector4 shaderVect = sprite.material.GetVector("_HSVAAdjust");
            Vector4 newVect = new Vector4(shaderVect.x, shaderVect.y, shaderVect.z, 0);
            sprite.material.SetVector("_HSVAAdjust", newVect);
            immune = false;
        }      
    }

   
}
