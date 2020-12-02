using UnityEngine;
using System.Collections;

public class ArrowScript : MonoBehaviour {
    public GameObject target;
    [SerializeField]
    float minX = 0, maxX = 0, minY = 0, maxY = 0, startSize = 0;
    Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update () {
        Vector3 diff = target.transform.position - transform.position;
        diff.Normalize();
        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);

        this.transform.parent.position = new Vector3(target.transform.position.x, target.transform.position.y, 35);

        float size = cam.orthographicSize;

        if (transform.parent.localPosition.x > maxX)
        {
            float x = (size * maxX) / startSize;
            this.transform.parent.localPosition = new Vector3(x, transform.parent.localPosition.y, 35);
        }
        else if (transform.parent.localPosition.x < minX) {
            float x = (size * minX) / startSize;
            this.transform.parent.localPosition = new Vector3(x, transform.parent.localPosition.y, 35);
        }

        if (transform.parent.localPosition.y > maxY)
        {
            float y = (size * maxY) / startSize;
            this.transform.parent.localPosition = new Vector3(transform.parent.localPosition.x, y, 35);
        }
        else if (transform.parent.localPosition.y < minY)
        {
            float y = (size * minY) / startSize;
            this.transform.parent.localPosition = new Vector3(transform.parent.localPosition.x, y, 35);
        } 
    }
}
