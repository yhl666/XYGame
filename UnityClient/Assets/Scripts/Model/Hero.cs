/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;

public class Hero : Entity
{
    public override bool Init()
    {
        base.Init();

        return true;
    }
    public override void UpdateMS()
    {

        base.UpdateMS();
    }
    public void SetPVPAIEnable(bool e)
    {
        this.enable_pvp_ai = e;
    }
    public virtual void AI_UpdateMSWithAI()
    {
      
    }
    public void SwitchTypeTo(string type)
    {
        this.OnSwitchTypeTo(type);
        this.type = type;
    }
    public virtual void OnSwitchTypeTo(string type)
    {

    }
    public DAO.User user = null;
    public string name_head;
    protected bool enable_pvp_ai = false;
}
