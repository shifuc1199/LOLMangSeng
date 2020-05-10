using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroUnitController : MonoBehaviour
{

    public HeroUnit unit { get; private set; }
    // Start is called before the first frame update
    void Awake()
    {
        unit = new HeroUnit(transform,GetComponentInChildren<Animator>(),GetComponent<Rigidbody>());
         
    }

    private void OnMouseEnter()
    {
        InputHandler.Instance.SelectEnemyUnit(unit);
        GameStaticMethod.ChangeCursor(CursorType.Fight);
    }
    private void OnMouseExit()
    {
        InputHandler.Instance.UnSelectEnemyUnit();
        GameStaticMethod.ChangeCursor(CursorType.Idle);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
