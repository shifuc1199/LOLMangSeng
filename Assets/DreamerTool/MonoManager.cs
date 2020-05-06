using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using DreamerTool.Singleton;
using DreamerTool.GameObjectPool;
public class UpdateAction
{
    public Func<bool> Action;
    public UnityAction OnComplete;
    public UpdateAction(Func<bool> ac)
    {
        Action = ac;
         
    }
}
public class InvokeRepeatAction
{
    public Func<bool> Action;
    public UnityAction FrameAction;
    public float repeatTime;
    float timer;
    public InvokeRepeatAction(float repeatTime, UnityAction FrameAction, Func<bool> ac)
    {
        this.FrameAction = FrameAction;
        this.repeatTime = repeatTime;
        this.Action = ac;
    }
    public bool Excute()
    {
        FrameAction();
        timer += Time.deltaTime;
        if(timer>=repeatTime)
        {
            timer = 0;
           return Action();
        }
        return true;
    }
}

public class MonoManager : MonoSingleton<MonoManager>
{
    public Dictionary<string, InvokeRepeatAction> InvokeRepeatAction = new Dictionary<string, InvokeRepeatAction>();
    public Dictionary<string, Dictionary<UpdateType, UpdateAction>> UpdateAction = new Dictionary<string, Dictionary<UpdateType, UpdateAction>>();
    void HandleInvokeRepeatAction()
    {
        if (InvokeRepeatAction.Count == 0)
        {
            return;
        }
        var actionList = new List<string>(InvokeRepeatAction.Keys);
        foreach (var actionKey in actionList)
        {   
            if (!InvokeRepeatAction[actionKey].Excute())
            {
                 
                InvokeRepeatAction.Remove(actionKey);
            }
 
        }
    }
    public void AddInvokeRepeatAction(string id, InvokeRepeatAction ac)
    {
        if(InvokeRepeatAction.ContainsKey(id))
        {
            InvokeRepeatAction[id] = ac;
        }
        else
        {
            InvokeRepeatAction.Add(id, ac);
        }
    }
    public void RemoveUpdateAction(string id, UpdateType updateType)
    {
        if (!UpdateAction.ContainsKey(id))
        {
            return;
        }
        if(!UpdateAction[id].ContainsKey(updateType))
        {
            return;
        }
        UpdateAction[id][updateType].OnComplete?.Invoke();
        UpdateAction[id].Remove(updateType);
        if (UpdateAction[id].Count == 0)
        {
            UpdateAction.Remove(id);
        }

    }
    public void AddUpdateAction(string id,UpdateType updateType, UpdateAction ac)
    {
        
        if (!UpdateAction.ContainsKey(id))
        {
            UpdateAction.Add(id, new Dictionary<UpdateType, UpdateAction>() { {updateType, ac } });
           
        }
        else
        {
            if (UpdateAction[id].ContainsKey(updateType))
            {
                UpdateAction[id][updateType] = ac;
            }
            else
            {
                UpdateAction[id] = new Dictionary<UpdateType, UpdateAction>() { { updateType, ac } };
            }
        }
    }
    void HandleUpdateAction()
    {
        if (UpdateAction.Count == 0)
            return;

        var gameObjectLst = new List<string>(UpdateAction.Keys);
        foreach (var gameObject in gameObjectLst)
        {
            var updateTypeLst = new List<UpdateType>(UpdateAction[gameObject].Keys);
            foreach (var updateType in updateTypeLst)
            {
                if (!UpdateAction[gameObject][updateType].Action())
                {
                    UpdateAction[gameObject][updateType].OnComplete?.Invoke();
                    UpdateAction[gameObject].Remove(updateType);
                }
            }
            if (UpdateAction[gameObject].Count == 0)
            {
                UpdateAction.Remove(gameObject);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        HandleInvokeRepeatAction();
        HandleUpdateAction();


    }
}
public enum UpdateType
{
    MoveTo
}
public static class Extra
{
    public static void StopMoveTo(this Transform transform)
    {
        MonoManager.Instance.RemoveUpdateAction(transform.gameObject.GetInstanceID().ToString(), UpdateType.MoveTo);
    }

    public static UpdateAction MoveTo(this Transform transform, Vector3 pos, float Movespeed, float distance = 0.1f, bool isRotate=false,float rotateSpeed=0)
    {
        var action = new UpdateAction(
               () => {
                   if(isRotate)
                   {
                       transform.forward = Vector3.Slerp(transform.forward, (pos - transform.position).normalized, Time.deltaTime * rotateSpeed);
                   }
                   transform.position = Vector3.MoveTowards(transform.position, pos, Time.deltaTime * Movespeed);
                   return Vector3.Distance(transform.position, pos) >= distance;
               }
            );
        MonoManager.Instance.AddUpdateAction(transform.gameObject.GetInstanceID().ToString(),UpdateType.MoveTo, action);
        return action;
    }
}