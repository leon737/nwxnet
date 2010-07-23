using System;
using System.IO;
using System.Xml.Linq;
using NUnit.Framework;
using NWXNet.Exceptions;

namespace NWXNet.UnitTests
{
    [TestFixture]
    internal class XmlSerializerTests
    {
        private static readonly NWXXmlSerializer Serializer = new NWXXmlSerializer();

        private static void TestResponse(Response response)
        {
            Assert.That(response, Is.Not.Null, "Response was null.");
            Assert.That(response.Version, Is.Not.Null, "Response version was null.");
            Assert.That(response.Version.ToString(), Is.EqualTo("0.3.5"), "Response version was not properly set.");
            Assert.That(response.Expires, Is.Not.Null);
            Assert.That(response.Expires, Is.EqualTo(DateTime.Parse("2010-07-19 18:20:00")));
        }

        [Test]
        public void Deserialize_METARDataWithP_GeneratesProperObject()
        {
            string xml =
                "<nwx version=\"0.3.5\"><Response id=\"NWXNet1.0.0\" expires=\"2010-07-19 18:20:00\"><Metar p=\"51.117,-114.017\" icao=\"CYYC\" dist=\"4.35\"><report epoch=\"2010-07-12 16:00:00\"><![CDATA[CYYC 121600Z 00000KT 40SM FEW032 FEW170 BKN220 21/13 A2941 RMK CU1AC2CI3 SLP946]]></report></Metar></Response></nwx>";

            Response response = Serializer.Deserialize(xml);
            TestResponse(response);

            METARResponse[] responseData = response.All<METARResponse>();

            Assert.That(responseData, Is.Not.Null, "Dataset for response was null.");
            Assert.That(responseData, Is.InstanceOf<METARResponse[]>(),
                            "METARResponseData array was not returned (wrong type).");
            var data = responseData[0];
            Assert.That(data, Is.Not.Null, "Empty dataset was returned.");
            Assert.That(data.ICAO, Is.EqualTo("CYYC"), "ICAO was not properly set.");
            Assert.That(data.Location.ToString("p"), Is.EqualTo("51.117,-114.017"), "Location was not properly set.");
            Assert.That(data.DistanceFromRequestedLocation, Is.EqualTo(4.35), "Distance from requested location was not properly set.");
            Assert.That(data.Reports[0].METAR,
                            Is.EqualTo("CYYC 121600Z 00000KT 40SM FEW032 FEW170 BKN220 21/13 A2941 RMK CU1AC2CI3 SLP946"),
                            "METAR report was not properly sent.");
            Assert.That(data.Reports[0].Time, Is.EqualTo(DateTime.Parse("2010-07-12 16:00:00")),
                            "Timestamp was not properly set.");
            Assert.That(data["2010-07-12 16:00:00"], Is.Not.Null,
                             "Indexer did not properly retrieve reports by epoch.");

            var withId = response.WithId<METARResponse>("CYYC");
            Assert.That(withId, Is.Not.Null, "Response not properly indexed by ICAO.");
            Assert.That(withId.ICAO, Is.EqualTo("CYYC"), "Response indexed to wrong ICAO.");
        }

        [Test]
        public void Deserialize_AvailableEpochsData_GeneratesProperObject()
        {
            string xml =
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<nwx version=\"0.3.5\"><Response id=\"NWXNet1.0.0\" expires=\"2010-07-19 18:20:00\"><AvailableEpochs><epoch>2010-07-19 12:00:00</epoch><epoch>2010-07-19 15:00:00</epoch></AvailableEpochs></Response></nwx>";

            Response response = Serializer.Deserialize(xml);
            TestResponse(response);

            var responseData = response.All<AvailableEpochsResponse>();

            Assert.That(responseData, Is.Not.Null, "Dataset for response was null");
            Assert.That(responseData, Is.InstanceOf<AvailableEpochsResponse[]>(), "AvailabEpochsData array was not returned (wrong type).");

            var data = responseData[0];
            Assert.That(data, Is.Not.Null, "Empty dataset was returned.");
            Assert.That(data.Epochs[0], Is.EqualTo(DateTime.Parse("2010-07-19 12:00:00")), "Epoch was not properly set.");

            var epochs = response.Epochs;
            Assert.That(epochs, Is.Not.Null, "Response not properly indexed to AvailableEpochs.");
            Assert.That(epochs.Epochs.Count, Is.GreaterThan(0), "Response indexed to wrong AvailableEpochs data.");

        }

        [Test]
        public void Deserialize_AvailableLevelsData_GeneratesProperObject()
        {
            string xml =
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<nwx version=\"0.3.5\"><Response id=\"NWXNet1.0.0\" expires=\"2010-07-19 18:20:00\"><AvailableLevels><level u=\"hPa\">200</level><level u=\"hPa\">300</level></AvailableLevels></Response></nwx>";
            
            Response response = Serializer.Deserialize(xml);
            TestResponse(response);

            var responseData = response.All<AvailableLevelsResponse>();

            Assert.That(responseData, Is.Not.Null, "Dataset for response was null");
            Assert.That(responseData, Is.InstanceOf<AvailableLevelsResponse[]>(), "AvailableEpochs array was not returned (wrong type).");

            var data = responseData[0];
            Assert.That(data, Is.Not.Null, "Empty dataset was returned.");
            Assert.That(data.Levels[0], Is.EqualTo(200), "Level was not properly set.");

            var levels = response.Levels;
            Assert.That(levels, Is.Not.Null, "Response not properly indexed to AvailableLevels.");
            Assert.That(levels.Levels.Count, Is.GreaterThan(0), "Response indexed to wrong AvailableLevels data.");
        }

        [Test]
        public void Deserialize_AvailableGeoMagModelsData_GeneratesProperObject()
        {
            string xml =
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<nwx version=\"0.3.5\"><Response expires=\"2010-07-19 18:20:00\"><AvailableGeomagModels><GeomagModel><name>WMM2010</name><epoch type=\"base\">2010</epoch><epoch type=\"first\">2010</epoch><epoch type=\"last\">2015</epoch><resolution type=\"lat\" u=\"deg\">0.5</resolution><resolution type=\"lon\" u=\"deg\">0.5</resolution></GeomagModel></AvailableGeomagModels></Response></nwx>";

            Response response = Serializer.Deserialize(xml);
            TestResponse(response);

            var responseData = response.All<AvailableGeoMagModelsResponse>();

            Assert.That(responseData, Is.Not.Null, "Dataset for response was null");
            Assert.That(responseData, Is.InstanceOf<AvailableGeoMagModelsResponse[]>(), "AvailableEpochs array was not returned (wrong type).");

            var data = responseData[0];
            Assert.That(data, Is.Not.Null, "Empty dataset was returned.");

            var model = data.Models[0];
            Assert.That(model, Is.Not.Null, "No geomag models were added.");
            Assert.That(model.Name, Is.EqualTo("WMM2010"), "Model name not properly set.");
            Assert.That(model.BaseGridYear, Is.EqualTo(2010), "Base grid year not properly set.");
            Assert.That(model.FirstValidYear, Is.EqualTo(2010), "First valid year not properly set.");
            Assert.That(model.LastValidYear, Is.EqualTo(2015), "Last valid year not properly set.");
            Assert.That(model.LatitudeResolution, Is.EqualTo(0.5), "Latitude resolution not properly set.");
            Assert.That(model.LongitudeResolution, Is.EqualTo(0.5), "Longitude resolution not properly set.");
        }

        [Test]
        public void Deserialize_WindData_GeneratesProperObject()
        {
            string xml =
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<nwx version=\"0.3.5\"><Response id=\"NWXNet1.0.0\" expires=\"2010-07-19 18:20:00\"><Wind id=\"id1\" p=\"51,114\" z=\"100\" u=\"F\" e=\"2010-07-23 21:00:00\"><dir>254</dir><speed>14</speed></Wind></Response></nwx>";

            Response response = Serializer.Deserialize(xml);
            TestResponse(response);

            var responseData = response.All<WindResponse>();

            Assert.That(responseData, Is.Not.Null, "Dataset for response was null");
            Assert.That(responseData, Is.InstanceOf<WindResponse[]>(), "Wind array was not returned (wrong type).");

            var data = responseData[0];
            Assert.That(data, Is.Not.Null, "Empty dataset was returned.");
            Assert.That(data.Direction, Is.EqualTo(254.0), "Direction was not properly set.");
            Assert.That(data.Speed, Is.EqualTo(14.0), "Speed was not properly set.");
            Assert.That(data.Coordinates.ToString("p"), Is.EqualTo("51,114"), "Coordinates were not properly set.");
            Assert.That(data.Altitude, Is.EqualTo(100), "Altitude was not properly set.");
            Assert.That(data.Unit, Is.EqualTo(AltitudeUnit.ImperialFlightLevel), "Altitude unit was not properly set.");
            Assert.That(data.Id, Is.EqualTo("id1"), "Id was not properly set.");
            Assert.That(data.Epoch, Is.Not.Null, "Epoch was not properly set.");
            Assert.That(data.Epoch, Is.EqualTo(DateTime.Parse("2010-07-23 21:00:00")));

            var winds = response.WithId<WindResponse>("id1");
            Assert.That(winds, Is.Not.Null, "Response not properly indexed to winds id.");
            Assert.That(winds.Coordinates.ToString("p"), Is.EqualTo("51,114"), "Response indexed to wrong winds id.");

        }

        [Test]
        public void Deserialize_MetarIdNotProvided_ThrowsException()
        {
            string xml =
                "<nwx version=\"0.3.5\"><Response id=\"NWXNet1.0.0\" expires=\"2010-07-19 18:20:00\"><Metar><report epoch=\"2010-07-12 16:00:00\"><![CDATA[CYYC 121600Z 00000KT 40SM FEW032 FEW170 BKN220 21/13 A2941 RMK CU1AC2CI3 SLP946]]></report></Metar></Response></nwx>";
            Assert.That(() => Serializer.Deserialize(xml),
                        Throws.InstanceOf<NWXServerException>().With.Message.EqualTo("METAR identifier not returned from server."),
                        "Expected to fail when LatLon or ICAO identifier not passed in.");
        }

        [Test]
        public void Deserialize_NoData_GeneratesProperXml()
        {
            string xml = "<nwx version=\"0.3.5\"><Response id=\"NWXNet1.0.0\" expires=\"2010-07-19 18:20:00\"/></nwx>";
            Response response = Serializer.Deserialize(xml);
            TestResponse(response);
            Assert.That(response.All<METARResponse>(), Is.Null, "Dataset for response was not blank.");
        }

        [Test]
        public void Deserialize_VersionNotProper_ThrowsException()
        {
            string xml = "<nwx version=\"0.3,.5ddddasd\"><Response/></nwx>";
            Assert.That(() => Serializer.Deserialize(xml),
                        Throws.InstanceOf<NWXServerException>().With.Message.EqualTo(
                            "Bad version returned from server."), "Expected to fail when bad version passed in.");
        }

        [Test]
        public void Deserialize_VersionNull_ThrowsException()
        {
            string xml = "<nwx><Response/></nwx>";
            Assert.That(() => Serializer.Deserialize(xml),
                        Throws.InstanceOf<NWXServerException>().With.Message.EqualTo(
                            "Bad version returned from server."),
                        "Expected to fail when no version number passed in.");
        }

        [Test]
        public void Deserialize_ServerError_ThrowsException()
        {
            string xml =
                "<nwx version=\"0.3.5\"><Response id=\"NWXNet1.0.0\"><error code=\"7\">Required attribute missing</error></Response></nwx>";
            Assert.That(() => Serializer.Deserialize(xml),
                        Throws.InstanceOf<NWXServerException>().With.Property("Errors").EqualTo(new[] { new ServerError("7", "Required attribute missing") }).AsCollection);
        }

        [Test]
        public void Deserialize_ServerWarning_ResponseContainsWarning()
        {
            string xml =
                "<nwx version=\"0.3.5\"><Response id=\"NWXNet1.0.0\"><Metar icao=\"KSFOd\"><warning code=\"11\">KSFOd: No data found</warning></Metar></Response></nwx>";
            var response = Serializer.Deserialize(xml);
            Assert.That(response.Warnings, Is.Not.Null);
            Assert.That(response.Warnings.Count, Is.EqualTo(1));
            Assert.That(response.Warnings[0], Is.EqualTo("KSFOd: No data found"));

        }

        [Test]
        public void Serialize_METARDataWithICAO_GeneratesProperXml()
        {
            Request request = NWX.Request.For(METAR.ForICAO("CYYC").Last(5));
            string xml = Serializer.Serialize(request);
            XElement nwx = XElement.Load(new StringReader(xml));

            Assert.That(nwx, Is.Not.Null);
            Assert.That(nwx.Attribute("version"), Is.Not.Null);
            Assert.That(nwx.Attribute("version").Value, Is.EqualTo("0.3.5"));

            XElement req = nwx.Element("Request");
            Assert.That(req, Is.Not.Null);

            XElement metar = req.Element("Metar");
            Assert.That(metar, Is.Not.Null);
            Assert.That(metar.Attribute("icao"), Is.Not.Null);
            Assert.That(metar.Attribute("icao").Value, Is.EqualTo("CYYC"));
            Assert.That(metar.Attribute("count"), Is.Not.Null);
            Assert.That(metar.Attribute("count").Value, Is.EqualTo("5"));
        }

        [Test]
        public void Serialize_METARDataWithP_GeneratesProperXml()
        {
            Request request = NWX.Request.For(METAR.ForCoordinates("-75,120.2").WithMaxAge(2));
            string xml = Serializer.Serialize(request);
            XElement nwx = XElement.Load(new StringReader(xml));

            Assert.That(nwx, Is.Not.Null);
            Assert.That(nwx.Attribute("version"), Is.Not.Null);
            Assert.That(nwx.Attribute("version").Value, Is.EqualTo("0.3.5"));

            XElement req = nwx.Element("Request");
            Assert.That(req, Is.Not.Null);

            XElement metar = req.Element("Metar");
            Assert.That(metar, Is.Not.Null);
            Assert.That(metar.Attribute("icao"), Is.Null);
            Assert.That(metar.Attribute("p"), Is.Not.Null);
            Assert.That(metar.Attribute("p").Value, Is.EqualTo("-75,120.2"));

            Assert.That(metar.Attribute("maxage"), Is.Not.Null);
            Assert.That(metar.Attribute("maxage").Value, Is.EqualTo("2"));
        }

        [Test]
        public void Serialize_AvailableEpochsData_GeneratesProperXml()
        {
            Request request = NWX.Request.For(Available.Epochs);
            string xml = Serializer.Serialize(request);
            XElement nwx = XElement.Load(new StringReader(xml));

            Assert.That(nwx, Is.Not.Null);
            Assert.That(nwx.Attribute("version"), Is.Not.Null);
            Assert.That(nwx.Attribute("version").Value, Is.EqualTo("0.3.5"));

            XElement req = nwx.Element("Request");
            Assert.That(req, Is.Not.Null);

            XElement epochs = req.Element("AvailableEpochs");
            Assert.That(epochs, Is.Not.Null);
            Assert.That(epochs.Value, Is.Null.Or.Empty);
        }

        [Test]
        public void Serialize_AvailableLevelsData_GeneratesProperXml()
        {
            Request request = NWX.Request.For(Available.Levels);
            string xml = Serializer.Serialize(request);
            XElement nwx = XElement.Load(new StringReader(xml));

            Assert.That(nwx, Is.Not.Null);
            Assert.That(nwx.Attribute("version"), Is.Not.Null);
            Assert.That(nwx.Attribute("version").Value, Is.EqualTo("0.3.5"));

            XElement req = nwx.Element("Request");
            Assert.That(req, Is.Not.Null);

            XElement levels = req.Element("AvailableLevels");
            Assert.That(levels, Is.Not.Null);
            Assert.That(levels.Value, Is.Null.Or.Empty);
        }

        [Test]
        public void Serialize_AvailableGeoMagModelsData_GeneratesProperXml()
        {
            Request request = NWX.Request.For(Available.GeoMagModels);
            string xml = Serializer.Serialize(request);
            XElement nwx = XElement.Load(new StringReader(xml));

            Assert.That(nwx, Is.Not.Null);
            Assert.That(nwx.Attribute("version"), Is.Not.Null);
            Assert.That(nwx.Attribute("version").Value, Is.EqualTo("0.3.5"));

            XElement req = nwx.Element("Request");
            Assert.That(req, Is.Not.Null);

            XElement geomag = req.Element("AvailableGeomagModels");
            Assert.That(geomag, Is.Not.Null);
            Assert.That(geomag.Value, Is.Null.Or.Empty);
        }

        [Test]
        public void Serialize_WindData_GeneratesProperXml()
        {
            Request request = NWX.Request.For(Wind.For("-50,65", 240, AltitudeUnit.ImperialFlightLevel, DateTime.Parse("2010-07-12 16:00:00")).WithId("testid"));
            string xml = Serializer.Serialize(request);
            XElement nwx = XElement.Load(new StringReader(xml));

            Assert.That(nwx, Is.Not.Null);
            Assert.That(nwx.Attribute("version"), Is.Not.Null);
            Assert.That(nwx.Attribute("version").Value, Is.EqualTo("0.3.5"));

            XElement req = nwx.Element("Request");
            Assert.That(req, Is.Not.Null);

            XElement wind = req.Element("Wind");
            Assert.That(wind, Is.Not.Null);
            Assert.That(wind.Value, Is.Null.Or.Empty);

            Assert.That(wind.Attribute("p"), Is.Not.Null);
            Assert.That(wind.Attribute("p").Value, Is.EqualTo("-50,65"));

            Assert.That(wind.Attribute("z"), Is.Not.Null);
            Assert.That(wind.Attribute("z").Value, Is.EqualTo("240"));

            Assert.That(wind.Attribute("u"), Is.Not.Null);
            Assert.That(wind.Attribute("u").Value, Is.EqualTo("F"));

            Assert.That(wind.Attribute("e"), Is.Not.Null);
            Assert.That(wind.Attribute("e").Value, Is.EqualTo(DateTime.Parse("2010-07-12 16:00:00").ToNWXString()));

            Assert.That(wind.Attribute("id"), Is.Not.Null);
            Assert.That(wind.Attribute("id").Value, Is.EqualTo("testid"));
        }

        [Test]
        public void Serialize_ChartData_GeneratesProperXml()
        {
            Request request =
                NWX.Request.For(Chart.For("30,-20", "65,35", 700, DateTime.Parse("2010-07-22 21:00:00"), 800, 600));

            string xml = Serializer.Serialize(request);
            XElement nwx = XElement.Load(new StringReader(xml));

            Assert.That(nwx, Is.Not.Null);
            Assert.That(nwx.Attribute("version"), Is.Not.Null);
            Assert.That(nwx.Attribute("version").Value, Is.EqualTo("0.3.5"));

            XElement req = nwx.Element("Request");
            Assert.That(req, Is.Not.Null);

            XElement chart = req.Element("Chart");
            Assert.That(chart, Is.Not.Null);
            Assert.That(chart.Value, Is.Null.Or.Empty);

            Assert.That(chart.Attribute("x"), Is.Not.Null);
            Assert.That(chart.Attribute("x").Value, Is.EqualTo("800"));

            Assert.That(chart.Attribute("y"), Is.Not.Null);
            Assert.That(chart.Attribute("y").Value, Is.EqualTo("600"));

            Assert.That(chart.Attribute("z"), Is.Not.Null);
            Assert.That(chart.Attribute("z").Value, Is.EqualTo("700"));

            Assert.That(chart.Attribute("e"), Is.Not.Null);
            Assert.That(chart.Attribute("e").Value, Is.EqualTo(DateTime.Parse("2010-07-22 21:00:00").ToNWXString()));

            Assert.That(chart.Attribute("lat0"), Is.Not.Null);
            Assert.That(chart.Attribute("lat0").Value, Is.EqualTo("30"));

            Assert.That(chart.Attribute("lat1"), Is.Not.Null);
            Assert.That(chart.Attribute("lat1").Value, Is.EqualTo("65"));

            Assert.That(chart.Attribute("lon0"), Is.Not.Null);
            Assert.That(chart.Attribute("lon0").Value, Is.EqualTo("-20"));

            Assert.That(chart.Attribute("lon1"), Is.Not.Null);
            Assert.That(chart.Attribute("lon1").Value, Is.EqualTo("35"));
        }

        [Test]
        public void Serialize_ApplicationAuthentication_GeneratesProperXml()
        {
            NWX.Authenticator = new ApplicationAuthenticator("nwxnet", 0, "2ea4d9f6f80bae06a098c47c39c28b69ae4e3d12");
            var request = NWX.Request.For(METAR.ForICAO("CYYC"));

            string xml = Serializer.Serialize(request);
            XElement nwx = XElement.Load(new StringReader(xml));

            Assert.That(nwx, Is.Not.Null);
            Assert.That(nwx.Attribute("version"), Is.Not.Null);
            Assert.That(nwx.Attribute("version").Value, Is.EqualTo("0.3.5"));

            XElement auth = nwx.Element("application");

            Assert.That(auth, Is.Not.Null);
            Assert.That(auth.Attribute("name"), Is.Not.Null);
            Assert.That(auth.Attribute("name").Value, Is.EqualTo("nwxnet"));
            Assert.That(auth.Attribute("instance"), Is.Not.Null);
            Assert.That(auth.Attribute("instance").Value, Is.EqualTo("0"));
            Assert.That(auth.Attribute("token"), Is.Not.Null); // TODO: Is there any way to unit test the value?
            Assert.That(auth.Attribute("timestamp"), Is.Not.Null);

            NWX.Authenticator = null;
        }

        [Test]
        public void Serialize_ApplicationAuthenticationThroughRequest_GeneratesProperXml()
        {
            var request =
                NWX.Request.For(METAR.ForICAO("CYYC")).UsingAuthenticator(new ApplicationAuthenticator("nwxnet", 0,
                                                                                                       "2ea4d9f6f80bae06a098c47c39c28b69ae4e3d12"));

            string xml = Serializer.Serialize(request);
            XElement nwx = XElement.Load(new StringReader(xml));

            Assert.That(nwx, Is.Not.Null);
            Assert.That(nwx.Attribute("version"), Is.Not.Null);
            Assert.That(nwx.Attribute("version").Value, Is.EqualTo("0.3.5"));

            XElement auth = nwx.Element("application");

            Assert.That(auth, Is.Not.Null);
            Assert.That(auth.Attribute("name"), Is.Not.Null);
            Assert.That(auth.Attribute("name").Value, Is.EqualTo("nwxnet"));
            Assert.That(auth.Attribute("instance"), Is.Not.Null);
            Assert.That(auth.Attribute("instance").Value, Is.EqualTo("0"));
            Assert.That(auth.Attribute("token"), Is.Not.Null); // TODO: Is there any way to unit test the value?
            Assert.That(auth.Attribute("timestamp"), Is.Not.Null);
        }

        [Test]
        public void Serialize_NoData_GeneratesProperXml()
        {
            Request request = NWX.Request;
            string xml = Serializer.Serialize(request);
            XElement nwx = XElement.Load(new StringReader(xml));

            Assert.That(nwx, Is.Not.Null);
            Assert.That(nwx.Attribute("version"), Is.Not.Null);
            Assert.That(nwx.Attribute("version").Value, Is.EqualTo("0.3.5"));
        }
    }
}