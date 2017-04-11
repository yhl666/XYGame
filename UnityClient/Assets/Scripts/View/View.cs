/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//说明
/*
 Model 和View 并不是一一对应关系，可能有的View绑定了model 有的没有，
 * 有的管理权在ViewMgr 有的在Model
 * 但是调度权 都在ViewMgr
 * 有的生命周期 跟随Model
 */
public class View : GAObject
{
    public override bool Init()
    {
        ViewMgr.ins.Add(this);
        return true;
    }

    public void BindModel(Model _model)
    {
        this._model = _model;
        //  _model.view = this;
    }


    protected Model _model;

    public Model model
    {
        set
        {
            this._model = value;
        }
        get
        {
            return this._model;
        }
    }
}

public class ViewBullet : View
{
    public override bool Init()
    {
        base.Init();
        m = model as Bullet;
        view_bullet = PrefabsMgr.Load("Prefabs/Animations");
        view_bullet.GetComponent<Animationstor>().file = m.plist;
        ani = view_bullet.GetComponent<Animationstor>();
        ani.Init();
        ani.ani.perFrame = m.frameDelay;
        float factor = 0.7f;
        ani.gameObject.transform.localScale = new Vector3(-m.flipX * factor, factor * ani.gameObject.transform.localScale.y,

             ani.gameObject.transform.localScale.z);

        ani.ani._OnComplete = () =>
            {
                m.OnComplete();
            };

        ani.gameObject.transform.position = new Vector3(m.x, m.y, ani.gameObject.transform.position.z);

        Vector3 rot = ani.gameObject.transform.localRotation.eulerAngles;
        ani.gameObject.transform.localRotation = Quaternion.Euler(new Vector3(rot.x, rot.y, m.rotate));

        ani.ani.SetLoop(1);
        ani.ani.Run();
        return true;
    }

    public override void OnDispose()
    {
        GameObject.DestroyImmediate(view_bullet);
    }


    public override void UpdateMS()
    {
        if (m.IsInValid())
        {
            this.SetInValid();
            return;
        }

        ani.ani.UpdateMS();
        //   SpriteFrame sp = ani.ani.GetCurrentSpriteFrame();
        /*  if (sp != null)
          {
              Debug.Log("11111111    "   +sp.rect.height );
              ani.gameObject.transform.position = new Vector3(m.x, sp.rect.height/2 / 100, ani.gameObject.transform.position.z);
          }
          else*/
        {
            ani.gameObject.transform.position = new Vector3(m.x, Utils.ConvertYZToView25DY(m.y, m.z), ani.gameObject.transform.position.z);

            float factor = 0.7f;
            ani.gameObject.transform.localScale = new Vector3(-m.flipX * factor * m.scale_x,
                factor * m.scale_y, ani.gameObject.transform.localScale.z);

        }

    }
    private GameObject view_bullet;
    private Animationstor ani;
    private Bullet m = null;

}

public class ViewEntity : View
{
    public override void OnDispose()
    {
        GameObject.DestroyImmediate(obj);
    }

    public override bool Init()
    {
        base.Init();
        this.m = model as Entity;

        this.obj = PrefabsMgr.Load(m.prefabsName);
        this.spine = obj.GetComponent<SkeletonAnimation>();
        spine.initialSkinName = m.skin;
        spine.skeleton.SetSkin(m.skin);
        this.transform = obj.GetComponent<Transform>();

        EventDispatcher.ins.AddEventListener(this, Events.ID_DIE);
        EventDispatcher.ins.AddEventListener(this, Events.ID_REVIVE);

        //init event

        spine.state._OnComplete = () =>
        {
            //   Debug.Log("_OnComplete");
            m.eventDispatcher.PostEvent("SpineComplete", spine.AnimationName);

        };

        spine.state._OnEnd = () =>
        {//
            // Debug.Log("end");
        };
        spine.state._OnStart = () =>
        {
            //   Debug.Log("start");
        };

        return true;
    }

    public override void OnEvent(int type, object userData)
    {
        Entity mm = userData as Entity;
        if (mm != m) return;

        if (type == Events.ID_DIE)
        {
            this.obj.SetActive(false);
        }
        else if (type == Events.ID_REVIVE)
        {
            this.obj.SetActive(true);
        }
    }

    public override void UpdateMS()
    {
        if (m.IsInValid())
        {
            this.SetInValid();
            return;
        }


        this.obj.name = m.no.ToString();
        if (Config.VIEW_MODE == ViewMode.M25D)
        {
            transform.position = new Vector3(m.x, m.GetReal25DY(), transform.position.z);
        }
        else
        {
            transform.position = new Vector3(m.x, m.GetRealY(), transform.position.z);
        }
        float factor = m.scale;
        transform.localScale = new Vector3(m.flipX * factor, factor, factor);

        string name = "";
        //优先级匹配，状态可能组合，但是动画只有一个
        if (m.ani_force != "")
        {
            name = m.ani_force;
        }
        else if (m.isDie)
        {
            ///   name = m.ani_die;
        }
        else if (m.isAttacking)
        {
            name = m.attackingAnimationName;
        }
        else if (m.isHurt)
        {
            name = m.ani_hurt;
        }
        else if (m.isJumpTwice)
        {
            name = m.ani_jumpTwice;
        }
        else if (m.isJumping)
        {
            name = m.ani_jump;
        }
        else if (m.isFalling)
        {
            name = m.ani_fall;
        }
        else if (m.isRunning)
        {
            name = m.ani_run;
        }
        else if (m.isStand)
        {
            name = m.ani_stand;
        }
        spine.loop = m.is_spine_loop;

        spine.AnimationName = name;

        if (m.delta_hp != 0)
        {

            BattleFlyTextInfo info = BattleFlyTextInfo.Create();
            if (m.delta_hp > 0)
            {
                //加血
                info.color = BattleFlyTextInfo.COLOR_HP_ADD;
            }
            else if (m.delta_hp < 0)
            {//减血

                info.color = BattleFlyTextInfo.COLOR_HP_REDUCE;
            }

            info.txt = m.delta_hp.ToString(); ;
            info.position_world_x = m.x + UnityEngine.Random.Range(-0.5f, 0.5f);
            info.position_world_y = m.GetReal25DY() + 0.8f;
            EventDispatcher.ins.PostEvent(Events.ID_BATTLE_FLYTEXT, info);
            m.delta_hp = 0;
        }
        spine.UpdateMS(Utils.deltaTime * m.spine_time_scale);
    }

    SkeletonAnimation spine = null;
    Transform transform = null;
    GameObject obj = null;
    Entity m = null;
}


public class ViewEnemy : View
{

    public override void OnDispose()
    {
        GameObject.DestroyImmediate(obj);
    }
    public override bool Init()
    {
        base.Init();
        this.m = model as Entity;

        this.obj = PrefabsMgr.Load(m.prefabsName);
        this.spine = obj.GetComponent<SkeletonAnimation>();
        spine.initialSkinName = m.skin;
        spine.skeleton.SetSkin(m.skin);
        this.transform = obj.GetComponent<Transform>();
        EventDispatcher.ins.AddEventListener(this, Events.ID_DIE);


        //init event

        spine.state._OnComplete = () =>
        {
            //   Debug.Log("_OnComplete");
            m.eventDispatcher.PostEvent("SpineComplete", spine.AnimationName);

        };
        spine.state._OnEnd = () =>
        {//
            // Debug.Log("end");
        };
        spine.state._OnStart = () =>
        {
            //   Debug.Log("start");
        };

        return true;
    }

    public override void OnEvent(int type, object userData)
    {
        if (type == Events.ID_DIE)
        {
            if (userData as Entity == m)
                this.SetInValid();
        }
    }

    public override void UpdateMS()
    {
        if (m.IsInValid())
        {
            this.SetInValid();
            return;
        }
        this.obj.name = m.no.ToString();
        transform.position = new Vector3(m.x, m.GetReal25DY(), transform.position.z);
        float factor = m.scale;
        transform.localScale = new Vector3(m.flipX * factor, factor, factor);

        string name = "";
        //优先级匹配，状态可能组合，但是动画只有一个
        if (m.ani_force != "")
        {
            name = m.ani_force;
        }
        else if (m.isDie)
        {
            name = m.ani_die;
        }
        else if (m.isHurt && m.ani_hurt != "")
        {
            name = m.ani_hurt;
        }
        else if (m.isAttacking)
        {
            name = m.attackingAnimationName;
        }
        /*  else if (m.isJumpTwice)
          {
              name = m.ani_jumpTwice;
          }
          else if (m.isJumping)
          {
              name = m.ani_jump;
          }
          else if (m.isFalling)
          {
              name = m.ani_fall;
          }*/
        else if (m.isRunning)
        {
            name = m.ani_run;
        }
        else if (m.isStand)
        {
            name = m.ani_stand;
        }

        spine.AnimationName = name;

        if (m.delta_hp != 0)
        {

            BattleFlyTextInfo info = BattleFlyTextInfo.Create();
            if (m.delta_hp > 0)
            {
                //加血
                info.color = BattleFlyTextInfo.COLOR_HP_ADD;
            }
            else if (m.delta_hp < 0)
            {//减血

                info.color = BattleFlyTextInfo.COLOR_HP_REDUCE;
            }

            info.txt = m.delta_hp.ToString(); ;
            info.position_world_x = m.x + UnityEngine.Random.Range(-0.5f, 0.5f);
            info.position_world_y = m.GetReal25DY() + 0.8f;
            EventDispatcher.ins.PostEvent(Events.ID_BATTLE_FLYTEXT, info);
            m.delta_hp = 0;
        }

        spine.UpdateMS(Utils.deltaTime * m.spine_time_scale);
    }

    SkeletonAnimation spine = null;
    Transform transform = null;
    GameObject obj = null;
    Entity m = null;
}

public class ViewBuffer : View
{
    public override bool Init()
    {
        base.Init();
        m = model as Buffer;
        view_bullet = PrefabsMgr.Load("Prefabs/Animations");
        view_bullet.GetComponent<Animationstor>().file = m.plist;
        ani = view_bullet.GetComponent<Animationstor>();

        ani.Init();
        ani.ani.Run();

        ani.ani.SetLoop(0xffffff);
        ani.gameObject.transform.localScale = new Vector3(m.scale, m.scale, ani.gameObject.transform.localScale.z);
        return true;
    }

    public override void OnDispose()
    {
        GameObject.Destroy(view_bullet);
    }


    public override void UpdateMS()
    {
        if (m.IsInValid())
        {
            this.SetInValid();
        }

        ani.ani.UpdateMS();

        //  ani.gameObject.transform.position = new Vector3(m.x, m.y, ani.gameObject.transform.position.z);
        ani.gameObject.transform.position = new Vector3(m.target.x, m.target.GetReal25DY() + 0.7f, ani.gameObject.transform.position.z);

    }
    private GameObject view_bullet;
    private Animationstor ani;
    private Buffer m = null;

}


public class ViewBuilding : View
{
    public override void OnDispose()
    {
        GameObject.DestroyImmediate(obj);
    }

    public override bool Init()
    {
        base.Init();
        this.m = model as Building;

        this.obj = PrefabsMgr.Load(m.prefabsName);
        this.spine = obj.GetComponent<SkeletonAnimation>();
        spine.initialSkinName = m.skin;
        spine.skeleton.SetSkin(m.skin);
        this.transform = obj.GetComponent<Transform>();
        obj.name = "Building-" + m.GetType().ToString();
        EventDispatcher.ins.AddEventListener(this, Events.ID_DIE);
        EventDispatcher.ins.AddEventListener(this, Events.ID_REVIVE);

        //init event

        spine.state._OnComplete = () =>
        {
            //   Debug.Log("_OnComplete");
            m.eventDispatcher.PostEvent("SpineComplete", spine.AnimationName);

        };

        spine.state._OnEnd = () =>
        {//
            // Debug.Log("end");
        };
        spine.state._OnStart = () =>
        {
            //   Debug.Log("start");
        };

        return true;
    }

    public override void OnEvent(int type, object userData)
    {
        Entity mm = userData as Entity;
        if (mm != m) return;

        if (type == Events.ID_DIE)
        {
            this.obj.SetActive(false);
        }
        else if (type == Events.ID_REVIVE)
        {
            this.obj.SetActive(true);
        }
    }

    public override void UpdateMS()
    {
        if (m.IsInValid())
        {
            this.SetInValid();
            return;
        }
        if (Config.VIEW_MODE == ViewMode.M25D)
        {
            transform.position = new Vector3(m.x, m.GetReal25DY(), transform.position.z);
        }
        else
        {
            transform.position = new Vector3(m.x, m.GetRealY(), transform.position.z);
        }
        float factor = m.scale;
        transform.localScale = new Vector3(m.flipX * factor, factor, factor);

        string name = "";
        //优先级匹配，状态可能组合，但是动画只有一个
        if (m.isDie)
        {

            ///    name = m.ani_die;
        }
        else if (m.isAttacking)
        {
            name = m.attackingAnimationName;
        }
        /*   else if (m.isHurt)
           {
               name = m.ani_hurt;
           }*/
        else if (m.isStand)
        {
            name = m.ani_stand;
        }
        spine.loop = m.is_spine_loop;

        spine.AnimationName = name;

        if (m.delta_hp != 0)
        {

            BattleFlyTextInfo info = BattleFlyTextInfo.Create();
            if (m.delta_hp > 0)
            {
                //加血
                info.color = BattleFlyTextInfo.COLOR_HP_ADD;
            }
            else if (m.delta_hp < 0)
            {//减血

                info.color = BattleFlyTextInfo.COLOR_HP_REDUCE;
            }

            info.txt = m.delta_hp.ToString(); ;
            info.position_world_x = m.x + UnityEngine.Random.Range(-0.5f, 0.5f);
            info.position_world_y = m.GetReal25DY() + 0.8f;
            EventDispatcher.ins.PostEvent(Events.ID_BATTLE_FLYTEXT, info);
            m.delta_hp = 0;
        }
        spine.UpdateMS(Utils.deltaTime * m.spine_time_scale);
    }

    SkeletonAnimation spine = null;
    Transform transform = null;
    GameObject obj = null;
    Building m = null;
}

