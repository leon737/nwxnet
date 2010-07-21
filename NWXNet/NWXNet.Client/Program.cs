using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                var epochsResponse = NWX.Request.For(Available.Epochs).Send();
                DateTime epoch = epochsResponse.Epochs.Latest;
                var response = NWX.Request.For(Wind.At("51,114", 100, AltitudeUnit.ImperialFlightLevel, epoch).WithId("1")).Send();
               
                if (!response.HasWarnings)
                {
                    Console.WriteLine(response.WithId<WindResponse>("1"));
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
            Console.ReadKey(true);
        }
    }
}
