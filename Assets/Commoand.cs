using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.Singleton;
 
public class FlashCommoand : SetPosCommoand
{
    public FlashCommoand(Unit unit, Vector3 pos) : base(unit, pos)
    {

    }
    public override void Execute()
    {
        if (Vector3.Distance(unit.GetPos(), aimPos) >= 3)
        {
            var dir = (aimPos - unit.GetPos()).normalized;
            aimPos = unit.GetPos() + dir * 3;
            unit.SetPos(aimPos);
        }
        else
            unit.SetPos(aimPos);
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
    public Unit controlUnit { get; private set; }
    public void SelectControlUnit(Unit unit)
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
                    return new MoveToCommoand(controlUnit, hit.point);
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
