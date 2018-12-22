public class InstantFireWordGun : BaseFireWordGun
{
	public InstantFireWordGun(float cooldown, float range, TimekeeperModel timekeeper) : base(cooldown, range, timekeeper) { }

	protected override bool FireLogics(WordsHp wordsHp, string wordToFireAsChars)
	{
		for(int i = 0, c = wordToFireAsChars.Length; i < c; i++)
		{
			wordsHp.Hit(wordToFireAsChars[i]);
		}

		return true;
	}
}
