using UnityEngine;

public interface ITargetingUser
{
	bool AddTargetingUserTagOnCreation
	{
		get;
	}

	Vector3 TargetingUserPosition
	{
		get;
	}

	void SetCurrentTargeting(Targeting targeting);
}
