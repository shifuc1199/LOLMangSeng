using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
 
public abstract class Commoand
{
    protected Unit unit;
    public abstract void Execute();
}
public class MoveToCommoand : Commoand
{
    protected Vector3 aimPos;

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