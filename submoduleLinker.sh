dos2unix submoduleLinker.config
while read -r line || [[ -n "$line" ]]; do
if [ -n "$projectSubmodulesPath" ]; 
	then
		linkingFolder="$line";
		echo "Symlink $linkingFolder into $projectSubmodulesPath"
		ln -s "$(cd "$(dirname $linkingFolder)"; pwd)/$(basename $linkingFolder)" "$projectSubmodulesPath";
		echo "";
	else
		projectSubmodulesPath="$line";
		echo "";
		echo "Setting up $projectSubmodulesPath as target submodules root folder.";
		rm -rf "$projectSubmodulesPath";
		mkdir "$projectSubmodulesPath" && touch "$projectSubmodulesPath/.gitignore" && echo "*" >> "$projectSubmodulesPath/.gitignore";
		echo "";
		sleep 1s;
fi
done <submoduleLinker.config
read -p "Press enter to continue"