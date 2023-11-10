using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AclManager.Server.Objects;
using CfxUtils.Logging;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using MySqlConnector;
using SqlKata.Compilers;
using SqlKata.Execution;
using Console = AclManager.Server.Helpers.Console;

namespace AclManager.Server.Adapters
{
    internal class MySqlAdapter : IDatabaseAdapter
    {
        public async Task<List<Ace>> GetAllAces()
        {
            Console.WriteDebug("Getting ace objects");

            using var db = CreateQueryFactory();

            var aces = await db.Query("aces")
                        .Select("ace_id as Id", "principal as Principal", "object as Object", "allow_type as AllowType").GetAsync<Ace>();

            Console.WriteDebug("Done getting ace objects");

            return aces.ToList();
        }

        public async Task<List<PrincipalInheritance>> GetAllPrincipals()
        {
            Console.WriteDebug("Getting principal inheritance");

            using var db = CreateQueryFactory();

            var principals = await db.Query("principals")
                               .Select("principal_id as Id", "child as Child", "parent as Parent").GetAsync<PrincipalInheritance>();

            Console.WriteDebug("Done getting principal inheritance");

            return principals.ToList();
        }

        public async Task SaveAces(IList<Ace> aces)
        {
            if (aces.Count == 0)
            {
                return;
            }

            Console.WriteDebug($"Saving {aces.Count} aces");

            using var db = CreateQueryFactory();
            
            db.Connection.Open();

            using var transaction = db.Connection.BeginTransaction();

            try
            {
                var firstId = await db.Query("aces").InsertGetIdAsync<int>(new List<KeyValuePair<string, object>>
                {
                    new ("principal", aces.First().Principal),
                    new ("object", aces.First().Object),
                    new ("allow_type", aces.First().AllowType),
                }, transaction);

                if (aces.Count > 1)
                {
                    var insertColumns = new[] { "principal", "object", "allow_type" };
                    var insertData = aces.Skip(1)
                                         .Select(ace => new[]
                                          {
                                              ace.Principal, 
                                              ace.Object, 
                                              ace.AllowType
                                          })
                                         .ToList();

                    await db.Query("aces").InsertAsync(insertColumns, insertData, transaction);
                }

                foreach (var ace in aces)
                {
                    ace.Id = firstId;

                    firstId++;
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{LogColors.Red}ERROR:{LogColors.Reset} SaveAce transaction failed: {ex.StackTrace}");

                transaction.Rollback();
            }

            Console.WriteDebug($"Done saving {aces.Count} aces");
        }

        public async Task DeleteAces(IList<Ace> aces)
        {
            Console.WriteDebug($"Deleting {aces.Count} aces");

            using var db = CreateQueryFactory();

            try
            {
                await db.Query("aces").WhereIn("ace_id", aces.Select(o => o.Id)).DeleteAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{LogColors.Red}ERROR:{LogColors.Reset} DeleteAce transaction failed: {ex.StackTrace}");
            }

            Console.WriteDebug($"Done deleting {aces.Count} aces");
        }

        public async Task SavePrincipals(IList<PrincipalInheritance> principals)
        {
            if (principals.Count == 0)
            {
                return;
            }

            Console.WriteDebug($"Saving {principals.Count} principals");

            using var db = CreateQueryFactory();

            db.Connection.Open();

            using var transaction = db.Connection.BeginTransaction();

            try
            {
                var firstId = await db.Query("principals").InsertGetIdAsync<int>(new List<KeyValuePair<string, object>>
                {
                    new ("child", principals.First().Child),
                    new ("parent", principals.First().Parent),
                }, transaction);

                if (principals.Count > 1)
                {
                    var insertColumns = new[] { "child", "parent" };
                    var insertData = principals.Skip(1)
                                               .Select(ace => new[]
                                                {
                                                    ace.Child,
                                                    ace.Parent
                                                })
                                               .ToList();

                    await db.Query("principals").InsertAsync(insertColumns, insertData, transaction);
                }

                foreach (var principal in principals)
                {
                    principal.Id = firstId;

                    firstId++;
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{LogColors.Red}ERROR:{LogColors.Reset} SavePrincipal transaction failed: {ex.StackTrace}");

                transaction.Rollback();
            }

            Console.WriteDebug($"Done saving {principals.Count} principals");
        }

        public async Task DeletePrincipals(IList<PrincipalInheritance> principals)
        {
            Console.WriteDebug($"Deleting {principals.Count} principals");

            using var db = CreateQueryFactory();

            try
            {
                await db.Query("principals").WhereIn("principal_id", principals.Select(o => o.Id)).DeleteAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{LogColors.Red}ERROR:{LogColors.Reset} DeletePrincipal transaction failed: {ex.StackTrace}");
            }

            Console.WriteDebug($"Done deleting {principals.Count} principals");
        }

        private QueryFactory CreateQueryFactory()
        {
            var conn = new MySqlConnection(API.GetConvar("mysql_connection_string", ""));

            var compiler = new MySqlCompiler();

            return new (conn, compiler)
            {
                Logger = result =>
                {
                    Console.WriteDebug(result.ToString());
                }
            };
        }
    }
}
