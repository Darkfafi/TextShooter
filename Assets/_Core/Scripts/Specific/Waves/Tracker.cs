using System;
using System.Collections.Generic;

namespace Game.WaveSystemInternal
{
	public class Tracker
	{
		public enum TrackingType
		{
			None,
			WaitForTime,
			WaitForExtinction
		}

		private TimekeeperModel _timekeeperModel;

		// Tracking
		private Action _trackerFinishCallback;
		private List<EnemyModel> _trackingEnemies = new List<EnemyModel>();

		public Tracker(TimekeeperModel timekeeper)
		{
			CurrentTrackingType = TrackingType.None;
			_timekeeperModel = timekeeper;
		}

		public void TrackExtinction(EnemyModel[] enemiesSpawned, Action callback)
		{
			CurrentTrackingType = TrackingType.WaitForExtinction;
			_trackingEnemies = new List<EnemyModel>(enemiesSpawned);
			_trackerFinishCallback = callback;

			for(int i = 0; i < enemiesSpawned.Length; i++)
			{
				enemiesSpawned[i].DestroyEvent += OnDestroyedEvent;
				enemiesSpawned[i].DeathEvent += OnDeathEvent;
			}
		}

		public void TrackEndOfTime(float time, Action callback)
		{
			CurrentTrackingType = TrackingType.WaitForTime;
			_timekeeperModel.ListenToFrameTick(Update);
			_trackerFinishCallback = callback;
			TimeToWaitInSeconds = time;
			TimeWaitedInSeconds = 0f;
		}

		public void Clean()
		{
			_trackerFinishCallback = null;
			FinishTracker();
			_timekeeperModel = null;
			_trackingEnemies = null;
		}

		public TrackingType CurrentTrackingType
		{
			get; private set;
		}

		public float TimeToWaitInSeconds
		{
			get; private set;
		}

		public float TimeWaitedInSeconds
		{
			get; private set;
		}

		private void Update(float deltaTime, float timeScale)
		{
			if(CurrentTrackingType == TrackingType.WaitForTime)
			{
				TimeWaitedInSeconds += deltaTime * timeScale;

				if(TimeWaitedInSeconds >= TimeToWaitInSeconds)
				{
					FinishTracker();
				}
			}
		}

		private void FinishTracker()
		{
			_timekeeperModel.UnlistenFromFrameTick(Update);
			CurrentTrackingType = TrackingType.None;

			for(int i = 0, c = _trackingEnemies.Count; i < c; i++)
			{
				_trackingEnemies[i].DestroyEvent -= OnDestroyedEvent;
				_trackingEnemies[i].DeathEvent -= OnDeathEvent;
			}

			_trackingEnemies.Clear();

			if(_trackerFinishCallback != null)
			{
				_trackerFinishCallback();
			}
		}

		private void OnDeathEvent(EnemyModel enemy)
		{
			enemy.DestroyEvent -= OnDestroyedEvent;
			enemy.DeathEvent -= OnDeathEvent;
			_trackingEnemies.Remove(enemy);

			if(_trackingEnemies.Count == 0 && CurrentTrackingType == TrackingType.WaitForExtinction)
			{
				FinishTracker();
			}
		}

		private void OnDestroyedEvent(BaseModel enemy)
		{
			OnDeathEvent((EnemyModel)enemy);
		}
	}
}