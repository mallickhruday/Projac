﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Projac.Tests
{
    [TestFixture]
    public class ProjectorTests
    {
        [Test]
        public void ResolverCanNotBeNull()
        {
            Assert.Throws<ArgumentNullException>(
                () => SutFactory((ProjectionHandlerResolver<object>)null));
        }

        [Test]
        public void ProjectAsync_ConnectionCanBeNull()
        {
            var sut = SutFactory();
            Assert.DoesNotThrow(() => sut.ProjectAsync((object)null, new object()));
        }

        [Test]
        public void ProjectAsync_MessageCanNotBeNull()
        {
            var sut = SutFactory();
            Assert.Throws<ArgumentNullException>(() => sut.ProjectAsync(new object(), (object)null));
        }

        [Test]
        public void ProjectAsyncToken_ConnectionCanBeNull()
        {
            var sut = SutFactory();
            Assert.DoesNotThrow(() => sut.ProjectAsync((object)null, new object(), CancellationToken.None));
        }

        [Test]
        public void ProjectAsyncToken_MessageCanNotBeNull()
        {
            var sut = SutFactory();
            Assert.Throws<ArgumentNullException>(() => sut.ProjectAsync(new object(), (object)null, CancellationToken.None));
        }

        [Test]
        public void ProjectAsyncMany_ConnectionCanBeNull()
        {
            var sut = SutFactory();
            Assert.DoesNotThrow(() => sut.ProjectAsync((object)null, new object[0]));
        }

        [Test]
        public void ProjectAsyncMany_MessagesCanNotBeNull()
        {
            var sut = SutFactory();
            Assert.Throws<ArgumentNullException>(() => sut.ProjectAsync(new object(), (IEnumerable<object>)null));
        }

        [Test]
        public void ProjectAsyncManyToken_ConnectionCanBeNull()
        {
            var sut = SutFactory();
            Assert.DoesNotThrow(() => sut.ProjectAsync((object)null, new object[0], CancellationToken.None));
        }

        [Test]
        public void ProjectAsyncManyToken_MessagesCanNotBeNull()
        {
            var sut = SutFactory();
            Assert.Throws<ArgumentNullException>(
                () => sut.ProjectAsync(new object(), (IEnumerable<object>) null, CancellationToken.None));
        }

        [TestCaseSource(typeof(ProjectorProjectCases), nameof(ProjectorProjectCases.ProjectMessageWithoutTokenCases))]
        public async Task ProjectAsyncMessageCausesExpectedCalls(
            ProjectionHandlerResolver<CallRecordingConnection> resolver,
            object message,
            Tuple<int, object, CancellationToken>[] expectedCalls)
        {
            var connection = new CallRecordingConnection();
            var sut = SutFactory(resolver);

            await sut.ProjectAsync(connection, message);

            Assert.That(connection.ObsoleteRecordedCalls, Is.EquivalentTo(expectedCalls));
        }

        [TestCaseSource(typeof(ProjectorProjectCases), nameof(ProjectorProjectCases.ProjectMessageWithTokenCases))]
        public async Task ProjectAsyncTokenMessageCausesExpectedCalls(
            ProjectionHandlerResolver<CallRecordingConnection> resolver,
            object message,
            CancellationToken token,
            Tuple<int, object, CancellationToken>[] expectedCalls)
        {
            var connection = new CallRecordingConnection();
            var sut = SutFactory(resolver);

            await sut.ProjectAsync(connection, message, token);

            Assert.That(connection.ObsoleteRecordedCalls, Is.EquivalentTo(expectedCalls));
        }

        [TestCaseSource(typeof(ProjectorProjectCases), nameof(ProjectorProjectCases.ProjectMessagesWithoutTokenCases))]
        public async Task ProjectAsyncMessagesCausesExpectedCalls(
            ProjectionHandlerResolver<CallRecordingConnection> resolver,
            object[] messages,
            Tuple<int, object, CancellationToken>[] expectedCalls)
        {
            var connection = new CallRecordingConnection();
            var sut = SutFactory(resolver);

            await sut.ProjectAsync(connection, messages);

            Assert.That(connection.ObsoleteRecordedCalls, Is.EquivalentTo(expectedCalls));
        }

        [TestCaseSource(typeof(ProjectorProjectCases), nameof(ProjectorProjectCases.ProjectMessagesWithTokenCases))]
        public async Task ProjectAsyncTokenMessagesCausesExpectedCalls(
            ProjectionHandlerResolver<CallRecordingConnection> resolver,
            object[] messages,
            CancellationToken token,
            Tuple<int, object, CancellationToken>[] expectedCalls)
        {
            var connection = new CallRecordingConnection();
            var sut = SutFactory(resolver);

            await sut.ProjectAsync(connection, messages, token);

            Assert.That(connection.ObsoleteRecordedCalls, Is.EquivalentTo(expectedCalls));
        }

        [Test]
        public void ProjectAsyncMessageResolverFailureCausesExpectedResult()
        {
            ProjectionHandlerResolver<object> resolver = m =>
            {
                throw new Exception("message");
            };
            var sut = SutFactory(resolver);
            Assert.That(async () =>
                await sut.ProjectAsync(new object(), new object()),
                Throws.TypeOf<Exception>().And.Message.EqualTo("message"));
        }

        [Test]
        public void ProjectAsyncTokenMessageResolverFailureCausesExpectedResult()
        {
            ProjectionHandlerResolver<object> resolver = m =>
            {
                throw new Exception("message");
            };
            var sut = SutFactory(resolver);
            Assert.That(async () =>
                await sut.ProjectAsync(new object(), new object(), new CancellationToken()),
                Throws.TypeOf<Exception>().And.Message.EqualTo("message"));
        }

        [Test]
        public void ProjectAsyncMessagesResolverFailureCausesExpectedResult()
        {
            ProjectionHandlerResolver<object> resolver = m =>
            {
                throw new Exception("message");
            };
            var sut = SutFactory(resolver);
            Assert.That(async () =>
                await sut.ProjectAsync(new object(), new[] { new object(), new object() }),
                Throws.TypeOf<Exception>().And.Message.EqualTo("message"));
        }

        [Test]
        public void ProjectAsyncTokenMessagesResolverFailureCausesExpectedResult()
        {
            ProjectionHandlerResolver<object> resolver = m =>
            {
                throw new Exception("message");
            };
            var sut = SutFactory(resolver);
            Assert.That(async () =>
                await sut.ProjectAsync(new object(), new[] { new object(), new object() }, new CancellationToken()),
                Throws.TypeOf<Exception>().And.Message.EqualTo("message"));
        }

        [Test]
        public void ProjectAsyncMessageFirstHandlerFailureCausesExpectedResult()
        {
            Func<object, object, CancellationToken, Task> handler =
                (connection, message, token) =>
                {
                    var source = new TaskCompletionSource<object>();
                    source.SetException(new Exception("message"));
                    return source.Task;
                };
            ProjectionHandlerResolver<object> resolver = m => new[] {new ProjectionHandler<object>(typeof (object), handler)};
            var sut = SutFactory(resolver);

            Assert.That(async () =>
                await sut.ProjectAsync(new object(), new object()),
                Throws.TypeOf<Exception>().And.Message.EqualTo("message"));
        }

        [Test]
        public void ProjectAsyncMessageSecondHandlerFailureCausesExpectedResult()
        {
            Func<object, object, CancellationToken, Task> handler1 =
                (connection, message, token) => Task.CompletedTask;
            Func<object, object, CancellationToken, Task> handler2 =
                (connection, message, token) =>
                {
                    var source = new TaskCompletionSource<object>();
                    source.SetException(new Exception("message"));
                    return source.Task;
                };
            ProjectionHandlerResolver<object> resolver = Resolve.WhenEqualToHandlerMessageType(new[]
            {
                new ProjectionHandler<object>(typeof(object), handler1),
                new ProjectionHandler<object>(typeof(int), handler2)
            });
            var sut = SutFactory(resolver);

            Assert.That(async () =>
                await sut.ProjectAsync(new object(), new int()),
                Throws.TypeOf<Exception>().And.Message.EqualTo("message"));
        }

        [Test]
        public void ProjectAsyncMessageSecondHandlerCancellationCausesExpectedResult()
        {
            Func<object, object, CancellationToken, Task> handler1 =
                (connection, message, token) => Task.CompletedTask;
            Func<object, object, CancellationToken, Task> handler2 =
                (connection, message, token) =>
                {
                    var source = new TaskCompletionSource<object>();
                    source.SetCanceled();
                    return source.Task;
                };
            ProjectionHandlerResolver<object> resolver = Resolve.WhenEqualToHandlerMessageType(new[]
            {
                new ProjectionHandler<object>(typeof(object), handler1),
                new ProjectionHandler<object>(typeof(int), handler2)
            });
            var sut = SutFactory(resolver);

            Assert.That(async () =>
                await sut.ProjectAsync(new object(), new int()),
                Throws.TypeOf<TaskCanceledException>());
        }

        [Test]
        public void ProjectAsyncTokenMessageFirstHandlerFailureCausesExpectedResult()
        {
            Func<object, object, CancellationToken, Task> handler =
                (connection, message, token) =>
                {
                    var source = new TaskCompletionSource<object>();
                    source.SetException(new Exception("message"));
                    return source.Task;
                };
            ProjectionHandlerResolver<object> resolver = m => new[] { new ProjectionHandler<object>(typeof(object), handler) };
            var sut = SutFactory(resolver);

            Assert.That(async () =>
                await sut.ProjectAsync(new object(), new object(), new CancellationToken()),
                Throws.TypeOf<Exception>().And.Message.EqualTo("message"));
        }

        [Test]
        public void ProjectAsyncTokenMessageSecondHandlerFailureCausesExpectedResult()
        {
            Func<object, object, CancellationToken, Task> handler1 =
                (connection, message, token) => Task.CompletedTask;
            Func<object, object, CancellationToken, Task> handler2 =
                (connection, message, token) =>
                {
                    var source = new TaskCompletionSource<object>();
                    source.SetException(new Exception("message"));
                    return source.Task;
                };
            ProjectionHandlerResolver<object> resolver = Resolve.WhenEqualToHandlerMessageType(new[]
            {
                new ProjectionHandler<object>(typeof(object), handler1),
                new ProjectionHandler<object>(typeof(int), handler2)
            });
            var sut = SutFactory(resolver);

            Assert.That(async () =>
                await sut.ProjectAsync(new object(), new int(), new CancellationToken()),
                Throws.TypeOf<Exception>().And.Message.EqualTo("message"));
        }

        [Test]
        public void ProjectAsyncTokenMessageSecondHandlerCancellationCausesExpectedResult()
        {
            Func<object, object, CancellationToken, Task> handler1 =
                (connection, message, token) => Task.CompletedTask;
            Func<object, object, CancellationToken, Task> handler2 =
                (connection, message, token) =>
                {
                    var source = new TaskCompletionSource<object>();
                    source.SetCanceled();
                    return source.Task;
                };
            ProjectionHandlerResolver<object> resolver = Resolve.WhenEqualToHandlerMessageType(new[]
            {
                new ProjectionHandler<object>(typeof(object), handler1),
                new ProjectionHandler<object>(typeof(int), handler2)
            });
            var sut = SutFactory(resolver);

            Assert.That(async () =>
                await sut.ProjectAsync(new object(), new int(), new CancellationToken()),
                Throws.TypeOf<TaskCanceledException>());
        }


        [Test]
        public void ProjectAsyncMessagesFirstHandlerFailureCausesExpectedResult()
        {
            Func<object, object, CancellationToken, Task> handler =
                (connection, message, token) =>
                {
                    var source = new TaskCompletionSource<object>();
                    source.SetException(new Exception("message"));
                    return source.Task;
                };
            ProjectionHandlerResolver<object> resolver = m => new[] { new ProjectionHandler<object>(typeof(object), handler) };
            var sut = SutFactory(resolver);

            Assert.That(async () =>
                await sut.ProjectAsync(new object(), new[] { new object(), new object() }),
                Throws.TypeOf<Exception>().And.Message.EqualTo("message"));
        }

        [Test]
        public void ProjectAsyncMessagesSecondHandlerFailureCausesExpectedResult()
        {
            Func<object, object, CancellationToken, Task> handler1 =
                (connection, message, token) => Task.CompletedTask;
            Func<object, object, CancellationToken, Task> handler2 =
                (connection, message, token) =>
                {
                    var source = new TaskCompletionSource<object>();
                    source.SetException(new Exception("message"));
                    return source.Task;
                };
            ProjectionHandlerResolver<object> resolver = Resolve.WhenEqualToHandlerMessageType(new[]
            {
                new ProjectionHandler<object>(typeof(object), handler1),
                new ProjectionHandler<object>(typeof(int), handler2)
            });
            var sut = SutFactory(resolver);

            Assert.That(async () =>
                await sut.ProjectAsync(new object(), new[] { new object(), new int() }),
                Throws.TypeOf<Exception>().And.Message.EqualTo("message"));
        }

        [Test]
        public void ProjectAsyncMessagesSecondHandlerCancellationCausesExpectedResult()
        {
            Func<object, object, CancellationToken, Task> handler1 =
                (connection, message, token) => Task.CompletedTask;
            Func<object, object, CancellationToken, Task> handler2 =
                (connection, message, token) =>
                {
                    var source = new TaskCompletionSource<object>();
                    source.SetCanceled();
                    return source.Task;
                };
            ProjectionHandlerResolver<object> resolver = Resolve.WhenEqualToHandlerMessageType(new[]
            {
                new ProjectionHandler<object>(typeof(object), handler1),
                new ProjectionHandler<object>(typeof(int), handler2)
            });
            var sut = SutFactory(resolver);

            Assert.That(async () =>
                await sut.ProjectAsync(new object(), new[] { new object(), new int() }),
                Throws.TypeOf<TaskCanceledException>());
        }

        [Test]
        public void ProjectAsyncTokenMessagesFirstHandlerFailureCausesExpectedResult()
        {
            Func<object, object, CancellationToken, Task> handler =
                (connection, message, token) =>
                {
                    var source = new TaskCompletionSource<object>();
                    source.SetException(new Exception("message"));
                    return source.Task;
                };
            ProjectionHandlerResolver<object> resolver = m => new[] { new ProjectionHandler<object>(typeof(object), handler) };
            var sut = SutFactory(resolver);

            Assert.That(async () =>
                await sut.ProjectAsync(new object(), new[] { new object(), new object() }, new CancellationToken()),
                Throws.TypeOf<Exception>().And.Message.EqualTo("message"));
        }

        [Test]
        public void ProjectAsyncTokenMessagesSecondHandlerFailureCausesExpectedResult()
        {
            Func<object, object, CancellationToken, Task> handler1 =
                (connection, message, token) => Task.CompletedTask;
            Func<object, object, CancellationToken, Task> handler2 =
                (connection, message, token) =>
                {
                    var source = new TaskCompletionSource<object>();
                    source.SetException(new Exception("message"));
                    return source.Task;
                };
            ProjectionHandlerResolver<object> resolver = Resolve.WhenEqualToHandlerMessageType(new[]
            {
                new ProjectionHandler<object>(typeof(object), handler1),
                new ProjectionHandler<object>(typeof(int), handler2)
            });
            var sut = SutFactory(resolver);

            Assert.That(async () =>
                await sut.ProjectAsync(new object(), new[] { new object(), new int() }, new CancellationToken()),
                Throws.TypeOf<Exception>().And.Message.EqualTo("message"));
        }

        [Test]
        public void ProjectAsyncTokenMessagesSecondHandlerCancellationCausesExpectedResult()
        {
            Func<object, object, CancellationToken, Task> handler1 =
                (connection, message, token) => Task.CompletedTask;
            Func<object, object, CancellationToken, Task> handler2 =
                (connection, message, token) =>
                {
                    var source = new TaskCompletionSource<object>();
                    source.SetCanceled();
                    return source.Task;
                };
            ProjectionHandlerResolver<object> resolver = Resolve.WhenEqualToHandlerMessageType(new[]
            {
                new ProjectionHandler<object>(typeof(object), handler1),
                new ProjectionHandler<object>(typeof(int), handler2)
            });
            var sut = SutFactory(resolver);

            Assert.That(async () =>
                await sut.ProjectAsync(new object(), new[] { new object(), new int() }, new CancellationToken()),
                Throws.TypeOf<TaskCanceledException>());
        }

        [Test]
        public void ProjectAsyncTokenMessagesHandlerObservingCancelledTokenCausesExpectedResult()
        {
            var tcs = new CancellationTokenSource();

            Func<object, object, CancellationToken, Task> handler1 =
                (connection, message, token) =>
                {
                    var source = new TaskCompletionSource<object>();
                    token.Register(() => source.SetCanceled());

                    tcs.Cancel();
                    return source.Task;
                };

            ProjectionHandlerResolver<object> resolver = Resolve.WhenEqualToHandlerMessageType(new[]
            {
                new ProjectionHandler<object>(typeof(object), handler1),
            });
            var sut = SutFactory(resolver);
            
            Assert.That(async () =>
                await sut.ProjectAsync(new object(), new[] { new object() }, tcs.Token),
                Throws.TypeOf<TaskCanceledException>());
        }

        [Test]
        public void ProjectAsyncTokenMessagesCancellationAfterSecondHandlerCausesExpectedResult()
        {
            var tcs = new CancellationTokenSource();

            Func<object, object, CancellationToken, Task> handler1 =
                (connection, message, token) => Task.CompletedTask;
            Func<object, object, CancellationToken, Task> handler2 =
                (connection, message, token) =>
                {
                    tcs.Cancel();
                    return Task.CompletedTask;
                };
            Func<object, object, CancellationToken, Task> handler3 =
                (connection, message, token) =>
                {
                    throw new InvalidOperationException("Should not happen!");
                };

            ProjectionHandlerResolver<object> resolver = Resolve.WhenEqualToHandlerMessageType(new[]
            {
                new ProjectionHandler<object>(typeof(object), handler1),
                new ProjectionHandler<object>(typeof(object), handler2),
                new ProjectionHandler<object>(typeof(object), handler3)
            });
            var sut = SutFactory(resolver);

            Assert.That(async () =>
                await sut.ProjectAsync(new object(), new[] { new object(), new object(), new object() }, tcs.Token),
                Throws.Nothing);
        }

        [Test]
        public void ProjectAsyncTokenMessagesCancellationAfterSecondHandlerInContinuationCausesExpectedResult()
        {
            var tcs = new CancellationTokenSource();
            tcs.Cancel();

            Func<object, object, CancellationToken, Task> handler1 =
                async (connection, message, token) =>
                {
                    await Task.Yield();
                    await Task.Delay(TimeSpan.FromDays(1), token);
                };

            ProjectionHandlerResolver<object> resolver = Resolve.WhenEqualToHandlerMessageType(new[]
            {
                new ProjectionHandler<object>(typeof(object), handler1),
            });
            var sut = SutFactory(resolver);

            Assert.That(async () =>
                await sut.ProjectAsync(new object(), new[] { new object() }, tcs.Token),
                Throws.TypeOf<TaskCanceledException>());
        }

        private static Projector<object> SutFactory()
        {
            return SutFactory(message => new ProjectionHandler<object>[0]);
        }

        private static Projector<TConnection> SutFactory<TConnection>(ProjectionHandlerResolver<TConnection> resolver)
        {
            return new Projector<TConnection>(resolver);
        }
    }
}
