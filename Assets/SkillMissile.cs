using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.GameObjectPool;
public class SkillMissile : AutoMoveObject
{
    public HeroUnit ownerHero;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Unit"))
        {
            
           AudioManager.Instance.PlayOneShot("skill_q_hit");
           GetComponent<ObjectRecover>().RecoverImmediately();
         
          ownerHero.skillDict[SkillType.Q] .SetSkillTarget( 
              other.GetComponent<HeroUnitController>().unit
              , (unit) => { (unit as HeroUnit).SetStatus(StatusType.Q,new Status(
                  (callBackType) =>
                  {
                     
                      switch (callBackType)
                      {
                          case CallBackType.Add:
                                GameObjectPoolManager.GetPool("skill_q_hit").Get(unit.GetTransform(),-1);
                              break;
                          case CallBackType.Remove:
 
                              GameObjectPoolManager.GetPool("skill_q_hit").RecoverAll();
                              break;
                          default:
                              break;
                      }
                  }
                  )); } 
              );
            
        }
    }
}
