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
            // Assert
            Assert.Throws<ArgumentNullException>(()=> 
            {
                // Act
                new QuanticBuilder(null,  typeof(QuanticBuilderTest).Assembly);
            });
        }

        [Fact]
        public void ShoultThrowArgumentNullExceptionForAssemblies()
        {  
            // Arrange
            var services = new ServiceCollection();

            // Assert
            Assert.Throws<ArgumentNullException>(()=> 
            {
                // Act
                new QuanticBuilder(services,  null);
            });
        }        

        [Fact]
        public void ShouldSuccess()
        {
            // Arrange
            var services = new ServiceCollection();
            var assemblies = new Assembly[] { typeof(QuanticBuilderTest).Assembly };

            // Act
            var builder = new QuanticBuilder(services,assemblies);
            
            // Assert
            Assert.Equal(services, builder.Services);
            Assert.Equal(assemblies, builder.Assemblies);
        }
    }
}