using UnityEngine;
using System.Collections;
using System;


public class CameraFollow : MonoBehaviour
{
    void LateUpdate()
    {
        if (AppMgr.GetCurrentApp() == null) return;
        Hero hero = HeroMgr.ins.GetSelfHero();
        if (hero == null) return;

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

        float delta = WIDTH / 100.0f / 4.0f;//推动滚动 的宽度值
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
}
