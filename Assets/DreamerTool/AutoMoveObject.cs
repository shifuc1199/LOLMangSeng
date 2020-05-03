/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoMoveObject : MonoBehaviour
{
    public float Speed;
    public Space space_type;
    public Vector3 Direction;

  
    void Update()
    {
     
        transform.Translate(Direction.normalized * Speed * Time.deltaTime, space_type);

    }
}
 