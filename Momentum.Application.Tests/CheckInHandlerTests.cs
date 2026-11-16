using FluentAssertions;
using Moq;
using Momentum.Application.Interfaces;
using Momentum.Application.UseCases.Habits.CheckIn;
using Momentum.Domain.Entities;
using Xunit;
using Momentum.Application.Common; // For Result and Error

namespace Momentum.Application.Tests;

public class CheckInHandlerTests
{
    private readonly Mock<IHabitRepository> _habitRepoMock;
    private readonly Mock<ICheckInRepository> _checkInRepoMock;
    private readonly Mock<IMoodRepository> _moodRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICurrentUserService> _currentUserMock;
    private readonly CheckInHandler _handler;

    public CheckInHandlerTests()
    {
        // 1. Initialize Mocks (Fake objects)
        _habitRepoMock = new Mock<IHabitRepository>();
        _checkInRepoMock = new Mock<ICheckInRepository>();
        _moodRepoMock = new Mock<IMoodRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _currentUserMock = new Mock<ICurrentUserService>();

        // Setup current user mock
        _currentUserMock.Setup(x => x.UserId).Returns(1);
        _currentUserMock.Setup(x => x.Today).Returns(new DateOnly(2026, 11, 16));

        // 2. Create the handler with mocked dependencies
        _handler = new CheckInHandler(
            _habitRepoMock.Object,
            _checkInRepoMock.Object,
            _moodRepoMock.Object,
            _unitOfWorkMock.Object,
            _currentUserMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenHabitIsNotScheduledForToday()
    {
        // Arrange (Setup the scenario)
        // Create a habit that is scheduled ONLY on Mondays (1) and Wednesdays (3)
        var habit = new Habit
        {
            HabitId = 1,
            UserId = 1,
            FrequencyType = "SpecificDays",
            DaysOfWeek = "1,3", // Monday, Wednesday
            StartDate = new DateOnly(2026, 1, 1),
            IsArchived = false
        };

        _habitRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(habit);
            
        
        _currentUserMock.Setup(x => x.Today).Returns(new DateOnly(2026, 11, 17)); 

        var command = new CheckInCommand(1, null, true, null, null);

        // Act (Execute the handler)
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert (Verify the result using FluentAssertions)
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Be("This habit is not scheduled for today.");

        // Verify that SaveChanges was NEVER called
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}