using UnityEngine;
using System.Collections;

public class AutoFlitScreen : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        trans = this.GetComponent<RectTransform>();

    }

    // Update is called once per frame
    void Update()
    {
        trans.sizeDelta = new Vector2(Screen.width, Screen.height);
        trans.localPosition = new Vector3(-Screen.width/2.0F,- Screen.height/2.0f, 0);
    }


    RectTransform trans = null;
}
