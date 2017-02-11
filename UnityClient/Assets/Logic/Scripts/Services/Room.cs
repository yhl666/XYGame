using UnityEngine;
using System.Collections;


namespace Services
{

    public class Room : RpcService
    {
        public void EnterRoom(string msg, VoidFuncString cb)
        {
            HashTable hash = Json.Decode(msg);
            BaseHero hero = BaseHeroMgr.Create<BaseHero>();
            hero.no = int.Parse(hash["no"]);

            cb("ret:ok,");
        }

        public void SelfEnterRoom(string msg, VoidFuncString cb)
        {
            Debug.Log("self enter room");
            HashTable hash = Json.Decode(msg);
            BaseHero hero = BaseHeroMgr.Create<BaseHero>();
            hero.no = int.Parse(hash["no"]);
            BaseHeroMgr.ins.self = hero;
            cb("ret:ok,");
        }

        public void NewPosition(string msg, VoidFuncString cb)
        {
            HashTable hash = Json.Decode(msg);
            float x = float.Parse(hash["x"]);
            float y = float.Parse(hash["y"]);
            int no = int.Parse(hash["no"]);

            BaseHero hero = BaseHeroMgr.ins.GetBaseHero(no);

            if (hero == null)
            {

                hero = BaseHeroMgr.Create<BaseHero>();
                hero.no = int.Parse(hash["no"]);

            }
            hero.eventDispatcher.PostEvent(Events.ID_LOGIC_NEW_POSITION, new Vector2(x, y));



            cb("ret:ok,");

        }


    }
}