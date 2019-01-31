using UnityEngine;

public class CharacterView : EntityView
{
	protected override void Update()
	{
		base.Update();

		if(Input.GetMouseButton(0))
		{
			Vector2 pressLocation = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			if(MVCUtil.GetModel<CharacterModel>(this).HasComponent<TopDownMovement>())
				MVCUtil.GetModel<CharacterModel>(this).GetComponent<TopDownMovement>().MoveTo(pressLocation);
		}

		if(Input.GetKeyDown(KeyCode.LeftControl))
		{
			if(MVCUtil.GetModel<CharacterModel>(this).HasComponent<TopDownMovement>())
				MVCUtil.GetModel<CharacterModel>(this).GetComponent<TopDownMovement>().SetEnabledState(!MVCUtil.GetModel<CharacterModel>(this).GetComponent<TopDownMovement>().IsEnabled);
		}

		if(Input.GetKeyDown(KeyCode.LeftAlt))
		{
			MVCUtil.GetModel<CharacterModel>(this).RemoveComponent<TopDownMovement>();
		}
	}
}
