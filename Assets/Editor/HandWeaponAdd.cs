using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;



public class HandWeaponAdd : EditorWindow
{
    
    /// <summary>
    /// 查找子物体（递归查找）  
    /// </summary> 
    /// <param name="trans">父物体</param>
    /// <param name="goName">子物体的名称</param>
    /// <returns>找到的相应子物体</returns>
    public static Transform FindChild(Transform trans, string goName)
    {
        Transform child = trans.Find(goName);
        if (child != null)
            return child;

        Transform go = null;
        for (int i = 0; i < trans.childCount; i++)
        {
            child = trans.GetChild(i);
            go = FindChild(child, goName);
            if (go != null)
                return go;
        }
        return null;
    }
    public static T FindChild<T>(Transform trans, string goName) where T : UnityEngine.Object
    {
        Transform child = trans.Find(goName);
        if (child != null)
        {
            return child.GetComponent<T>();
        }

        Transform go = null;
        for (int i = 0; i < trans.childCount; i++)
        {
            child = trans.GetChild(i);
            go = FindChild(child, goName);
            if (go != null)
            {
                return go.GetComponent<T>();
            }
        }
        return null;
    }
    
       [MenuItem("Plugins/增加手部武器节点")]
     	static void Init()
         {
             HandWeaponAdd abt = EditorWindow.GetWindow<HandWeaponAdd>();
             abt.Show();
         }

        public void OnGUI()
        {
            if (GUILayout.Button("增加手部节点"))
            {
               var target = Selection.GetFiltered<GameObject>(SelectionMode.Editable);
               if (target.Length > 0)
               {
                   var ani = target[0].GetComponent<Animator>();
                   var lhand = ani.GetBoneTransform(HumanBodyBones.LeftHand);
                   var rhand  = ani.GetBoneTransform(HumanBodyBones.RightHand);
                   if (FindChild<Transform>(lhand, "LeftHandPoint") == null)
                   {
                       GameObject.Instantiate(Resources.Load<GameObject>("LeftHandPoint"), lhand, false).name = "LeftHandPoint";
                   }
                   if (FindChild<Transform>(rhand, "RightHandPoint") == null)
                   {
                       GameObject.Instantiate(Resources.Load<GameObject>("RightHandPoint"), rhand, false).name = "RightHandPoint";
                   }
               
               }
            }
        }
}
