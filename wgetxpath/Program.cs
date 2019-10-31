using System;
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

            Options options = null;
            Parser.Default.ParseArguments<Options>(args).WithParsed(o =>
            { 
                options = o;
            });

            if (options == null)
            {
                Console.Error.WriteLine("Looks like there was an error parsing the command line arguments");
                return 1;
            }

            if (options.SslNoCheck)
            {
                ServicePointManager.ServerCertificateValidationCallback += (o, cert, chain, errors) => true;
            }
            
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc;
            try
            {
                 doc = web.Load(options.Uri);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Looks like there was an error loading the URI:\n{ex}");
                return 1;
            }

            HtmlNodeCollection nodes;
            try
            {
                nodes = doc.DocumentNode.SelectNodes(options.Xpath);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Looks like there was an error with the XPath:\n{ex}");
                return 1;
            }

            if (nodes.Count == 0)
            {
                return 0;
            }

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
            
            return 0;
        }
    }
}
