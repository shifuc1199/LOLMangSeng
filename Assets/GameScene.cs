using DreamerTool.GameObjectPool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : MonoBehaviour
{
    public UnitController controlUnit;
    // Start is called before the first frame update
    void Awake()
    {
        GameObjectPoolManager.InitByScriptableObject();
        
    }
    private void Start()
    {
        InputHandler.Instance.SelectControlUnit(controlUnit.unit);
    }
    // Update is called once per frame
    void Update()
    {
        var commoand = InputHandler.Instance.HandleInput();
        if(commoand!=null)
        {
            commoand.Execute();
        }
        controlUnit.unit.Update();
    }
}
