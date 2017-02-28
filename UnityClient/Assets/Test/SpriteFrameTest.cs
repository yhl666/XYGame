using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SpriteFrameTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Image img = this.GetComponent<Image>();


        SpriteFrame frame = SpriteFrame.CreateWithPng("hd/interface/items/2307.png");
        img.sprite = frame.sprite;

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
