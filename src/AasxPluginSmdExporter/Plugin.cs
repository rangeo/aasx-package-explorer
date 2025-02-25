﻿/*
Copyright (c) 2021 KEB Automation KG <https://www.keb.de/>,
Copyright (c) 2021 Lenze SE <https://www.lenze.com/en-de/>,
author: Jonas Grote, Denis Göllner, Sebastian Bischof

This source code is licensed under the Apache License 2.0 (see LICENSE.txt).

This source code may use other Open Source software components (see LICENSE.txt).
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AasxPackageExplorer;
using AasxPluginSmdExporter;
using AasxPluginSmdExporter.View;
using JetBrains.Annotations;

namespace AasxIntegrationBase // the namespace has to be: AasxIntegrationBase
{
    [UsedImplicitlyAttribute]
    public class AasxPlugin : IAasxPluginInterface
    {
        public LogInstance Log = new LogInstance();

        public AasxPluginResultBase ActivateAction(string action, params object[] args)
        {
            if (action == "generate-SMD")
            {
                Queue<string> logs;
                string modeltype = "";
                string machineName = "";
                // To work three arguments are needed
                if (args[0] is AasxIntegrationBase.IFlyoutProvider &&
                    args[1] is Queue<string> &&
                    args[2] is string &&
                    args[3] is string)
                {
                    var fop = args[0] as IFlyoutProvider;
                    if (fop == null) return null;

                    // Flyout for the name
                    var tb = new TextBoxFlyout("Enter name:", System.Windows.MessageBoxImage.Question);
                    fop.StartFlyoverModal(tb);
                    if (!tb.Result) return null;
                    machineName = tb.Text;

                    // Flyout for choosing type of simulationmodel
                    var pd = new PhysicalDialog();
                    fop.StartFlyoverModal(pd);
                    modeltype = pd.Result;
                    if (modeltype == null) return null;

                    // Gets the queue from the argument list
                    // The queue is used to be able to display log messages in the Package Explorer
                    logs = args[1] as Queue<string>;


                }
                else
                {
                    return new AasxPluginResultBase();
                }

                TextUI ui = new TextUI(logs);
                // The host and the machine are given as arguments(2&3) by the package explorer
                ui.Start(args[2] as string, machineName, modeltype);

            }



            return new AasxPluginResultBase();
        }

        public object CheckForLogMessage()
        {
            return Log.PopLastShortTermPrint();
        }

        public string GetPluginName()
        {
            return "AasxPluginSmdExporter";
        }

        public void InitPlugin(string[] args)
        {
            Log.Info("InitPlugin() called with args = {0}", (args == null) ? "" : string.Join(", ", args));
        }

        public AasxPluginActionDescriptionBase[] ListActions()
        {

            Log.Info("ListActions() called");
            var res = new List<AasxPluginActionDescriptionBase>();
            res.Add(new AasxPluginActionDescriptionBase("generate-SMD", "Generates a SMD"));
            return res.ToArray();
        }
    }
}
