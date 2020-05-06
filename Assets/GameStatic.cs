using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 

public static class GameStaticMethod  
{
    public static RaycastHit GetMouseRayCastHit(int mask)
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Physics.Raycast(ray, out hit, float.MaxValue,mask);
        return hit;
    }
    public static void ChangeCursor(CursorType cursor)
    {
        Cursor.SetCursor(Resources.Load<Texture2D>(cursor.ToString()), Vector2.zero, CursorMode.Auto);
    }
}
