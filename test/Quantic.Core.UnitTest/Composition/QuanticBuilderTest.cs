using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Quantic.Core.Test
{
    public class QuanticBuilderTest
    {
        [Fact]
        public void ShoultThrowArgumentNullExceptionForServices()
        {
            bool exceptionThrown = false;
            string paramName = "services";

            try
            {
                var builder = new QuanticBuilder(null,  typeof(QuanticBuilderTest).Assembly );
            }
            catch (ArgumentNullException ex)
            {
                exceptionThrown = true;
                Assert.Equal(paramName, ex.ParamName);
            }

            Assert.True(exceptionThrown);
        }

        [Fact]
        public void ShoultThrowArgumentNullExceptionForAssemblies()
        {
            // Arrange
            var services = new ServiceCollection();
            bool exceptionThrown = false;
            string paramName = "assemblies";

            try
            {
                // Act
                var builder = new QuanticBuilder(services,  null);
            }
            catch (ArgumentNullException ex)
            {
                exceptionThrown = true;
                Assert.Equal(paramName, ex.ParamName);
            }

            // Assert
            Assert.True(exceptionThrown);
        }        

        [Fact]
        public void ShouldSuccess()
        {
            var services = new ServiceCollection();
            var assemblies = new Assembly[] { typeof(QuanticBuilderTest).Assembly };
            var builder = new QuanticBuilder(services,assemblies) ;

            Assert.Equal(services, builder.Services);
            Assert.Equal(assemblies, builder.Assemblies);
        }
    }
}