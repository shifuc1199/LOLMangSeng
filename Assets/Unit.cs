using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.Singleton;
using DreamerTool.GameObjectPool;
using UnityEngine.Events;
public class SkillModel
{
    private Vector3 skillDir;
    private Unit skillTarget;
    public Unit GetSkillTarget()
    {
        return skillTarget;
    }
    public Vector3 GetSkillDir()
    {
        return skillDir;
    }
    public void SetSkillTarget(Unit skillTarget, UnityAction<Unit> callBack=null)
    {
        if(skillTarget == null)
        {
            callBack?.Invoke(skillTarget);
            this.skillTarget = skillTarget;
        }
        else
        {
            this.skillTarget = skillTarget;
            callBack?.Invoke(skillTarget);
        }
              
    }
    public void SetSkillDir(Vector3 skillDir)
    {
        this.skillDir = skillDir;
    }
}
public class Status
{
    public UnityAction<CallBackType> CallBack;
  
    public Status(UnityAction<CallBackType> CallBack)
    {
        this.CallBack = CallBack;
    }
}
public class HeroUnit : Unit
{
    public HeroState heroState = HeroState.Idle;
    
    protected Animator anim;
    protected Rigidbody rigi;

    Vector3 aimPos;
    public Unit selectEnemyUnit { get; private set; }

    public Dictionary<StatusType, Status>  statusDict = new Dictionary<StatusType, Status>();
    public Dictionary<SkillType, SkillModel> skillDict = new Dictionary<SkillType, SkillModel>() {
        { SkillType.Q,new SkillModel()},
        { SkillType.W,new SkillModel()},
        { SkillType.R,new SkillModel()}
    };
    public void AddForce(Vector3 dir,float force)
    {
        if(rigi!=null)
        {
            rigi.AddForce(dir * force, ForceMode.Impulse);
        }
    }
    public void RemoveStatus(StatusType status)
    {
        if (!statusDict.ContainsKey(status))
        {
            return;
        }
        else
        {
 
            statusDict[status].CallBack?.Invoke(CallBackType.Remove);
            statusDict.Remove(status);
        }
    }
    public void SetStatus(StatusType statusType,Status status)
    {
        if(!statusDict.ContainsKey(statusType))
        {
            
            statusDict.Add(statusType, status);
            statusDict[statusType].CallBack?.Invoke(CallBackType.Add);
        }
        else
        {
     
            statusDict[statusType] = status;
            statusDict[statusType].CallBack?.Invoke(CallBackType.Add);
        }
    }
    public HeroUnit(Transform transform, Animator anim = null, Rigidbody rigi = null) : base(transform)
    {
        this.rigi = rigi;
        this.anim = anim;
    }
 
    public virtual void ExcuteSkill(SkillType skillType, object[] skillParam)
    {
        SetHeroState(HeroState.Idle);
       // var skillExcuteType = (SkillExcuteType)skillParam[0];
        switch (skillType)
        {
            case SkillType.Q:
                if(skillDict[SkillType.Q].GetSkillTarget() != null)
                {
                    PlayAnim(AnimParamType.Trigger, skillType.ToString()+"_2");
                    return;
                }
                skillDict[SkillType.Q].SetSkillDir((Vector3)skillParam[1]);
                break;
            case SkillType.W:
                skillDict[SkillType.W].SetSkillTarget(skillParam[1] as Unit);
                break;
            case SkillType.E:
                break;
            case SkillType.R:
                skillDict[SkillType.R].SetSkillTarget(skillParam[1] as Unit);
                break;
            default:
                break;
        }
        PlayAnim(AnimParamType.Trigger, skillType.ToString());
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
    private SkillCommoand currentSkill;
     
    public void SetHeroState(HeroState state, params object[] param)
    {
        switch (state)
        {
            case HeroState.Idle:
                PlayAnim(AnimParamType.Bool, "run", false);
                break;
            case HeroState.MoveTo:
                this.aimPos = (Vector3)param[0];
                PlayAnim(AnimParamType.Bool, "run", true);
                break;
            case HeroState.Attack:
                selectEnemyUnit = param[0] as Unit;
                break;
            case HeroState.PutEye:
                this.aimPos = (Vector3)param[0];
                PlayAnim(AnimParamType.Bool, "run", true);
                break;
            case HeroState.SkillMoveTo:
                this.aimPos = (Vector3)param[0];
                currentSkill = param[2] as SkillCommoand;
                PlayAnim(AnimParamType.Bool, "run", true);
                break;
            default:
                break;
        }
        heroState = state;
    }
    public void Flash(Vector3 aimPos)
    {
        AudioManager.Instance.PlayOneShot("flash");
        GameObjectPoolManager.GetPool("flash_effect").Get(transform.position, Quaternion.identity, 2);
        transform.forward = (aimPos - transform.position).normalized;
        base.SetPos(aimPos);
        GameObjectPoolManager.GetPool("flash_effect2").Get(transform.position, Quaternion.identity, 2);
    }


    float timer=1;

    public override void Update()
    {
 
        switch (heroState)
        {
            case HeroState.Idle:
                break;
            case HeroState.MoveTo:
                if(Vector3.Distance(transform.position,aimPos)<=0.1f)
                {
                    SetHeroState(HeroState.Idle);
                    return;
                }
                transform.forward = Vector3.Slerp(transform.forward, (aimPos - transform.position).normalized, Time.deltaTime * 20);
                transform.position = Vector3.MoveTowards(transform.position, aimPos, Time.deltaTime * 2.5f);
                break;
            case HeroState.Attack:
                transform.forward = Vector3.Slerp(transform.forward, (selectEnemyUnit.GetPosNoY() - transform.position).normalized, Time.deltaTime * 20);
                if (Vector3.Distance(transform.position, selectEnemyUnit.GetPos()) <= 2f)
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
                transform.position = Vector3.MoveTowards(transform.position, selectEnemyUnit.GetPosNoY(), Time.deltaTime * 2.5f);
                break;
            case HeroState.PutEye:
                if (Vector3.Distance(transform.position, aimPos) <= 5f)
                {
                    AudioManager.Instance.PlayOneShot("eye");
                    GameObjectPoolManager.GetPool("eye").Get(aimPos, Quaternion.identity, 5);
                    SetHeroState(HeroState.Idle);
                    return;
                }
                transform.forward = Vector3.Slerp(transform.forward, (aimPos - transform.position).normalized, Time.deltaTime * 20);
                transform.position = Vector3.MoveTowards(transform.position, aimPos, Time.deltaTime * 2.5f);
                break;
            case HeroState.SkillMoveTo:
                if (Vector3.Distance(transform.position, aimPos) <= 3)
                {
                    currentSkill.Execute();
                    return;
                }
                transform.forward = Vector3.Slerp(transform.forward, (aimPos - transform.position).normalized, Time.deltaTime * 20);
                transform.position = Vector3.MoveTowards(transform.position, aimPos, Time.deltaTime * 2.5f);
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
    public Transform GetTransform()
    {
        return transform;
    }
    public void SetForward(Vector3 forward)
    {
        transform.forward = forward;
    }
    public Vector3 GetPosNoY()
    {
        return new Vector3(transform.position.x,0, transform.position.z);
    }
    public Vector3 GetBack()
    {
        return -transform.forward;
    }
    public Vector3 GetForward()
    {
        return transform.forward;
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

