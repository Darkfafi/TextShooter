public static class TimelineSpecificGlobals
{
	// -- XML -- \\
	// <<-- MobsTimelineEvent -->> \\

	// Event/MobsTimelineEvent/Data
	public const string NODE_MOBS_EVENT_DATA_SPAWN = "spawn";

	// Event/MobsTimelineEvent/Data/Spawn
	public const string NODE_MOBS_EVENT_DATA_SPAWN_ENEMY_ID = "enemyID";
	public const string NODE_MOBS_EVENT_DATA_SPAWN_ENEMY_AMOUNT = "amount";
	public const string NODE_MOBS_EVENT_DATA_SPAWN_DELAY_AFTER_SPAWN = "delayAfterSpawn";
	public const string NODE_MOBS_EVENT_DATA_SPAWN_TIME_BETWEEN = "timeBetween";
	public const string NODE_MOBS_EVENT_DATA_SPAWN_SIDE = "side";
	public const string CONST_MOBS_EVENT_DATA_SPAWN_SIDE_ANY = CameraUtils.SIDE_ANY;
	public const string CONST_MOBS_EVENT_DATA_SPAWN_SIDE_TOP = CameraUtils.SIDE_TOP;
	public const string CONST_MOBS_EVENT_DATA_SPAWN_SIDE_RIGHT = CameraUtils.SIDE_RIGHT;
	public const string CONST_MOBS_EVENT_DATA_SPAWN_SIDE_BOTTOM = CameraUtils.SIDE_BOTTOM;
	public const string CONST_MOBS_EVENT_DATA_SPAWN_SIDE_LEFT = CameraUtils.SIDE_LEFT;

	public const string CONST_MOBS_EVENT_DATA_ENDING_TYPE_DESTROY = "destroy";


	// Progressor Names 
	public const string PROGRESSOR_NAME_KILLS = "kills";
	public const string PROGRESSOR_NAME_TIME = "time";
}
