using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.Singleton;
using DreamerTool.GameObjectPool;

public class FlashCommoand : SetPosCommoand
{
    public FlashCommoand(HeroUnit unit, Vector3 pos) : base(unit, pos)
    {

    }
    public override void Execute()
    {
        if (Vector3.Distance(unit.GetPos(), aimPos) >= 3)
        {
            var dir = (aimPos - unit.GetPos()).normalized;
            aimPos = unit.GetPos() + dir * 3;
            (unit as HeroUnit).Flash(aimPos);
        }
        else
            (unit as HeroUnit).Flash(aimPos);
    }
}
public class SetPosCommoand : Commoand
{
    protected Vector3 aimPos;
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
public class AttackCommoand : Commoand
{
    Unit attackUnit;
    public AttackCommoand(Unit unit,Unit attackUnit)
    {
        this.unit = unit;
        this.attackUnit = attackUnit;
    }
    public override void Execute()
    {
        (unit as HeroUnit).SetHeroState(HeroState.Attack, attackUnit);
    }
}

 

public class InputHandler : Singleton<InputHandler>
{
    public Unit enemyUnit { get; private set; }
    public HeroUnit controlUnit { get; private set; }
    public void SelectControlUnit(HeroUnit unit)
    {
        controlUnit = unit;
    }
    
    public void UnSelectEnemyUnit()
    {
       enemyUnit = null;
    }
    public void SelectEnemyUnit(Unit unit)
    {
        enemyUnit = unit;
    }
    public Commoand HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (enemyUnit != null)
            {
                return new AttackCommoand(controlUnit,enemyUnit);
            }
            else
            {
                var hit = GameStaticMethod.GetMouseRayCastHit(LayerMask.GetMask("Ground"));
                if (hit.collider != null)
                {
                    GameObjectPoolManager.GetPool("click_move").Get(hit.point, Quaternion.identity, 1);
                    return new MoveToCommoand(controlUnit, hit.point);
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            if(enemyUnit!=null)
                return new SkillCommoand(controlUnit, SkillType.W, SkillExcuteType.Directivity,enemyUnit);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (controlUnit.skillDict[SkillType.Q].GetSkillTarget() != null)
            {
                return new SkillCommoand(controlUnit, SkillType.Q, SkillExcuteType.Directivity);
            }
            else
            {
                var hit = GameStaticMethod.GetMouseRayCastHit(LayerMask.GetMask("Ground"));
                if (hit.collider != null)
                    return new SkillCommoand(controlUnit, SkillType.Q, SkillExcuteType.Line, (hit.point-controlUnit.GetPos()).normalized);
            }
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            var hit = GameStaticMethod.GetMouseRayCastHit(LayerMask.GetMask("Ground"));
            if (hit.collider != null)
                return new FlashCommoand(controlUnit, hit.point);
        }
       
        return null;
    }
}
public class SkillCommoand : Commoand
{
    SkillType skillType;
    object[] skillParam;
    public SkillCommoand (Unit unit,SkillType skillType,params object[] skillParam)
    {
        this.skillParam = skillParam;
        this.skillType = skillType;
        this.unit = unit;
    }
    public override void Execute()
    {
 
        (unit as HeroUnit).ExcuteSkill(skillType, skillParam);
    }
}
public class MoveToCommoand : Commoand
{
    Vector3 aimPos;

    public MoveToCommoand(Unit unit, Vector3 pos)
    {

        this.unit = unit;
        this.aimPos = pos;
    }
    public override void Execute()
    {
        unit.MoveTo(aimPos);
    }
}
