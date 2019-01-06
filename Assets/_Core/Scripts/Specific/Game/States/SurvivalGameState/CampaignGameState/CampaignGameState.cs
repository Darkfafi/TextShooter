namespace SurvivalGame
{
	public class CampaignGameState : SubGameState<SurvivalGameState, GameModel>
	{
		public Timeline<GameModel> Timeline
		{
			get; private set;
		}

		protected override void OnSetupState()
		{
			Timeline = new Timeline<GameModel>(MasterGame);
			SetupTimeline();
		}

		private void SetupTimeline()
		{
			Timeline.EnqueueTimelineSlot(PotentialEvent.Create<GameModel, MobsTimelineEvent, MobTimelineEventData>(
			new MobTimelineEventData()
			{
				UseKillsProgressor = false,
				TimeForMobsInSeconds = 3,
				MobSpawnInstructions = new SpawnData[] 
				{
					new SpawnData()
					{
						EnemyType = "A",
						Amount = 5,
					},
				}
			}));

			Timeline.EnqueueTimelineSlot(PotentialEvent.Create<GameModel, MobsTimelineEvent, MobTimelineEventData>(
			new MobTimelineEventData()
			{
				UseKillsProgressor = true,
				MobSpawnInstructions = new SpawnData[]
				{
					new SpawnData()
					{
						EnemyType = "Cool",
						Amount = 3,
					},
				}
			}));
		}

		protected override void OnStartState()
		{
			Timeline.TimelineEndReachedEvent += OnEndReachedEvent;
			Timeline.TimelineEventEndedEvent += OnTimelineEventEndedEvent;

			// Start First Timeline Event
			Timeline.SetNewTimelinePosition(0);
		}

		protected override void OnEndState()
		{
			Timeline.TimelineEndReachedEvent -= OnEndReachedEvent;
			Timeline.TimelineEventEndedEvent -= OnTimelineEventEndedEvent;
		}

		private void OnTimelineEventEndedEvent(IReadableTimelineEvent timelineEvent)
		{
			UnityEngine.Debug.LogFormat("End of event {0} reached!", timelineEvent.GetType().ToString());
			Timeline.Up();
		}

		private void OnEndReachedEvent()
		{
			UnityEngine.Debug.Log("End of timeline reached!");
		}
	}
}
