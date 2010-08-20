using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NWXNet
{
    public class Response
    {
        /// <summary>
        /// The version of the server response.
        /// </summary>
        /// <value>The version.</value>
        public Version Version { get; set; }

        /// <summary>
        /// The time when this server response expires - can be used for caching.
        /// </summary>
        /// <value>The time when this server response expires</value>
        public DateTime? Expires { get; set; } // TODO: Implement in-library caching.

        /// <summary>
        /// List of warnings that may have been returned from the server (non-fatal).
        /// </summary>
        public readonly List<string> Warnings = new List<string>();

        public readonly Dictionary<string, IResponseData> Responses = new Dictionary<string, IResponseData>();

        //public T[] All<T>() where T : IResponseData
        //{
        //    return All<T>("NWXNet1.0.0");
        //    //var data = Data["NWXNet1.0.0"].Values.All<T>();
        //    //return data.Count() == 0 ? null : data.ToArray();
        //}

        /// <summary>
        /// Returns all response entries of the specified type.
        /// </summary>
        /// <typeparam name="T">Type inheriting IResponseData</typeparam>
        /// <returns>List of response entries of specified type.</returns>
        public T[] All<T>() where T : IResponseData
        {
            var data = Responses.Values.OfType<T>();
            return !data.Any() ? null : data.ToArray();
        }

        /// <summary>
        /// Grabs the response entry with specified identifier (from the request).
        /// </summary>
        /// <typeparam name="T">Type of response entry to return, to simplify type casting.</typeparam>
        /// <param name="id">The id of the response entry you wish to retrieve.</param>
        /// <returns>The response entry with the specified id.</returns>
        public T WithId<T>(string id) where T : IResponseData
        {
            IResponseData data;
            var responsesOfType = Responses.Where(r => r.Value.GetType() == typeof (T)).ToDictionary(r => r.Key,
                                                                                                     r => r.Value);
            if(responsesOfType.TryGetValue(id, out data))
            {
                return (T) data;
            }
            return default(T);
        }

        /// <summary>
        /// Helper method to quickly return responses from an Available.Epochs request.
        /// </summary>
        /// <value>The epochs.</value>
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

        /// <summary>
        /// Helper method to quickly return responses from an Available.Levels request.
        /// </summary>
        /// <value>The levels.</value>
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


        /// <summary>
        /// Gets a value indicating whether this instance is empty (ie. no repsonse was given).
        /// May be phased out as server usually returns warnings in this case.
        /// </summary>
        /// <value><c>true</c> if this instance is empty; otherwise, <c>false</c>.</value>
        public bool IsEmpty
        {
            get
            {
                return  !Responses.Values.Any();
            }
        }

        /// <summary>
        /// Gets a value indicating whether this response has warnings.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this response has warnings; otherwise, <c>false</c>.
        /// </value>
        public bool HasWarnings
        {
            get
            {
                return Warnings.Count > 0;
            }
        }
    }
}
