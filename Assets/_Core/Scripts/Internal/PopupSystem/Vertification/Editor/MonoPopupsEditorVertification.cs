using UnityEditor;

namespace Vertification
{
	public static class MonoPopupsVertificationEditor
	{
		[MenuItem("Vertification/Popups")]
		public static void VertifyPopupsEditor()
		{
			MonoPopupsVertification.Run();
		}
	}
}