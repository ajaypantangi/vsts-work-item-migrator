﻿using System;
using System.Threading.Tasks;
using Logging;
using Microsoft.Extensions.Logging;

namespace Common.Validation
{
    [RunOrder(1)]
    public class ValidateSourcePermissions : IConfigurationValidator
    {
        private ILogger Logger { get; } = MigratorLogging.CreateLogger<ValidateSourcePermissions>();

        public string Name => "Source account permissions";

        public async Task Validate(IValidationContext context)
        {
            //if optional postmovetag is not specified, then we need only read permissions to the source account 
            //else if postmovetag is specified in the config file, then we will need write permissions to the source account 
            if (String.IsNullOrEmpty(context.Config.SourcePostMoveTag))
            {
                Logger.LogInformation(LogDestination.File, "Checking read permissions on the source project");
                
                await ValidationHelpers.CheckConnection(context.SourceClient, context.Config.SourceConnection.Project, ValidationHelpers.ReadPermission);
            }
            else
            {
                Logger.LogInformation(LogDestination.File, "source-post-move-tag is specified, checking write permissions on the source project");
                
                await ValidationHelpers.CheckConnection(context.SourceClient, context.Config.SourceConnection.Project, ValidationHelpers.WritePermission);
                await ValidationHelpers.CheckConnection(context.SourceClient, context.Config.SourceConnection.Project, ValidationHelpers.BypassRulesPermission);
                await ValidationHelpers.CheckConnection(context.SourceClient, context.Config.SourceConnection.Project, ValidationHelpers.SuppressNotificationsPermission);
                //await ValidationHelpers.CheckIdentity(context.SourceClient, context.Config.SourceConnection.Project);
            }
        }
    }
}
