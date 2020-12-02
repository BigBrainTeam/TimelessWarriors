using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAI : Character {
    public Entity target;
    public GameObject restpoint;
    public Character my;
    public Vector2 direction;
    public int augmentArea;
    public float augmentAngl;
    public bool attack = true;
    public bool dodge = true;
    public Vector2 location;
    public bool salto = true;
    public TeamFight actualfight;
    Entity friend;

    [HideInInspector] public AICharacterStates currentState;
    [HideInInspector] public AttackState attackState;
    [HideInInspector] public EvadeState evadeState;
    [HideInInspector] public FollowState followState;


    private void Awake()
    {
        my = this;
        target = GameObject.FindGameObjectWithTag("Character").GetComponent<Entity>();
        restpoint = GameObject.Find("AIrestpoint");
        attackState = new AttackState(this);
        evadeState = new EvadeState(this);
        followState = new FollowState(this);
    }

    new void Start()
    {
        base.Start();
        currentState = followState;
        //Si el combate es team fight le asigna el compañero de equipo a la variable friend
        if (GameManager.Instance.currentFight is TeamFight)
        {
            friend = ((TeamFight)GameManager.Instance.currentFight).getTeammate(my.player);
        }
        StartCoroutine(detectLocation());
    }

    new void Update()
    {
        base.Update();
        //Calcula la distancia entre la IA y el jugador mas cercano(target)
        Vector2 heading = target.transform.position - transform.position;
        float distance = heading.magnitude;
        direction = heading / distance;
        currentState.UpdateState();
        checkFacingDirection();
        target = GetClosestObject();
    }

 Entity GetClosestObject()
 {
        //Guarda todos los characters del combate en una lista
     GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("Character");
     Entity closestObject = null;
     for (int i = 0; i<objectsWithTag.Length;i++)
     {
         if(!closestObject)
         {
            closestObject = objectsWithTag[i].GetComponent<Entity>();
         }
         //compara las distancias y si es inferior asigna a closestobject el nuevo character
         if(Vector3.Distance(transform.position, objectsWithTag[i].GetComponent<Entity>().transform.position) <= Vector3.Distance(transform.position, closestObject.transform.position) && objectsWithTag[i].GetComponent<Entity>() != my)
         {
            closestObject = objectsWithTag[i].GetComponent<Entity>();
                if (friend != null)
                {
                        if (closestObject == friend)
                        {
                            if (player > 2)
                            {
                                int x = i - 1;
                                if (x == player) { x = x - 1; }
                                closestObject = objectsWithTag[x].GetComponent<Entity>();
                            }
                            else
                            {
                                int x = i + 1;
                                if (x == player) { x = x + 1; }
                                closestObject = objectsWithTag[x].GetComponent<Entity>();
                        }
                    }
                }
         }
     }
     return closestObject;
 }
/// <summary>
/// Revisa la posicion del target cada cierto tiempo
/// </summary>
/// <returns></returns>
    IEnumerator detectLocation()
    {
        location = target.transform.position;
        yield return new WaitForSeconds(1f);
        StartCoroutine(detectLocation());
    }
    public void startDelay()
    {
        StartCoroutine(delay());
    }
    IEnumerator delay()
    {
        yield return new WaitForSeconds(1f);
        attack = true;
        dodge = true;
    }
    public void startblockDelay()
    {
        StartCoroutine(blockdelay());
    }
    IEnumerator blockdelay()
    {
        yield return new WaitForSeconds(0.6f);
        dodge = true;
    }
    public int RandomNum()
    {
        int num = Random.Range(1, 100);
        return num;
    }
}
