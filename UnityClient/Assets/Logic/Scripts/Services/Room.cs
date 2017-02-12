using UnityEngine;
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
            hero.x = 5f;
            hero.y = 1.5f;

            cb("ret:ok,");
        }

        public void NewPosition(string msg, VoidFuncString cb)
        {
            cb("ret:ok,");

            HashTable hash = Json.Decode(msg);
            float x = float.Parse(hash["x"]);
            float y = float.Parse(hash["y"]);
            int no = int.Parse(hash["no"]);

            Hero hero = HeroMgr.ins.GetHero(no);

            if (hero == null)
            {
                return;
                hero = HeroMgr.Create<BaseHero>();
                hero.no = int.Parse(hash["no"]);

            }
            hero.eventDispatcher.PostEvent(Events.ID_LOGIC_NEW_POSITION, new Vector2(x, y));



       

        }

        public void LeaveRoom(string msg, VoidFuncString cb)
        {
            HashTable hash = Json.Decode(msg);
            int no = int.Parse(hash["no"]);
            HeroMgr.ins.Remove(no);
            cb("ret:ok,");
        }
    }
}