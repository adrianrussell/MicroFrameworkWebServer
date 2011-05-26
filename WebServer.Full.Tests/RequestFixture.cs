using NUnit.Framework;
using Server.Network;

namespace Server.Full.Tests
{
    [TestFixture]
    public class RequestFixture
    {
        IClientSocket _socket;

        [SetUp]
        public void Setup() {
            _socket = Rhino.Mocks.MockRepository.GenerateMock<IClientSocket>();
        }

        [Test]
        public void CanCreate() {
          
            var request = new Request(_socket,new char[2]);
            Assert.That(request, Is.Not.Null);

 
        }
    }
}
