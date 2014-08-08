#/usr/bin/env/sh
set +e # even a command fails bellow, continue the execution
hdid "MonoFramework-MDK-${MONO_VER}.macos10.xamarin.x86.dmg"
sudo installer -pkg "/Volumes/Mono Framework MDK ${MONO_VER}/MonoFramework-MDK-${MONO_VER}.macos10.xamarin.x86.pkg" -target /
sudo installer -pkg "MonoFramework-MDK-${MONO_VER}.macos10.xamarin.x86.pkg" -target /
set -e # set back to defaults
