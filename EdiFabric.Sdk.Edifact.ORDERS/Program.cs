﻿using EdiFabric.Core.Model.Edi;
using EdiFabric.Core.Model.Edi.Edifact;
using EdiFabric.Core.Model.Edi.ErrorContexts;
using EdiFabric.Framework.Readers;
using EdiFabric.Framework.Writers;
using EdiFabric.Rules.EDIFACT_D96A;
using EdiFabric.Sdk.Helpers;
using EdiFabric.Sdk.Helpers.Edifact;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EdiFabric.Sdk.Edifact.ORDERS
{
    class Program
    {
        static void Main(string[] args)
        {
            Read();
            Write();
        }

        /// <summary>
        /// Read Purchase Orders 
        /// </summary>
        static void Read()
        {
            var ediStream = File.OpenRead(Directory.GetCurrentDirectory() + @"\..\..\..\Files.Edifact\PurchaseOrder.txt");

            List<IEdiItem> ediItems;
            using (var ediReader = new EdifactReader(ediStream, TemplateFactory.FullTemplateFactory))
                ediItems = ediReader.ReadToEnd().ToList();

            var transactions = ediItems.OfType<TSORDERS>();

            foreach (var transaction in transactions)
            {
                MessageErrorContext mec;
                if (transaction.IsValid(out mec))
                {
                    //  valid
                }
                else
                {
                    //  invalid
                    var errors = mec.Flatten();
                }
            }
        }

        /// <summary>
        /// Write Purchase Orders
        /// </summary>
        static void Write()
        {
            using (var stream = new MemoryStream())
            {
                var transaction = TransactionBuilders.BuildPurchaseOrder("1");

                MessageErrorContext mec;
                if (transaction.IsValid(out mec, true))
                {
                    //  valid
                    using (var writer = new EdifactWriter(stream))
                    {
                        writer.Write(SegmentBuilders.BuildUnb("1"));
                        writer.Write(transaction);
                    }

                    var ediString = stream.LoadToString();
                }
                else
                {
                    //  invalid
                    var errors = mec.Flatten();
                }
            }
        }
    }
}
