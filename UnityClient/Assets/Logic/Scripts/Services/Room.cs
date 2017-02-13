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
            hero.no = hash.GetInt("no");

            cb("ret:ok,");
        }

        public void SelfEnterRoom(string msg, VoidFuncString cb)
        {
            HashTable hash = Json.Decode(msg);
            Hero hero = HeroMgr.Create<BaseHero>();
            hero.name = PublicData.GetInstance().self_name;

            hero.no = hash.GetInt("no");
            HeroMgr.ins.self = hero;
            hero.x = 5f;
            hero.y = 1f;

            cb("ret:ok,");
        }

        public void NewPosition(string msg, VoidFuncString cb)
        {
            cb("ret:ok,");

            HashTable hash = Json.Decode(msg);
            float x = hash.GetFloat("x");
            float y = float.Parse(hash["y"]);
            int no = int.Parse(hash["no"]);

            BaseHero hero = HeroMgr.ins.GetHero(no) as BaseHero;
            if (hero == HeroMgr.ins.GetSelfHero()) return;
            if (hero == null)
            {
                /// return;
                hero = HeroMgr.Create<BaseHero>();
                hero.no = int.Parse(hash["no"]);
                hero.x = 5f;
                hero.y = 1f;
            }
            hero.ResetTick();
            hero.eventDispatcher.PostEvent(Events.ID_LOGIC_NEW_POSITION, new Vector2(x, y));
        }
        public void CheckAlive(string msg, VoidFuncString cb)
        {
            cb("ret:ok,");

            HashTable hash = Json.Decode(msg);
            int no = hash.GetInt("no");

            BaseHero hero = HeroMgr.ins.GetHero(no) as BaseHero;
            if (hero == HeroMgr.ins.GetSelfHero())
            {
                AppMgr.GetCurrentApp<TownApp>().ResetTick();
            }
            if (hero == null)
            {
                return;
            }
            hero.ResetTick();
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