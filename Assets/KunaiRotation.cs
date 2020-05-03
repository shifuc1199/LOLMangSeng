using UnityEngine;
using System.Collections;

public class KunaiRotation : MonoBehaviour 
{
	public float xSpeed;
	public float ySpeed;
	public float zSpeed;
    Quaternion rotation;
    private void Awake()
    {
        rotation = transform.localRotation;
    }
    void OnDisable ()
	{
         
        transform.localRotation = rotation;

    }
	void Update ()
	{
		transform.Rotate (xSpeed * Time.deltaTime, ySpeed * Time.deltaTime, zSpeed * Time.deltaTime);
	}
}
