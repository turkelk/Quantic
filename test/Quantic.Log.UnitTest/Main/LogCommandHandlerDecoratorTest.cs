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
    public class LogCommandHandlerDecoratorTest
    {   
        [Fact]
        public async Task ShouldLogIfSettingsIsNotProvided()
        {
            // Arrange 
            LogSettings logSettings = null;

            Mock<IOptionsSnapshot<LogSettings>> mockOptions = new Mock<IOptionsSnapshot<LogSettings>>();     
            mockOptions.Setup(x=>x.Value).Returns(logSettings);      

            Mock<IRequestLogger> mockRequestLogger = new Mock<IRequestLogger>();  
            Mock<ICommandHandler<TestCommand>> mockCommandHandler = new Mock<ICommandHandler<TestCommand>>();
            var command = new TestCommand();

            mockCommandHandler
            .Setup(x=>x.Handle(command,It.IsAny<RequestContext>()))
            .ReturnsAsync(CommandResult.Success);     

            var decorator = new LogCommandHandlerDecorator<TestCommand>(mockRequestLogger.Object, mockCommandHandler.Object, mockOptions.Object);

            // Act
            var result = await decorator.Handle(command, Helper.Context);
            
            // Assert
            mockRequestLogger.Verify(x=>x.Log(It.IsAny<RequestLog>()),Times.Exactly(1)); 
            Assert.Equal(Messages.Success, result.Code);             
        }

        [Fact]
        public async Task ShouldLogIfCommandIsNotInExcludeList()
        {
            // Arrange 
            LogSettings logSettings = new LogSettings
            {
                Exclude = new List<string>()           
            };

            Mock<IOptionsSnapshot<LogSettings>> mockOptions = new Mock<IOptionsSnapshot<LogSettings>>();     
            mockOptions.Setup(x=>x.Value).Returns(logSettings);      

            Mock<IRequestLogger> mockRequestLogger = new Mock<IRequestLogger>();  
            Mock<ICommandHandler<TestCommand>> mockCommandHandler = new Mock<ICommandHandler<TestCommand>>();
            var command = new TestCommand();

            mockCommandHandler
            .Setup(x=>x.Handle(command,It.IsAny<RequestContext>()))
            .ReturnsAsync(CommandResult.Success);     

            var decorator = new LogCommandHandlerDecorator<TestCommand>(mockRequestLogger.Object, mockCommandHandler.Object, mockOptions.Object);

            // Act
            var result = await decorator.Handle(command, Helper.Context);
            
            // Assert
            mockRequestLogger.Verify(x=>x.Log(It.IsAny<RequestLog>()),Times.Exactly(1)); 
            Assert.Equal(Messages.Success, result.Code);             
        }  

        [Fact]
        public async Task ShouldNotLogIfCommandIsInExcludeList()
        {
            // Arrange 
            LogSettings logSettings = new LogSettings
            {
                Exclude = new List<string>{ typeof(TestCommand).Name }       
            };

            Mock<IOptionsSnapshot<LogSettings>> mockOptions = new Mock<IOptionsSnapshot<LogSettings>>();     
            mockOptions.Setup(x=>x.Value).Returns(logSettings);      

            Mock<IRequestLogger> mockRequestLogger = new Mock<IRequestLogger>();  
            Mock<ICommandHandler<TestCommand>> mockCommandHandler = new Mock<ICommandHandler<TestCommand>>();
            var command = new TestCommand();

            mockCommandHandler
            .Setup(x=>x.Handle(command,It.IsAny<RequestContext>()))
            .ReturnsAsync(CommandResult.Success);     

            var decorator = new LogCommandHandlerDecorator<TestCommand>(mockRequestLogger.Object, mockCommandHandler.Object, mockOptions.Object);

            // Act
            var result = await decorator.Handle(command, Helper.Context);
            
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
            Mock<ICommandHandler<TestCommand>> mockCommandHandler = new Mock<ICommandHandler<TestCommand>>();
            var command = new TestCommand();

            mockCommandHandler
            .Setup(x=>x.Handle(command,It.IsAny<RequestContext>()))
            .ThrowsAsync(new Exception());    

            var decorator = new LogCommandHandlerDecorator<TestCommand>(mockRequestLogger.Object, mockCommandHandler.Object, mockOptions.Object);

            // Act
            var result = await decorator.Handle(command, Helper.Context);
            
            // Assert
            Assert.True(result.HasError);
            Assert.Equal(Constants.UnhandledException, result.Errors[0].Code);                  
        }                

        public class TestCommand : ICommand{ }
    }
}
