using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementObj : MonoBehaviour
{
    private Vector3 originalPos;
    private Vector3 newPos;
    private float posAngle;
    float x = 0;
    float y = 0;
    float radius = 1f;

    GameObject cylinderPivot;
    float cylinderAngle;
    float cylinderDirection = 1f;
    float cylinderMoveAmount = 30f;
    float cylinderMoveSpeed = 10f;

    // Start is called before the first frame update
    void Start()
    {
        originalPos = transform.position;
        posAngle = 0f;
        cylinderPivot = transform.Find("Pivot").gameObject;
        print(cylinderPivot);
        cylinderAngle = Random.Range(-10f, 10f);
    }

    // Update is called once per frame
    void Update()
    {
        //game object movement on circle
        posAngle += 1 * Time.deltaTime;
        x = Mathf.Cos(posAngle) * radius;
        y = Mathf.Sin(posAngle) * radius;

        newPos = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);
        transform.position = newPos;


        //cylinder movement back and forth
        cylinderAngle += (cylinderMoveSpeed * cylinderDirection) * Time.deltaTime;

        if(cylinderAngle > cylinderMoveAmount || cylinderAngle < -cylinderMoveAmount)
        {
            cylinderAngle = cylinderMoveAmount * cylinderDirection;
            cylinderDirection *= -1;
        }

        cylinderPivot.transform.rotation = Quaternion.Euler(new Vector3(90, 0, cylinderAngle));

    }
}
