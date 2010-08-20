using System;
using System.Linq;
using NUnit.Framework;
using NWXNet.Exceptions;
using Moq;

namespace NWXNet.UnitTests
{
    [TestFixture]
    class Tests
    {
        [Test]
        public void CreateRequest_ProvideVersion_VersionNumberSet()
        {
            var request = NWX.Request.UsingVersion("0.3.2");
            Assert.That(request.Version.ToString(), Is.EqualTo("0.3.2"), "Version number not set.");
            //var request = NWX.Request("0.3.5").For(Metar.ForICAO("CYYC").WithID("CYYC"), Winds.AtLatLon("34,56").ForFlightLevel(100).WithID("winds1"));
            //var response = request.Send();
            //int dir = response.Winds.ForFlightLevel(100).Direction;
        }

        [Test]
        public void CreateRequest_NoVersionProvided_SetsDefaultVersion()
        {
            var request = NWX.Request;
            Assert.That(request.Version.ToString(), Is.EqualTo("0.3.5"), "Version number not set to default.");
        }

        [Test]
        public void CreateRequest_InvalidVersionProvided_ThrowsArgumentException()
        {
            Assert.That(() => NWX.Request.UsingVersion("bad version"), Throws.ArgumentException);
        }

        [Test]
        public void RequestFor_RequestDataProvided_PopulatesDataField()
        {
            var request = NWX.Request.For(METAR.ForICAO("CYYC"));
            Assert.That(request.Data, Is.Not.Null);
            Assert.That(request.Data.Count, Is.EqualTo(1));
        }

        [Test]
        public void Send_DataProvided_CallsSerializerAndCommunicationHandler()
        {
            var mockSerializer = new Mock<INWXSerializer>();
            var mockCommunicationHandler = new Mock<INWXCommunicationHandler>();
           // mockCommunicationHandler.Setup(x => x.Send(It.IsAny<string>())).Returns("");

            NWX.Serializer = mockSerializer.Object;
            NWX.CommunicationHandler = mockCommunicationHandler.Object;
            NWX.Request.For(METAR.ForICAO("CYYC")).Send();
            mockSerializer.Verify(x => x.Serialize(It.IsAny<Request>()), Times.Once(), "Did not call serializer.");
            mockCommunicationHandler.Verify(x => x.Send(It.IsAny<string>()), Times.Once(), "Did not call communication handler.");

            NWX.Serializer = new NWXXmlSerializer();
            NWX.CommunicationHandler = new HttpCommunicationHandler();
            //Assert.AreEqual("Called", NWX.Request.For(METAR.ForICAO("CYYC")).Send());


        }

        [Test]
        public void Send_NoDataProvided_ThrowsException()
        {
            Assert.That(() => NWX.Request.Send(), Throws.InstanceOf<NWXClientException>());
        }

       // [Test]
       // public void WithId_IDE

        [Test]
        public void Test()
        {
           // Console.WriteLine(NWX.Request.For("moo", METAR.ForICAO("CYYC")).For("cow", METAR.ForICAO("KEWR")).Send());
            //Console.WriteLine(NWX.Request.For(METAR.ForICAO("CYYC")).Send().All<METARResponseData>().First());
            //Console.WriteLine(NWX.Request.For(METAR.ForCoordinates("40.446195,-79.948862")).Send().All<METARResponse>().First());
            var response = NWX.Request.For(METAR.ForICAO("CYADE").Last(5)).Send();
            if(!response.HasWarnings)
            {
                Console.WriteLine(response.All<METARResponse>().First());
            }
        }
    }
}
