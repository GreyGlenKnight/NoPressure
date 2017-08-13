using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Skills Allow fine-tuned modification to LivingEntity Objects, 
// can produce random variance to enemies in the future, or for the player to 
// gain skills from a skill tree or items. Still a work in progress.  
public interface ISkill{
    string mName { get; }
    int mID { get; }
    PersistentEntity mOwner { get; }
}

// Skill that applies a stat boost 
// is never checked otherwise
public class PassiveSkill: ISkill
{
    public string mName { get; protected set; }
    public int mID { get; protected set; }
    public PersistentEntity mOwner { get; protected set; }
}


// Skill that allows the user to perform an action not enabled to all LivingEntity objects
// Is checked when nearby an object that allows a special action to be taken
public class ActionSkill: ISkill
{
    public string mName { get; protected set; }
    public int mID { get; protected set; }
    public PersistentEntity mOwner { get; protected set; }

    public ActionSkill (string lName, int lID)
    {
        mName = lName;
        mID = lID;
        //mOwner = lOwner;
    }

}
