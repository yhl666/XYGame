using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ClientServerText : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        Application.targetFrameRate = 0xfffffff;

        this.GetComponent<Text>().text = "队列剩余请求:" + ClientServerApp.ins.GetCurrentQueueCount();

    }

    // Update is called once per frame
    void Update()
    {

    }
}
