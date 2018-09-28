using System;
using System.Collections.Generic;

public class MethodPermitter
{
    private Dictionary<int, int> _permissionBlockers = new Dictionary<int, int>();
    private Dictionary<int, Action> _blockedActions = new Dictionary<int, Action>();
	
    public void BlockPermission(int permissionID, out Action permissionGrantedSignaller)
    {
        if(_permissionBlockers.ContainsKey(permissionID))
        {
            _permissionBlockers[permissionID]++;
        }
        else
        {
            _permissionBlockers.Add(permissionID, 1);
        }

        int pID = permissionID;
        permissionGrantedSignaller = () => { PermissionGrantedSignal(pID); };
    }

    public void ExecuteWhenPermitted(int permissionID, Action method)
    {
        if(!_permissionBlockers.ContainsKey(permissionID))
        {
            method(); // No Blocker, so execute method because we have permission
        }
        else
        {
            if(_blockedActions.ContainsKey(permissionID))
            {
                _blockedActions[permissionID] += method;
            }
            else
            {
                _blockedActions.Add(permissionID, method);
            }
        }
    }

    private void PermissionGrantedSignal(int permissionID)
    {
        if(_permissionBlockers.ContainsKey(permissionID))
        {
            // On permission granted, check amount of blockers left
            int blockers = --_permissionBlockers[permissionID];
            if(blockers == 0) // If no blockers left, check if there were actions waiting for the id to be permitted
            {
                _permissionBlockers.Remove(permissionID);
                if(_blockedActions.ContainsKey(permissionID)) // If there are actions, execute them and remove the permissionID from the dictionary, for it is dealt with
                {
                    _blockedActions[permissionID]();
                    _blockedActions.Remove(permissionID);
                }
            }
        }
    }
}
