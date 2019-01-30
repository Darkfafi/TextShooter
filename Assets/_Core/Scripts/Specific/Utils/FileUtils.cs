public static class FileUtils
{
	public static string PathToFile(string fileName, params string[] pathParts)
	{
		string s = "";
		for(int i = 0; i < pathParts.Length; i++)
		{
			s += pathParts[i] + "/";
		}
		return s + fileName;
	}
}
