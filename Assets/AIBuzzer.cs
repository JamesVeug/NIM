using UnityEngine;
using System.Collections;

public class AIBuzzer : MonoBehaviour {
    private GameObject origin;
    bool incX = true;
    bool incY = false;
    bool incZ = true;

    bool spanned = false;
    private float scatterDistance = 0f;
    private float maxScatterDistance = 1f;
    private float scatterSpeed = 0.025f;
    private Vector3 minRange = new Vector3(-2, -2, -2);
    private Vector3 maxRange = new Vector3(2, 2, 2);
    private float moveScale = 500;
    private float maxSpeed = 0.10f;
    private float minSpeed = 0.025f;
    private float xInc;
    private float yInc;
    private float zInc;

    float x = 0;
    float y = 0;
    float z = 0.5f;
    float speed;

    // Use this for initialization
    void Start () {
        origin = transform.parent.gameObject;
        xInc = randomRange();
        yInc = randomRange();
        zInc = randomRange();
        speed = randomSpeed();

        x = -maxRange.x;
        y = maxRange.y;
        z = maxRange.z/2;
    }

    public void spanRange()
    {
        spanned = true;
        moveScale *= 2;
    }

    public void shrinkRange()
    {
        spanned = false;
        moveScale /= 2;
    }

    public float randomRange()
    {
        return Random.Range(minSpeed, maxSpeed);
    }

    public float randomSpeed()
    {
        return Random.Range(minSpeed, maxSpeed)* moveScale;
    }

    // Update is called once per frame
    void Update() {


        transform.LookAt(origin.transform);
        transform.Rotate(0, 180, 0);
        float zTransform = spanned ? scatterDistance + scatterSpeed : scatterDistance - scatterSpeed;
        if (zTransform > 0 && zTransform < maxScatterDistance)
        {
            transform.Translate(new Vector3(0, 0, spanned ? scatterSpeed : -scatterSpeed));
            scatterDistance = zTransform;
        }

        transform.RotateAround(origin.transform.position, new Vector3(x,y,z), speed * Time.deltaTime);

        if (incX) { x += xInc; } else { x -= xInc; };
        if (incY) { y += yInc; } else { y -= yInc; };
        if (incZ) { z += zInc; } else { z -= zInc; };

        if (x >= maxRange.x || x <= minRange.x) { incX = !incX; xInc = randomRange(); speed = randomSpeed(); }
        if (y >= maxRange.y || y <= minRange.y) { incY = !incY; yInc = randomRange(); speed = randomSpeed(); }
        if (z >= maxRange.z || z <= minRange.z) { incZ = !incZ; zInc = randomRange(); speed = randomSpeed(); }
        //Debug.Log("x " + x);
    }
}
