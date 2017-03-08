/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;

public class MapDesignedSpineController : MonoBehaviour {



    [SerializeField]
    private float pos_x_start;


    [SerializeField]
    private float pos_x_end;

   
    Entity entity = null;

    float time = 0.0f;
    int fps = 0;
	// Use this for initialization
	void Start () {
	spine=this.GetComponent<SkeletonAnimation>();
    Application.targetFrameRate = 40;

  /*  entity = HeroMgr.Create<BattleHero>();
        */
    spine.transform.position = new Vector3( pos_x_start, spine.transform.position.y, 0.0f);


	}
	
	// Update is called once per frame
	void Update()
    {
        spine.UpdateMS(Time.deltaTime);
        const float speed = 0.05f;
        if (spine.transform.position.x + speed < pos_x_end)
        {
            fps++;
            time += Time.deltaTime;
            Debug.Log("帧数:" + fps + "  当前时间:" + time + "秒");
            spine.transform.position = new Vector3(spine.transform.position.x + speed, spine.transform.position.y, 0.0f);
        }

    }

    SkeletonAnimation spine;
}
