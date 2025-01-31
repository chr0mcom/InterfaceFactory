

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.InterfaceFactory;

namespace InterfaceFactory.Tests
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestMyServiceCollection()
        {
            ServiceCollection serviceCollection = new ();
            serviceCollection.RegisterInterfaceFactories(true);
      
      ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
            serviceProvider.UseInterfaceFactory();
            IExample myExmaple2 = IExample.GetInstance();
            IExample myExmaple = IExample.GetRequiredKeyedInstance(nameof(MyExample));
    }

        public interface IExample : IFactory<IExample>;


        public class MyExample : IExample
        {
          public static object? FactoryKeyName { get; set; } = nameof(MyExample);
        }

        public class MyExample2 : IExample;
        public interface IExample2 : IFactory<IExample2>;
        public class MyExample3 : IExample2;
    }
}
