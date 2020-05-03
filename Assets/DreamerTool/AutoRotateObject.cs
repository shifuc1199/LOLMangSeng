using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotateObject : MonoBehaviour
{
    public float Speed;
    public Vector3 dir;
    public Space space_type;

    private void Update()
    {
        transform.Rotate(dir * Time.deltaTime * Speed, space_type);
    }
}
