using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.Singleton;
using DreamerTool.GameObjectPool;

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
                return new AttackCommoand(controlUnit, enemyUnit);
            }
            else
            {
                var hit = GameStaticMethod.GetMouseRayCastHit(LayerMask.GetMask("Ground"));
                if (hit.collider != null)
                {
                    GameObjectPoolManager.GetPool("click_move").Get(hit.point, Quaternion.identity, 1);
                    return new HeroMoveToCommoand(controlUnit, hit.point);
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.W))
        {

            if (enemyUnit != null)
            {
                if (Vector3.Distance(enemyUnit.GetPosNoY(), controlUnit.GetPosNoY()) <= 3)
                {
                    return new SkillCommoand(controlUnit, SkillType.W, SkillExcuteType.Directivity, enemyUnit);
                }
                else
                {
                    return new SkillMoveToCommoand(controlUnit, enemyUnit.GetPos(),3, new SkillCommoand(controlUnit, SkillType.W, SkillExcuteType.Directivity, enemyUnit));
                }
            }

        }
        if (Input.GetKeyDown(KeyCode.R))
        {

            if (enemyUnit != null)
            {
                if (Vector3.Distance(enemyUnit.GetPosNoY(), controlUnit.GetPosNoY()) <= 3)
                {
                    return new SkillCommoand(controlUnit, SkillType.R, SkillExcuteType.Directivity, enemyUnit);
                }
                else
                {
                    return new SkillMoveToCommoand(controlUnit, enemyUnit.GetPos(), 3, new SkillCommoand(controlUnit, SkillType.R, SkillExcuteType.Directivity, enemyUnit));
                }
            }

        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            var hit = GameStaticMethod.GetMouseRayCastHit(LayerMask.GetMask("Ground"));
            if (hit.collider != null)
                return new PutEyeCommoand(controlUnit, hit.point);
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
                    return new SkillCommoand(controlUnit, SkillType.Q, SkillExcuteType.Line, (hit.point - controlUnit.GetPos()).normalized);
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