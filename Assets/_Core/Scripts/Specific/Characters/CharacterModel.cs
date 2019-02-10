using UnityEngine;

public class CharacterModel : EntityModel, IStateMachineAffected
{
	public WordsLive WordsLive
	{
		get; private set;
	}

	public TopDownMovement TopDownMovement
	{
		get; private set;
	}

	public CharacterModel(TimekeeperModel timekeeper, float movementSpeed, Vector3 position) : base(position)
	{
		TopDownMovement = AddComponent<TopDownMovement>();
		TopDownMovement.Setup(timekeeper, movementSpeed);
	}

	public void Initialize(string currentWord, params string[] nextWords)
	{
		if(WordsLive != null)
			return;

		WordsLive = AddComponent<WordsLive>();
		WordsLive.Setup(currentWord, nextWords);

		WordsLive.Lives.DeathEvent += OnDeathEvent;

		ModelTags.AddTag(Tags.DISPLAY_WORD);
	}

	protected override void OnModelDestroy()
	{
		base.OnModelDestroy();

		WordsLive.Lives.DeathEvent -= OnDeathEvent;
		WordsLive = null;
	}

	private void OnDeathEvent(Lives livesComponent)
	{
		Destroy();
	}
}
