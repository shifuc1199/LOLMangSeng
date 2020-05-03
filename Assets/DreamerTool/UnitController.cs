using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
 
    Unit unit;
    // Start is called before the first frame update
    void Awake()
    {
        unit = new Unit(transform,GetComponentInChildren<Animator>());
        InputHandler.Instance.SelectUnit(unit);
    }

    // Update is called once per frame
    void Update()
    {
        var commoand = InputHandler.Instance.HandleInput();
        if(commoand !=null)
        {
            commoand.Execute();
        }

        unit.Update();
    }
}
