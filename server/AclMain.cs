using System;
using System.Collections.Generic;
using System.Linq;
using AclManager.Server.Enums;
using AclManager.Server.Extensions;
using AclManager.Server.Handlers;
using AclManager.Server.Objects;
using CfxUtils.Logging;
using CitizenFX.Core;
using Microsoft.Extensions.DependencyInjection;
using static CitizenFX.Core.Native.API;
using Console = AclManager.Server.Helpers.Console;

namespace AclManager.Server
{
    public class AclMain : BaseScript
    {
        private ServiceProvider _serviceProvider;

        public AclMain()
        {
            CheckResourceCommandAccess();

            var dbAdapterType = GetConvar("acl_databaseAdapter", "kvp").ToLower();

            if (dbAdapterType == "mysql")
            {
                var connectionString = GetConvar("mysql_connection_string", "");

                if (string.IsNullOrEmpty(connectionString))
                {
                    Console.SendReply(ReplyType.Error, $"The {LogColors.Yellow}mysql_connection_string{LogColors.Reset} convar is unset. Please fix this to use mysql!");
                }
            }

            _serviceProvider = new ServiceCollection()
                              .AddDatabaseAdapter(dbAdapterType)
                              .AddSingleton<AclHandler>()
                              .AddSingleton<AclCommandHandler>()
                              .AddSingleton<AclExportHandler>()
                              .BuildServiceProvider();

            AddExports();

            if (dbAdapterType == "kvp")
            {
                _serviceProvider.GetRequiredService<AclHandler>().LoadAcl();
            }
        }

        /// <summary>
        /// Checks if this resource has access to the required commands
        /// </summary>
        private void CheckResourceCommandAccess()
        {
            var resourceName = GetCurrentResourceName();
            var resourcePrincipal = $"resource.{resourceName}";

            var commands = new[] { "command.add_ace", "command.add_principal", "command.remove_ace", "command.remove_principal" };

            foreach (var command in commands)
            {
                if (!IsPrincipalAceAllowed(resourcePrincipal, command))
                {
                    Debug.WriteLine($"You have not granted {command.ToYellow()} access to resource {resourceName.ToYellow()}. Please ensure you've read the documentation");
                }
            }
        }

        /// <summary>
        /// Adds the exports for this resource
        /// </summary>
        private void AddExports()
        {
            var exportHandler = _serviceProvider.GetRequiredService<AclExportHandler>();

            Exports.Add("HasLoaded", new Func<bool>(exportHandler.HasLoaded));

            Exports.Add("AddAce", new Action<string, string, string>(exportHandler.AddAceExport));
            Exports.Add("AddAces", new Action<List<object>>(exportHandler.AddAcesExport));

            Exports.Add("RemoveAce", new Action<string, string, string>(exportHandler.RemoveAceExport));
            Exports.Add("RemoveAces", new Action<List<object>> (exportHandler.RemoveAcesExport));

            Exports.Add("AddPrincipal", new Action<string, string>(exportHandler.AddPrincipalExport));
            Exports.Add("AddPrincipals", new Action<List<object>> (exportHandler.AddPrincipalsExport));

            Exports.Add("RemovePrincipal", new Action<string, string>(exportHandler.RemovePrincipalExport));
            Exports.Add("RemovePrincipals", new Action<List<object>> (exportHandler.RemovePrincipalsExport));
        }

        [Command("acl", Restricted = true)]
        private void OnAclCommand(Player source, string[] args)
        {
            if (args.Length < 1)
            {
                source.SendReply(ReplyType.Error, "Usage: acl [command]");
                return;
            }

            var command = args[0];

            var argsList = args.ToList();
            argsList.RemoveAt(0);

            _serviceProvider.GetRequiredService<AclCommandHandler>().HandleCommand(source, command, argsList);
        }

        [EventHandler("acl:onLoaded")]
        private void OnAclLoaded()
        {
            _serviceProvider.GetRequiredService<AclHandler>().AddAce(new ("system.console", "acl", "allow"));
        }

        [EventHandler("onResourceStop")]
        private void OnResourceStop(string resource)
        {
            if (GetCurrentResourceName() != resource)
            {
                return;
            }

            _serviceProvider.GetRequiredService<AclHandler>().UnloadAcl();
        }

        [EventHandler("fxmigrant:resourceDone")]
        private async void OnMigrantReady(string resourceName, bool success)
        {
            if (resourceName != GetCurrentResourceName())
            {
                return;
            }

            if (!success)
            {
                Debug.WriteLine("Migration of database was unsuccessful");
                return;
            }

            await _serviceProvider.GetRequiredService<AclHandler>().LoadAcl();
        }
    }
}
