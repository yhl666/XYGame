/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;


//TODO should use callback and configuration  OnEnter UpdateMS in a super class  ?


/// <summary>
///  in battle all skill and attack is bullet or buffer
/// </summary>
public class Bullet : Model
{
    AttackInfo info = null;

    public Entity owner = null;


    public float x = 0.0f;
    public float y = 0.0f;
    public float distance = 10.0f;

    public float flipX = -1.0f;
    public float speed = 0.1f;

    public string prefabsName = "";
    public string plist = "88"; // animation plist file  

    public virtual void OnComplete()
    {
        //for view to call when animation done
    }
    public override void UpdateMS()
    {

    }

    public override void OnEnter()
    {
        base.OnEnter();
    }

    public override bool Init()
    {
        base.Init();
        return true;
    }

}

public delegate void OnTakeAttack(Bullet bullet);


public sealed class BulletConfigInfo
{

    //public string className = ""; // 子弹类名

    public string brief_detail = "法术详细描述";
    public string brief_short = "法术简单描述";

    public string plistAnimation = "hd/roles/role_2/bullet/role_2_bul_2001/role_2_bul_2001.plist"; // 子弹 view 的plist 帧动画文件
    public int lastTime = 0; // 持续时间,为0表示 使用距离来标示，不为0 表示 距离和 时间 一起来标示
    //  public Vector2 range;//攻击范围
    public int number = 0xffff;// 范围内攻击敌人个数，1表示 单体，2表示 2个敌人.....
    public float speed = 0.1f; // 移动速度
    public float distance = 10.0f;// 移动距离
    public float distance_atk = 1.0f;// 普通，攻击距离
    public ArrayList buffers = new ArrayList();//附加buffer 名字 string
    public Vector2 launch_pivot;//初始位置位于 角色锚点位置

    //   public int realValidTimes = 1;//真实有效次数 ，计算真实命中敌人的次数{比如法术命中玩家3次后失效，否则一直存在}
    public int validTimes = 1;//有效次数    //表示 命中目标后  的 伤害 次数

    // eg.

    //比如 某法术 效果是 持续3秒 原地 2米内 造成2次伤害
    //lastTime= 3*40  distance_atk=2 validTimes =2,distance=0;

    //比如 某法术 效果是 命中目标后 对范围2米内所有目标 造成 1次伤害
    //lastTime=0 distance_atk=2 validTimes =1,distance=10;

    //比如 某法术 效果是 群体攻击，生效一次，攻击范围是2米 命中的目标附加 眩晕 和 引燃效果
    //lastTime=0 distance_atk=2 validTimes =1,distance=10,buffers="buffer0,buffer1";

    //触发攻击时回调
    public OnTakeAttack _OnTakeAttack = null;

    public void InVokeOnTakeAttack(Bullet bullet)
    {
        if (_OnTakeAttack != null) _OnTakeAttack.Invoke(bullet);
    }




    //----------------------creator
    public static BulletConfigInfo CreateWithJson(string json)
    {
        BulletConfigInfo ret = new BulletConfigInfo();
        return ret;
    }

    public static BulletConfigInfo Create()
    {
        BulletConfigInfo ret = new BulletConfigInfo();
        return ret;
    }

    private BulletConfigInfo() { }
}


/// <summary>
///  可配置 子弹的超类
/// </summary>
public sealed class BulletConfig : Bullet
{
    private int tick = 0;
    private int validTimes = 0;
    public override void UpdateMS()
    {
        tick++;
        float dis = flipX * info.speed;
        this.x += dis;
        distance -= Mathf.Abs(dis);
        if (distance <= 0) distance = 0;

        if (distance <= 0 && tick > info.lastTime)
        {//both dis and  time done // 时间 和 距离 都 完成，，如果validTime
            this.SetInValid();
            return;
        }


        //scan heros
        ArrayList heros = HeroMgr.ins.GetHeros();
        int hitNumber = 0; // 命中怪物数量
        bool tagForValidTimes = false;
        foreach (Hero h in heros)
        {
            if (hitNumber >= info.number) break;// 如果命中 敌人数 大于限制数量，忽略

            if (h == owner) continue;

            if (h.team != owner.team)
            {
                float diss = h.ClaculateDistance(x, y);
                //    Debug.Log("dis = " + diss);
                if (diss < info.distance_atk)
                {
                    hitNumber++;
                    h.TakeAttack(owner);
                    info.InVokeOnTakeAttack(this);
                    tagForValidTimes = true;
                    foreach (string buffer in info.buffers)
                    {//add buffer
                        h.AddBuffer(buffer);
                    }
                }
            }
            else
            {
                continue;
            }
        }



        //scan enemy
        ArrayList enemys = EnemyMgr.ins.GetEnemys();
        foreach (Enemy h in enemys)
        {
            if (hitNumber >= info.number) break; // 如果命中 敌人数 大于限制数量，忽略

            if (h.team != owner.team)
            {
                float diss = h.ClaculateDistance(x, y);
                //    Debug.Log("dis = " + diss);
                if (diss < info.distance_atk)
                {
                    h.TakeAttack(owner);
                    info.InVokeOnTakeAttack(this);
                    tagForValidTimes = true;
                    hitNumber++;
                    foreach (string buffer in info.buffers)
                    {//add buffer
                        h.AddBuffer(buffer);
                    }
                }
            }
            else
            {
                continue;
            }
        }
        if (tagForValidTimes)
        {//命中过敌人 有效次数
            validTimes++;
        }
        if (validTimes >= info.validTimes)
        {//超过 有效次数 
            this.SetInValid();
        }



    }

    public override void OnEnter()
    {
        base.OnEnter();

        this.y = owner.y + owner.height + 0.3f;
        this.flipX = -owner.flipX;
        if (this.flipX < 0)
        {
            this.x = owner.x - 1f;
        }
        else
        {
            this.x = owner.x + 1f;
        }
    }

    public override bool Init()
    {
        base.Init();
        return true;
    }



    public void LoadConfig(BulletConfigInfo info)
    {
        this.info = info;
        this.InitWithConfig();
    }
    private void InitWithConfig()
    {
        this.plist = info.plistAnimation;
        this.distance = info.distance;

    }
    private BulletConfigInfo info = null;
}


/// <summary>
///  role 2  的0 号 普通攻击技能
/// </summary>
public class Bullet2_0 : Bullet
{
    public override void OnComplete()
    {
        //for view to call when animation done
    }
    public override void UpdateMS()
    {

        float dis = flipX * speed;
        this.x += dis;
        distance -= Mathf.Abs(dis);

        if (distance < 0.0f)
        {
            this.SetInValid();
        }


        //scan heros
        ArrayList heros = HeroMgr.ins.GetHeros();
        bool isDirty = false;
        foreach (Hero h in heros)
        {
            if (h == owner) continue;
            if (h.team != owner.team)
            {
                float diss = h.ClaculateDistance(x, y);
          //     Debug.Log("dis = " + diss);
                if (diss < 1)
                {
                    h.TakeAttack(owner);
                    isDirty = true;
                }
            }
            else
            {
                continue;
            }
        }



        //scan enemy
        ArrayList enemys = EnemyMgr.ins.GetEnemys();
        foreach (Enemy h in enemys)
        {
            if (h.team != owner.team)
            {
                float diss = h.ClaculateDistance(x, y);
                  Debug.Log("dis = " + diss);
                if (diss < 1)
                {
                    h.TakeAttack(owner);
                    isDirty = true;
                }
            }
            else
            {
                continue;
            }
        }

        if (isDirty)
        {
            this.SetInValid();
        }

    }

    public override void OnEnter()
    {
        base.OnEnter();

        this.y = owner.y + 0.3f + owner.height;
        this.flipX = -owner.flipX;
        if (this.flipX < 0)
        {
            this.x = owner.x - 1f;
        }
        else
        {
            this.x = owner.x + 1f;
        }
    }

    public override bool Init()
    {
        base.Init();
        this.plist = "hd/roles/role_2/bullet/role_2_bul_2001/role_2_bul_2001.plist";
        return true;
    }

}



/// <summary>
///  role 2 的 1号技能
/// </summary>
public class Bullet2_1 : Bullet
{

    public override void UpdateMS()
    {

        this.x = owner.x;
        this.y = owner.y + owner.height;
    }

    public override void OnEnter()
    {
        this.owner.AddBuffer<Buffer2_1>();

    }

    public override void OnExit()
    {
        base.OnExit();
        Debug.Log(" exit");

    }
    public override void OnDispose()
    {
        base.OnDispose();
        Debug.Log(" dispose");

    }
    public override void OnComplete()
    {
        base.OnComplete();
        this.SetInValid();
        //     owner.eventDispatcher.PostEvent(Events.id_skill)
    }
    public override bool Init()
    {
        this.plist = "hd/roles/role_2/bullet/role_2_bul_2111/role_2_bul_2111.plist";
        return true;
    }
}



/// <summary>
/// enemy 444  的0 号 普通攻击技能
/// </summary>
public class Bullet444_0 : Bullet2_0
{

    public override bool Init()
    {
        base.Init();
        this.plist = "hd/enemies/enemy_444/bullet/enemy_444_bul_444001/enemy_444_bul_444001.plist";
        return true;
    }

}
