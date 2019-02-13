using UnityEngine;
using System.Collections.Generic;

public static class ResourceLocator
{
	public static T Locate<T>(string name, string resourceFolderLocation, params string[] additionalLocationsToSearch) where T : Object
	{
		name = name.ToLower();
		int countLeftAfterContainsFound = -1;
		T notExactMatchLocatedResource = null;

		List<string> locations = new List<string>(additionalLocationsToSearch);
		locations.Add(resourceFolderLocation);

		for(int i = 0; i < locations.Count; i++)
		{
			T[] resources = Resources.LoadAll<T>(locations[i]);
			foreach(T resource in resources)
			{
				string fileName = resource.name.ToLower();

				if(fileName == name)
					return resource;

				if(fileName.Contains(name))
				{
					string[] nameRulesSplit = fileName.Split(new string[] { name }, System.StringSplitOptions.RemoveEmptyEntries);
					int count = -1;
					if(nameRulesSplit.Length > 0)
					{
						count = 0;
						for(int j = 0; j < nameRulesSplit.Length; j++)
						{
							count += nameRulesSplit[j].Length;
						}
					}

					if(notExactMatchLocatedResource == null || count < countLeftAfterContainsFound)
					{
						countLeftAfterContainsFound = count;
						notExactMatchLocatedResource = resource;
					}
				}
			}
		}

		return notExactMatchLocatedResource;
	}
}