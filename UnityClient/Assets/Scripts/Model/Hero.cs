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
    public DAO.User user = null;
    public string name_head;
}


