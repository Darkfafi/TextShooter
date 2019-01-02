using System;

namespace SurvivalGame
{
	public class CampaignGameState : SubGameState<SurvivalGameState, GameModel>
	{
		public Timeline Timeline
		{
			get; private set;
		}

		protected override void OnSetupState()
		{
			Timeline = new Timeline(MasterGame.TimekeeperModel);
			SetupTimeline();
		}

		private void SetupTimeline()
		{
			Timeline.EnqueueTimelineEvent<MobsTimelineEvent, MobTimelineEventData>(new MobTimelineEventData()
			{
				UseKillsProgressor = true,
				TimeForMobsInSeconds = 5,
				MobSpawnInstructions = new SpawnData[] 
				{
					new SpawnData()
					{
						EnemyType = "Boss",
						Amount = 1,
					},
				}
			});
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

		private void OnTimelineEventEndedEvent(IReadableTimelineEvent timelineEvent, bool success)
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
