using Balda.DataAccess;
using Balda.DI;
using Moq;
using Ninject;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Balda.Test
{
    [TestFixture]
    public class DataRepositoryTest
    {
        IDataRepository repository;
        [SetUp]
        public void Setup()
        {
            var kernel = new StandardKernel(new TestDIModule());
            repository = kernel.Get<IDataRepository>();
        }
        [Test]
        public void IfAddWordsThanTheyAdded()
        {
            var count = repository.GetWords().Count;
            repository.AddWords(new List<string>() { "hell", "take" });
            Assert.AreEqual(repository.GetWords().Count - count, 2);
        }
        [Test]
        public void ShouldReturnData()
        {
            var res = repository.GetWords();
            Assert.IsNotNull(res);
        }
    }
}
