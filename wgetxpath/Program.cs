using System;
using System.Collections.Generic;
using System.Net;
using CommandLine;
using HtmlAgilityPack;

namespace wgetxpath
{
    internal class Program
    {
        public class Options
        {
            [Value(0,
                MetaName = "Input URI",
                HelpText = "Input URI to be processed.",
                Required = true)]
            public string Uri { get; set; }

            [Value(1,
                MetaName = "XPath",
                HelpText = "XPath to be applied to the URI.",
                Required = true)]
            public string Xpath { get; set; }

            [Option('d', "disable-ssl-checks", Required = false, HelpText = "Disables SSL checks.")]
            public bool SslNoCheck { get; set; }
        }

        public static int Main(string[] args)
        {
            #if DEBUG
            args = new[] {@"https://test.com", @"//table//tr//td[position()= 1 or position() = 2]", "-d"};
            #endif

            // Command line
            Options currentOptions = ParseCommandLineArgs(args);
            if (currentOptions == null)
            {
                return 1;
            }

            // Get HTML
            HtmlNodeCollection nodes = GetHtmlMatchingNodes(currentOptions.Uri, currentOptions.Xpath);
            if (nodes == null || nodes.Count == 0)
            {
                return 1;
            }

            // Print
            PrintResults(nodes);

            // Done
            return 0;
        }

        protected static Options ParseCommandLineArgs(IEnumerable<string> args)
        {
            Options currentOptions = null;
            Parser.Default.ParseArguments<Options>(args).WithParsed(o => { currentOptions = o; });

            if (currentOptions == null)
            {
                Console.Error.WriteLine("Looks like there was an error parsing the command line arguments.");
                return null;
            }

            if (currentOptions.SslNoCheck)
            {
                ServicePointManager.ServerCertificateValidationCallback += (o, cert, chain, errors) => true;
            }

            return currentOptions;
        }

        protected static HtmlNodeCollection GetHtmlMatchingNodes(string uri, string xpath)
        {
            HtmlWeb web = new HtmlWeb();
            
            HtmlDocument doc;
            try
            {
                doc = web.Load(uri);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Looks like there was an error loading the URI:\n{ex}");
                return null;
            }

            HtmlNodeCollection nodes;
            try
            {
                nodes = doc.DocumentNode.SelectNodes(xpath);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Looks like there was an error with the XPath:\n{ex}");
                return null;
            }

            if (nodes == null || nodes.Count == 0)
            {
                Console.Error.WriteLine("Looks like the XPath did not match anything.");
                return null;
            }
            
            return nodes;
        }


        protected static void PrintResults(HtmlNodeCollection nodes)
        {
            HtmlNode lastParentNode = null;
            foreach (HtmlNode node in nodes)
            {
                if (lastParentNode == null)
                {
                    Console.Write(node.InnerText);
                }
                else if (lastParentNode == node.ParentNode)
                {
                    Console.Write($" {node.InnerText}");
                }
                else
                {
                    Console.WriteLine();
                    Console.Write(node.InnerText);
                }

                lastParentNode = node.ParentNode;
            }
        }
    }
}
