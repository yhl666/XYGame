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
    public DAO.User user = null;
    public string name_head;
    protected bool enable_pvp_ai = false;
}


