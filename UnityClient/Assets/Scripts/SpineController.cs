/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;
using Spine.Unity;

public class SpineController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	spine=this.GetComponent<SkeletonAnimation>();
	}
	
	// Update is called once per frame
	void Update()
    {
        spine.UpdateMS(Time.deltaTime);

    }

    SkeletonAnimation spine;
}
