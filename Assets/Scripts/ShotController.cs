using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotController : AttacksController
{
        static public int dmg = 6;
        internal float direction;
        //internal float direccion;

        protected override void initializeComponents()
        {
            base.initializeComponents();
            user = GetComponentInParent<Character>();
            attacks = new Attack[1];
            attacks[0] = new BasicSideAttack(user, 3, 6, false);
        }
    // Update is called once per frame
    /*void Update()
    {
    }*/
    public class BasicSideAttack : Attack
        {
            public BasicSideAttack(Entity user, float damage, float knockBack, bool air) : base(user, damage, knockBack)
            {
                if (air)
                {
                    name = "AirSide";
                    //animationName = "BasicAirSide";
                    stops = false;
                    chargeable = false;
                }
                else
                {
                    name = "FloorSide";
                    //animationName = "BasicFloorSide";
                    stops = true;
                    chargeable = true;
                }
                type = "Side";
            }
        }
    }
