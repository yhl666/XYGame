using UnityEngine;
using System.Collections;
using System;


public class CameraFollow : MonoBehaviour
{
    void LateUpdate()
    {
        Hero hero = HeroMgr.ins.GetSelfHero();
        if (hero == null) return;

        GameObject obj = GameObject.Find(hero.no.ToString());
        if (obj == null) return;

        float x = obj.transform.position.x;
        float y = obj.transform.position.y;


        Terrain terrain = BattleApp.ins.GetCurrentWorldMap().GetTerrain();


        float x_min = Screen.width / 100.0f / 2.0f + terrain.limit_x_left;//
        float x_max = -Screen.width / 100.0f / 2.0f + terrain.limit_x_right;



        float y_min = Screen.height / 100.0f / 2.0f + terrain.limit_y_down;//
        float y_max = -Screen.height / 100.0f / 2.0f + terrain.limit_y_up;


        if (x < x_min) x = x_min;
        if (x > x_max) x = x_max;

        if (y < y_min) y = y_min;
        if (y > y_max) y = y_max;


        //   this.transform.position = new Vector3(x, y, this.transform.position.z);



        queue.Enqueue(new Vector3(x, y, this.transform.position.z));

        if (queue.Count > 40)
        {
            t = 0;
            last = this.transform.position;
            while (queue.Count > 0)
            {
                next = (Vector3)queue.Dequeue();
            }

            queue.Clear();

        }
        t += 0.01f;
        if (t >= 1.0f) t = 1.0f;

      

      //  this.transform.position = new Vector3(Mathf.Lerp(last.x, next.x, t),Mathf.Lerp(last.y, next.y, t), -900);
      //  this.transform.position = new Vector3(Mathf.SmoothStep(last.x, next.x, t), Mathf.SmoothStep(last.y, next.y, t), -900);
        this.transform.position = new Vector3(Mathf.SmoothDamp(transform.position.x, next.x, ref speed, Time.deltaTime,3.0f), 
          //  Mathf.SmoothDamp(transform.position.y, next.y, ref speed, Time.deltaTime,3.0f)
            
            Mathf.Lerp(last.y, next.y, t)
            
            , -900);



        var bg = GameObject.Find("bg_static");
        bg.transform.position = Utils.Vector3To2DVector3(this.transform.position, bg.transform.position);



    }

    void Start()
    {

    }

    float speed = 0f;
    float t = 0.0f;
    Vector3 next;
    Vector3 last;
    Queue queue = new Queue();
}
