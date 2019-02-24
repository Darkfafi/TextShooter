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
		echo "Preparing $projectSubmodulesPath as target submodules root folder.";
		rm -rf "$projectSubmodulesPath";
		mkdir "$projectSubmodulesPath";
		echo "Successfully setup $projectSubmodulesPath";
		echo "";
		sleep 1s;
fi
done <submoduleLinker.config
sleep 2s