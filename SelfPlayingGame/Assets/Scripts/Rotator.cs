using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float Speed;
    public RotationDirection Direction;

    // Start is called before the first frame update
    void Start()
    {
        if( Direction == RotationDirection.CW)
        {
            gameObject.transform.localRotation = Quaternion.Euler(0,180,0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.localRotation *= Quaternion.Euler(0, 0, Time.deltaTime * Speed );
    }
}
