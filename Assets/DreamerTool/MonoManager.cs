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

public class MonoManager : MonoSingleton<MonoManager>
{
    public Dictionary<string, Dictionary<UpdateType, UpdateAction>> UpdateAction = new Dictionary<string, Dictionary<UpdateType, UpdateAction>>();
    // Start is called before the first frame update
    void Awake()
    {
        GameObjectPoolManager.InitByScriptableObject();
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
    // Update is called once per frame
    void Update()
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
}
public enum UpdateType
{
    MoveTo
}
public static class Extra
{
    public static UpdateAction MoveTo(this Transform transform, Vector3 pos,bool isRotate=false)
    {
        var action = new UpdateAction(
               () => {
                   if(isRotate)
                   {
                       transform.forward = Vector3.Slerp(transform.forward, (pos - transform.position).normalized, Time.deltaTime * 5);
                   }
                   transform.position = Vector3.MoveTowards(transform.position, pos, Time.deltaTime * 2);
                   return Vector3.Distance(transform.position, pos) >= 0.1f;
               }
            );
        MonoManager.Instance.AddUpdateAction(transform.gameObject.GetInstanceID().ToString(),UpdateType.MoveTo, action);
        return action;
    }
}