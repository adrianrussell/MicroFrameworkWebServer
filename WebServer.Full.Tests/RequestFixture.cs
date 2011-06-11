using System;
using System.Net;
using NUnit.Framework;
using Rhino.Mocks;
using Server.Network;

namespace Server.Full.Tests
{
    [TestFixture]
    public class RequestFixture
    {
        IClientSocket _socket;

        [SetUp]
        public void Setup() {
            _socket = MockRepository.GenerateMock<IClientSocket>();
        }

        [Test]
        public void CanCreate() {
            Request request = GetRequest(new char[2]);
            Assert.That(request, Is.Not.Null);
        }

        private Request GetRequest(char[] data) {
            return new Request(_socket,data);
        }

        //[MethodName_StateUnderTest_ExpectedBehavior]
        [Test]
        public void Client_ReturnsIPAddress_IPAddressIsValid() {

            _socket.Expect(x => x.RemoteEndPoint).Return(new IPEndPoint(new IPAddress(123456), 80));

            Request request = GetRequest(new char[2]);

            Assert.That(request.Client, Is.Not.Null);
            Assert.That(request.Client, Is.EqualTo(new IPAddress(123456)));
            
            _socket.VerifyAllExpectations();
        }

        [Test]
        public void ProcessRequest_ValidHeader_SetsMethod() {
            Request request = GetRequest((@"GET /path/ HTTP / 1.1" + Environment.NewLine).ToCharArray());

            request.ProcessRequestHeader();

            Assert.That(request.Method, Is.EqualTo("GET"));
        }

        [Test]
        public void ProcessRequest_ValidHeader_SetsURL()
        {
            Request request = GetRequest((@"GET /path/ HTTP / 1.1" + Environment.NewLine).ToCharArray());

            request.ProcessRequestHeader();

            Assert.That(request.URL, Is.EqualTo("/path/"));
        }

        [Test]
        public void Send404_Sends404() {
            _socket.Expect(y => y.Send("HTTP/1.1 404 Not Found\r\nContent-Length: 0\r\nConnection: close\r\n\r\n")).Return(0);
            
            Request request = GetRequest(new char[2]);
            request.Send404();

            _socket.VerifyAllExpectations();
            
        }

        [Test]
        public void SendResponse_SendsHeader() {
            const string responseString = "responsestring";
            const string type = "text/html";

            _socket.Expect(y => y.Send("HTTP/1.0 200 OK\r\nContent-Type: " + type + "; charset=utf-8\r\nContent-Length: " + responseString.Length +
                                "\r\nConnection: close\r\n\r\n")).Return(0);
            _socket.Expect(y => y.Send(responseString)).Return(0);

            Request request = GetRequest(new char[2]);

            request.SendResponse("responsestring");

            _socket.VerifyAllExpectations();

        }

    }
}
