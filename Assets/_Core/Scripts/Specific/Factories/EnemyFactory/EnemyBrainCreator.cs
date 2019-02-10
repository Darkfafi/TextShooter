using UnityEngine;

public static class EnemyBrainCreator
{
	public static void ApplyBrain(CharacterModel enemyModel, EnemyData enemyData, TimekeeperModel timekeeperModel)
	{
		ModelBrain<EntityModel> brain = enemyModel.AddComponent<EntityBrain>().Setup(timekeeperModel);

		brain.SetupNoStateSwitcher(
			new BrainSwitcher<EntityModel, TargetsSwitcherDataObject>(
			(affected, dataObject) =>
			{
				EntityModel target = dataObject.TargetsFilter.GetFirst<EntityModel>(e => e.HasComponent<Lives>() && e.GetComponent<Lives>().IsAlive, affected.SortOnClosestTo());

				if(target == null)
					return null;

				WeaponHolder weaponHolder = affected.GetComponent<WeaponHolder>();
				float targetDistance = weaponHolder == null || !weaponHolder.HasWeapon ? 1f : weaponHolder.Weapon.Radius * 0.7f;
				return dataObject.CreatePotentialSwitchToState(new MovementStateRequest(target, targetDistance), SwitcherSettings.NORMAL_BASE_FULL_PRIO);
			}, new TargetsSwitcherDataObject(FilterRules.CreateHasAllTagsFilter(Tags.ENEMY_TARGET)))
		);

		if(enemyData.WeaponData.DataID == SuicideBombWeapon.ID_WEAPON)
		{
			// When suicider and moving, rush the last 2 meters to range in order to explode quickly!
			brain.SetupStateSwitcher<LinearMovementState>(
				new BrainSwitcher<EntityModel, TargetsSwitcherDataObject>(
				(affected, dataObject) =>
				{
					EntityModel target = dataObject.TargetsFilter.GetFirst<EntityModel>(e => e.HasComponent<Lives>() && e.GetComponent<Lives>().IsAlive, affected.SortOnClosestTo());

					if(target == null || target.IsDestroyed)
						return null;

					TopDownMovement tdMovement = affected.GetComponent<TopDownMovement>();
					float rushSpeed = tdMovement.BaseSpeed * 2f;

					if(tdMovement.MovementSpeed >= rushSpeed)
						return null; // Already having a rush speed or greater than a rush speed. No need to interupt that.

					WeaponHolder weaponHolder = affected.GetComponent<WeaponHolder>();
					float range = weaponHolder == null || !weaponHolder.HasWeapon ? 1f : weaponHolder.Weapon.Radius;
					float distance = Vector2.Distance(affected.ModelTransform.Position, target.ModelTransform.Position);
					MovementStateRequest msr = new MovementStateRequest(target, range * 0.7f);
					msr.SpecifySpeed(rushSpeed);
					int prio = SwitcherSettings.NORMAL_BASE_FULL_PRIO + (int)((1f - distance / (range + 2f)) * SwitcherSettings.NORMAL_BASE_FULL_PRIO);
					return dataObject.CreatePotentialSwitchToState(msr, prio);

				}, new TargetsSwitcherDataObject(FilterRules.CreateHasAllTagsFilter(Tags.ENEMY_TARGET)))
			);
		}

		brain.SetupGlobalSwitcher(new BrainSwitcher<EntityModel, TargetsSwitcherDataObject>(
			(affected, dataObject) =>
			{
				EntityModel target = dataObject.TargetsFilter.GetFirst<EntityModel>(e => e.HasComponent<Lives>() && e.GetComponent<Lives>().IsAlive, affected.SortOnClosestTo());

				if(target == null)
					return null;

				WeaponHolder weaponHolder = enemyModel.GetComponent<WeaponHolder>();
				float distance = Vector2.Distance(affected.ModelTransform.Position, target.ModelTransform.Position);

				if(weaponHolder == null || !weaponHolder.HasWeapon || distance > weaponHolder.Weapon.Radius)
					return null;

				return dataObject.CreatePotentialSwitchToState(new UseWeaponStateRequest(target.GetComponent<Lives>()), SwitcherSettings.NORMAL_BASE_FULL_PRIO);

			}, new TargetsSwitcherDataObject(FilterRules.CreateHasAllTagsFilter(Tags.ENEMY_TARGET))));
	}
}
