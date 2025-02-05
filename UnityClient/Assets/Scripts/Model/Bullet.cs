﻿/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */

using System;
using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;


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
    public float z = 0.0f;
    public float distance = 10.0f;
    public float rotate = 0.0f;
    public float flipX = -1.0f;
    public float speed = 0.1f;
    public float scale_x = 1.0f;
    public float scale_y = 1.0f;
    public int frameDelay = 4;
    public string prefabsName = "";
    public string plist = "88"; // animation plist file  


    public Vector3 bounds_size = new Vector3(0.5f, 1.0f, 0.2f);
    public BoundsImpl bounds
    {
        get
        {
            this.SyncBounds();
            return _bounds;
        }
        set
        {
            this._bounds = value;
        }
    }

    private BoundsImpl _bounds = null;
    public void SyncBounds()
    {
        this._bounds.center = new Vector3(this.x, this.y, this.z);
        this._bounds.size = new Vector3(this.bounds_size.x, this.bounds_size.y, this.bounds_size.z);
    }
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
        this._bounds = BoundsImpl.Create();
        return true;
    }

}
public enum ColliderType
{
    Box, // 3D 盒子
    Sphere,//3D 球
    Circle, // 2D 圆
    Rect,//2D 矩形
    Sector,//2D扇形
}

public enum DamageType
{
    RATIO,// 比例伤害
    REAL,//真实伤害
    MIXED,//混合伤害
}

public delegate void OnBulletFunc(Bullet bullet, object userData = null);
public delegate Vector2 OnMoveFunc(BulletConfig bullet, Vector2 current);

public sealed class BulletConfigInfo
{

    //public string className = ""; // 子弹类名

    public string brief_detail = "法术详细描述";
    public string brief_short = "法术简单描述";

    public string plistAnimation = "hd/roles/role_2/bullet/role_2_bul_2001/role_2_bul_2001.plist"; // 子弹 view 的plist 帧动画文件
    public int frameDelay = 4;//帧动画延时
    public int lastTime = 0; // 持续时间,为0表示 使用距离来标示，不为0 表示 距离和 时间 一起来标示

    //   public int deltaTime = 0;//延迟多久才开始 伤害判定
    public bool isHitDestory = true;//命中即可销毁
    //  public Vector2 range;//攻击范围
    public int number = 0xffff;// 范围内攻击敌人个数，1表示 单体，2表示 2个敌人.....
    public float speed = 0.1f; // 移动速度
    public float distance = 10.0f;// 移动距离
    public float distance_atk = 1.0f;// 普通，攻击距离
    public ArrayList buffers_string = new ArrayList();//附加buffer 名字 string
    public ArrayList buffers = new ArrayList();//附加的buffer为class
    public float scale_x = 1.0f;//视图缩放大小
    public float scale_y = 1.0f;//视图缩放大小

    public Vector3 launch_delta_xyz = new Vector3(0.5f, 0.3f, 0f);//初始位置位于 角色锚点位置
    public float rotate = 0.0f;
    public float damage_ratio = 1.0f;//伤害系数
    public float damage_real = 0.0f;//真实伤害数值
    //   public int realValidTimes = 1;//真实有效次数 ，计算真实命中敌人的次数{比如法术命中玩家3次后失效，否则一直存在}
    public int validTimes = 1;//总有效次数 ，击中物体的 帧次数 ，如果当前帧没有命中任何物体 那么不会计算帧次
    public int oneHitTimes = 1;//同一个物体能被命中的次数

    public int sector_angle;//扇形评定框 角度
    public float sector_radius; // 扇形 评定半径
    public DamageType damage_type = DamageType.RATIO;//伤害类型 默认为 比例伤害
    public int dir_2d = -1;//2d 移动方向 x z轴
    public ArrayList hit_targets = new ArrayList();//指定攻击目标 如果有指定 那么不自动判定目标（只会在运动轨迹范围内判定 指定的目标是否在攻击范围内）， 没有的话，大小为0
    /// <summary>
    /// 添加指定攻击目标
    /// </summary>
    /// <param name="who"></param>
    public void AddHitTarget(Entity who)
    {
        hit_targets.Add(who);
    }
    /// <summary>
    /// 是否指定攻击目标
    /// </summary>
    /// <returns></returns>
    public bool HasHitTarget()
    {
        return hit_targets.Count > 0;
    }
    // eg.

    //比如 某法术 效果是 持续3秒 原地 2米内 造成2次伤害
    //lastTime= 3*40  distance_atk=2 validTimes =2,distance=0;

    //比如 某法术 效果是 命中目标后 对范围2米内所有目标 造成 1次伤害
    //lastTime=0 distance_atk=2 validTimes =1,distance=10;

    //比如 某法术 效果是 群体攻击，生效一次，攻击范围是2米 命中的目标附加 眩晕 和 引燃效果
    //lastTime=0 distance_atk=2 validTimes =1,distance=10,buffers="buffer0,buffer1";

    //触发攻击时回调
    public OnBulletFunc _OnTakeAttack = null; //，命中的回调 第二参数是命中的目标
    public OnBulletFunc _OnLaunch = null; // 创建时回调
    public OnBulletFunc _OnUpdateMS = null; // 有效帧时间 回调
    public OnBulletFunc _OnExit = null; //  销毁时 回调

    public int onTakeAttackFuncCallTimes = 0xfffffff;//攻击函数回调次数
    public OnMoveFunc _OnMoveFunc = null;

    public ColliderType collider_type = ColliderType.Rect;
    public Vector3 collider_size = new Vector3(1.0f, 1.0f, 0.2f);
    public void InVokeOnTakeAttack(Bullet bullet, object userData = null)
    {
        if (_OnTakeAttack != null)
        {
            if (current_call_times >= onTakeAttackFuncCallTimes) return;
            current_call_times++;
            _OnTakeAttack.Invoke(bullet, userData);
        }

    }

    public void InVokeOnLaunch(Bullet bullet)
    {
        if (_OnLaunch != null)
        {
            _OnLaunch.Invoke(bullet);
        }
    }
    public void InVokeOnExit(Bullet bullet)
    {
        if (_OnExit != null)
        {
            _OnExit.Invoke(bullet);
        }
    }
    public void InVokeOnUpdateMS(Bullet bullet)
    {
        if (_OnUpdateMS != null)
        {
            _OnUpdateMS.Invoke(bullet);
        }

    }
    private int current_call_times = 0;
    //-------------------helper function

    public void AddBuffer(string buffer)
    {
        this.buffers_string.Add(buffer);
    }

    public void AddBuffer(Buffer buffer)
    {
        this.buffers.Add(buffer);
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
    public int tick = 0;
    private int dir_2d = -1;
    private int validTimes = 0;
    private Vector hasHit = new Vector();//命中物体的集合 ，可判定命中次数
    /// <summary>
    /// 命中检查
    /// 命中后 会产生Attackinfo 进行伤害计算 调用者 负责部分配置化 参数检测
    /// </summary>
    /// <param name="h"></param>
    /// <returns></returns>
    private bool ColliderCheckOne(Entity h)
    {
        if (h == owner) return false;
        //目标已经死亡
        if (h == null) return false;

        if (h.team == owner.team) return false;
        bool hit = false;
        if (this.info.collider_type == ColliderType.Rect || this.info.collider_type == ColliderType.Box)
        {
            hit = h.IsCast(this.bounds);
        }
        else if (ColliderType.Circle == info.collider_type)
        {
            //    Debug.Log("dis = " + diss);
            hit = h.ClaculateDistance(x, y) < info.distance_atk;
        }
        else if (ColliderType.Sector == info.collider_type)
        {
            //扇形评定框
            do
            {
                int base_angle = 0;//评定角度
                if (this.flipX < 0)//往左边走
                {
                    if (this.x < h.x) break; // 目标在右边 跳过
                    base_angle = 180;
                }
                else
                {
                    if (this.x >= h.x) break; // 目标在左边 跳过
                    base_angle = 0;
                }
                if (h.ClaculateDistance(x, h.y, z) < info.sector_radius)
                {//范围内
                    int degree = (int)Utils.GetAngle(new Vector3(x, h.y, z), new Vector3(h.x, h.y, h.z));
                    int angle = info.sector_angle;//扇形大小 
                    int delta = base_angle - angle / 2;
                    while (delta < 0)
                    {
                        delta += 360;
                    }
                    while (degree - delta < 0)
                    {
                        degree += 360;
                    }
                    if (angle >= degree - delta)
                    {
                        hit = true;
                    }
                }
            }
            while (false);
        }
        if (hit == false) return false;

        if (hasHit.GetCount(h) >= info.oneHitTimes) return false;

        AttackInfo inf = AttackInfo.Create(owner, h, info.damage_type);
        if (info.damage_type == DamageType.RATIO)
        {
            inf.InitWithCommon();
            inf.damage = (int)((float)inf.damage * info.damage_ratio);
        }
        else if (info.damage_type == DamageType.REAL)
        {
            inf.damage = (int)info.damage_real;
        }
        else
        {
            Debug.LogError("unsuport damage type");
        }
        foreach (string buf in info.buffers_string)
        {
            inf.AddBuffer(buf);
        }
        foreach (Buffer buf in info.buffers)
        {
            inf.AddBuffer(buf);
        }
        ///     inf.buffers_string = info.buffers_string;
        h.TakeAttacked(inf);
        info.InVokeOnTakeAttack(this, h);

        hasHit.PushBack(h);

        return true;
    }

    public override void UpdateMS()
    {
        tick++;
        if (distance <= 0 && tick > info.lastTime)
        {//both dis and  time done // 时间 和 距离 都 完成，，如果validTime
            this.SetInValid();
            return;
        }
        if (distance > 0)
        {
            float dis = 0.0f;
            if (info._OnMoveFunc == null)
            {// default move function
                // dis = flipX * speed;
                //  this.x += dis;

                float dd = dir_2d * DATA.ONE_DEGREE;//一度的弧度

                float z_delta = Mathf.Sin(dd) * speed;
                float x_delta = Mathf.Cos(dd) * speed;

                this.x += x_delta;
                this.z += z_delta;
                dis = Mathf.Abs(z_delta) + Mathf.Abs(x_delta);
            }
            else
            {
                Vector2 pos_now = new Vector2(this.x, this.y);
                Vector2 pos_next = info._OnMoveFunc(this, pos_now);
                dis = Utils.ClaculateDistance(pos_now, pos_next);
                this.x = pos_next.x;
                this.y = pos_next.y;
            }
            distance -= Mathf.Abs(dis);
            if (distance <= 0) distance = 0;
        }
        if (validTimes >= info.validTimes)
        {//超过 有效次数 
            if (info.isHitDestory)
            {
                this.SetInValid(); return;
            }
            else
            {
                return;
            }
        }
        info.InVokeOnUpdateMS(this);
        bool tagForValidTimes = false;
        //----开始命中check
        if (info.HasHitTarget())
        {//指定了目标 直接判定目标是否进入范围
            ArrayList targets = info.hit_targets;
            int hitNumber = 0; // 命中数量
            foreach (Entity h in targets)
            {
                if (hitNumber >= info.number) break;// 如果命中 数 大于限制数量，忽略
                if (this.ColliderCheckOne(h))
                {//命中
                    hitNumber++;
                    tagForValidTimes = true;
                }
            }
        }
        else
        {//未指定目标
            //scan heros
            ArrayList heros = HeroMgr.ins.GetHeros();
            int hitNumber = 0; // 命中怪物数量
            foreach (Hero h in heros)
            {
                if (hitNumber >= info.number) break;// 如果命中 敌人数 大于限制数量，忽略
                if (this.ColliderCheckOne(h))
                {//命中
                    hitNumber++;
                    tagForValidTimes = true;
                }
            }
            //scan enemy
            ArrayList enemys = EnemyMgr.ins.GetEnemys();
            foreach (Enemy h in enemys)
            {
                if (hitNumber >= info.number) break; // 如果命中 敌人数 大于限制数量，忽略
                if (this.ColliderCheckOne(h))
                {//命中
                    hitNumber++;
                    tagForValidTimes = true;
                }
            }
            //scan buinding
            ArrayList buildings = BuildingMgr.ins.GetBuildings();
            foreach (Building h in buildings)
            {
                if (hitNumber >= info.number) break; // 如果命中 敌人数 大于限制数量，忽略
                if (this.ColliderCheckOne(h))
                {//命中
                    hitNumber++;
                    tagForValidTimes = true;
                }
            }
        }
        if (tagForValidTimes)
        {//命中过敌人 有效次数
            validTimes++;
        }
    }

    public override void OnEnter()
    {
        base.OnEnter();

        this.y = owner.y + owner.height + info.launch_delta_xyz.y;

        this.z = owner.z + info.launch_delta_xyz.z;

        this.flipX = -owner.flipX;

        info.InVokeOnLaunch(this);
        this.dir_2d = info.dir_2d;
        if (dir_2d >= 0)
        {
            if (dir_2d > 90 && dir_2d < 270)
            {
                flipX = -1f;
            }
            else
            {
                flipX = 1f;
            }
        }
        else
        {
            if (this.flipX > 0)
            {
                dir_2d = 0;
            }
            else
            {
                dir_2d = 180;
            }
        }
        if (this.flipX < 0)
        {
            this.x = owner.x - info.launch_delta_xyz.x;
        }
        else
        {
            this.x = owner.x + info.launch_delta_xyz.x;
        }
    }

    public override bool Init()
    {
        base.Init();
        return true;
    }
    public override void OnExit()
    {
        info.InVokeOnExit(this);
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
        this.speed = info.speed;
        this.rotate = info.rotate;
        this.frameDelay = info.frameDelay;
        this.scale_x = info.scale_x;
        this.scale_y = info.scale_y;
        this.bounds_size = info.collider_size;

    }

    public static BulletConfig CreateWithJson(string json = "brief_detail:1233.0,brief_short:1113.0,plistAnimation:3333.0,frameDelay:37456.0,lastTime:4689.0,isHitDestory:456.0,number:237.0,speed:148.0,distance:922.0,distance_atk:333.0,buffers_string:2256.0,scale_x:3397.0,scale_y:2258.0,launch_delta_xy:1139.0,rotate:1139.0,damage_ratio:123.0,validTimes:132.0,oneHitTimes:85.0,")
    {

        HashTable table = Json.Decode(json);
        BulletConfig bulletConfig = new BulletConfig();

        bulletConfig.info.brief_detail = table["brief_detail"];
        bulletConfig.info.brief_short = table["brief_short"];
        bulletConfig.info.plistAnimation = table["plistAnimation"];
        bulletConfig.info.frameDelay = Convert.ToInt32(table["frameDelay"]);
        bulletConfig.info.lastTime = Convert.ToInt32(table["lastTime"]);
        bulletConfig.info.isHitDestory = Convert.ToBoolean(table["lastTime"]);
        bulletConfig.info.number = Convert.ToInt32(table["number"]);
        bulletConfig.info.speed = Convert.ToSingle(table["speed"]);
        bulletConfig.info.distance = Convert.ToSingle(table["distance"]);
        bulletConfig.info.distance_atk = Convert.ToSingle(table["distance_atk"]);
        bulletConfig.info.scale_x = Convert.ToSingle(table["scale_x"]);
        bulletConfig.info.scale_y = Convert.ToSingle(table["scale_y"]);
        bulletConfig.info.rotate = Convert.ToSingle(table["rotate"]);
        bulletConfig.info.damage_ratio = Convert.ToSingle(table["damage_ratio"]);
        bulletConfig.info.validTimes = Convert.ToInt32(table["validTimes"]);
        bulletConfig.info.oneHitTimes = Convert.ToInt32(table["oneHitTimes"]);

        //bulletConfig.info.buffers_string = table["buffers_string"];

        return bulletConfig;

    }
    public BulletConfigInfo info = null;
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
                    AttackInfo inf = AttackInfo.Create(owner, h);
                    inf.InitWithCommon();
                    h.TakeAttacked(inf);

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
                    AttackInfo inf = AttackInfo.Create(owner, h);
                    inf.InitWithCommon();
                    h.TakeAttacked(inf);

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

    }
    public override void OnDispose()
    {
        base.OnDispose();

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


/// <summary>

/// </summary>
public class Bullet221_0 : Bullet
{

    public override void UpdateMS()
    {
    }

    public override void OnEnter()
    {
        //  this.owner.AddBuffer<BufferDamage>();
        this.SetInValid();
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

    public override bool Init()
    {
        this.plist = "";
        return true;
    }
}


/// <summary>

/// </summary>
public class BulletStateMachineTest : Bullet
{

    public override void UpdateMS()
    {

        if (tick1.Tick())
        {
            owner.machine.PauseAllStack();
            ///  owner.attackingAnimationName = "";
            owner.isAttacking = false;
            owner.machine.GetState<FallState>().stack.Resume();
            owner.x_auto += 0.05f;
            return;
        }

        // process with oover;
        owner.machine.ResumeAllStack();
        this.SetInValid();
    }

    public override void OnEnter()
    {
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

    public override bool Init()
    {
        this.plist = "";
        return true;
    }Counter tick1 = Counter.Create(120);//3s
}