#!/bin/bash

function clean_up {
	kill $child_pid
	exit
}

trap clean_up SIGHUP SIGINT SIGTERM

if [[ ! -f /app/initdone ]]
then
	echo "This is the first start. Running setup..."
	touch /app/initdone
	groupadd --non-unique --gid $PGID nuget
	useradd --non-unique --gid $PGID --uid $PUID --shell /bin/bash --create-home nuget
fi

echo "Launching NuGetServer..."
su --login nuget --command "umask $UMASK; dotnet /app/Server.dll UpstreamHost=https://api.nuget.org CacheDirectory=/mnt/cache/ Urls=http://+:80 PublicUrl=$PublicUrl"
child_pid=$!
wait $child_pid
