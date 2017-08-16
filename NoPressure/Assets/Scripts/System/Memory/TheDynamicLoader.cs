using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum Priority
{
    Low,
    Medium,
    High
};

public class TheDynamicLoader : MonoBehaviour
{
    // Singleton refrence
    static TheDynamicLoader instance;
    public bool Rest = false;

    // Three queues all act as one, effectivly a priority queue with three settings
    Queue<Action> mLowPriorityActions;
    Queue<Action> mMediumPriorityActions;
    Queue<Action> mHighPriorityActions;

    // TODO, add functionality to test speed of each action ran
    // if it would be above target time give warning
    System.Diagnostics.Stopwatch mTimer;
    float TargetTime;

    private void Awake()
    {
        Debug.Log("Dynamic Loader Awake 2");
        instance = this;

        mLowPriorityActions = new Queue<Action>();
        mMediumPriorityActions = new Queue<Action>();
        mHighPriorityActions = new Queue<Action>();
    }

    // Singleton getter
    public static TheDynamicLoader getDynamicLoader()
    {
        if (instance == null)
        {
            Debug.Log("Trying to access Dynamic loader before it loaded");
            return null;
        }
        return instance;
    }

    // Update is called once per frame
    void Update()
    {
        // Do nothing for one frame if Rest == true
        if (Rest == true)
        {
            Rest = false;
            return;
        }

        // Perform 1 action per frame, performing the higher priority ones first
        Action nextAction = GetNextAction();
        if (nextAction != null)
        {
            nextAction();
        }
    }

    // Returns the next action in the priority queue, 
    // it will also remove that item from the queue.
    private Action GetNextAction()
    {
        // Perform 1 action per frame, performing the higher priority ones first
        if (mHighPriorityActions.Count != 0)
        {
            return mHighPriorityActions.Dequeue();
        }

        if (mMediumPriorityActions.Count != 0)
        {
            return mMediumPriorityActions.Dequeue();
        }

        if (mLowPriorityActions.Count != 0)
        {
            return mLowPriorityActions.Dequeue();
        }
        return null;
    }

    public void AddActionToQueue(Action lAction, Priority lPriority)
    {
        switch (lPriority)
        {
            case Priority.Low:
                mLowPriorityActions.Enqueue(lAction);
                break;

            case Priority.Medium:
                mMediumPriorityActions.Enqueue(lAction);
                break;

            case Priority.High:
                mHighPriorityActions.Enqueue(lAction);
                break;
        }
    }

}

