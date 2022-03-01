#!/usr/bin/env bash
SCRIPT_DIR=$( cd -- "$( dirname -- "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )

echo $SCRIPT_DIR

gh repo create $1

cd $1
cp $SCRIPT_DIR/.gitignore .
cp $SCRIPT_DIR/README.md .
# "" is needed for macOS version
# double quote is needed to expand variable in 'string literals'
sed -i "" "s/<REPO_NAME>/${1}/g" README.md

git add .
git commit -am "init"
git push --set-upstream origin master
git fetch
cd ..