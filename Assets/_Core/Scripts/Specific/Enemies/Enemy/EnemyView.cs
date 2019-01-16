using UnityEngine;

public class EnemyView : EntityView
{
	protected override void Update()
	{
		base.Update();

		if(Input.GetMouseButton(0))
		{
			Vector2 pressLocation = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			if(MVCUtil.GetModel<EnemyModel>(this).HasComponent<TopDownMovement>())
				MVCUtil.GetModel<EnemyModel>(this).GetComponent<TopDownMovement>().MoveTo(pressLocation);
		}
		if(Input.GetMouseButtonDown(1))
		{
			MVCUtil.GetModel<EnemyModel>(this).WordsHolder.CycleToNextWord();
		}

		if(Input.GetKeyDown(KeyCode.LeftControl))
		{
			if(MVCUtil.GetModel<EnemyModel>(this).HasComponent<TopDownMovement>())
				MVCUtil.GetModel<EnemyModel>(this).GetComponent<TopDownMovement>().SetEnabledState(!MVCUtil.GetModel<EnemyModel>(this).GetComponent<TopDownMovement>().IsEnabled);
		}

		if(Input.GetKeyDown(KeyCode.LeftAlt))
		{
			MVCUtil.GetModel<EnemyModel>(this).RemoveComponent<TopDownMovement>();
		}
	}
}
