# ACL Persistence Manager

A persistence layer for the FXServer access control list. 

Information on how the ACL works can be [found here](https://forum.cfx.re/t/basic-aces-principals-overview-guide/90917)

## Features

- Automatic loading of ACEs and principals on server start
- Support for different database adapters. Currently `kvp` and `mysql` are supported
- Commands for registering ACEs & principals
- Exports for registering ACEs & principals

## To-do

- Management UI (webadmin-esque type thing)

## Dependencies

- [FxMigrant](https://github.com/Jazzuh/fxmigrant/releases)
## Installation

1. Download the latest release [from here](https://github.com/Jazzuh/AclPersistenceManager/releases).
2. Extract it and place into your `resources` folder. 
3. Edit the `config.cfg` file found within the root of the resource.
4. Add `exec @aclmanager/exec.cfg` to your `server.cfg`.
    
## Building

1. Clone the repository.
2. Make sure you have .NET Core SDK installed.
3. Run build.ps1.



## Configuration

All configuration options can be found in the `config.cfg` file
## API

### Commands


```
acl grant <principal> <object> <allow|deny> -- Equivalent to add_ace
acl revoke <principal> <object> <allow|deny> -- Equivalent to remove_ace

acl inherit <child> <parent> -- Equivalent to add_principal
acl disinherit <child> <parent> -- Equivalent to remove_principal
```

### Exports

```
exports['aclmanager']:HasLoaded() -> bool

exports['aclmanager']:AddAce(string: principal, string: object, string: allowType) -> void
exports['aclmanager']:AddAces(Ace[]: aces) -> void

exports['aclmanager']:RemoveAce(string: principal, string: object, string: allowType) -> void
exports['aclmanager']:RemoveAces(Ace[]: aces) -> void

exports['aclmanager']:AddPrincipal(string: child, string: parent) -> void
exports['aclmanager']:AddPrincipals(Principal[]: principals) -> void

exports['aclmanager']:RemovePrincipal(string: child, string: parent) -> void
exports['aclmanager']:RemovePrincipals(Principal[]: principals) -> void
```

### Events

``` lua
-- All these events are only called on the server

-- Called on inital load of the ACL
AddEventHandler('acl:onLoaded', function() end)

-- Called whenever the ACL has entries added or removed
AddEventHandler('acl:onReloaded', function() end)

-- Called on resource stop once all ACL entries have been unloaded
AddEventHandler('acl:onUnloaded', function() end)
```
## Permissions

To keep the nesting around ACE objects `.`s into account, any action will be hit with a permission check to ensure the calling source is allowed to perform the specified action. 

Although nesting is only taken into account for ACEs, this check will also be applied to Principals as a "just in case" measure to protect against bad actors trying to give themselves permissions they were not explicitly granted. 

Each action has a corresponding ACE that must be permitted in order for the action to take effect:

- `acl.grant.{ace}`
    - This applies to the `acl grant` command and the `AddAce(s)` exports

- `acl.revoke.{ace}`
    - This applies to the `acl revoke` command and the `RemoveAce(s)` exports

- `acl.inherit.{parentPrincipal}`
    - This applies to the `acl inherit` command and the `AddPrincipal(s)` exports

- `acl.disinherit.{parentPrincipal}`
    - This applies to the `acl disinherit` command and the `RemovePrincipal(s)` exports


## Usage


### Command

```
 -- Make sure the player doing this has the `acl.grant.command.killall` ace granted
 > acl grant group.admin command.killall allow

 -- Make sure the player doing this has the `acl.inherit.group.admin` ace granted
 > acl inherit group.superadmin group.admin
```

### Lua

Lua comes with a helper class which you can use by adding `server_script '@aclmanager/helper.lua'` to your resource manifest

#### generic_admin_script.lua
```lua
-- Ensures that the ACL has been fully loaded before trying to edit it
ACL.OnLoaded(function()
    -- Give admin permission to all commands. Make sure the resource calling this has the 'acl.grant.command' granted
    ACL.AddAce("group.admin", "command", "allow")

    ACL.AddAces({
        Ace("group.moderator", "command.kick", "allow"), -- Helper function which will format the ace for you
        { "group.moderator", "auditlog", "allow"}, -- Can also just do it yourself if you want to
    })

    -- Let the admin group in inherit from the moderator group. Make sure the source calling this has the 'acl.inherit.group.moderator' granted
    ACL.AddPrincipal("group.admin", "group.moderator")
end)
```

### C#

#### GenericAdminScript.cs

```cs
using CitizenFX.Core;
using System.Collections.Generic;

public class GenericAdminScript : BaseScript 
{
    // Ensures that the ACL has been fully loaded before trying to edit it
    [EventHandler("acl:onLoaded")]
    private void OnAclLoaded()
    {
        var aclManager = Exports["aclmanager"]

        // Give admin permission to all commands. Make sure the resource calling this has the 'acl.grant.command' granted
        aclManager.AddAce("group.admin", "command", "allow")

        aclManager.AddAces(new List<object>
        {
            new List<object> { "group.moderator", "command.kick", "allow" }, 
            new List<object> { "group.moderator", "auditlog", "allow"}, 
        })

        // Let the admin group in inherit from the moderator group. Make sure the source calling this has the 'acl.inherit.group.moderator' granted
        aclManager.AddPrincipal("group.admin", "group.moderator")
    }
}
```

