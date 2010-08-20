using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using NWXNet;
using NWXNet.Exceptions;

namespace NWXNet.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                NWX.Authenticator = new ApplicationAuthenticator("nwxnet", 0, "2ea4d9f6f80bae06a098c47c39c28b69ae4e3d12");
                var epochsResponse = NWX.Request.For(Available.Epochs).Send();
                DateTime epoch = epochsResponse.Epochs.ClosestToNow;
                //var response = NWX.Request.For(METAR.ForICAO("CYYC")).Send();
                var response = NWX.Request.For(Chart.For("46,-150", "63,-54", 700, epoch, 1280, 1024).WithId("chart1").UsingMapLayout.WithFeatures(ChartFeatures.SurfaceTemperature(true), ChartFeatures.CountriesAndCoastlines)).Send();
               
                if (!response.HasWarnings)
                {
                    //Console.WriteLine(response.WithId<METARResponse>("CYYC"));
                    response.WithId<ChartResponse>("chart1").Chart.Save(@"C:\chart.png");
                    System.Diagnostics.Process.Start(@"C:\chart.png");
                }
                else
                {
                    Console.WriteLine(response.Warnings[0]);
                }

            }
            catch (NWXServerException ex)
            {
                Console.WriteLine("Server exception: " + ex);
            }
            catch(NWXClientException ex)
            {
                Console.WriteLine("Client exception: " + ex);
            }
           //Console.ReadKey(true);
        }
    }
}
