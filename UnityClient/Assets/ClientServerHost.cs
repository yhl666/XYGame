using UnityEngine;
using System.Collections;

public class ClientServerHost : MonoBehaviour
{
    static ClientServerApp app = null;
    // Use this for initialization
    void Awake()
    {
      ///  GameObject.DontDestroyOnLoad(this);
        if (app == null)
        {
            app = ClientServerApp.ins;
        }
    }

    // Update is called once per frame
    void Update()
    {
        app.UpdateMS();
    }
    void OnDestroy()
    {
       // app.Dispose();
    }
}
