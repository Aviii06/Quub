using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldScript : MonoBehaviour
{
    private float PI = 3.14159f;
    public GameObject player;
    public float speed = 3.25f;
    bool transition = false;
    Quaternion currentRotation;
    Vector3 currentEulerAngles;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find ("Player");
    }

    void ChangeRotation()
    {
        transform.Rotate(0.0f, 0.0f, PI/2);

        currentEulerAngles = player.transform.eulerAngles;
        currentEulerAngles.z +=PI/2; 
        currentRotation.eulerAngles = currentEulerAngles; 
        player.transform.rotation = currentRotation;
        Vector3 translate = -transform.right * speed ;
        //player.transform.position += translate;

    }

    // Update is called once per frame
    void Update()
    {
        if(player.transform.position.x > 5.0f)
        {
           ChangeRotation();
        }
    }
}

