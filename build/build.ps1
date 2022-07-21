$root = Resolve-Path (Join-Path $PSScriptRoot "..")
dotnet pack "$root/KingForms.sln" --configuration Release --output "$root/artifacts" -p:ContinuousIntegrationBuild=true
