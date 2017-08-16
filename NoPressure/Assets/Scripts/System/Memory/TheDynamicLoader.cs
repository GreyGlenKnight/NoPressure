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

public class TheDynamicLoader
{
    // Singleton refrence
    static TheDynamicLoader instance;
    public bool Rest = false;

    // Three queues all act as one, effectivly a priority queue with three settings

    // Low priority Actions should be reserved for upkeep calls
    Queue<Action> mLowPriorityActions;

    // Medium priority Actions are anything that is not an upkeep call or could affect
    // the game experence in a meaningful way.
    Queue<Action> mMediumPriorityActions;

    // High priority Actions are actions that could affect the game experence if
    // they are delayed too long. 
    Queue<Action> mHighPriorityActions;

    // TODO, add functionality to test speed of each action ran
    // if it would be above target time give warning
    System.Diagnostics.Stopwatch mTimer;
    float TargetTime;

    private TheDynamicLoader()
    {
        ClearQueue();
    }
    public void ClearQueue()
    {
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
            instance = new TheDynamicLoader();
        }
        return instance;
    }

    // Perform 1 action a frame
    public void DoNextAction()
    {
        // Do nothing for one frame if Rest == true
        if (Rest == true)
        {
            Rest = false;
            return;
        }

        // Perform 1 action per frame, performing the higher priority ones first
        Action nextAction = GetNextAction();
        if(nextAction != null)
            nextAction(); 

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

