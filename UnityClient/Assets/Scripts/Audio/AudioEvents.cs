/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;

public class AudioEvents
{
    public static AudioEvents ins = null;
    public AudioEvents()
    {
        ins = this;
    }
    class Pair
    {
        public Events first = Events.HERO_HURT;
        public AudioClip second = null;
    }
    public enum Events
    {
        MIN = 0,
        BATTLE_BG,
        BATTLE_BOSS_BG,
        HERO_HURT,
        HERO_ATK1,
        HERO_ATK2,
        HERO_ATK3,
        HERO_SKILL11,
        HERO_SKILL12,
        HERO_SKILL13,
        HERO_SKILL21,
       // HERO_SKILL22,
        HERO_SKILL23,
        MAX,
    }

    public AudioClip GetClip(Events e)
    {
        return (events[(int)e] as Pair).second;
    }
    public void LoadAllEvents()
    {
        string[] ss = { 
                          "",
                          "bg", 
                          "bgboss", 
                          "effect/hurt",
                          "effect/atk1",
                           "effect/atk2",
                           "effect/atk3",
      
                            "effect/skill11",
                           "effect/skill12",
                           "effect/skill13",
                           "effect/skill21",
                           "effect/skill23",
          
                          "","" };
        for (int i = (int)Events.MIN; i <= (int)Events.MAX; i++)
        {
         //   if (ss[i] == "") continue;
            Pair p = new Pair();
            var clip = Resources.Load<AudioClip>("Audio/" + ss[i]);

            p.first = (Events)i;
            p.second = clip;
            events.Add(p);
        }
    }
    ArrayList events = new ArrayList();
}

