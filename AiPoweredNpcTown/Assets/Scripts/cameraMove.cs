using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraMove : MonoBehaviour
{

    public float speed;
    private Vector3 startPos;
    void Start()
    {
        startPos = transform.position;
    }

    
    void Update()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.position = new Vector3(transform.position.x + ( speed * Time.deltaTime), transform.position.y, transform.position.z);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.position = new Vector3(transform.position.x - (speed * Time.deltaTime), transform.position.y, transform.position.z);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + (speed * Time.deltaTime), transform.position.z);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - (speed * Time.deltaTime), transform.position.z);
        }
        if (Input.GetKey(KeyCode.Space))
        {
            transform.position = startPos;
        }


        //border check
        if (transform.position.x <= startPos.x)
        {
            transform.position = new Vector3(startPos.x, transform.position.y, transform.position.z);
        }
        if (transform.position.y >= startPos.y)
        {
            transform.position = new Vector3(transform.position.x, startPos.y, transform.position.z);
        }

    }
}
