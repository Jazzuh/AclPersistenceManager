Remove-Item -Recurse dist

mkdir dist\aclmanager

Push-Location server
dotnet publish -c Release
Pop-Location

mkdir dist\aclmanager\migrations\
Copy-Item -Recurse -Force -Filter *.cs migrations\* dist\aclmanager\migrations\

Copy-Item -Recurse -Force config.cfg dist\aclmanager
Copy-Item -Recurse -Force exec.cfg dist\aclmanager
Copy-Item -Recurse -Force fxmanifest.lua dist\aclmanager
Copy-Item -Recurse -Force helper.lua dist\aclmanager

mkdir dist\aclmanager\server\bin\Release\netstandard2.0\publish\
Copy-Item -Recurse -Force server\bin\Release\netstandard2.0\publish dist\aclmanager\server\bin\Release\netstandard2.0\

Compress-Archive -Path dist\* -CompressionLevel Optimal -DestinationPath dist\aclmanager