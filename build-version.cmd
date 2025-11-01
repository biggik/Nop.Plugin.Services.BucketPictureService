@echo off
set sln_file=Nop.Plugin.Misc.BucketPictureService

cd %1

cd "%sln_file%"
dotnet restore

echo Starting %1 Debug build...
dotnet build "%sln_file%.csproj" --no-restore

echo Starting %1 Release build...
dotnet build "%sln_file%.csproj" --configuration=Release --no-restore

cd ..
cd ..
