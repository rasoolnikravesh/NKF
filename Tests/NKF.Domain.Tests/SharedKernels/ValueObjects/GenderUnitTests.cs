using FluentAssertions;
using NKF.Domain.SharedKernels.ValueObjects;

namespace NKF.Domain.Tests.SharedKernels.ValueObjects;

public class GenderUnitTests
{
    [Fact]
    public void Should_Fail_Result_If_Value_Is_Null()
    {
        var gender = Gender.GetByValue(null);
        gender.IsFailed.Should().BeTrue();
        gender.Errors[0].Metadata["Code"].Should().Be("Gender.Required");
    }

    [Fact]
    public void Should_Fail_Result_If_Value_In_Valid()
    {
        var gender = Gender.GetByValue(6);
        gender.IsFailed.Should().BeTrue();
        gender.Errors[0].Metadata["Code"].Should().Be("Gender.InValidCode");
    }

    [Fact]
    public void Should_Ok_Result_If_Value_Is_Valid()
    {
        var gender = Gender.GetByValue(Gender.Male.Value);
        var expected = Gender.Male;
        gender.IsFailed.Should().BeFalse();
        gender.Errors.Should().BeEmpty();
        gender.IsSuccess.Should().BeTrue();
        gender.Value.Value.Should().Be(expected.Value);
    }
}