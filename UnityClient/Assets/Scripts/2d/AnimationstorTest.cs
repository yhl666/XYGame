/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;

public class AnimationstorTest : MonoBehaviour
{


    [SerializeField]
    public string file;

    [SerializeField]
    public int FrameDelay;

    public Animations ani = null;
    void Awake()
    {
      //  Utils.SetTargetFPS(40);
        this.Init();
    }
    float time = 0f;
    public void Update()
    {


        if (ani == null)
        {
            this.Init();
        }
        ///   if (ani != null)
        time += Time.deltaTime;
        if (time > 0.025f)
        {
            ani.UpdateMS();
            time = 0f;
            if(  ani.IsDone())
            {
                GameObject.Destroy(this.gameObject);
            }
        }
    }

    void OnDestroy()
    {
        if (ani != null)
            ani.Dispose();
    }
    public bool Init()
    {
        string name = Utils.GetFileName(file);

        ani = AnimationsCache.ins.GetAnimations(name);
        if (ani == null)
        {
            ani = AnimationsCache.ins.AddAnimatons(name, Animations.CreateWithFile(file));
            ani.Init();
            ani.Run();
        }
        else
        {
            ani.Init();
            ani.Run();
        }
        ani.SetLoop(0xffffff);
        ani.target = this.GetComponent<SpriteRenderer>();
        ani.perFrame = FrameDelay;

        return true;
    }
    public static Animationstor Create(string file)
    {

        return null;

    }
}
