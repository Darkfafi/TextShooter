using UnityEngine;

public class EnemyViewFactory : BaseMonoViewFactory<CharacterModel, CharacterView>
{
	public const string ENEMY_VIEWS_PREFABS_FOLDER = "Prefabs/EnemyViews";

	protected override CharacterView GetViewPrefabForModel(CharacterModel model)
	{
		EnemyPassport enemyPassport = model.GetComponent<EnemyPassport>();
		CharacterView view = Resources.Load<CharacterView>(FileUtils.PathToFile(enemyPassport.EnemyData.DataID, ENEMY_VIEWS_PREFABS_FOLDER));

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
