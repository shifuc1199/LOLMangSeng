using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.GameObjectPool;
public class HeroAnimationEvent : MonoBehaviour
{
     
    HeroUnit controlUnit;
   
    private void Start()
    {
        controlUnit = GetComponentInParent<HeroUnitController>().unit;
    }

    public void Skill_Q_Enter()
    {
        AudioManager.Instance.PlayOneShot("skill_q_2");
    }
        public void Skill_Q_2_Update()
    {
        var skillTargetUnit = controlUnit.skillDict[SkillType.Q].GetSkillTarget();
        if (Vector3.Distance(controlUnit.GetPosNoY(), skillTargetUnit.GetPosNoY())<=0.1f)
        {
            (skillTargetUnit as HeroUnit).RemoveStatus(StatusType.Q);
            controlUnit.skillDict[SkillType.Q].SetSkillTarget(null);
            (controlUnit as HeroUnit).PlayAnim(AnimParamType.Trigger, "Q_2_End");
            return;
        }
        controlUnit.SetForward((skillTargetUnit.GetPosNoY()-controlUnit.GetPosNoY ()).normalized);
        controlUnit.SetPos(Vector3.MoveTowards(controlUnit.GetPosNoY(), skillTargetUnit.GetPosNoY(),Time.deltaTime*10));
    }
    public void SkillWEnter()
    {
        AudioManager.Instance.PlayOneShot("skill_w");
    }
    public void SkillWUpdate()
    {
        var skillTargetUnit = controlUnit.skillDict[SkillType.W].GetSkillTarget();
        if (skillTargetUnit == null)
            return;
        if (Vector3.Distance(controlUnit.GetPosNoY(), skillTargetUnit.GetPosNoY()) <= 0.1f)
        {
            controlUnit.skillDict[SkillType.W].SetSkillTarget(null);
            (controlUnit as HeroUnit).PlayAnim(AnimParamType.Trigger, "W_End");
            return;
        }
        controlUnit.SetForward((skillTargetUnit.GetPosNoY() - controlUnit.GetPosNoY()).normalized);
        controlUnit.SetPos(Vector3.MoveTowards(controlUnit.GetPosNoY(), skillTargetUnit.GetPosNoY(), Time.deltaTime * 10));
    }
    public void SkillQEnter()
    {
        AudioManager.Instance.PlayOneShot("skill_q");
        var skillDir = controlUnit.skillDict[SkillType.Q].GetSkillDir();
        controlUnit.SetForward(skillDir);
    }
    public void ExceuteSkillQ()
    {
         
        var skill_q = GameObjectPoolManager.GetPool("skill_q_projectile").Get(transform.position + new Vector3(0,0.5f,0),transform.rotation, 1);
        skill_q.GetComponent<SkillMissile>().ownerHero = controlUnit;
    }
     public void Hit()
    {
        AudioManager.Instance.PlayOneShot("attack");
        GameObjectPoolManager.GetPool("hit").Get(transform.position+transform.forward*2+new Vector3(0,1f,0), Quaternion.identity, 1);
    }
}
