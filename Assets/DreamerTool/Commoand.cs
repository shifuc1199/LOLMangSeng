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
    public void MoveTo(Vector3 aimPos)
    { 
        GameObjectPoolManager.GetPool("click_move").Get(aimPos, Quaternion.identity, 2);
        isMoveTo = true;
        pos = aimPos;
        anim.SetBool("run", true);
    }
    public void SetPos(Vector3 pos)
    {
        if (isMoveTo)
        {
            isMoveTo = false;
            anim.SetBool("run", false);
        }
        AudioManager.Instance.PlayOneShot("flash");
        GameObjectPoolManager.GetPool("flash_effect2").Get(transform.position, Quaternion.identity, 2);
        transform.forward =  (pos - transform.position).normalized;
        transform.position = pos;
        GameObjectPoolManager.GetPool("flash_effect").Get(transform.position, Quaternion.identity, 2);
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
    }
}
 
public class SetPosCommoand:Commoand
{
    Vector3 aimPos;
    public SetPosCommoand(Unit unit, Vector3 pos)
    {
        this.unit = unit;
        this.aimPos = pos;
    }
    public override void Execute()
    {
        unit.SetPos(aimPos);
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
                return new SetPosCommoand(unit, hit.point);
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
