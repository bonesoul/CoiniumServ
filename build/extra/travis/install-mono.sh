#/usr/bin/env/sh
set +e # even a command fails bellow, continue the execution
sudo installer -pkg "MonoFramework-MDK-${MONO_VER}.macos10.xamarin.x86.pkg" -target /
set -e # set back to defaults
