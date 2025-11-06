param([string]\='Release',[string]\='net9.0-ios')

& dotnet restore ..\Contoso.Mobile.sln
& dotnet workload restore
& dotnet publish ..\Contoso.Mobile.csproj -c \ -f \ -p:ArchiveOnBuild=true -p:BuildIpa=true -p:IpaPackageDir=..\artifacts\ipa
