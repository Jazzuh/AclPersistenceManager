fx_version 'bodacious'
game 'common'

author 'Jazzuh'
description 'Persistence layer for the FXServer access control list'
version '1.0.0'

fxdk_watch_command 'dotnet' {'watch', '--project', 'Server/AclManager.Server.csproj', 'publish', '--configuration', 'Release'}

server_script 'Server/bin/Release/**/publish/*.net.dll'

migration_files {
    'Migrations/0001_create_acl_tables.cs'
}

--dependency 'fxmigrant'

convar_category 'Database' {
    '',
    {
        { "Database adapter", "acl_databaseAdapter", "CV_MULTI", {
            "kvp",
            "mysql"
        } },
    }
}
