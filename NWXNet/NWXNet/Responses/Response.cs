using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NWXNet
{
    public class Response
    {
        public Version Version { get; set; }

        public DateTime? Expires { get; set; }

        public readonly List<string> Warnings = new List<string>();

        public readonly Dictionary<string, IResponseData> Responses = new Dictionary<string, IResponseData>();

        //public T[] All<T>() where T : IResponseData
        //{
        //    return All<T>("NWXNet1.0.0");
        //    //var data = Data["NWXNet1.0.0"].Values.All<T>();
        //    //return data.Count() == 0 ? null : data.ToArray();
        //}

        public T[] All<T>() where T : IResponseData
        {
            var data = Responses.Values.OfType<T>();
            return data.Count() == 0 ? null : data.ToArray();
        }

        public T WithId<T>(string id) where T : IResponseData
        {
            IResponseData data;
            if(Responses.TryGetValue(id, out data))
            {
                return (T) data;
            }
            return default(T);
        }

        public AvailableEpochsResponse Epochs
        {
            get
            {
                IResponseData data;
                if(Responses.TryGetValue("AvailableEpochs", out data))
                {
                    return data as AvailableEpochsResponse;
                }
                return null;
            }
        }

        public AvailableLevelsResponse Levels
        {
            get
            {
                IResponseData data;
                if (Responses.TryGetValue("AvailableLevels", out data))
                {
                    return data as AvailableLevelsResponse;
                }
                return null;
            }
        }
        

        public bool IsEmpty
        {
            get
            {
                if(Responses.Values.Count() == 0)
                //if (Responses.Values.Any(response => response.Values.Count() == 0))
                {
                    return true;
                }
                return false;
            }
        }

        public bool HasWarnings
        {
            get
            {
                return Warnings.Count > 0;
            }
        }
    }
}
