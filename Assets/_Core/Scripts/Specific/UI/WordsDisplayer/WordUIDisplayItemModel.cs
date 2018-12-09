using System;

public class WordUIDisplayItemModel : EntityModel
{
	public event Action<string> NewWordDisplayingEvent;

	public string CurrentlyDisplayingWord
	{
		get; private set;
	}

	public EntityModel EntityModelLinkedTo;

	private WordsHolder _wordsHolder;
	private TimekeeperModel _timekeeper;

	public WordUIDisplayItemModel(EntityModel entityModel, TimekeeperModel timekeeper)
	{
		EntityModelLinkedTo = entityModel;
		_timekeeper = timekeeper;

		_wordsHolder = entityModel.GetComponent<WordsHolder>();
		SetCurrentlyDisplayingWord(_wordsHolder.CurrentWord);

		_wordsHolder.WordCycledEvent += OnWordCycledEvent;
		_timekeeper.ListenToFrameTick(OnUpdate);
	}

	protected override void OnModelDestroy()
	{
		base.OnModelDestroy();
		_wordsHolder.WordCycledEvent -= OnWordCycledEvent;
		_timekeeper.UnlistenFromFrameTick(OnUpdate);
		_timekeeper = null;
		_wordsHolder = null;
		EntityModelLinkedTo = null;
	}

	private void OnWordCycledEvent(string previousWord, string newWord)
	{
		SetCurrentlyDisplayingWord(newWord);
	}

	private void SetCurrentlyDisplayingWord(string word)
	{
		CurrentlyDisplayingWord = word;

		if(NewWordDisplayingEvent != null)
		{
			NewWordDisplayingEvent(word);
		}
	}

	private void OnUpdate(float deltaTime, float timeScale)
	{
		if(EntityModelLinkedTo != null && !EntityModelLinkedTo.IsDestroyed)
			ModelTransform.SetPos(EntityModelLinkedTo.ModelTransform.Position);
	}
}
