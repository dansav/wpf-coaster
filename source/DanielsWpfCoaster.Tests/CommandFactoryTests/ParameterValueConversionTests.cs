using System.Threading.Tasks;
using DanielsWpfCoaster.Mvvm;
using NUnit.Framework;

namespace DanielsWpfCoaster.Tests
{
    public class ParameterValueConversionTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ParameterIsNull_RefType_Async()
        {
            var command = CommandFactory.Create<string>(DummyExecute);

            Assert.That(command.CanExecute(null), Is.True);

            Assert.DoesNotThrow(() => command.Execute(null));

            // local functions
            Task DummyExecute(string a)
            {
                return Task.CompletedTask;
            }
        }
        [Test]
        public void ParameterIsNull_RefType_Sync()
        {
            var command = CommandFactory.Create<string>(DummyExecute);

            Assert.That(command.CanExecute(null), Is.True);

            Assert.DoesNotThrow(() => command.Execute(null));

            // local functions
            void DummyExecute(string a)
            {
            }
        }

        [Test]
        public void ParameterIsNull_ValueType_Async()
        {
            var command = CommandFactory.Create<int>(DummyExecute);

            Assert.That(command.CanExecute(null), Is.True);

            Assert.DoesNotThrow(() => command.Execute(null));

            // local functions
            Task DummyExecute(int a)
            {
                return Task.CompletedTask;
            }
        }

        [Test]
        public void ParameterIsNull_ValueType_Sync()
        {
            var command = CommandFactory.Create<int>(DummyExecute);

            Assert.That(command.CanExecute(null), Is.True);

            Assert.DoesNotThrow(() => command.Execute(null));

            // local functions
            void DummyExecute(int a)
            {
            }
        }
    }
}