public class Campaign<T> where T : class, IGame
{
	public CampaignData CampaignData
	{
		get; private set;
	}

	public Timeline<T> Timeline
	{
		get; private set;
	}

	public bool CampaignRunning
	{
		get; private set;
	}

	public Campaign(CampaignData campaignData, Timeline<T> timeline)
	{
		CampaignData = CampaignData;
		Timeline = timeline;
	}

	public void StartCampaign()
	{
		if(CampaignRunning)
			return;

		CampaignRunning = true;

		Timeline.TimelineEndReachedEvent += OnEndReachedEvent;
		Timeline.TimelineEventEndedEvent += OnTimelineEventEndedEvent;
		Timeline.SetNewTimelinePosition(0);
	}

	public void EndCampaign()
	{
		if(!CampaignRunning)
			return;

		CampaignRunning = false;
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

public struct CampaignData
{
	public string Name;
	public string Description;
}
