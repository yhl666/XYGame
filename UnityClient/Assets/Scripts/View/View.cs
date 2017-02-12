using UnityEngine;
using System.Collections;
using UnityEngine.UI;

using Spine.Unity;

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

        float factor = 0.7f;
        ani.gameObject.transform.localScale = new Vector3(-m.flipX*factor,factor* ani.gameObject.transform.localScale.y,

             ani.gameObject.transform.localScale.z);

        ani.ani._OnComplete = () =>
            {
                m.OnComplete();
            };

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
            ani.gameObject.transform.position = new Vector3(m.x, m.y, ani.gameObject.transform.position.z);

            float factor = 0.8f;

        
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



    public override void UpdateMS()
    {
        if (m.IsInValid())
        {
            this.SetInValid();
            return;
        }


        this.obj.name = m.no.ToString();
        transform.position = new Vector3(m.x, m.y + m.height, transform.position.z);
        float factor   = m.scale;
        transform.localScale = new Vector3(m.flipX * factor, factor, factor);

        string name = "";
        //优先级匹配，状态可能组合，但是动画只有一个
        if (m.isHurt)
        {
            name = m.ani_hurt;
        }
        else if (m.isAttacking)
        {
            name = m.attackingAnimationName;
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


        spine.AnimationName = name;


        spine.UpdateMS(Utils.deltaTime);
    }


    SkeletonAnimation spine = null;
    Transform transform = null;
    GameObject obj = null;
    Entity m = null;
}




public class ViewBuffer2_1 : View
{
    public override bool Init()
    {
        base.Init();
        m = model as Buffer2_1;
        /*   view_bullet = PrefabsMgr.Load("Prefabs/Animations");
           view_bullet.GetComponent<Animationstor>().file = m.plist;
           ani = view_bullet.GetComponent<Animationstor>();

           ani.gameObject.transform.localScale = new Vector3(ani.gameObject.transform.localScale.x,
                 -m.flipX,
                ani.gameObject.transform.localScale.z);
           */
        return true;
    }

    public override void OnDispose()
    {
        //  GameObject.DestroyImmediate(view_bullet);
    }


    public override void UpdateMS()
    {
        if (m.IsInValid())
        {
            this.SetInValid();


        }
        //   ani.ani.UpdateMS();

        //  ani.gameObject.transform.position = new Vector3(m.x, m.y, ani.gameObject.transform.position.z);

    }
    private GameObject view_bullet;
    private Animationstor ani;
    private Buffer2_1 m = null;

}

