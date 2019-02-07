public static class EnemyDatabaseVertification
{
	public static bool VertifyViews(StaticDatabase<EnemyData> enemyDatabaseToVertify, out string message)
	{
		foreach(var pair in enemyDatabaseToVertify.GetAllDataCopy())
		{
			CharacterView v = EnemyViewFactory.GetCharacterViewPrefabForEnemyId(pair.Key);

			if(v == null)
			{
				message = string.Format("No Prefab in folder {0} for enemyID {1}", EnemyViewFactory.ENEMY_VIEWS_PREFABS_FOLDER, pair.Key);
				return false;
			}
		}

		message = "Success";
		return true;
	}
}
