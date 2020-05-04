using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.Singleton;
using DreamerTool.GameObjectPool;
 
public class Unit
{
   
    Transform transform;
    Animator anim;

    bool isMoveTo;
    Vector3 pos;

    public Unit(Transform transform, Animator anim = null)
    {
        this.anim = anim;
        this.transform = transform;
    }
    public Vector3 GetPos()
    {
        return transform.position;
    }
    public void Setpos(Vector3 aimPos)
    {
        var FlashEffet = Resources.Load<GameObject>("FlashEffet");
        var FlashEffet2 = Resources.Load<GameObject>("FlashEffet_1");
        if (isMoveTo)
        {
            isMoveTo = false;
            anim.SetBool("run", false);
        }
        AudioManager.Instance.PlayOneShot("flash");
        Object.Instantiate(FlashEffet2, transform.position, Quaternion.identity);
        transform.forward = (aimPos - transform.position).normalized;
        transform.position = aimPos;
        Object.Instantiate(FlashEffet, transform.position, Quaternion.identity);
    }
    public void MoveTo(Vector3 aimPos)
    { 
        isMoveTo = true;
        pos = aimPos;
    }
    public void Update()
    {
        if (isMoveTo)
        {
            if(Vector3.Distance(transform.position, pos) <= 0.1f)
            {
                anim.SetBool("run", false);
                isMoveTo = false;
                return;
            }
            transform.forward = Vector3.Slerp(transform.forward, (pos - transform.position).normalized, Time.deltaTime * 5);
            transform.position = Vector3.MoveTowards(transform.position, pos, Time.deltaTime * 2);
        }
        anim.SetBool("run", isMoveTo);
    }
}
public class FlashCommoand : SetPosCommoand
{
    public FlashCommoand(Unit unit, Vector3 pos):base(unit,pos)
    {

    }
    public override void Execute()
    {
        if(Vector3.Distance(unit.GetPos(),aimPos)>=3)
        {
            var dir = (aimPos - unit.GetPos()).normalized;
            aimPos = unit.GetPos() + dir * 3;
            unit.Setpos(aimPos);
        }
        else
            unit.Setpos(aimPos);
    }
}
public class SetPosCommoand:Commoand
{
    protected Vector3 aimPos;
    public SetPosCommoand(Unit unit, Vector3 pos)
    {
        this.unit = unit;
        this.aimPos = pos;
    }
    public override void Execute()
    {
        unit.Setpos(aimPos);
    }
 
}
public abstract class Commoand 
{
    protected Unit unit;
    public abstract void Execute();
}
public class InputHandler : Singleton<InputHandler>
{
    Unit unit;
    public void SelectUnit(Unit unit)
    {
        this.unit = unit;
    }
    public Commoand HandleInput()
    {
        if(Input.GetKeyDown(KeyCode.Mouse1))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray,out hit,LayerMask.GetMask("Ground")))
            {
                return new MoveToCommoand(unit, hit.point);
            }
            else
            {
                return null;
            }
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, LayerMask.GetMask("Ground")))
            {
                return new FlashCommoand(unit, hit.point);
            }
            else
            {
                return null;
            }
        }
        return null;
    }
}

public class MoveToCommoand : Commoand
{
    Vector3 aimPos;
    public MoveToCommoand(Unit unit,Vector3 pos)
    {
        this.unit = unit;
        this.aimPos = pos;
    }
    public override void Execute()
    {
        unit.MoveTo(aimPos);
    }
}
