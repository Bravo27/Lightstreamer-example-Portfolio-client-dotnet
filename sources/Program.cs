﻿#region License
/*
* Copyright (c) Lightstreamer Srl
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/
#endregion License

using Lightstreamer.DotNet.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TwoLevelPush_Example
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Start TwoLevel Push example");

            if (args.Length < 2) {
                Console.WriteLine("Arguments missing. Exit.");

                Thread.Sleep(7000);

                return;
            }
            string pushServerHost = args[0];
            int pushServerPort = Int32.Parse(args[1]);

            Thread.Sleep(2000);

            ConnectionInfo connInfo = new ConnectionInfo();
            connInfo.PushServerUrl = "http://" + pushServerHost + ":" + pushServerPort;
            connInfo.Adapter = "DEMO";
            
            Console.WriteLine("Opening Lightstreamer connection (" + pushServerHost + ":" + pushServerPort +")");

            LSClient myClient = new LSClient();
            TestConnectionListener myConnectionListener = new TestConnectionListener();
            try {
                myClient.OpenConnection(connInfo, myConnectionListener);
            } catch (PushUserException eu)
            {
                Console.WriteLine("Error: " + eu.Message + ". Exit.");

                Thread.Sleep(7000);

                return;
            }
            catch (PushConnException ec)
            {
                Console.WriteLine("Error: " + ec.Message + ". Exit.");

                Thread.Sleep(7000);

                return;
            }

            while (!myConnectionListener.isSessionStarted())
            {
                Thread.Sleep(700);
            }

            ExtendedTableInfo tableInfo = new ExtendedTableInfo(
                        new String[] { "portfolio1" },
                        "COMMAND",
                        new String[] { "key", "command", "qty" },
                        true
                        );

            tableInfo.DataAdapter = "PORTFOLIO_ADAPTER";

            SubscribedTableKey tableRef = myClient.SubscribeTable(
                tableInfo,
                new TestPortfolioListenerForExtended(myClient, "portfolio1"),
                true
                );


            Thread.Sleep(5000);
        }
    }
}
