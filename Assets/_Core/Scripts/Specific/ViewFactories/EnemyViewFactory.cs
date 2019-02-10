using UnityEngine;

public class EnemyViewFactory : BaseMonoViewFactory<CharacterModel, CharacterView>
{
	public const string ENEMY_VIEWS_PREFABS_FOLDER = "Prefabs/EnemyViews";

	public static CharacterView GetCharacterViewPrefabForEnemyId(string enemyID)
	{
		return Resources.Load<CharacterView>(FileUtils.PathToFile(enemyID, ENEMY_VIEWS_PREFABS_FOLDER));
	}

	protected override CharacterView GetViewPrefabForModel(CharacterModel model)
	{
		EnemyPassport enemyPassport = model.GetComponent<EnemyPassport>();
		CharacterView view = GetCharacterViewPrefabForEnemyId(enemyPassport.EnemyData.DataID);

		if(view == null)
			throw new System.Exception("Could not find view for enemy with id `" + enemyPassport.EnemyData.DataID + "`");

		return view;
	}

	protected override FilterRules CreateFilterRulesForFactory()
	{
		FilterRules r;
		FilterRules.OpenConstructNoTags();
		FilterRules.AddComponentToConstruct<EnemyPassport>(true);
		FilterRules.CloseConstruct(out r);
		return r;
	}
}
