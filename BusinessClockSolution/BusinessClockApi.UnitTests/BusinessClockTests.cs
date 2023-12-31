﻿using BusinessClockApi.Services;
using NSubstitute;

namespace BusinessClockApi.UnitTests;

public class BusinessClockTests
{
    [Fact]
    public void ClosedOnSaturday()
    {
        var stubbedSystemTime = Substitute.For<ISystemTime>();
        stubbedSystemTime.GetCurrent().Returns(new DateTime(2023, 8, 26));

        var clock = new BusinessClock(stubbedSystemTime);

        bool isOpen = clock.IsOpen();
        
        Assert.False(isOpen);
    }

    [Fact]
    public void ClosedOnSunday()
    {
        var stubbedSystemTime = Substitute.For<ISystemTime>();
        stubbedSystemTime.GetCurrent().Returns(new DateTime(2023, 8, 26));

        var clock = new BusinessClock(stubbedSystemTime);

        bool isOpen = clock.IsOpen();

        Assert.False(isOpen);
    }

    [Theory]
    [InlineData("8/21/2023 9:00:00")]
    [InlineData("8/21/2023 16:59:59")]
    public void WeAreOpen(string dateTime)
    {
        var stubbedSystemTime = Substitute.For<ISystemTime>();
        stubbedSystemTime.GetCurrent().Returns(DateTime.Parse(dateTime));
        var clock = new BusinessClock(stubbedSystemTime);

        Assert.True(clock.IsOpen());
    }
}
