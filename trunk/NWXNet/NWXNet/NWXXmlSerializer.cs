using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using NWXNet.Exceptions;
using System.Drawing;

namespace NWXNet
{
    public class NWXXmlSerializer : INWXSerializer
    {

        // TODO: Possibly remove this depending on whether we can ensure proper entries intrinsically
        private static IEnumerable<string> Validate(Request request)
        {
            foreach (var requestData in request.Data.Where(requestData => !requestData.IsValid))
            {
                yield return string.Concat("Request of type ", requestData.Type, " with id: ", requestData.Id, " is missing data.", Environment.NewLine);
            }
        }

        #region Implementation of INWXSerializer

        public string Serialize(Request request)
        {
            var validationErrors = Validate(request);
            if (validationErrors.Any())
                throw new NWXClientException(string.Concat(validationErrors));
            var doc = new XElement("nwx",
                                   new XAttribute("version", request.Version.ToString()),
                                   request.Authenticator != null ? request.Authenticator.GenerateXml() : null,
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
            if (errors.Any())
            {
                List<ServerError> errorData =
                    errors.Select(
                        element => new ServerError(element.Attribute("code").Value, element.Value)).ToList();
                throw new NWXServerException(errorData);
            }

            IEnumerable<XElement> warnings = doc.Descendants("warning");
            if (warnings.Any())
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

                        case "AVAILABLEGEOMAGMODELS":
                            GenerateAvailableGeoMagModelsResponseData(responseData, responseObject);
                            break;

                        case "WIND":
                            GenerateWindResponseData(responseData, responseObject);
                            break;

                        case "CHART":
                            GenerateChartResponseData(responseData, responseObject);
                            break;

                        case "TAF":
                            GenerateTAFResponseData(responseData, responseObject);
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

                case RequestTypes.AvailableGeoMagModels:
                    return new XElement("AvailableGeomagModels");

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

                case RequestTypes.Chart:
                    var chartData = data as Chart;
                    if (chartData != null)
                    {
                        var id = chartData.Id != null ? new XAttribute("id", chartData.Id) : null;
                        var width = new XAttribute("x", chartData.Width);
                        var height = new XAttribute("y", chartData.Height);
                        var level = new XAttribute("z", chartData.Level);
                        var epoch = new XAttribute("e", chartData.Epoch.ToNWXString());
                        var lat0 = new XAttribute("lat0", chartData.LowerLeftCoordinates.Latitude);
                        var lat1 = new XAttribute("lat1", chartData.UpperRightCoordinates.Latitude);
                        var lon0 = new XAttribute("lon0", chartData.LowerLeftCoordinates.Longitude);
                        var lon1 = new XAttribute("lon1", chartData.UpperRightCoordinates.Longitude);
                        var transparentBg = chartData.TransparentBackground
                                                ? new XAttribute("background", "transparent")
                                                : null;
                        var layout = chartData.MapLayout ? new XAttribute("layout", "map") : null;
                        return new XElement("Chart",
                                            id,
                                            width,
                                            height,
                                            level,
                                            epoch,
                                            lat0,
                                            lat1,
                                            lon0,
                                            lon1,
                                            transparentBg,
                                            layout,
                                            from feature in chartData.Features 
                                            select new XElement("theme", feature));
                    }
                    break;

                case RequestTypes.TAF:
                    var TAFData = data as TAF;
                    if (TAFData != null)
                    {
                        XAttribute id = TAFData.Icao == null
                                            ? new XAttribute("p", TAFData.Coordinates.ToString("p"))
                                            : new XAttribute("icao", TAFData.Icao);
                        XAttribute count = TAFData.Count == 0 ? null : new XAttribute("count", TAFData.Count);
                        XAttribute maxAge = TAFData.MaxAge == 0 ? null : new XAttribute("maxage", TAFData.MaxAge);
                        var element = new XElement("Taf",
                                                   id,
                                                   count,
                                                   maxAge);
                        return element;
                    }
                    break;
            }
            return null;
        }

        private static void GenerateChartResponseData(XElement responseData, Response responseObject)
        {
            XElement image = responseData.Element("data");
            if (image == null || string.IsNullOrEmpty(image.Value))
                throw new NWXServerException("No image returned from server for chart request.");

            byte[] imageBytes = Convert.FromBase64String(image.Value);
            var ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            ms.Write(imageBytes, 0, imageBytes.Length);
            Image img = Image.FromStream(ms, true);

            XAttribute id = responseData.Attribute("id");
            bool idSet = (id != null && !string.IsNullOrEmpty(id.Value));

            XElement expiry = responseData.Element("expires");
            if (expiry == null || string.IsNullOrEmpty(expiry.Value))
                throw new NWXServerException("No expiry time returned from server for chart request.");

            XAttribute width = responseData.Attribute("x");
            if (width == null || string.IsNullOrEmpty(width.Value))
                throw new NWXServerException("No width returned from server for chart request.");

            XAttribute height = responseData.Attribute("y");
            if (height == null || string.IsNullOrEmpty(height.Value))
                throw new NWXServerException("No height returned from server for chart request.");

            XAttribute level = responseData.Attribute("z");
            if (level == null || string.IsNullOrEmpty(level.Value))
                throw new NWXServerException("No level returned from server for chart request.");

            XAttribute epoch = responseData.Attribute("e");
            if (epoch == null || string.IsNullOrEmpty(epoch.Value))
                throw new NWXServerException("No epoch returned from server for chart request.");

            XAttribute lat0 = responseData.Attribute("lat0");
            if (lat0 == null || string.IsNullOrEmpty(lat0.Value))
                throw new NWXServerException("No lower-left latitude returned from server for chart request.");

            XAttribute lat1 = responseData.Attribute("lat1");
            if (lat1 == null || string.IsNullOrEmpty(lat1.Value))
                throw new NWXServerException("No upper-right latitude returned from server for chart request.");

            XAttribute lon0 = responseData.Attribute("lon0");
            if (lon0 == null || string.IsNullOrEmpty(lon0.Value))
                throw new NWXServerException("No lower-left longitude returned from server for chart request.");

            XAttribute lon1 = responseData.Attribute("lon1");
            if (lon1 == null || string.IsNullOrEmpty(lon1.Value))
                throw new NWXServerException("No upper-right longitude returned from server for chart request.");

            var newData = new ChartResponse
                              {
                                  Id = idSet ? id.Value : lat0 + "," + lon0 + " - " + lat1 + "," + lon1,
                                  Expires = DateTime.Parse(expiry.Value),
                                  Width = Int32.Parse(width.Value),
                                  Height = Int32.Parse(height.Value),
                                  Level = Int32.Parse(level.Value),
                                  Epoch = DateTime.Parse(epoch.Value),
                                  LowerLeftCoordinates = new LatLon(lat0.Value + "," + lon0.Value),
                                  UpperRightCoordinates = new LatLon(lat1.Value + "," + lon1.Value),
                                  Chart = img
                              };

            responseObject.Responses.Add(newData.Id, newData);
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
                                  Id = idSet ? id.Value : coords.Value
                              };

            responseObject.Responses.Add(newData.Id, newData);
        }

        private static void GenerateAvailableLevelsResponseData(XElement responseData, Response responseObject)
        {
            IEnumerable<XElement> levels = responseData.Elements("level");
            if (levels == null || !levels.Any())
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
            if (epochs == null || !epochs.Any())
                throw new NWXServerException("No available epochs returned from server.");

            var newData = new AvailableEpochsResponse();

            foreach (XElement epoch in epochs)
            {
                newData.Epochs.Add(DateTime.Parse(epoch.Value));
            }

            responseObject.Responses.Add("AvailableEpochs", newData);
        }

        private static void GenerateAvailableGeoMagModelsResponseData(XElement responseData, Response responseObject)
        {
            var models = responseData.Elements("GeomagModel");
            if (models == null || !models.Any())
                throw new NWXServerException("No available geomag models returned from server.");

            var newData = new AvailableGeoMagModelsResponse();

            foreach (var model in models)
            {
                XElement name = model.Element("name");
                if (name == null || string.IsNullOrEmpty(name.Value))
                    throw new NWXServerException("No name returned from server for geomag models request.");

                var years = model.Elements("epoch").Where(x => x.Attribute("type") != null);

                XElement baseYear = years.Where(x => x.Attribute("type").Value == "base").First();
                if (baseYear == null || string.IsNullOrEmpty(baseYear.Value))
                    throw new NWXServerException("No base grid year returned from server for geomag models request.");

                XElement firstYear = years.Where(x => x.Attribute("type").Value == "first").First();
                if (firstYear == null || string.IsNullOrEmpty(firstYear.Value))
                    throw new NWXServerException("No first valid year returned from server for geomag models request.");

                XElement lastYear = years.Where(x => x.Attribute("type").Value == "last").First();
                if (lastYear == null || string.IsNullOrEmpty(lastYear.Value))
                    throw new NWXServerException("No last valid year returned from server for geomag models request.");

                var resolutions =
                    model.Elements("resolution").Where(x => (x.Attribute("type") != null) && (x.Attribute("u") != null));

                XElement latRes = resolutions.Where(x => x.Attribute("type").Value == "lat").First();
                if (latRes == null || string.IsNullOrEmpty(latRes.Value))
                    throw new NWXServerException(
                        "No latitude resolution returned from server for geomag models request.");

                XElement lonRes = resolutions.Where(x => x.Attribute("type").Value == "lon").First();
                if (lonRes == null || string.IsNullOrEmpty(lonRes.Value))
                    throw new NWXServerException(
                        "No longitude resolution returned from server for geomag models request.");

                newData.Models.Add(new GeoMagModel
                                       {
                                           Name = name.Value,
                                           BaseGridYear = Int32.Parse(baseYear.Value),
                                           FirstValidYear = Int32.Parse(firstYear.Value),
                                           LastValidYear = Int32.Parse(lastYear.Value),
                                           LatitudeResolution = Double.Parse(latRes.Value),
                                           LongitudeResolution = Double.Parse(lonRes.Value)
                                       });
            }

            responseObject.Responses.Add("AvailableGeoMagModels", newData);
        }

        private static void GenerateTAFResponseData(XElement responseData, Response responseObject)
        {
            bool icaoProvided = responseData.Attribute("icao") != null &&
                                !String.IsNullOrEmpty(responseData.Attribute("icao").Value);
            bool latlonProvided = responseData.Attribute("p") != null &&
                                  !String.IsNullOrEmpty(responseData.Attribute("p").Value);
            if (!icaoProvided && !latlonProvided)
                throw new NWXServerException("TAF identifier not returned from server.");
            if ((!responseData.Elements("report").Any() ||
                 responseData.Elements("report") == null) && responseObject.Warnings.Count == 0)
                throw new NWXServerException("No report data returned from server.");

            var newData = new TAFResponse
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
                newData.Reports.Add(new TAFReport
                {
                    TAF = report.Value,
                    Time = DateTime.Parse(report.Attribute("epoch").Value)
                });
            }
            responseObject.Responses.Add(
                icaoProvided ? responseData.Attribute("icao").Value : responseData.Attribute("p").Value, newData);
            //  }
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
            if ((!responseData.Elements("report").Any() ||
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