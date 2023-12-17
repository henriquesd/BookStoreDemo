using AutoFixture;

namespace BookStore.Domain.Tests.Helpers
{
    public static class FixtureFactory
    {
        public static Fixture Create()
        {
            var fixture = new Fixture();

            fixture.Behaviors
                .OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(x => fixture.Behaviors.Remove(x));

            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            fixture.Register(() => fixture.Create<int>() * 1.33m);

            return fixture;
        }
    }
}