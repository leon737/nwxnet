using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using NWXNet.Exceptions;

namespace NWXNet
{
    public class NWXXmlSerializer : INWXSerializer
    {
        #region Implementation of INWXSerializer

        public string Serialize(Request request)
        {
            var doc = new XElement("nwx",
                                   new XAttribute("version", request.Version.ToString()),
                                   new XElement("Request",
                                                from d in request.Data
                                                select SerializeXml(d.Type, d)));
            //select d.GenerateXml()));
            return doc.ToString();


            //var doc = new XmlDocument();
            //doc.AppendChild(doc.CreateXmlDeclaration("1.0", "utf-8", null));

            //XmlElement nwx = doc.CreateElement("nwx");
            //nwx.SetAttribute("version", request.Version.ToString());

            //XmlElement req = doc.CreateElement("Request");

            //foreach(IRequestData data in request.Data)
            //{
            //    req.AppendChild(data.GenerateXml())
            //}

            //nwx.AppendChild(req);
            //doc.AppendChild(nwx);
            //return doc.InnerXml;
        }

        public Response Deserialize(string data)
        {
            var responseObject = new Response();

            XElement doc = XElement.Load(new StringReader(data));

            XAttribute version = doc.Attribute("version");
            Version docVersion;
            if (version == null || !Version.TryParse(version.Value, out docVersion))
                throw new NWXServerException("Bad version returned from server.");

            responseObject.Version = docVersion;


            IEnumerable<XElement> errors = doc.Descendants("error");
            if (errors.Count() > 0)
            {
                List<ServerError> errorData =
                    errors.Select(
                        element => new ServerError(Int32.Parse(element.Attribute("code").Value), element.Value)).ToList();
                throw new NWXServerException(errorData);
            }

            IEnumerable<XElement> warnings = doc.Descendants("warning");
            if (warnings.Count() > 0)
            {
                foreach (XElement warning in warnings)
                {
                    responseObject.Warnings.Add(warning.Value);
                }
            }

            //var warnings = doc.Elements("warning");
            //if(warnings.Count() > 0)
            //{
            //    foreach (var warning in warnings)
            //    {
            //        responseObject.Warnings.Add(warning.Value);
            //    }
            //    return responseObject;
            //}


            IEnumerable<XElement> responses = doc.Elements("Response");

            foreach (XElement response in responses)
            {
                XAttribute expires = response.Attribute("expires");

                if (expires != null)
                {
                    DateTime expiresTime;
                    if (!DateTime.TryParse(expires.Value, out expiresTime))
                        throw new NWXServerException("Bad expiry time returned from server.");
                    responseObject.Expires = expiresTime;
                }
                else
                {
                    responseObject.Expires = null;
                }
                //if (response.Attribute("id") == null || String.IsNullOrEmpty(response.Attribute("id").Value))
                //  throw new NWXServerException("Request ID not returned from server.");

                //var responseId = response.Attribute("id").Value;
                //responseObject.Responses.Add(responseId, new Dictionary<string, IResponseData>());

                foreach (XElement responseData in response.Elements())
                {
                    switch (responseData.Name.ToString().ToUpper())
                    {
                        case "METAR":
                            GenerateMETARResponseData(responseData, responseObject);
                            break;

                        case "AVAILABLEEPOCHS":
                            GenerateAvailableEpochsResponseData(responseData, responseObject);
                            break;

                        case "AVAILABLELEVELS":
                            GenerateAvailableLevelsResponseData(responseData, responseObject);
                            break;

                        case "WIND":
                            GenerateWindResponseData(responseData, responseObject);
                            break;
                    }
                }
            }

            return responseObject;
        }

        private static XElement SerializeXml(RequestTypes type, IRequestData data)
        {
            switch (type)
            {
                case RequestTypes.METAR:
                    var metarData = data as METAR;
                    if (metarData != null)
                    {
                        XAttribute id = metarData.Icao == null
                                            ? new XAttribute("p", metarData.Coordinates.ToString("p"))
                                            : new XAttribute("icao", metarData.Icao);
                        XAttribute count = metarData.Count == 0 ? null : new XAttribute("count", metarData.Count);
                        XAttribute maxAge = metarData.MaxAge == 0 ? null : new XAttribute("maxage", metarData.MaxAge);
                        var element = new XElement("Metar",
                                                   id,
                                                   count,
                                                   maxAge);
                        return element;
                    }
                    break;

                case RequestTypes.AvailableEpochs:
                    return new XElement("AvailableEpochs");

                case RequestTypes.AvailableLevels:
                    return new XElement("AvailableLevels");

                case RequestTypes.Wind:
                    var windData = data as Wind;
                    if (windData != null)
                    {
                        var coords = new XAttribute("p", windData.Coordinates.ToString("p"));
                        var altitude = new XAttribute("z", windData.Altitude);
                        var unit = new XAttribute("u", Altitude.EnumToUnitCode(windData.Unit));
                        var epoch = new XAttribute("e", windData.Epoch.ToNWXString());
                        XAttribute id = windData.Id == null ? null : new XAttribute("id", windData.Id);
                        return new XElement("Wind",
                                            coords,
                                            altitude,
                                            unit,
                                            epoch,
                                            id);
                    }
                    break;
            }
            return null;
        }

        private static void GenerateWindResponseData(XElement responseData, Response responseObject)
        {
            XElement direction = responseData.Element("dir");
            if (direction == null || string.IsNullOrEmpty(direction.Value))
                throw new NWXServerException("No direction returned from server for wind request.");

            XElement speed = responseData.Element("speed");
            if (speed == null || string.IsNullOrEmpty(speed.Value))
                throw new NWXServerException("No speed returned from server for wind request.");

            XAttribute coords = responseData.Attribute("p");
            if (coords == null || string.IsNullOrEmpty(coords.Value))
                throw new NWXServerException("No coordinates returned from server for wind request.");

            XAttribute altitude = responseData.Attribute("z");
            if (altitude == null || string.IsNullOrEmpty(altitude.Value))
                throw new NWXServerException("No altitude returned from server for wind request.");

            XAttribute unit = responseData.Attribute("u");
            if (unit == null || string.IsNullOrEmpty(unit.Value))
                throw new NWXServerException("No altitude unit returned from server for wind request.");

            XAttribute epoch = responseData.Attribute("e");
            if (epoch == null || string.IsNullOrEmpty(epoch.Value))
                throw new NWXServerException("No epoch returned from server for wind request.");

            XAttribute id = responseData.Attribute("id");
            bool idSet = (id != null && !string.IsNullOrEmpty(id.Value));


            var newData = new WindResponse
                              {
                                  Coordinates = new LatLon(coords.Value),
                                  Direction = Double.Parse(direction.Value),
                                  Speed = Double.Parse(speed.Value),
                                  Altitude = Int32.Parse(altitude.Value),
                                  Unit = Altitude.UnitCodeToEnum(unit.Value),
                                  Epoch = DateTime.Parse(epoch.Value),
                                  Id = idSet ? id.Value : null
                              };

            responseObject.Responses.Add(idSet ? id.Value : coords.Value, newData);
        }

        private static void GenerateAvailableLevelsResponseData(XElement responseData, Response responseObject)
        {
            IEnumerable<XElement> levels = responseData.Elements("level");
            if (levels == null || levels.Count() == 0)
                throw new NWXServerException("No available levels returned from server.");

            var newData = new AvailableLevelsResponse();
            foreach (XElement level in levels)
            {
                newData.Levels.Add(Int32.Parse(level.Value));
            }

            responseObject.Responses.Add("AvailableLevels", newData);
        }

        private static void GenerateAvailableEpochsResponseData(XElement responseData, Response responseObject)
        {
            IEnumerable<XElement> epochs = responseData.Elements("epoch");
            if (epochs == null || epochs.Count() == 0)
                throw new NWXServerException("No available epochs returned from server.");

            var newData = new AvailableEpochsResponse();

            foreach (XElement epoch in epochs)
            {
                newData.Epochs.Add(DateTime.Parse(epoch.Value));
            }

            responseObject.Responses.Add("AvailableEpochs", newData);
        }

        private static void GenerateMETARResponseData(XElement responseData, Response responseObject)
        {
            //var warnings = responseData.Elements("warning");
            // if (warnings.Count() > 0)
            // {
            //     foreach (var warning in warnings)
            //     {
            //         responseObject.Warnings.Add(warning.Value);
            //     }
            //  }
            //  else
            //  {
            bool icaoProvided = responseData.Attribute("icao") != null &&
                                !String.IsNullOrEmpty(responseData.Attribute("icao").Value);
            bool latlonProvided = responseData.Attribute("p") != null &&
                                  !String.IsNullOrEmpty(responseData.Attribute("p").Value);
            if (!icaoProvided && !latlonProvided)
                throw new NWXServerException("METAR identifier not returned from server.");
            if ((responseData.Elements("report").Count() == 0 ||
                 responseData.Elements("report") == null) && responseObject.Warnings.Count == 0)
                throw new NWXServerException("No report data returned from server.");

            var newData = new METARResponse
                              {
                                  ICAO = icaoProvided ? responseData.Attribute("icao").Value : "",
                                  Location = latlonProvided ? new LatLon(responseData.Attribute("p").Value) : null,
                                  DistanceFromRequestedLocation =
                                      responseData.Attribute("dist") == null
                                          ? 0.0
                                          : Double.Parse(responseData.Attribute("dist").Value)
                              };
            foreach (XElement report in responseData.Elements("report"))
            {
                if (report.Attribute("epoch") == null ||
                    String.IsNullOrEmpty(report.Attribute("epoch").Value))
                    throw new NWXServerException("No epoch data returned for report.");
                newData.Reports.Add(new METARReport
                                        {
                                            METAR = report.Value,
                                            Time = DateTime.Parse(report.Attribute("epoch").Value)
                                        });
            }
            responseObject.Responses.Add(
                icaoProvided ? responseData.Attribute("icao").Value : responseData.Attribute("p").Value, newData);
            //  }
        }

        #endregion
    }
}