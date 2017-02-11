﻿using UnityEngine;
using System.Collections;


namespace Services
{

    public class Room : RpcService
    {
        public void EnterRoom(string msg, VoidFuncString cb)
        {
            HashTable hash = Json.Decode(msg);
            Hero hero = HeroMgr.Create<BaseHero>();
            hero.no = int.Parse(hash["no"]);

            cb("ret:ok,");
        }

        public void SelfEnterRoom(string msg, VoidFuncString cb)
        {
            Debug.Log("self enter room");
            HashTable hash = Json.Decode(msg);
            Hero hero = HeroMgr.Create<BaseHero>();
            hero.no = int.Parse(hash["no"]);
            HeroMgr.ins.self = hero;
            cb("ret:ok,");
        }

        public void NewPosition(string msg, VoidFuncString cb)
        {
            HashTable hash = Json.Decode(msg);
            float x = float.Parse(hash["x"]);
            float y = float.Parse(hash["y"]);
            int no = int.Parse(hash["no"]);

            Hero hero = HeroMgr.ins.GetHero(no);

            if (hero == null)
            {

                hero = HeroMgr.Create<BaseHero>();
                hero.no = int.Parse(hash["no"]);

            }
            hero.eventDispatcher.PostEvent(Events.ID_LOGIC_NEW_POSITION, new Vector2(x, y));



            cb("ret:ok,");

        }


    }
}