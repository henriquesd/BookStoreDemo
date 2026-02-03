using AutoFixture;
using BookStore.Domain.Models;

namespace BookStore.Infrastructure.Tests.Helpers
{
    public static class FixtureFactory
    {
        public static Fixture Create()
        {
            var fixture = new Fixture();

            fixture.Behaviors.OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => fixture.Behaviors.Remove(b));

            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            fixture.Customize<Book>(composer => composer
                .Without(b => b.Category));

            fixture.Customize<Category>(composer => composer
                .Without(c => c.Books));

            return fixture;
        }
    }
}
