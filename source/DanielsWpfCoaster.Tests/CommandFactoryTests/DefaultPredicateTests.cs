using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DanielsWpfCoaster.Mvvm;
using NUnit.Framework;

namespace DanielsWpfCoaster.Tests
{
    public class DefaultPredicateTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void CommandParameterIsNull_GenericRefTypeSync_ShouldReturnTrue()
        {
            var commandWithDefaultPredicate = CommandFactory.Create<string>(DummyExecute);

            Assert.That(commandWithDefaultPredicate.CanExecute(null), Is.True);

            // local functions
            void DummyExecute(string a)
            {
            }
        }

        [Test]
        public void CommandParameterIsNull_GenericRefTypeAsync_ShouldReturnTrue()
        {
            var commandWithDefaultPredicate = CommandFactory.Create<string>(DummyExecute);

            Assert.That(commandWithDefaultPredicate.CanExecute(null), Is.True);

            // local functions
            Task DummyExecute(string a) => Task.CompletedTask;
        }

        [Test]
        public void CommandParameterIsNull_GenericValueTypeSync_ShouldReturnTrue()
        {
            var commandWithDefaultPredicate = CommandFactory.Create<int>(DummyExecute);

            Assert.That(commandWithDefaultPredicate.CanExecute(null), Is.True);

            // local functions
            void DummyExecute(int a)
            {
            }
        }

        [Test]
        public void CommandParameterIsNull_GenericValueTypeAsync_ShouldReturnTrue()
        {
            var commandWithDefaultPredicate = CommandFactory.Create<int>(DummyExecute);

            Assert.That(commandWithDefaultPredicate.CanExecute(null), Is.True);

            // local functions
            Task DummyExecute(int a) => Task.CompletedTask;
        }

        [Test]
        public void CommandParameterHasValue_GenericRefTypeSync_ShouldReturnTrue()
        {
            var commandWithDefaultPredicate = CommandFactory.Create<string>(DummyExecute);

            Assert.That(commandWithDefaultPredicate.CanExecute(""), Is.True);

            // local functions
            void DummyExecute(string a)
            {
            }
        }

        [Test]
        public void CommandParameterHasValue_GenericRefTypeAsync_ShouldReturnTrue()
        {
            var commandWithDefaultPredicate = CommandFactory.Create<string>(DummyExecute);

            Assert.That(commandWithDefaultPredicate.CanExecute(""), Is.True);

            // local functions
            Task DummyExecute(string a) => Task.CompletedTask;
        }

        [Test]
        public void CommandParameterHasValue_GenericValueTypeSync_ShouldReturnTrue()
        {
            var commandWithDefaultPredicate = CommandFactory.Create<int>(DummyExecute);

            Assert.That(commandWithDefaultPredicate.CanExecute("1"), Is.True);

            // local functions
            void DummyExecute(int a)
            {
            }
        }

        [Test]
        public void CommandParameterHasValue_GenericValueTypeAsync_ShouldReturnTrue()
        {
            var commandWithDefaultPredicate = CommandFactory.Create<int>(DummyExecute);

            Assert.That(commandWithDefaultPredicate.CanExecute("1"), Is.True);

            // local functions
            Task DummyExecute(int a) => Task.CompletedTask;
        }
    }
}