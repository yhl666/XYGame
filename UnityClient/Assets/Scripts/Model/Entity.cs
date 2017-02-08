using UnityEngine;
using System.Collections;
using System;




public class AttackInfo
{
    public static AttackInfo Create()
    {
        AttackInfo ret = new AttackInfo();

        return ret;
    }
    private AttackInfo()
    {
    }
    public Entity target = null;//为null 标示 群体攻击 为一个Entity 表示单体攻击 //TODO 为数组 表示部分群体攻击
    public Entity ownner = null;
    public Func<AttackInfo, bool> atk_func = null;

    public void Invoke()
    {
        atk_func.Invoke(this);
    }


    //add buffer to atk info
    public void AddBuffer(Buffer buf)
    {
        this.buffers.Add(buf);
    }
    public ArrayList buffers = new ArrayList();
}






public class Entity : Model
{

    //---------------for player input state and frame sync

    public bool left = false;//left
    public bool right = false;//right
    public bool jump = false;//jump
    public bool atk = false;//atk
    public bool s1 = false;//skill 1
    public bool stand = false;//stand


    //--------------------------------------------------interface for outside game logic-----
    /// <summary>
    ///  outside to add buffer by type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public Buffer AddBuffer<T>() where T : new()
    {
        return this.bufferMgr.Create<T>();
    }

    public Buffer AddBuffer(string buffer)
    {
        return this.bufferMgr.Create(buffer);
    }
    /// <summary>
    ///  accpet attacl
    /// </summary>
    /// <param name="who"></param>
    public void TakeAttack(Entity who)
    {
        this.current_hp -= who.GetRealAttackDamage();
        Debug.Log(this.current_hp);
        //  this.isHurt = true;
        eventDispatcher.PostEvent(Events.ID_HURT);
    }



    public System.Random _random = new System.Random(0);
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
        return this.ClaculateDistance(other.x, other.y);
    }
    public float ClaculateDistance(float x, float y)
    {
        return Utils.ClaculateDistance(new Vector2(this.x, this.y), new Vector2(x, y));
    }
    //------------------------------------------------------some event 
    /// <summary>
    /// 血量为0 时触发
    /// </summary>
    public void OmEmptyHp()
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

    // base property
    public float x = 0.0f;
    public float y = 0.0f;
    public int no = 0;//临时识别编号 比如战斗
    public string id = ""; // 唯一id
    public float flipX = 1.0f;

    public bool attackAble = true;

    //for dynamic data
    private int _hp = 1000;//气血
    private int _current_hp = 500;//当前气血

    private int _mp = 500;//魔法
    private int _current_mp = 0;//当前魔法

    private int _exp = 100;//经验
    private int _current_exp = 0;//当前经验


    private float _height = 0.0f;//海拔
    public bool isInOneTerrainRight = false;
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
                _current_hp = _hp;
            }
            else if (value > 0)
            {
                _current_hp = value;
            }
            else
            {
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
    public float speed = 1.0f;//移动速度


    //for view 
    public string prefabsName = "Prefabs/Entity"; // default
    public string skin = "default";



    // for state machine
    public bool isStand = true;
    public bool isJumping = false;
    public bool isJumpTwice = false;
    public bool isFalling = false;
    public bool isAttacking = false;
    public bool isRunning = false;
    public bool isHurt = false;

    //public bool right_enable = true;//是否允许右移
    //  public bool left_enable = true;//是否允许左移动

    public string ani_stand = "stand";
    public string ani_jump = "jump";
    public string ani_jumpTwice = "doubleJump";
    public string ani_fall = "fall";
    public string ani_atk = "2000";
    public string ani_run = "run";
    public string ani_hurt = "hurt";

    public string attackingAnimationName = "";


    //-----------------   for  config bullet type bu string
    public string bulleClassName_atk1 = "BulletConfig";//普通攻击 1段  的子弹名字
    public BulletConfigInfo bullet_atk1_info = null;

    public string bulleClassName_s1 = "BulletConfig"; // 1 号技能 子弹名字



    public override void UpdateMS()
    {
        machine.UpdateMS();
        bufferMgr.UpdateMS();//self buffer mgr update
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

        machine = StateMachine.Create(this);
        eventDispatcher = EventDispatcher.Create("Entity");
        bufferMgr = BufferMgr.Create(this);

        return true;
    }

    public StateMachine machine = null;
    public EventDispatcher eventDispatcher = null;
    public BufferMgr bufferMgr = null;


}


