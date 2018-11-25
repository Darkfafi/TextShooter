using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyView : EntityView
{
    protected override void Update()
    {
        base.Update();

        if(Input.GetMouseButton(0))
        {
            Vector2 pressLocation = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            MVCUtil.GetModel<EnemyModel>(this).GetComponent<TopDownMovement>().MoveTo(pressLocation);
        }
		if(Input.GetMouseButtonDown(1))
		{
			MVCUtil.GetModel<EnemyModel>(this).WordsHolder.CycleToNextWord();
		}
    }
}
