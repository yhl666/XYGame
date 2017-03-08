/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;
using System;

//UNITY 驱动层，APP级 向这里注册回调，app分管各自的update updateMS
// this class drive on the game app update  and event dispatcher;
public sealed class EventSystem : MonoBehaviour
{
    //
    void Awake()
    {
        ins = this;

    }
    void OnDestroy()
    {
        EventDispatcher.ins.PostEvent(Events.ID_EXIT);
        ins = null;
        this.list_update.Clear();
        /*if (BattleApp._ins != null)
        {
            BattleApp.ins.Dispose();
        }*/

    }

    // Use this for initialization
    void Start()
    {


    }
    // Update is called once per frame
    void Update()
    {
        EventDispatcher.ins.Update();
        for (int i = 0; i < list_update.Count; i++)
        {
            (list_update[i] as GAObject).Update();
        }
    }


    public void AddEvent_Update(GAObject obj)
    {
        if (IsExist(obj))
        {
            Debug.LogWarning("EventSystem has been in list");
        }
        else
        {
            this.list_update.Add(obj);
        }
    }

    public bool IsExist(GAObject what)
    {
        foreach (GAObject obj in list_update)
        {
            if (obj == what) return true;
        }
        return false;
    }
    public void RemoveEvent_Update(GAObject obj)
    {

        if (IsExist(obj))
        {
            this.list_update.Remove(obj);
        }
        else
        {
            Debug.LogWarning("EventSystem remove not found");
        }

    }

    public void Clear()
    {
        this.list_update.Clear();
    }

    ArrayList list_update = new ArrayList();
    public static EventSystem ins = null;

}
