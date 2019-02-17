using System;

namespace Vertification
{
	public static class EnemyDatabaseVertification
	{
		public static void Run()
		{
			Vertifier.GenericVerification("Enemies",
				new Vertifier.TestStepData("EnemyViews", () =>
				{
					StaticDatabase<EnemyData> database;

					try
					{
						database = EnemyDatabaseParser.ParseXml(DatabaseContents.GetEnemyDatabaseText());
					}
					catch(Exception e)
					{
						return new Vertifier.TestResult(false, e.Message);
					}

					string message;
					bool success = VertifyViews(database, out message);

					if(success)
						message = null;

					return new Vertifier.TestResult(success, message);

				})
			);
		}

		private static bool VertifyViews(StaticDatabase<EnemyData> enemyDatabaseToVertify, out string message)
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
}