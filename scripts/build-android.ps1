param([string]\='Release',[string]\='net9.0-android')

& dotnet restore ..\Contoso.Mobile.sln
& dotnet workload restore
& dotnet build ..\Contoso.Mobile.csproj -c \ -f \
& dotnet publish ..\Contoso.Mobile.csproj -c \ -f \ -p:AndroidPackageFormat=aab -o ..\artifacts\android
