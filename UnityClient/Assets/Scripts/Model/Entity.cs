/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;
using System;

public class AttackInfo
{
    /// <summary>
    /// 如果使用默认 调用InitWithCommon，否则自行修改info信息
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static AttackInfo Create(Entity owner, Entity target, DamageType type = DamageType.RATIO)
    {
        AttackInfo ret = new AttackInfo();
        ret.type = type;
        ret.ownner = owner;
        ret.target = target;
        ret.atk_func = (object _param) =>
            {
                AttackInfo info = _param as AttackInfo;
                info.ownner.IncreaseCombo();
            };

        return ret;
    }
    /// <summary>
    ///  可用于群体攻击时 可拷贝一样的info
    /// </summary>
    /// <returns></returns> 
    public AttackInfo Clone()
    {
        AttackInfo ret = Create(this.ownner, this.target);
        ret.damage = damage;
        ret.is_crits = is_crits;
        ret.crits_damage = crits_damage;
        ret.atk_func = atk_func;
        ret.buffers_string = buffers_string;
        return ret;

    }
    private AttackInfo()
    {
    }
    public Entity target = null;//为null 标示 群体攻击 为一个Entity 表示单体攻击 //TODO 为数组 表示部分群体攻击
    public Entity ownner = null;

    //public Func<AttackInfo, bool> atk_func = null;
    public VoidFuncObject atk_func = null;
    //---参与数值计算的属性
    public int damage = 0;
    public float crits_damage = 0.0f;
    public bool is_crits = false;

    public DamageType type = DamageType.RATIO;
    /// <summary>
    /// 调用攻击函数接口
    /// </summary>
    public void Invoke()
    {
        if (atk_func != null)
            atk_func.Invoke(this);
    }

    /// <summary>
    ///通用属性初始化
    /// </summary>
    public void InitWithCommon()
    {
        this.damage = ownner.GetRealAttackDamage();

        if (damage > ownner.damage)
        {
            this.is_crits = true;
            crits_damage = ownner.crits_damage;
        }

    }

    /// <summary>
    /// add buffer to atk info
    /// </summary>
    /// <param name="buf"></param>
    public void AddBuffer(string buf)
    {
        this.buffers_string.Add(buf);
    }

    public void AddBuffer(Buffer buf)
    {
        this.buffers.Add(buf);
    }
    public ArrayList buffers = new ArrayList();
    public ArrayList buffers_string = new ArrayList();
}


public class Entity : Model
{

    //---------------for player input state and frame sync
    public int dir = -1;
    public bool left = false;//left
    public bool right = false;//right
    public bool jump = false;//jump
    public bool atk = false;//atk
    public int s1 = 0;//skill 1
    public bool stand = false;//stand


    //--------------------------------------------------interface for outside game logic-----
    /// <summary>
    ///  outside to add buffer by type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public Buffer AddBuffer<T>(Entity owner) where T : new()
    {
        return this.bufferMgr.Create<T>(owner);
    }

    public Buffer AddBuffer(string buffer, Entity owner)
    {
        return this.bufferMgr.Create(buffer, owner);
    }
    public Buffer AddBuffer<T>() where T : new()
    {
        return this.bufferMgr.Create<T>();
    }

    public Buffer AddBuffer(string buffer)
    {
        return this.bufferMgr.Create(buffer);
    }
    /// <summary>
    /// 添加，通过tag ， 逻辑 来自定义标示
    /// 不去重 ，谨慎使用
    /// </summary>
    /// <param name="tag"></param>
    public void AddTag(string tag, object userData = null)
    {
        Pair<string, object> obj = new Pair<string, object>();
        obj.key = tag;
        obj.value = userData;
        tags.Add(obj);
    }
    /// <summary>
    /// 是否拥有，通过tag ， 逻辑 来自定义标示
    /// 不去重 ，谨慎使用
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public bool HasTag(string tag)
    {
        foreach (Pair<string, object> str in tags)
        {
            if (str.key == tag) return true;
        }
        return false;
    }
    /// <summary>
    /// Tag 对应的userData，通过tag ， 逻辑 来自定义标示
    /// 不去重 ，谨慎使用
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public Pair<string, object> GetTagPair(string tag)
    {
        foreach (Pair<string, object> str in tags)
        {
            if (str.key == tag) return str;
        }
        return null;
    }
    /// <summary>
    /// 删除，通过tag ， 逻辑 来自定义标示
    /// 不去重 ，谨慎使用
    /// </summary>
    /// <param name="tag"></param>
    public void RemoveTag(string tag)
    {
        foreach (Pair<string, object> str in tags)
        {
            if (str.key == tag)
            {
                tags.Remove(str);
                return;
            }
        }
    }
    /// <summary>
    /// remove all  pair by tag
    /// </summary>
    /// <param name="tag"></param>
    public void RemoveTags(string tag)
    {
        for (int i = 0; i < tags.Count; )
        {
            string b = tags[i] as string;
            if (b == tag)
            {
                tags.Remove(b);
            }
            else
            {
                ++i;
            }
        }
    }
    public void ClearTags()
    {
        this.tags.Clear();
    }
    private ArrayList tags = new ArrayList();// 状态标记变量
    public Buffer AddBuffer(Buffer buffer)
    {
        this.bufferMgr.Add(buffer);
        return buffer;
    }
    public Buffer RemoveBuffer(Buffer buffer)
    {
        this.bufferMgr.Remove(buffer);
        return buffer;
    }
    /// <summary>
    ///  accpet attacked
    /// </summary>
    /// <param name="who"></param>
    /*  public void TakeAttacked(Entity who)
      {
          this.current_hp -= who.GetRealAttackDamage();
          //  this.isHurt = true;
          eventDispatcher.PostEvent(Events.ID_HURT);

      }*/

    public void TakeAttacked(AttackInfo info)
    {
        EventDispatcher.ins.PostEvent(Events.ID_BATTLE_ENTITY_BEFORE_TAKEATTACKED, info);

        foreach (string buf in info.buffers_string)
        {
            this.AddBuffer(buf, info.ownner);
        }
        foreach (Buffer buf in info.buffers)
        {
            this.AddBuffer(buf);
        }
        ///    this.TakeAttacked(info.ownner);
        ///    

        if (info.damage > 0)
        {
            this.current_hp -= info.damage;
            eventDispatcher.PostEvent(Events.ID_HURT);//notify ui
        }

        info.Invoke();

        EventDispatcher.ins.PostEvent(Events.ID_BATTLE_ENEITY_AFTER_TAKEATTACKED, info);
    }
    /// <summary>
    /// 是否存活
    /// </summary>
    public bool IsAlive
    {
        get
        {
            return _current_hp > 0;
        }
    }

    public System.Random _random = new System.Random(0);


    //-----------------------------------------------physics-------------------------
    public Vector3 bounds_size = new Vector3(0.5f, 1.0f, 0.1f);
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
        this._bounds.center = new Vector3(this.x, this.GetRealY(), this.z);
        this._bounds.size = new Vector3(this.bounds_size.x, this.bounds_size.y, this.bounds_size.z);
    }
    public bool IsCast(Entity other)
    {
        bool ret = this.bounds.Intersects(other.bounds);
        return ret;
    }

    public bool IsCast(BoundsImpl other)
    {
        this.SyncBounds();
        bool ret = this.bounds.Intersects(other);
        return ret;
    }
    public bool IsCast(RayImpl ray)
    {
        bool ret = this.bounds.IntersectRayImpl(ray);
        return ret;
    }
    public bool IsContains(float x, float y)
    {
        return this.bounds.Contains(new Vector2(x, y));
    }


    //--------------------------------------------------some helper function

    /// <summary>
    /// 返回真实伤害数值，内部会计算暴击
    /// </summary>
    /// <returns></returns>
    public int GetRealAttackDamage()
    {
        int next = _random.Next(0, 100);

        if (next <= crits_ratio)
        {
            return (int)(this.crits_damage / 100.0f * damage);
        }
        else
        {
            return damage;
        }
    }
    public float ClaculateDistance(Entity other)
    {
        return this.ClaculateDistance(other.x, other.y, other.z);
    }
    public float ClaculateDistance(float x, float y, float z = 0f)
    {
        return Utils.ClaculateDistance(new Vector3(this.x, this.y + this.height, this.z), new Vector3(x, y, z));
    }

    public void SetRealY(float y)
    {
        this._height = 0.0f;
        this.y = y;
    }
    public float GetRealY()
    {
        return this.y + height;
    }
    /// <summary>
    /// // y和 z 的混合比例 默认为 1：1 45度视角
    /// </summary>
    /// <returns></returns>
    public float GetReal25DY()
    {
        return (this.y + this.height + this.z);
    }
    /// <summary>
    /// 返回包含 海拔的 位置
    /// </summary>
    /// <returns></returns>
    public Vector3 GetRealPosVector3()
    {
        return new Vector3(this.x, this.GetRealY(), this.z);
    }
    //------------------------------------------------------some event 
    /// <summary>
    /// 血量为0 时触发
    /// </summary>
    public void OnEmptyHp()
    {

    }
    /// <summary>
    ///当血量第一次满 触发，触发后 如果没减血 是不会再次触发
    ///满血后减血 在满血 会触发
    /// </summary>
    public void OnOnceFullHp()
    {

    }
    /// and etc.
    /// 


    //-------------------------------------------------member
    public int combo_time = 0;//连击次数
    public Counter tick_combo = Counter.Create(120);//连击重置定时器

    public void IncreaseCombo(int times = 1)
    {
        this.combo_time += times;
        tick_combo.Reset();
    }
    public float atk_range = 2.0f;
    // base property

    public Vector3 pos = new Vector3(1f, 0f, 0f); //坐标 不包含海拔高度

    public int no = 0;//临时识别编号 比如战斗
    public string id = ""; // 唯一id
    public float flipX = -1.0f;//默认面向右边

    public bool attackAble = true;

    //for dynamic data
    private int _hp = 1000;//气血
    private int _current_hp = 500;//当前气血
    public int delta_hp = 0;//气血变化差值 可用于UI显示

    private int _mp = 500;//魔法
    private int _current_mp = 0;//当前魔法

    private int _exp = 100;//经验
    private int _current_exp = 0;//当前经验


    private float _height = 0.0f;//海拔
    public bool isInOneTerrainRight = false;

    public float scale = 1.0f;//view scale
    public float x
    {
        set
        {
            pos.x = value;
        }
        get
        {
            return pos.x;
        }
    }
    public float y
    {
        set
        {
            pos.y = value;
        }
        get
        {
            return pos.y;
        }
    }
    public float z
    {
        set
        {
            pos.z = value;
        }
        get
        {
            return pos.z;
        }
    }
    /// <summary>
    /// 设置x坐标，自动处理 地形撞墙等
    /// </summary>
    public float x_auto
    {
        set
        {
            AppMgr.GetCurrentApp().GetCurrentWorldMap().ClipPositionX(this, value);

        }
        get
        {
            return this.x;
        }
    }
    public float height
    {
        set
        {
            if (value == _height) return;

            if (isStand == false)
            {
                if (this.height + y > value)
                {
                    this.y = y + _height - value;
                }
                else
                {
                    stand = true;
                    if (isInOneTerrainRight)
                    {//当前位置位于地图块右边
                        right = false; //限制右走
                    }
                    else
                    {//限制左走
                        left = false;
                    }
                    return;
                }
            }
            else if (value < _height)
            {//高度变低
                this.y = y + _height - value;

            }
            else if (value > _height)
            {// 高度变高
                stand = true;
                if (isInOneTerrainRight)
                {//当前位置位于地图块右边
                    right = false; //限制右走
                }
                else
                {//限制左走
                    left = false;
                }
                return;
            }

            _height = value;
        }
        get
        {
            return _height;
        }
    }

    public float height_platform
    {
        set
        {
            if (value == _height) return;

            this.y = y + _height - value;

            _height = value;

        }
    }
    //---------------setter   getter for mp  hp exp
    public int hp
    {
        get
        {
            return _hp;
        }
        set
        {
            _hp = value;
        }
    }
    public int current_hp
    {
        get
        {
            return _current_hp;
        }
        set
        {
            if (value >= _hp)
            {
                delta_hp = 0;
                _current_hp = _hp;
            }
            else if (value > 0)
            {
                delta_hp = value - _current_hp;
                _current_hp = value;
            }
            else
            {
                delta_hp = -_current_hp;
                _current_hp = 0;
            }
        }
    }
    public float CalculateHeight(float x)
    {
        return 0.0f;
    }
    public int mp
    {
        get
        {
            return _mp;
        }
        set
        {
            _mp = value;
        }
    }

    public int current_mp
    {
        get
        {
            return _current_mp;
        }
        set
        {
            if (value >= _mp)
            {
                _current_mp = _mp;
            }
            else if (value > 0)
            {
                _current_mp = value;
            }
            else
            {
                _current_mp = 0;
            }
        }
    }
    public int exp
    {
        get
        {
            return _exp;
        }
        set
        {
            _exp = value;
        }
    }
    public int current_exp
    {
        get
        {
            return _current_exp;
        }
        set
        {
            if (value >= _exp)
            {
                _current_exp = _exp;
            }
            else if (value > 0)
            {
                _current_exp = value;
            }
            else
            {
                _current_exp = 0;
            }
        }
    }

    //-----------------------end of setter getter if hp mp exp

    public int level = 0;//等级

    public string name = "测试玩家";//名字
    public int team = 0; // 阵营，可用于敌我判断

    public float crits_ratio = 10.0f;//暴击率//0~100 %
    public float crits_damage = 200.0f;//暴击伤害 表示200% 倍数

    public int damage = 10;//攻击力
    public int defends = 0;//防御力

    public int atk_level = 1;//基本攻击连招段数 默认不连招
    // for static data
    public float speed = 0.05f;//移动速度
    public float spine_time_scale = 1.0f;//骨骼动画播放速率比

    //for view 
    public string prefabsName = "Prefabs/Entity"; // default
    public string skin = "default";
    public string type = "6";

    // for state machine
    public bool isStand = true;
    public bool isJumping = false;
    public bool isJumpTwice = false;
    public bool isFalling = false;
    public bool isAttacking = false;
    public bool isRunning = false;
    public bool isHurt = false;
    public bool isDie = false; // die state but this will not sunc to other clients
    //public bool right_enable = true;//是否允许右移
    //  public bool left_enable = true;//是否允许左移动
    public string ani_atk3 = "2000";
    public string ani_atk2 = "2000";
    public string ani_atk1 = "2000";
    public string ani_stand = "stand";
    public string ani_jump = "jump";
    public string ani_jumpTwice = "doubleJump";
    public string ani_fall = "fall";
    public string ani_run = "run";
    public string ani_hurt = "hurt";
    public string ani_die = "die";

    public string attackingAnimationName = "";

    public bool is_spine_loop = true;
    //-----------------   for  config bullet type bu string
    public string bulleClassName_atk1 = "BulletConfig";//普通攻击 1段  的子弹名字
    public BulletConfigInfo bullet_atk1_info = null;
    public string bulleClassName_atk2 = "BulletConfig";//普通攻击 1段  的子弹名字
    public BulletConfigInfo bullet_atk2_info = null;
    public string bulleClassName_atk3 = "BulletConfig";//普通攻击 1段  的子弹名字
    public BulletConfigInfo bullet_atk3_info = null;

    public string bulleClassName_s1 = "BulletConfig"; // 1 号技能 子弹名字

    public override void UpdateMS()
    {
        machine.UpdateMS();
        bufferMgr.UpdateMS();//self buffer mgr update
        if (tick_combo.Tick())
        {

        }
        else
        {
            combo_time = 0;
        }

    }
    public override void OnDispose()
    {
        base.OnDispose();
        eventDispatcher.Dispose();
    }

    public override void OnExit()
    {
        base.OnExit();
        eventDispatcher.Dispose();
    }

    public override void OnEnter()
    {
        base.OnEnter();
        machine.OnEnter();
        bufferMgr.OnEnter();
    }

    public override bool Init()
    {
        base.Init();
        bounds = BoundsImpl.Create(new Vector3(0f, 0f, 0f), new Vector3(0.5f, 1f, 0.2f));
        _machine = StateMachine.Create(this);
        _eventDispatcher = EventDispatcher.Create("Entity");
        _bufferMgr = BufferMgr.Create(this);
        this.InitStateMachine();
        this.InitInfo();
        return true;
    }
    public virtual void InitStateMachine()
    {

    }
    public virtual void InitInfo()
    {

    }
    private StateMachine _machine = null;
    private EventDispatcher _eventDispatcher = null;
    private BufferMgr _bufferMgr = null;

    public StateMachine machine { get { return _machine; } }
    public EventDispatcher eventDispatcher { get { return _eventDispatcher; } }
    public BufferMgr bufferMgr { get { return _bufferMgr; } }

}