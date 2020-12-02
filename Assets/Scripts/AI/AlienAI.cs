using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienAI : Entity {

    public GameObject PJ;
    Animator animator;
    bool attack = false;
    public shot shots;
    internal float movementX = 0;
    bool canshot = true;
    //new private Utilities.direction facingDirection;

    // Use this for initialization
    private void Awake()
    {
        PJ = GameObject.FindGameObjectWithTag("Character");
        animator = GetComponent<Animator>();
        cam = Camera.main;
    }
    new void Start () {
        base.Start();
        
        
    }
	
	// Update is called once per frame
	new void Update () {
        base.Update();
        //Sigue la misma tecnica que el follow state un poco mas simple para perseguir al objetivo
        rbd.gravityScale = 1;
        if (getLife() > 0)
        {
            if (state == Utilities.state.Hit) {
                //rbd.constraints = RigidbodyConstraints2D.None;
                animator.Play("Hit");
            }
            else
            {
                if (transform.position.x < PJ.transform.position.x - 1)
                {
                    movementX = -0.6f;
                    facingDirection = Utilities.direction.Right;
                    
                }
                if (transform.position.x > PJ.transform.position.x + 1)
                {
                    movementX = 0.6f;
                    facingDirection = Utilities.direction.Left;

                }
                if (!attack)
                {
                    //rbd.constraints = RigidbodyConstraints2D.None;
                    if (Mathf.Abs(movementX) > 0.1f)
                    {
                        if (Mathf.Abs(movementX) > 0.5f)
                        {
                            if (movementX > 0) movementX = 1;
                            else movementX = -1;
                        }
                        else
                        {
                            if (movementX > 0) movementX = 0.5f;
                            else movementX = -0.5f;
                        }
                        GetComponent<Rigidbody2D>().velocity = new Vector2(movementX * 4, GetComponent<Rigidbody2D>().velocity.y);
                    }
                    else GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x * 0.9f, GetComponent<Rigidbody2D>().velocity.y);

                    if (transform.position.x < PJ.transform.position.x - 3)
                    {
                        GetComponent<Rigidbody2D>().velocity = new Vector2(1 * 4, GetComponent<Rigidbody2D>().velocity.y);
                        movementX = -0.6f;
                    }
                    else if (transform.position.x > PJ.transform.position.x + 3)
                    {
                        GetComponent<Rigidbody2D>().velocity = new Vector2(-1 * 4, GetComponent<Rigidbody2D>().velocity.y);
                        movementX = 0.6f;
                    }
                }
                if (movementX > 0 && facingDirection == Utilities.direction.Left)
                {
                    transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
                }
                else if (movementX < 0 && facingDirection == Utilities.direction.Right)
                {
                    transform.localScale = new Vector3(-2.5f, 2.5f, 2.5f);
                }
                detectDistance();
            }
        }else { 
            anim.Play("alien_death");
        }
    }
    public void detectDistance()
    {
        float distanceSqr = (transform.position - PJ.transform.position).sqrMagnitude;
        //En caso que el objetivo este a menos de 30 de distancia dispararan un proyectil
        if (distanceSqr < 30)
        {
            attack = true;
            if (canshot)
            {
                    animator.SetBool("idle", false);
                    animator.SetBool("shot", true);
                    shot shot = Instantiate(shots, new Vector2(transform.localPosition.x - (movementX * 2), transform.localPosition.y), transform.rotation);
                    shot.GetComponent<shot>().direction = movementX*-1;
                    shot.GetComponent<shot>().alien = this;
                    canshot = false;
                    startDelay();
            }
            else { animator.SetBool("idle", true); }
        }
        else { attack = false; animator.SetBool("shot", false); }
           
    }
    public void death()
    {
        //En caso que el alien muera aumenta la cuenta de enemigos eliminados de la pelea
        if (GameManager.Instance.currentFight is WaveFight)
        {
            ((WaveFight)GameManager.Instance.currentFight).enemiesdead += 1;
        }else
        ((TeamWaveFight)GameManager.Instance.currentFight).enemiesdead += 1;
        Destroy(this.gameObject);
    }
    public void startDelay()
    {
        StartCoroutine(delay());
    }
    IEnumerator delay()
    {
        yield return new WaitForSeconds(2f);
        canshot = true;
    }
}

