using System;
using UnityEngine;

public class TargetingSystem
{
	public Targeting Targeting
	{
		get; private set;
	}

	public bool IsEnabled
	{
		get
		{
			if(Targeting == null)
				return false;

			return Targeting.IsEnabled;
		}
	}

	public event Action<bool> EnabledStateChangedEvent;

	private TimekeeperModel _timekeeperModel;
	private EntityFilter<EntityModel> _targetingSystemUsers;

	public TargetingSystem(CharInputModel charInputModel, TimekeeperModel timekeeperModel)
	{
		Targeting = new Targeting(charInputModel, FilterRules.CreateHasAllTagsFilter(Tags.TARGETABLE));
		Targeting.TargetingEnabledStateChangedEvent += OnTargetingEnabledStateChangedEvent;
		_timekeeperModel = timekeeperModel;
		_timekeeperModel.ListenToFrameTick(OnUpdate);

		EntityTracker.Instance.TrackedEvent += OnAnyTrackedEvent;

		FilterRules fr;
		FilterRules.OpenConstructHasAllTags(Tags.TARGETING_USER);
		FilterRules.CloseConstruct(out fr);

		_targetingSystemUsers = EntityFilter<EntityModel>.Create(fr);
		_targetingSystemUsers.TrackedEvent += OnTrackedEvent;
		_targetingSystemUsers.UntrackedEvent += OnUntrackedEvent;
	}

	public static void TrackTargetingUserEntity<T>(T targetingUser) where T : EntityModel, ITargetingUser
	{
		targetingUser.ModelTags.AddTag(Tags.TARGETING_USER);
	}

	public static void UnTrackTargetingUserEntity<T>(T targetingUser) where T : EntityModel, ITargetingUser
	{
		targetingUser.ModelTags.RemoveTag(Tags.TARGETING_USER);
	}

	public static void TrackTargetingUserEntity(ITargetingUser targetingUser)
	{
		EntityModel e = targetingUser as EntityModel;
		if(e != null)
		{
			e.ModelTags.AddTag(Tags.TARGETING_USER);
		}
	}

	public static void UnTrackTargetingUserEntity(ITargetingUser targetingUser)
	{
		EntityModel e = targetingUser as EntityModel;
		if(e != null)
		{
			e.ModelTags.RemoveTag(Tags.TARGETING_USER);
		}
	}

	public void Clean()
	{
		Targeting.SetEnabledState(false);
		Targeting.TargetingEnabledStateChangedEvent -= OnTargetingEnabledStateChangedEvent;
		Targeting = null;

		EntityTracker.Instance.TrackedEvent -= OnAnyTrackedEvent;
		_targetingSystemUsers.TrackedEvent -= OnTrackedEvent;
		_targetingSystemUsers.UntrackedEvent -= OnUntrackedEvent;

		_targetingSystemUsers.Clean();
		_targetingSystemUsers = null;

		_timekeeperModel.UnlistenFromFrameTick(OnUpdate);
		_timekeeperModel = null;
	}

	private void OnTargetingEnabledStateChangedEvent(Targeting targeting, bool enabledState)
	{
		if(EnabledStateChangedEvent != null)
			EnabledStateChangedEvent(enabledState);
	}

	public void SetEnabledState(bool enabledState)
	{
		Targeting.SetEnabledState(enabledState);
	}

	private void OnAnyTrackedEvent(EntityModel entity)
	{
		ITargetingUser ItargetingUser = entity as ITargetingUser;
		if(ItargetingUser != null && ItargetingUser.AddTargetingUserTagOnCreation)
		{
			TrackTargetingUserEntity(ItargetingUser);
		}
	}

	private void OnTrackedEvent(EntityModel entity)
	{
		ITargetingUser user = entity as ITargetingUser;
		user.SetCurrentTargeting(Targeting);
	}

	private void OnUntrackedEvent(EntityModel entity)
	{
		ITargetingUser user = entity as ITargetingUser;
		user.SetCurrentTargeting(null);
	}

	private void OnUpdate(float deltaTime, float timeScale)
	{
		EntityModel[] entities = _targetingSystemUsers.GetAll();
		Vector3 min =  Vector3.zero, max = Vector3.zero;
		for(int i = 0, c = entities.Length; i < c; i++)
		{
			float x = entities[i].ModelTransform.Position.x;
			float y = entities[i].ModelTransform.Position.y;
			float z = entities[i].ModelTransform.Position.z;

			if(x < min.x)
				min.x = x;

			if(x > max.x)
				max.x = x;

			if(y < min.y)
				min.y = y;

			if(y > max.y)
				max.y = y;

			if(z < min.z)
				min.z = z;

			if(z > max.z)
				max.z = z;
		}

		Targeting.SetOriginPosition(Vector3.Lerp(min, max, 0.5f));
	}
}
