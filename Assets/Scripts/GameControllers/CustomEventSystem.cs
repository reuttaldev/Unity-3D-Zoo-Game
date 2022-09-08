using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EventType
{
    EnemyCollision,
    Inventory,
    Objective
}
public class CustomEventSystem : MonoBehaviour
{

    // using singleton
    private static CustomEventSystem instance;
    public static CustomEventSystem Instance { get { return instance; } }

    internal event Action<GameObject> onEnemyCollision;
    private void Awake()
    {

        // using singleton to instantiate
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }
    // create methods for scripts to subscribe to
    // methods for scriptd to call when event occurs
    public void OnEvent(EventType type, GameObject caller)
    {
        Debug.Log("onevent");
        switch (type)
        {
            case EventType.EnemyCollision:
                if (onEnemyCollision != null)
                {
                    onEnemyCollision(caller);
                }
                break;
                // can be expanded
        }
    }
}
