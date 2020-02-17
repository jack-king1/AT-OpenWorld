using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.IO;
using System;

public class ThreadQueuer : MonoBehaviour
{
    //Start thread in this script with complete file path sent to thread so a call to Application.datapath isnt needed.
    static List<Action> functionsToRunInMainThread;

    void Start()
    {
        functionsToRunInMainThread = new List<Action>();
    }

    void Update()
    {
        //assign the mesh and chunk data in the main thread which is called from the function inside the queue
        if(functionsToRunInMainThread.Count != 0)
        {
            while (functionsToRunInMainThread.Count > 0)
            {
                Debug.Log("Running Functions created in thread on main update loop");
                Action func = functionsToRunInMainThread[0];
                functionsToRunInMainThread.RemoveAt(0);
                func();
            }
        }
    }

    public void StartThreadedFunction(Action function)
    {
        Thread t = new Thread(new ThreadStart(function));
        t.Start();
    }

    public void QueueMainThreadFunction(Action function)
    {
        functionsToRunInMainThread.Add(function);
    }
}
