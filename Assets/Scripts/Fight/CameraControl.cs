using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

    Camera cam;
    public List<Transform> playersAlive;
    public List<Entity> playersEntity;
    Transform leftPlayer, rightPlayer;
    int totalPlayers;
    Vector3 middlePoint;
    float maxZ = -40, minZ = -30, maxX = 0, minX = 0, maxY = 0, minY = 0, z = -30;
    [SerializeField]
    float mapWidth, mapHeight;

     


    private void Start()
    {
        cam = GetComponent<Camera>();
        totalPlayers = playersEntity.Count;

        calculatelimits(); 
    }

    private void calculatelimits()
    {
        float vertExtent = cam.orthographicSize;
        float horzExtent = vertExtent * Screen.width / Screen.height;

        minX = horzExtent - mapWidth / 2.0f;
        maxX = mapWidth / 2.0f - horzExtent;
        minY = vertExtent - mapHeight / 2.0f;
        maxY = mapHeight / 2.0f - vertExtent;
    }

    private void FixedUpdate()
    {
        if (anyoneAlive())
        {
            if (playersAlive.Count > 1)
            {
                leftPlayer = playersAlive[0];
                rightPlayer = playersAlive[1];
                foreach(Transform pl in playersAlive)
                {
                    if (pl.position.x < leftPlayer.position.x) leftPlayer = pl;
                    else if (pl.position.x > rightPlayer.position.x) rightPlayer = pl;
                }
                float distance = Vector3.Distance(leftPlayer.position, rightPlayer.position);
                float half = (distance / 2);

                middlePoint = (rightPlayer.position - leftPlayer.position).normalized * half;
                middlePoint += leftPlayer.position;
                middlePoint = new Vector3(middlePoint.x, middlePoint.y+2, z);

                z = half * -2f; //cam.orthographicSize = 2 * (half/2);
                if (z < maxZ) z = maxZ;
                if (z > minZ) z = minZ;

                calculatelimits();                
                if (middlePoint.x < minX) middlePoint = new Vector3(minX, middlePoint.y, z);
                if (middlePoint.x > maxX) middlePoint = new Vector3(maxX, middlePoint.y, z);
                if (middlePoint.y < minY) middlePoint = new Vector3(middlePoint.x, minY, z);
                if (middlePoint.y > maxY) middlePoint = new Vector3(middlePoint.x, maxY, z);
                transform.position = Vector3.Lerp(transform.position, middlePoint, Time.deltaTime * 2.5f);

            }
            else
            {
                middlePoint = new Vector3(playersAlive[0].position.x, playersAlive[0].position.y+2, z);

                z = minZ;

                calculatelimits();
                if (middlePoint.x < minX) middlePoint = new Vector3(minX, middlePoint.y, z);
                if (middlePoint.x > maxX) middlePoint = new Vector3(maxX, middlePoint.y, z);
                if (middlePoint.y < minY) middlePoint = new Vector3(middlePoint.x, minY, z);
                if (middlePoint.y > maxY) middlePoint = new Vector3(middlePoint.x, maxY, z);
                transform.position = Vector3.Lerp(transform.position, middlePoint, Time.deltaTime * 2.5f);
            }
        }
        else
        {
            z = maxZ;
            middlePoint = new Vector3(0, 0, z);
            transform.position = Vector3.Lerp(transform.position, middlePoint, Time.deltaTime * 5);
        }
    }


    private bool anyoneAlive()
    {
        playersAlive.Clear();
        foreach(Entity e in playersEntity)
        {
            if (!e.isDead) playersAlive.Add(e.transform);
        }
        if (playersAlive.Count > 0) return true;
        else return false;
    }
}
