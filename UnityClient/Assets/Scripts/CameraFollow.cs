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


        this.transform.position = new Vector3( obj.transform.position.x , obj.transform.position.y,this.transform.position.z);


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
