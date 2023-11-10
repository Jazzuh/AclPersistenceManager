ACL = {}

function Ace(principal, object, allowType)
    return { principal, object, allowType }
end

function Principal(child, parent)
    return { child, parent }
end

function ACL.AddAce(principal, object, allowType)
    if not principal or not object or not allowType then
        return 
    end

    exports['aclmanager']:AddAce(principal, object, allowType)
end

function ACL.AddAces(aces)
    if type(aces) ~= "table" then
        return
    end

    exports['aclmanager']:AddAces(aces)
end

function ACL.RemoveAce(principal, object, allowType)
    if not principal or not object or not allowType then
        return 
    end

    exports['aclmanager']:RemoveAce(principal, object, allowType)
end

function ACL.RemoveAces(aces)
    if type(aces) ~= "table" then
        return
    end

    exports['aclmanager']:RemoveAces(aces)
end

function ACL.AddPrincipal(child, parent)
    if not child or not parent then 
        return 
    end

    exports['aclmanager']:AddPrincipal(child, parent)
end

function ACL.AddPrincipals(principals)
    if type(principals) ~= "table" then
        return
    end

    exports['aclmanager']:AddPrincipals(principals)
end

function ACL.RemovePrincipal(child, parent)
    if not child or not parent then 
        return 
    end

    exports['aclmanager']:RemovePrincipal(child, parent)
end

function ACL.RemovePrincipals(principals)
    if type(principals) ~= "table" then
        return
    end

    exports['aclmanager']:RemovePrincipals(aces)
end

function ACL.OnLoaded(cb)
    Citizen.CreateThread(function ()
        while GetResourceState('aclmanager') ~= 'started' do
            Citizen.Wait(0)
        end

        while not exports['aclmanager']:HasLoaded() do
            Citizen.Wait(0)
        end

        cb()
    end)
end