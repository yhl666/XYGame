using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
class BufferAssemble:Buffer
{
    public override void UpdateMS()
    {
        //base.UpdateMS();
        //if (this.IsInValid()) return;

        //if (IsComplete() == false)
        //{
        //    target.x_auto -= 10 * target.flipX;

        //}
    }
    public override void OnEnter()
    {
        base.OnEnter();
        this.owner.hp = 100000;
        this.owner.current_hp = 100000;
        var hero_x = this.owner.x_auto;
        var forward = this.owner.flipX;
        //Debug.Log("juji");
        var EnemysArray= EnemyMgr.ins.GetEnemys();
        float dis = 5.0f;
        foreach (var enemy in EnemysArray)
        {
            if (Math.Min(hero_x, hero_x - dis * forward) < (enemy as Enemy).x_auto && (enemy as Enemy).x_auto < Math.Max(hero_x, hero_x - dis * forward))
            {
                //Debug.Log("EnemysArray ==null");
                (enemy as Enemy).x_auto = (hero_x + (hero_x -dis * forward)) / 2;
            }
        }
        //this.SetLastTime(1);
        ///   target.x_auto -= 1 * target.flipX;
        /// 
        ///
    }


    public override void OnExit()
    {
        base.OnExit();
    }

    public override bool Init()
    {
        base.Init();

        //  ViewMgr.Create<ViewBuffer2_1>(this);
        //this.target = this.owner;
        return true;
    }
}

class BufferLighting : Buffer
{
    public override void UpdateMS()
    {
        base.UpdateMS();
        if (this.IsInValid()) return;

        if (IsComplete() == false)
        {


        }
    }
    public override void OnEnter()
    {
        base.OnEnter();

        var hero_x = this.owner.x_auto;
        var forward = this.owner.flipX;
        //Debug.Log("juji");
        {
            var EnemysArray = EnemyMgr.ins.GetEnemys();

            foreach (Enemy enemy in EnemysArray)
            {
                if (Math.Min(hero_x, hero_x - 3 * forward) < (enemy as Enemy).x_auto && (enemy as Enemy).x_auto < Math.Max(hero_x, hero_x - 3 * forward))
                {
                    if (this.owner.team != enemy.team)
                    //Debug.Log("EnemysArray ==null");
                    (enemy as Enemy).current_hp -= 100;

                }
            }



        }

        {
            var EnemysArray = HeroMgr.ins.GetHeros();

            foreach (Hero enemy in EnemysArray)
            {
                if (Math.Min(hero_x, hero_x - 3 * forward) < (enemy as Hero).x_auto && (enemy as Hero).x_auto < Math.Max(hero_x, hero_x - 3 * forward))
                {
                    if (this.owner.team != enemy.team)
                        //Debug.Log("EnemysArray ==null");
                        (enemy as Hero).current_hp -= 100;

                }
            }
        }

        ///   target.x_auto -= 1 * target.flipX;
    }


    public override void OnExit()
    {
        base.OnExit();
    }

    public override bool Init()
    {
        base.Init();

        //  ViewMgr.Create<ViewBuffer2_1>(this);
        //this.target = this.owner;
        return true;
    }
}

class BufferRegen:Buffer
{
    public override void UpdateMS()
    {
        base.UpdateMS();
        if (this.IsInValid()) return;

        if (IsComplete() == false)
        {
            target.hp += 10;

        }
    }
    public override void OnEnter()
    {
        base.OnEnter();
        this.SetLastTime(1);
        ///   target.x_auto -= 1 * target.flipX;
    }


    public override void OnExit()
    {
        base.OnExit();
    }

    public override bool Init()
    {
        base.Init();

        //  ViewMgr.Create<ViewBuffer2_1>(this);
        this.target = this.owner;
        return true;
    }
}