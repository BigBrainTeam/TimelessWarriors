using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base Class for everything that fights.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Entity : MonoBehaviour
{
    public Camera cam;
    //Unity Components//
    public LayerMask whatIsFloor, pickableItems;
    protected Transform groundChecker, pickUpSpot;
    protected Rigidbody2D rbd;
    protected SpriteRenderer sprite;
    protected Collider2D boxCollider, hurtBox, hitbox;
    protected Animator anim;
    public PickableItem grabbedItem;
    protected Utilities.direction throwDirection;
    public Utilities.direction facingDirection;
    /// <summary>
    /// Attacks of the Entity.
    /// </summary>
    public AttacksController attacks;
    public Speciality speciality;
    /// <summary>
    /// Current state of the Entity.
    /// </summary>
    [SerializeField]
    public Utilities.state state;
    [SerializeField]
    protected Collider2D closerPlatform;
    Coroutine hitlag, hitstun;
    public GameObject attackEffect;
    public GameObject[] effects;
    /// <summary>
    /// Curent status condition of the Entity.
    /// </summary>
    public Utilities.statusCondition status;
    protected Transform statusSpot;
    protected GameObject statusEffect;
    Coroutine statusCondition;
    protected AudioSource audioSource;


    //Stats//
    [SerializeField]
    public Stat life;
    public float movementSpeed, jumpForce, lastVelocityY, gravityAmount;
    protected float chargeAmount, maxThrowForce, minThrowForce, damage;
    [SerializeField]
    public bool hasItem, charging, isIgnoringPlatforms;
    public bool isGrounded, isOnPlatform, isDead;
    public int maxJumps;
    public int totalJumps;
    public int hittedBy;





    protected void Start()
    {
        initializeComponents();
    }
    protected void FixedUpdate()
    {
        physUpdate();
    }
    protected void Update()
    {
        entityUpdate();
    }

    protected virtual void entityUpdate()
    {
        checkStates();
    }
    //Start//
    /// <summary>
    /// gives value to the different components.
    /// </summary>
    protected virtual void initializeComponents()
    {
        audioSource = GetComponent<AudioSource>();
        rbd = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        hurtBox = transform.GetChild(2).GetComponent<BoxCollider2D>();
        hitbox = transform.GetChild(3).GetComponent<BoxCollider2D>();
        statusSpot = transform.GetChild(5).transform;
        sprite = GetComponent<SpriteRenderer>();
        gravityAmount = rbd.gravityScale;
        totalJumps = maxJumps;
        hasItem = false;
        charging = false;
        maxThrowForce = 10f;
        isOnPlatform = false;
        isIgnoringPlatforms = false;
        state = Utilities.state.Idle;
        hittedBy = -1;
        cam = Camera.main;

        groundChecker = transform.GetChild(0).transform;
        pickUpSpot = transform.GetChild(1).transform;
    }

    //States//
    /// <summary>
    /// Checks the different states of a Character and works according to them.
    /// </summary>
    /// 
    virtual protected void checkStates()
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
            case Utilities.state.Attacking:
                attackingUpdate();
                break;
        }
    }

    //Declaration of Updates for inheritance implementation
    protected virtual void airUpdate(){}
    protected virtual void movingUpdate(){}
    protected virtual void idleUpdate(){}
    protected virtual void attackingUpdate(){}



    /// <summary>
    /// Sets new status to Entity.
    /// </summary>
    /// <param name="newStatus"></param>
    internal void setStatus(Utilities.statusCondition newStatus)
    {
        Destroy(statusEffect);
        status = newStatus;
        if (statusCondition != null) StopCoroutine(statusCondition);

        switch (newStatus)
        {
            case Utilities.statusCondition.BURN:
                {
                    statusEffect = Instantiate(effects[6], statusSpot, false);
                    statusCondition = StartCoroutine(statusConditionCoroutine());
                }
                break;
            case Utilities.statusCondition.SLOW:
                {

                }
                break;
            case Utilities.statusCondition.POISON:
                {

                }
                break;
        }
    }
    /// <summary>
    /// Damage ticks of status condition.
    /// </summary>
    /// <returns></returns>
    IEnumerator statusConditionCoroutine()
    {
        float ticks = 10;
        while (ticks > 0)
        {
            life.CurrentVal -= 1.5f;
            yield return new WaitForSeconds(0.5f);
            ticks--;
        }
        setStatus(Utilities.statusCondition.NONE);
    }

    /////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////Collisions///////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////
    //Collision//
    /// <summary>
    /// Check different possible collision with the Character body.
    /// </summary>
    /// <param name="collider">the external collider</param>
    virtual protected void OnCollisionEnter2D(Collision2D collider)
    {
        // Touch floor.
        if (collider.gameObject.layer == 8)
        {
            if (collider.transform.position.y < transform.position.y && state != Utilities.state.Ultimate)
            {
                if (rbd.velocity.y <= 0) totalJumps = maxJumps;
                if (state == Utilities.state.Hit)
                {
                    rbd.velocity = new Vector2(rbd.velocity.x, -lastVelocityY * 0.85f);
                }
                if (state == Utilities.state.Throwing) rbd.velocity = new Vector2(0, 0);
                if (state == Utilities.state.Air)
                {
                    if (rbd.velocity.x != 0) state = Utilities.state.Moving;
                    else state = Utilities.state.Idle;
                }
            }            
        }
        //Platform
        if (collider.gameObject.layer == 15)
        {          
            if (rbd.velocity.y <= 0 && state != Utilities.state.Ultimate)
            {
                isOnPlatform = true;
                totalJumps = maxJumps;
                if (state == Utilities.state.Hit)
                {
                    rbd.velocity = new Vector2(rbd.velocity.x, -lastVelocityY * 0.85f);
                }
                if (state == Utilities.state.Throwing) rbd.velocity = new Vector2(0, 0);
                if (state == Utilities.state.Air)
                {
                    if (rbd.velocity.x != 0) state = Utilities.state.Moving;
                    else state = Utilities.state.Idle;
                }
           }
        }
    }
    virtual protected void OnCollisionExit2D(Collision2D collider)
    {
        //Platform
        if (collider.gameObject.layer == 15 && state != Utilities.state.Attacking && state != Utilities.state.Throwing && state != Utilities.state.Ultimate)
        {
            isOnPlatform = false;
            if (state != Utilities.state.Hit) state = Utilities.state.Air;
        }
    }
    virtual protected void OnCollisionStay2D(Collision2D collider)
    {
        //Platform       
        if (collider.gameObject.layer == 15)
        {
            if (rbd.velocity.y <= 0 && !isOnPlatform)
            {
                isOnPlatform = true;
            }
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////Animation////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Updates the different animation parameters for correct state-animation relation.
    /// </summary>
    protected virtual void setAnimationParameters()
    {
        anim.SetFloat("speed", Mathf.Abs(rbd.velocity.x));
        anim.SetFloat("verticalspeed", rbd.velocity.y);
        anim.SetBool("grounded", isGrounded);
    }
    /// <summary>
    /// Makes the entity stop Attacking and changes state.
    /// </summary>
    public virtual void stopAttacking()
    {
        rbd.gravityScale = gravityAmount;
        anim.speed = 1;
        chargeAmount = 0;
        if (attackEffect) Destroy(attackEffect);
        if( attacks.currentAttack != Utilities.attackType.None) attacks.attacks[(int)attacks.currentAttack].resetHittedEntities();
        if (state == Utilities.state.Ultimate) speciality.endUltimate();
        attacks.currentAttack = Utilities.attackType.None;      
        if (isGrounded) state = Utilities.state.Idle;          
        else state = Utilities.state.Air;
    }
    public void recoverGravity()
    {
        rbd.gravityScale = gravityAmount;
    }

    //Mechanics//
    /// <summary>
    /// Starts blocking, and prevents other movements
    /// </summary>
    protected void startBlocking()
    {
        state = Utilities.state.Blocking;
        rbd.velocity = new Vector2(0, 0);
        anim.Play("Block");
        anim.SetBool("holdAnimation", true);
    }
    /// <summary>
    /// Stops blocking, allowing other movements
    /// </summary>
    protected void stopBlocking()
    {
        state = Utilities.state.Idle;
        anim.SetBool("holdAnimation", false);
    }
    /// <summary>
    /// Throws the grabbed item.
    /// </summary>
    /// <param name="dir">direction of throw</param>
    protected virtual void throwItem()
    {
        if (grabbedItem != null)
        {
            grabbedItem.transform.parent = null;
            grabbedItem.getRigidBody().isKinematic = false;
            grabbedItem.setIsPicked(false);
            grabbedItem.setIsUsed(true);
            grabbedItem.GetComponent<ThrowableItem>().onThrow();

            float throwForce;
            if (chargeAmount > 0)
            {
                throwForce = grabbedItem.getVelocity() + (chargeAmount * maxThrowForce/2);
                chargeAmount = 0;
            }
            else throwForce = grabbedItem.getVelocity();

            switch (throwDirection)
            {
                case Utilities.direction.Up: grabbedItem.getRigidBody().AddForce(new Vector2(0, throwForce),ForceMode2D.Impulse);break;      
                case Utilities.direction.Down: grabbedItem.getRigidBody().AddForce(new Vector2(0, -throwForce), ForceMode2D.Impulse); break;
                case Utilities.direction.Left: grabbedItem.getRigidBody().AddForce(new Vector2(-throwForce, 0), ForceMode2D.Impulse); break;
                case Utilities.direction.Right: grabbedItem.getRigidBody().AddForce(new Vector2(throwForce, 0), ForceMode2D.Impulse); break;
                case Utilities.direction.UpLeft: grabbedItem.getRigidBody().AddForce(new Vector2(-throwForce, throwForce), ForceMode2D.Impulse); break;
                case Utilities.direction.UpRight: grabbedItem.getRigidBody().AddForce(new Vector2(throwForce, throwForce), ForceMode2D.Impulse); break;
                case Utilities.direction.DownLeft: grabbedItem.getRigidBody().AddForce(new Vector2(-throwForce, -throwForce), ForceMode2D.Impulse); break;
                case Utilities.direction.DownRight: grabbedItem.getRigidBody().AddForce(new Vector2(throwForce, -throwForce), ForceMode2D.Impulse); break;
            }
            grabbedItem.GetComponent<ThrowableItem>().setDirection(throwDirection);
            grabbedItem = null;
            hasItem = false;
        }
        if (isGrounded) state = Utilities.state.Idle;
        else state = Utilities.state.Air;      
    }

    /// <summary>
    /// Called when an Entity block incoming Attack or Projectile.
    /// </summary>
    public void onBlock()
    {
        Instantiate(effects[5], transform, false);
        if (speciality is Templar) speciality.onBlock();
    }
    /// <summary>
    /// Drops the current grabbedItem.
    /// </summary>
    public void dropItem()
    {
        grabbedItem.transform.parent = null;
        grabbedItem.getRigidBody().isKinematic = false;
        grabbedItem = null;
        hasItem = false;
    }
    /// <summary>
    /// Picks up the closest item in range.
    /// </summary>
    protected void pickupItem()
    {
        Collider2D[] itemsInRange = Physics2D.OverlapCircleAll(transform.position, 2f, pickableItems);
        int nearestItem = 0;
        float lowestDistance = 1000f;
        if(itemsInRange.Length > 0)
        {
            int totalItems = itemsInRange.Length;
            for(int i = 0; i < totalItems; i++)
            {
                float distance = Vector2.Distance(transform.position, itemsInRange[i].transform.position);
                if (distance < lowestDistance)
                {
                    lowestDistance = distance;
                    nearestItem = i;
                }
            }
            grabbedItem = itemsInRange[nearestItem].GetComponent<PickableItem>();
            if (!grabbedItem.getIsUsed() && !grabbedItem.getIsPicked()) {
                hasItem = true;
                grabbedItem.setIsUsed(false);
                grabbedItem.setIsPicked(true);
                grabbedItem.pickUp(pickUpSpot);
            }       
        }
        
    }
    /// <summary>
    /// Checks the item type, and does action according to it.
    /// </summary>
    /// <param name="dir"></param>
    protected virtual void useItem()
    {
        if (isGrounded) rbd.velocity = new Vector2(0, 0);

        if (grabbedItem is ThrowableItem)
        {
            state = Utilities.state.Throwing;
            anim.Play("Throw");
            anim.SetBool("holdAnimation", true);
        } 
        if(grabbedItem is OnUseItem)
        {
            grabbedItem.setIsUsed(true);
            grabbedItem.setIsPicked(false);
            grabbedItem.itemAction(this);
        }   
    }
    public virtual void checkChargeThrowing()
    {

    }
    public virtual void receiveDamage(float damage)
    {
        life.CurrentVal -= damage;
        if (life.CurrentVal < 0) life.CurrentVal = 0;
    }

    //Setters and Getters

    public bool isCharging()
    {
        return charging;
    }

    public float getChargeAmount()
    {
        return chargeAmount;
    }
    public float getLife()
    {
        return life.CurrentVal;
    }
    public void setLife(float l)
    {
        life.CurrentVal = l;
    }
    public Animator Animator
    {
        get { return anim; }
    }
    public Rigidbody2D RigidBody
    {
       get { return rbd; }
    }
    public Collider2D HurtBox
    {
        get { return hurtBox; }
    }
    public Collider2D HitBox
    {
        get { return hitbox; }
    }
    public SpriteRenderer Sprite
    {
        get { return sprite; }
    }
    public Utilities.state State
    {
        get { return state; }
        set { state = value; }
    }
    public Stat Life
    {
        get { return life; }
    }

    public bool getIsGrounded()
    {
        return isGrounded;
    }
    public float getMaxLife()
    {
        return life.MaxVal;
    }
    public Collider2D BoxCollider
    {
        get { return boxCollider; }
    }
    public void setAttackCharge(float c)
    {
        chargeAmount = c;
    }
    public float getGravityAmount() { return gravityAmount; }
    public Utilities.direction FacingDirection
    {
        get { return facingDirection; }
    }

    //Fixed Update//
    protected virtual void physUpdate()
    {
        lastVelocityY = rbd.velocity.y;

        doPhysicCasts();
    }
    /// <summary>
    /// Make all Physics OverlapCircles and Raycast for correct function of Character.
    /// </summary>
    protected virtual void doPhysicCasts()
    {
        isGrounded = Physics2D.OverlapBox(groundChecker.position, new Vector2(0.3f, 0.3f), 0, whatIsFloor);
        if(rbd.velocity.y!=0) isGrounded = false;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 30, 1<<15);
        if (hit.collider != null)
        {
            if (closerPlatform != hit.collider)
            {
                if (isIgnoringPlatforms)
                {
                    Physics2D.IgnoreCollision(boxCollider, closerPlatform, false);
                    Physics2D.IgnoreCollision(boxCollider, hit.collider, true);
                }
                closerPlatform = hit.collider;

            }
        }
    }



    //Coroutines
    /// <summary>
    /// Stuns Entity for duration of knockback.
    /// </summary>
    /// <param name="knockBack"></param>
    /// <returns></returns>
    virtual public IEnumerator hitStun(float knockBack)
    {
        if (attackEffect) Destroy(attackEffect);
        int frames = Mathf.FloorToInt(knockBack * 1.2f);
        int passedFrames = 0;
        int passedeffectFrames = 0;
        while(frames != passedFrames)
        {
            if(passedeffectFrames > 5)
            {
                if (!isGrounded && rbd.velocity.magnitude > 25)
                {
                    Instantiate(effects[3], groundChecker.position, Quaternion.identity);
                    passedeffectFrames = 0;
                }
            }
            yield return 0;
            passedFrames++;
            passedeffectFrames++;
        }
             
        if (isGrounded)
        {
            anim.Play("Ground_Movement");
            state = Utilities.state.Idle;
        }
        else
        {
            anim.Play("Falling");
            state = Utilities.state.Air;
        }
    }
    /// <summary>
    /// Small freeze delay for hit effect.
    /// </summary>
    /// <param name="frames"></param>
    /// <param name="isHit"></param>
    /// <returns></returns>
    public IEnumerator hitLag(float frames, bool isHit)
    {
        frames = Mathf.FloorToInt(frames/2);
        /*if (isHit)
        {
            yield return 0;
            GetComponent<SpriteRenderer>().color = new Color32(255, 235, 152, 255);
        }*/

        anim.enabled = false;
        int passedFrames = 0;
        while (frames != passedFrames)
        {
            yield return 0;
            passedFrames++;
        }

        //if (isHit) GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
        anim.enabled = true;
    }
    /// <summary>
    /// Full process of getting hit.
    /// </summary>
    /// <param name="hitDamage"></param>
    /// <param name="knockBack"></param>
    /// <param name="hit"></param>
    /// <returns></returns>
    public IEnumerator receiveHit(float hitDamage, float knockBack, Hitter hit)
    {
        hitEffect();
        rbd.gravityScale = gravityAmount;
        if (attackEffect) Destroy(attackEffect);
        state = Utilities.state.Hit;
        if (!(hit is Item))
        {
            if(hitlag!=null) StopCoroutine(hitlag);
            yield return hitlag = StartCoroutine(hitLag(hitDamage, true));
        }
        if (hitstun != null) StopCoroutine(hitstun);
        hit.knockBackEntity(this, knockBack);       
        hitstun = StartCoroutine(hitStun(knockBack));
        yield return 0;
    }
    /// <summary>
    /// Spawns effect when getting hit.
    /// </summary>
    public void hitEffect()
    {
        int r = UnityEngine.Random.Range(0, 3);
        Instantiate(effects[r], this.transform.position, Quaternion.identity);
    }
}
