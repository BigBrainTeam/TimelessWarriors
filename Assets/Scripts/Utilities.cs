using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public static class Utilities {

    /// <summary>
    /// Enum for Fight statistics.
    /// </summary>
    public enum statistic { DAMAGEDONE, DAMAGERECEIVED, KOS, PERFECTS, KILLSPREE, DEATHS, FALLS, ULTIMATES, RANK, CURRENTSPREE };

    /// <summary>
    /// Enum for status conditions
    /// </summary>
    public enum statusCondition { NONE, BURN, SLOW, POISON};

    public enum state { Idle, Moving, Air, Edge, Blocking, AirDodging, Dodging, Attacking, WallSliding, Hit, Throwing, WaveDashing, Ultimate}
    /// <summary>
    /// Enum for every possible direction in-game.
    /// </summary>
    public enum direction { Up, Down, Left, Right, UpLeft, UpRight, DownLeft, DownRight };
    /// <summary>
    /// Enum for all the attack types in-game.
    /// </summary>
    public enum attackType { BasicFloorSide, BasicFloorUp, BasicFloorMoving, BasicAirSide, BasicAirUp, BasicAirDown, SpecialSide, SpecialUp, SpecialDown, Final, None = -1};
    public enum jumpType { Small, Normal, Double};


    /// <summary>
    /// Fade an image
    /// </summary>
    /// <param name="img">image that you want to fade</param>
    /// <param name="value">alpha value between 0f - 1f</param>
    /// <param name="time">time to end the fade</param>
    public static void fadeIn(Image img, float value, float time)
    {
        img.DOFade(value, time).SetUpdate(true);
    }

    /// <summary>
    /// Clear an image
    /// </summary>
    /// <param name="img">image that you want to fade</param>
    /// <param name="value">alpha value between 0f - 1f</param>
    /// <param name="time">time to end the clear</param>
    public static void fadeOut(Image img, float value, float time)
    {
        img.DOFade(value, time);
    }

    public static void AddExplosionForce(Rigidbody2D body, float expForce, Vector3 expPosition, float expRadius, Item it)
    {
        Entity target = body.gameObject.GetComponent<Entity>();
        target.receiveDamage(it.getDamage());
        expForce = expForce* 3;
        var dir = (body.transform.position - expPosition);
        float calc = 1 - (dir.magnitude / expRadius);
        if (calc <= 0)
        {
            calc = 0;
        }
        body.AddForce(dir.normalized*expForce * calc,ForceMode2D.Impulse);
        float lostLife = target.getMaxLife() - target.getLife();
        float totalKnckBack = calculateKnockBack(lostLife, it.getDamage(), it.getRigidBody().mass, it.getKnockBack());
        target.StartCoroutine(body.GetComponent<Entity>().hitStun(totalKnckBack));
    }


    public static void calculateDirectionForce(Entity target, ThrowableItem it)
    {
        target.receiveDamage(it.getDamage());
        Rigidbody2D trbd = target.GetComponent<Rigidbody2D>() ;
        Rigidbody2D itrbd = it.GetComponent<Rigidbody2D>();
        float lostLife = target.getMaxLife() - target.getLife();
        float itemknockBack = calculateKnockBack(lostLife, it.getDamage(), itrbd.mass, it.getKnockBack());

        switch (it.getDirection())
        {
            case Utilities.direction.Up: trbd.AddForce(new Vector2(0f, itemknockBack * 1.5f), ForceMode2D.Impulse); break;
            case Utilities.direction.Down: trbd.AddForce(new Vector2(0f, -itemknockBack), ForceMode2D.Impulse); break;
            case Utilities.direction.Left: trbd.AddForce(new Vector2(-itemknockBack, itemknockBack), ForceMode2D.Impulse); break;
            case Utilities.direction.Right: trbd.AddForce(new Vector2(itemknockBack, itemknockBack), ForceMode2D.Impulse); break;
            case Utilities.direction.UpLeft: trbd.AddForce(new Vector2(-itemknockBack, itemknockBack * 1.5f), ForceMode2D.Impulse); break;
            case Utilities.direction.UpRight: trbd.AddForce(new Vector2(itemknockBack, itemknockBack * 1.5f), ForceMode2D.Impulse); break;
            case Utilities.direction.DownLeft: trbd.AddForce(new Vector2(-itemknockBack, -itemknockBack), ForceMode2D.Impulse); break;
            case Utilities.direction.DownRight: trbd.AddForce(new Vector2(itemknockBack, -itemknockBack), ForceMode2D.Impulse); break;
        }

        target.StartCoroutine(target.hitStun(itemknockBack));
    }


    /// <summary>
    /// Returns the amount of KnockBack calculated.
    /// </summary>
    /// <param name="lostLife">Life entity has lost</param>
    /// <param name="damage">Amount of damage</param>
    /// <param name="mass">Mass of the target</param>
    /// <param name="knockBack">Base knockback</param>
    /// <returns></returns>
    public static float calculateKnockBack(float lostLife, float damage, float mass, float knockBack)
    {
        float factorEscalado = 1f; // este factor es para que ataques aumenten mas rapido el knockback que otros.
        float totalKnockBack = (((((lostLife) / 10) + (damage * lostLife / 50)) * (200 / (mass + 100))) * factorEscalado) + knockBack;
        /*Debug.Log("Damage: " + damage);
        Debug.Log("KnockBack: " + totalKnockBack);*/
        return totalKnockBack;
    }


}
