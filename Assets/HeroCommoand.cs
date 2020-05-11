using UnityEngine;
public class HeroMoveToCommoand : MoveToCommoand
{
    public HeroMoveToCommoand(HeroUnit unit, Vector3 pos) : base(unit, pos)
    {
    }
    public override void Execute()
    {
        (unit as HeroUnit).SetHeroState(HeroState.MoveTo, aimPos);
    }
}
public class SkillMoveToCommoand : MoveToCommoand
{
    SkillCommoand commoand;
    float distance;
    public SkillMoveToCommoand(HeroUnit unit, Vector3 pos,float distance,SkillCommoand commoand) : base(unit, pos)
    {
        this.distance = distance;
        this.commoand = commoand;
    }
    public override void Execute()
    {
        (unit as HeroUnit).SetHeroState(HeroState.SkillMoveTo, aimPos, distance,commoand);
    }
}
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
public class SkillCommoand : Commoand
{
    SkillType skillType;
    object[] skillParam;
    public SkillCommoand(Unit unit, SkillType skillType, params object[] skillParam)
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

public class AttackCommoand : Commoand
{
    Unit attackUnit;
    public AttackCommoand(Unit unit, Unit attackUnit)
    {
        this.unit = unit;
        this.attackUnit = attackUnit;
    }
    public override void Execute()
    {
        (unit as HeroUnit).SetHeroState(HeroState.Attack, attackUnit);
    }
}
public class PutEyeCommoand : Commoand
{
    protected Vector3 aimPos;
    public PutEyeCommoand(Unit unit, Vector3 aimPos)
    {
        this.unit = unit;
        this.aimPos = aimPos;


    }
    public override void Execute()
    {
        (unit as HeroUnit).SetHeroState(HeroState.PutEye, aimPos);
    }
}
