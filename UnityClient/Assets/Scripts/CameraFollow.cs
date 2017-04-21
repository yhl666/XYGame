/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;
using System;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow ins = null;
    void Awake()
    {
        ins = this;
    }
    void Start()
    {
        ins = this;
        orthographicSize = this.GetComponent<Camera>().orthographicSize;
        obj_bg_static = GameObject.Find("bg_static");
        obj_bg = GameObject.Find("Terrain/bg");
    }
    bool is_other = false;
    Entity last_trace = null;
    void LateUpdate()
    {
        if (_enable == false) return;
        if (AppMgr.GetCurrentApp() == null) return;
        Hero hero = HeroMgr.ins.GetSelfHero();
        if (hero == null) return;
        if (hero.isDie && PublicData.ins.left_revive_times <= 0)
        {//自身角色死亡 开始 查看其它玩家视角
            ArrayList list = HeroMgr.ins.GetHeros();
            if (list.Count <= 0)
            {

            }
            else
            {
                hero = list[0] as Hero;
                is_other = true;
            }
        }
        last_trace = hero;

        GameObject obj = GameObject.Find(hero.no.ToString());
        if (obj == null) return;

        //design  camera 's width and height  not  Screen wieth and height
        float WIDTH = 1136.0f;// Screen.width;
        float HEIGHT = 640.0f;// Screen.height;

        if ((float)Screen.width / (float)Screen.height - 1.5 <= 0.01)
        {
            WIDTH = 960.0f;
            HEIGHT = 640.0f;
        }


        /* WIDTH = Screen.width;
         HEIGHT = Screen.height;

         this.GetComponent<Camera>().orthographicSize = Screen.height / 100.0f / 2.0f;
         */
        //跟随 地形滚动
        float x = obj.transform.position.x;
        float y = obj.transform.position.y;


        Terrain terrain = AppMgr.GetCurrentApp().GetCurrentWorldMap().GetTerrain();

        float x_min = WIDTH / 100.0f / 2.0f + terrain.limit_x_left;//
        float x_max = -WIDTH / 100.0f / 2.0f + terrain.limit_x_right;

        float y_min = HEIGHT / 100.0f / 2.0f + terrain.limit_y_down;//
        float y_max = -HEIGHT / 100.0f / 2.0f + terrain.limit_y_up;

        if (y < y_min) y = y_min;
        if (y > y_max) y = y_max;

        float delta = WIDTH / 100.0f / 8.0f;//推动滚动 的宽度值
        if (Mathf.Abs(this.transform.position.x - x) > delta)
        {
            if (this.transform.position.x < x)
            {
                x -= delta;
            }
            else
            {
                x += delta;
            }
            if (x < x_min) x = x_min;
            if (x > x_max) x = x_max;
        }
        else
        {
            x = this.transform.position.x;
        }

        this.transform.position = new Vector3(x, y, this.transform.position.z);


        var bg = GameObject.Find("bg_static");

        //静态 背景 图 滚动
        var sp = bg.GetComponent<SpriteRenderer>().sprite;


        float HALF = (sp.texture.width - WIDTH) / 100.0f / 2.0f;

        float MAX = HALF * 2.0f;
        float percent = 0.5f;

        //计算相机百分比

        //相机坐标最大值
        float max_camera = (terrain.limit_x_right - WIDTH / 100.0f / 2.0f - WIDTH / 100.0f / 2.0f);

        percent = (this.transform.position.x - WIDTH / 100.0f / 2.0f) / max_camera;


        bg.transform.position = new Vector3(
          this.transform.position.x + HALF - percent * MAX,//起始位置加上起始偏移量，  减去比例大小
         this.transform.position.y
         , bg.transform.position.z);
    }


    private void SharpX()
    {

    }
    /// <summary>
    /// 开始Hero特写
    /// </summary>
    public void ShowHeroFinal()
    {
        is_skill = true;
        _enable = false;
        ///  obj_bg.SetActive(false);
        ///   obj_bg_static.SetActive(false);
        delta = 1.0f;
        Hero self = HeroMgr.ins.self;
        pos_pre = this.transform.position;
        this.transform.position = new Vector3(self.x, self.GetReal25DY() + 2.0f, this.transform.position.z);
        UIBattleRoot.ins.Hide();

        FadeOut.Create(obj_bg_static.GetComponent<SpriteRenderer>(), 1.0f);
        FadeOut.Create(obj_bg.GetComponent<SpriteRenderer>(), 1.0f);
    }
    public void HideHeroFinal()
    {
        is_skill = false;
        _enable = true;
        UIBattleRoot.ins.Show();
        this.transform.position = pos_pre;
        //  obj_bg.SetActive(true);
        //  obj_bg_static.SetActive(true);
        this.GetComponent<Camera>().orthographicSize = orthographicSize;
        FadeIn.Create(obj_bg_static.GetComponent<SpriteRenderer>(), 0f);
        FadeIn.Create(obj_bg.GetComponent<SpriteRenderer>(), 0f);
    }

    public void ShowBossDie()
    {
        _enable = false;
        delta = 1.0f;
        UIBattleRoot.ins.Hide();
        TimerQueue.ins.AddTimer(0.025f, () =>
           {
               delta -= 0.01f;
               this.GetComponent<Camera>().orthographicSize = orthographicSize * delta;
           }, 30);

        if (PublicData.ins.battle_result == BattleResult.Win)
        {
            EnemyBoss boss = EnemyMgr.ins.GetEnemy<EnemyBoss>();
            boss.machine.Pause();
            boss.enable_ai = false;
            boss.isHurt = true;
            boss.is_spine_loop = true;
            Config.fps_delay = 0.1f;
            ///    Utils.SetTargetFPS(10);
            // transform.localPosition = new Vector3(23f, transform.localPosition.y, transform.localPosition.z);
            float x = transform.localPosition.x;
            Hero self = last_trace as Hero;
            //  self.x = 23f;
            float selfx = self.x;//23f;
            //   MoveTo.Create(this.gameObject, 0.5f / 23f * Mathf.Abs(selfx - boss.x), boss.x, transform.localPosition.y).OnComptele = () =>
            if (Mathf.Abs(selfx - boss.x) > 5f)
            {//距离大于5 才会滚动
                MoveTo.Create(this.gameObject, 0.5f / 23f * Mathf.Abs(selfx - boss.x), boss.x, transform.localPosition.y).OnComptele = () =>
                {
                    //  HeroMgr.ins.self.ani_force = "6230_0";
                    //   HeroMgr.ins.self.is_spine_loop = true;

                    CallFunc.Create(this.gameObject, 1f, null
                        ).OnComptele = () =>
                        {
                            TimerQueue.ins.AddTimer(0.025f, () =>
                            {
                                delta += 0.01f;
                                this.GetComponent<Camera>().orthographicSize = orthographicSize * delta;
                            }, 30);
                            //恢复正常
                            Config.fps_delay = 0.01f;
                            MoveTo.Create(this.gameObject, 0.5f / 23f * Mathf.Abs(selfx - boss.x), x, transform.localPosition.y).OnComptele = () =>
                            {//滚回视角


                                CallFunc.Create(this.gameObject, 2, null
                         ).OnComptele = () =>
                         {//2秒后 执行正常结束游戏逻辑
                             UIBattleRoot.ins.Show();

                             AppMgr.GetCurrentApp<BattleApp>().SetGameOver();
                         };
                            };
                        };
                };
            }
            else
            {//不滚动视角
                CallFunc.Create(this.gameObject, 1, null
                    ).OnComptele = () =>
                    {
                        //   HeroMgr.ins.self.ani_force = "6230_0";
                        //  HeroMgr.ins.self.is_spine_loop = true;

                        //恢复正常
                        Config.fps_delay = 0.01f;
                        CallFunc.Create(this.gameObject, 1f, null
        ).OnComptele = () =>
        {
            TimerQueue.ins.AddTimer(0.025f, () =>
            {
                delta += 0.01f;
                this.GetComponent<Camera>().orthographicSize = orthographicSize * delta;
            }, 30);

        };
                        CallFunc.Create(this.gameObject, 2, null
                 ).OnComptele = () =>
                 {//2秒后 执行正常结束游戏逻辑
                     AppMgr.GetCurrentApp<BattleApp>().SetGameOver();
                     UIBattleRoot.ins.Show();

                 };
                    };
            }
        }
        else if (PublicData.ins.battle_result == BattleResult.Lose)
        {
     
            Config.fps_delay = 0.1f;

            float x = transform.localPosition.x;
            Hero self = last_trace as Hero;
            //  self.x = 23f;
            float selfx = self.x;//23f;

            //不滚动视角
                CallFunc.Create(this.gameObject, 2, null
                    ).OnComptele = () =>
                    {
                        //恢复正常  View 也是视图也是2秒 后隐藏
                        Config.fps_delay = 0.01f;
                        TimerQueue.ins.AddTimer(0.025f, () =>
                        {
                            delta += 0.01f;
                            this.GetComponent<Camera>().orthographicSize = orthographicSize * delta;
                        }, 30);
                        CallFunc.Create(this.gameObject, 1, null
                 ).OnComptele = () =>
                 {//2秒后 执行正常结束游戏逻辑
                     AppMgr.GetCurrentApp<BattleApp>().SetGameOver();
                     UIBattleRoot.ins.Show();

                 };
                    };


        }
        /*  MoveTo.Create(this.gameObject, 0.0001f, 23, transform.localPosition.y).OnComptele = () =>
           {
               MoveTo.Create(this.gameObject, 0.5f, x, transform.localPosition.y).OnComptele = () =>
               {
                   CallFunc.Create(this.gameObject, 1, null
                       ).OnComptele = () =>
                   {
                       //   boss.SetInValid();
                       //恢复正常
                       Config.fps_delay = 0.01f;
                       CallFunc.Create(this.gameObject, 2, null
                ).OnComptele = () =>
                {//2秒后 执行正常结束游戏逻辑

                    AppMgr.GetCurrentApp<BattleApp>().SetGameOver();
                };


                   };
               };
           };
          */
    }

    void Update()
    {
        if (_enable == false)
        {
            if (is_skill && this.GetComponent<Camera>().orthographicSize > 2.0f)
            {
                delta -= 0.01f;
                this.GetComponent<Camera>().orthographicSize = orthographicSize * delta;
            }
        }
    }
    bool is_skill = false;

    float delta = 1.0f;
    Vector3 pos_pre;
    GameObject obj_bg = null;
    GameObject obj_bg_static = null;
    float orthographicSize;
    bool _enable = true;
}