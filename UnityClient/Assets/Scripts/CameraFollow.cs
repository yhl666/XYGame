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


        Terrain terrain=BattleApp.ins.GetCurrentWorldMap().GetTerrain();


        float x_min = Screen.width / 100.0f / 2.0f +  terrain.limit_x_left   ;//
        float x_max = -Screen.width / 100.0f / 2.0f + terrain.limit_x_right;



        float y_min = Screen.height / 100.0f / 2.0f + terrain.limit_y_down;//
        float y_max = -Screen.height / 100.0f / 2.0f + terrain.limit_y_up;


   


        if (x < x_min) x = x_min;
        if (x > x_max) x = x_max;

        if (y < y_min) y = y_min;
        if (y > y_max) y = y_max;


        this.transform.position = new Vector3(x, y, this.transform.position.z);

        var bg = GameObject.Find("bg_static");
        bg.transform.position = Utils.Vector3To2DVector3(this.transform.position, bg.transform.position);


        /*  Ball self = BallsMgr.ins.GetSelfBalls();
          Debug.LogError("3333333333  +" + self.GetHashCode().ToString() + "   " + this.self.GetHashCode());
          if (this.self == self)
          {

          }
          else
          {
            //  Debug.Log("11111111122222222222211111111");
             Ball b=BallsMgr.ins.GetSelfBalls();
              if(b==null)return;
              if(b.view==null)return;
              if (b.view._root == null) return;

              GameObject obj = b.view._root;
              if (obj == null)
              {
                  Debug.Log("Can not Find");
                  return;
              }
              target = obj;
           //   this.no = self.no;
            //  this.self = self;
          }

          this.transform.position = new Vector3(target.transform.position.x, target.transform.position.y, transform.position.z);


          */
    }



}
