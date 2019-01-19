public class KillsProgressor : BaseTimelineEventProgressor
{
	public override string ProgressorName
	{
		get
		{
			return TimelineSpecificGlobals.PROGRESSOR_NAME_KILLS;
		}
	}

	private EntityFilter<EntityModel> _spawnedKillablesTrackerFilter;
	private string _eventEntityTag;
	private int _entitiesSpawned;

	public KillsProgressor(string eventEntityTag, int goalKills) : base(goalKills)
	{
		_eventEntityTag = eventEntityTag;
	}

	public override void StartProgressor(string optionalValueString)
	{
		_entitiesSpawned = 0;
		FilterRules f;
		FilterRules.OpenConstructHasAllTags(_eventEntityTag);
		FilterRules.AddComponentToConstruct<Lives>(true);
		FilterRules.CloseConstruct(out f);

		_spawnedKillablesTrackerFilter = EntityFilter<EntityModel>.Create(f);
		_spawnedKillablesTrackerFilter.TrackedEvent += OnTrackedEvent;
		_spawnedKillablesTrackerFilter.UntrackedEvent += OnUntrackedEvent;
	}

	public override void EndProgressor()
	{
		EntityModel[] spawnedKillables = _spawnedKillablesTrackerFilter.GetAll(e => e.GetComponent<Lives>().IsAlive);

		for(int i = 0; i < spawnedKillables.Length; i++)
		{
			spawnedKillables[i].GetComponent<Lives>().DeathEvent -= OnDeathEvent;
		}

		_spawnedKillablesTrackerFilter.TrackedEvent -= OnTrackedEvent;
		_spawnedKillablesTrackerFilter.UntrackedEvent -= OnUntrackedEvent;
		_spawnedKillablesTrackerFilter.Clean();
		_spawnedKillablesTrackerFilter = null;
	}

	private void OnTrackedEvent(EntityModel entity)
	{
		entity.GetComponent<Lives>().DeathEvent += OnDeathEvent;
		_entitiesSpawned++;
	}

	private void OnUntrackedEvent(EntityModel entity)
	{
		Lives lives = entity.GetComponent<Lives>();
		lives.DeathEvent -= OnDeathEvent;
		if(lives.IsAlive)
		{
			OnDeathEvent(lives);
		}
	}

	private void OnDeathEvent(Lives livesComponent)
	{
		if(_spawnedKillablesTrackerFilter != null)
		{
			livesComponent.DeathEvent -= OnDeathEvent;
			int entityAmountToGo = (GoalValue - _entitiesSpawned);
			UpdateValue(GoalValue - (_spawnedKillablesTrackerFilter.GetAll((e) => e.GetComponent<Lives>().IsAlive).Length + entityAmountToGo));
		}
	}
}
