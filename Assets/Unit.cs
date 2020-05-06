using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.Singleton;
using DreamerTool.GameObjectPool;
public class HeroUnit : Unit
{
    public HeroState heroState = HeroState.Idle;
    protected Animator anim;
    protected Rigidbody rigi;

    Vector3 aimPos;
    HeroUnit enemyUnit;
    public HeroUnit(Transform transform, Animator anim = null, Rigidbody rigi = null) : base(transform)
    {
        this.rigi = rigi;
        this.anim = anim;
    }
 
    public void PlayAnim(AnimParamType apt,params object[] param)
    {
        if (anim == null)
        {
            Debug.LogError("Anim is Null");
            return;
        }
        switch (apt)
        {
            case AnimParamType.Bool:
                anim.SetBool(param[0].ToString(), (bool)param[1]);
                break;
            case AnimParamType.Trigger:
                anim.SetTrigger(param[0].ToString());
                break;
            default:
                break;
        }
    }
  
    public override void MoveTo(Vector3 aimPos)
    {
        GameObjectPoolManager.GetPool("click_move").Get(aimPos, Quaternion.identity, 1);
        this.aimPos = aimPos;
        heroState = HeroState.Run;
        PlayAnim(AnimParamType.Bool, "run", true);
    }
    public void SetHeroState(HeroState state, params object[] param)
    {
        switch (state)
        {
            case HeroState.Idle:
                break;
            case HeroState.Run:
                break;
            case HeroState.Attack:
                enemyUnit = (param[0] as HeroUnit);
                break;
            default:
                break;
        }
        heroState = state;
    }
    public override void SetPos(Vector3 aimPos)
    {
        AudioManager.Instance.PlayOneShot("flash");
        GameObjectPoolManager.GetPool("flash_effect").Get(transform.position, Quaternion.identity,2);
        transform.forward = (aimPos - transform.position).normalized;
        transform.position = aimPos;
        GameObjectPoolManager.GetPool("flash_effect2").Get(transform.position, Quaternion.identity, 2);
    }


    float timer;

    public override void Update()
    {
 
        switch (heroState)
        {
            case HeroState.Idle:
                break;
            case HeroState.Run:
                if(Vector3.Distance(transform.position,aimPos)<=0.1f)
                {
                    PlayAnim(AnimParamType.Bool, "run", false);
                    heroState = HeroState.Idle;
                    return;
                }
                transform.forward = Vector3.Slerp(transform.forward, (aimPos - transform.position).normalized, Time.deltaTime * 20);
                transform.position = Vector3.MoveTowards(transform.position, aimPos, Time.deltaTime * 2.5f);
                break;
            case HeroState.Attack:
                if (Vector3.Distance(transform.position, enemyUnit.GetPos()) <= 1.5f)
                {
                    PlayAnim(AnimParamType.Bool, "run", false);
                    timer += Time.deltaTime;
                    if (timer >= 1)
                    {
                        PlayAnim(AnimParamType.Trigger, "attack");
                        timer = 0;
                    }
                    return;
                }
                PlayAnim(AnimParamType.Bool, "run", true);
     
                transform.forward = Vector3.Slerp(transform.forward, (enemyUnit.GetPosNoY() - transform.position).normalized, Time.deltaTime * 20);
                transform.position = Vector3.MoveTowards(transform.position, enemyUnit.GetPosNoY(), Time.deltaTime * 2.5f);
                break;
            default:
                break;
        }
    }
}
public class Unit
{
    protected Transform transform;

    public Unit(Transform transform)
    {
        this.transform = transform;
    }
    public void SetForward(Vector3 forward)
    {
        transform.forward = forward;
    }
    public Vector3 GetPosNoY()
    {
        return new Vector3(transform.position.x,0, transform.position.z);
    }
    public Vector3 GetPos()
    {
        return transform.position;
    }
    public virtual void SetPos(Vector3 aimPos)
    {
        transform.position = aimPos;
    }
    public virtual void MoveTo(Vector3 aimPos)
    {
        transform.MoveTo(new Vector3(aimPos.x,transform.position.y, aimPos.z), 10, 0.1f, true,20);
    }
    public virtual void Update()
    {

    }
}

