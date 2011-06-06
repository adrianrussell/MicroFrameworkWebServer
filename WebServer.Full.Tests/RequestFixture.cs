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
            Request request = GetRequest();
            Assert.That(request, Is.Not.Null);
        }

        private Request GetRequest() {
            return new Request(_socket,new char[2]);
        }

        //[MethodName_StateUnderTest_ExpectedBehavior]
        [Test]
        public void Client_ReturnsIPAddress_IPAddressIsValid() {

            _socket.Expect(x => x.RemoteEndPoint).Return(new IPEndPoint(new IPAddress(123456), 80));
            Request request = GetRequest();

            Assert.That(request.Client, Is.Not.Null);
            Assert.That(request.Client, Is.EqualTo(new IPAddress(123456)));

            _socket.VerifyAllExpectations();
        }
    }
}
