/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;
using System;


public class CameraFollow2 : MonoBehaviour
{
  
    void LateUpdate()
    {
        if (AppMgr.GetCurrentApp() == null) return;
        Hero hero = HeroMgr.ins.GetSelfHero();
        if (hero == null) return;
        Vector3 pos= this.transform.position ;
    ///    pos.y = hero.z+ hero.y;
        pos.x = hero.x;
     ///   pos.z = -20f;
        this.transform.position = pos;
      

    }

}
