//------------------------------------------------------------------------------
// <copyright file="CSSqlTrigger.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;

public partial class Triggers
{        
    /// <summary>
    /// Insert user trigger.
    /// </summary>
    [Microsoft.SqlServer.Server.SqlTrigger(Name = "InsertUserTrigger", Target = "[User]", Event = "FOR INSERT")]
    public static void InsertUserTrigger ()
    {
        // Get the trigger context.
        SqlTriggerContext triggContext = SqlContext.TriggerContext;

        // Select the trigger.
        switch (triggContext.TriggerAction)
        {
            case TriggerAction.Insert:
                // Insert new user.
                SqlXml data = triggContext.EventData;
                break;
        }

    }
}

