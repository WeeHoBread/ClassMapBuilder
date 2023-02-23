using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    private static List<Timer> activeTimerList;

    private static GameObject timerGameObject;

    private static void InitIfNeeded()
    {
        if (timerGameObject == null)
        {
            timerGameObject = new GameObject("TimerFunction_InitGameObject");
            activeTimerList = new List<Timer>();
        }
    }

    public static Timer Create(Action action, float time, string timerName = null)
    {
        InitIfNeeded();

        GameObject gameObject = new GameObject("FunctionTimer", typeof(MonoBehaviourHook));

        Timer timerFunction = new Timer(action, time, timerName ,gameObject);

        gameObject.GetComponent<MonoBehaviourHook>().onUpdate = timerFunction.Update;

        activeTimerList.Add(timerFunction); 

        return timerFunction;
    }

    private static void RemoveTimerFromList(Timer timer)
    {
        InitIfNeeded();
        activeTimerList.Remove(timer);
    }

    public static void StopTimer(string timerName)
    {
        for(int i = 0; i <activeTimerList.Count; i++)
        {
            if (activeTimerList[i].timerName == timerName)
            {
                activeTimerList[i].DestroySelf();
                i--; //timerlist count will decreased when destroyed, this will make sure nothing in the list is skipped
            }
        }
    }

    public static void ResetTimer(Action action, float newTime, string timerName)
    {
        StopTimer(timerName);
        Create(action, newTime, timerName);
    }

    //Class to get access to MonoBehaiviour functions
    public class MonoBehaviourHook : MonoBehaviour
    {
        public Action onUpdate;

        private void Update()
        {
            if (onUpdate != null)
            {
                onUpdate();
            }
        }
    }

    #region Timer class and variables
    private Action action;
    private float timer;
    private string timerName;
    private GameObject gameObject;
    private bool destroyed;

    private Timer(Action action, float timer, string timerName, GameObject gameObject)
    {
        this.action = action;
        this.timer = timer;
        this.timerName = timerName;
        this.gameObject = gameObject;
        destroyed = false;
    }
    #endregion

    // Update is called once per frame
    public void Update()
    {
        if (!destroyed)
        {
            timer -= Time.deltaTime;

            if (timer < 0)
            {
                //Trigger action
                action();
                DestroySelf();
            }
        }
    }

    private void DestroySelf()
    {
        destroyed = true;
        RemoveTimerFromList(this);
        UnityEngine.Object.Destroy(gameObject);
    }

}
