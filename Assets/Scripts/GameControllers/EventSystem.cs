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
public class EventSystem : MonoBehaviour
{

    // using singleton
    private static EventSystem instance;
    public static EventSystem Instance { get { return instance; } }

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
