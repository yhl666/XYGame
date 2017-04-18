using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
    class SkillImagePath:MonoBehaviour
    {
        public Sprite skill1;
        public Sprite skill2;
        public Sprite skill3;
        public static SkillImagePath ins;

        void Awake()
        {
            ins = this;
        }
    }

