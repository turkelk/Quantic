using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Moq;
using Quantic.Core;
using Quantic.Log;
using Quantic.Log.UnitTest;
using Xunit;

namespace Genesis.Log.Test
{
    public class LogQueryHandlerDecoratorTest
    {   
        [Fact]
        public async Task ShouldLogIfSettingsIsNotProvided()
        {
            // Arrange 
            LogSettings logSettings = null;

            Mock<IOptionsSnapshot<LogSettings>> mockOptions = new Mock<IOptionsSnapshot<LogSettings>>();     
            mockOptions.Setup(x=>x.Value).Returns(logSettings);      

            Mock<IRequestLogger> mockRequestLogger = new Mock<IRequestLogger>();  
            Mock<IQueryHandler<TestQuery, string>> mockQueryHandler = new Mock<IQueryHandler<TestQuery, string>>();
            var query = new TestQuery();

            mockQueryHandler
            .Setup(x=>x.Handle(query,It.IsAny<RequestContext>()))
            .ReturnsAsync(new QueryResult<string>("OK"));     

            var decorator = new LogQueryHandlerDecorator<TestQuery, string>(mockRequestLogger.Object, mockQueryHandler.Object, mockOptions.Object);

            // Act
            var result = await decorator.Handle(query, Helper.Context);
            
            // Assert
            mockRequestLogger.Verify(x=>x.Log(It.IsAny<RequestLog>()),Times.Exactly(1)); 
            Assert.Equal(Messages.Success, result.Code);             
        }

        public async Task ShouldLogIfQueryIsNotInExcludeList()
        {
            // Arrange 
            LogSettings logSettings = new LogSettings
            {
                Exclude = new List<string>()           
            };

            Mock<IOptionsSnapshot<LogSettings>> mockOptions = new Mock<IOptionsSnapshot<LogSettings>>();     
            mockOptions.Setup(x=>x.Value).Returns(logSettings);      

            Mock<IRequestLogger> mockRequestLogger = new Mock<IRequestLogger>();  
            Mock<IQueryHandler<TestQuery, string>> mockQueryHandler = new Mock<IQueryHandler<TestQuery, string>>();
            var query = new TestQuery();

            mockQueryHandler
            .Setup(x=>x.Handle(query,It.IsAny<RequestContext>()))
            .ReturnsAsync(new QueryResult<string>("OK"));     

            var decorator = new LogQueryHandlerDecorator<TestQuery, string>(mockRequestLogger.Object, mockQueryHandler.Object, mockOptions.Object);

            // Act
            var result = await decorator.Handle(query, Helper.Context);
            
            // Assert
            mockRequestLogger.Verify(x=>x.Log(It.IsAny<RequestLog>()),Times.Exactly(1)); 
            Assert.Equal(Messages.Success, result.Code);               
        }  

        [Fact]
        public async Task ShouldNotLogIfQueryIsInExcludeList()
        {
            // Arrange 
            LogSettings logSettings = new LogSettings
            {
                Exclude = new List<string>{ typeof(TestQuery).Name }       
            };

            Mock<IOptionsSnapshot<LogSettings>> mockOptions = new Mock<IOptionsSnapshot<LogSettings>>();     
            mockOptions.Setup(x=>x.Value).Returns(logSettings);      

            Mock<IRequestLogger> mockRequestLogger = new Mock<IRequestLogger>();  
            Mock<IQueryHandler<TestQuery, string>> mockQueryHandler = new Mock<IQueryHandler<TestQuery, string>>();
            var query = new TestQuery();

            mockQueryHandler
            .Setup(x=>x.Handle(query,It.IsAny<RequestContext>()))
            .ReturnsAsync(new QueryResult<string>("OK"));     

            var decorator = new LogQueryHandlerDecorator<TestQuery, string>(mockRequestLogger.Object, mockQueryHandler.Object, mockOptions.Object);

            // Act
            var result = await decorator.Handle(query, Helper.Context);
            
            // Assert
            mockRequestLogger.Verify(x=>x.Log(It.IsAny<RequestLog>()),Times.Exactly(0)); 
            Assert.Equal(Messages.Success, result.Code);            
        }        

        [Fact]
        public async Task ShouldFormatExceptionAsUnHandlerExceptionResult()
        {
            // Arrange 
            LogSettings logSettings = new LogSettings();

            Mock<IOptionsSnapshot<LogSettings>> mockOptions = new Mock<IOptionsSnapshot<LogSettings>>();     
            mockOptions.Setup(x=>x.Value).Returns(logSettings);      

            Mock<IRequestLogger> mockRequestLogger = new Mock<IRequestLogger>();  
            Mock<IQueryHandler<TestQuery, string>> mockQueryHandler = new Mock<IQueryHandler<TestQuery, string>>();
            var query = new TestQuery();

            mockQueryHandler
            .Setup(x=>x.Handle(query,It.IsAny<RequestContext>()))
            .ThrowsAsync(new Exception());      

            var decorator = new LogQueryHandlerDecorator<TestQuery, string>(mockRequestLogger.Object, mockQueryHandler.Object, mockOptions.Object);

            // Act
            var result = await decorator.Handle(query, Helper.Context);
            
            // Assert
            Assert.True(result.HasError);
            Assert.Equal(Constants.UnhandledException, result.Errors[0].Code);              
        }                 

        public class TestQuery : IQuery<string>{ }
    }
}
