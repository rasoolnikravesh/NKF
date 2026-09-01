using FluentAssertions;
using FluentResults;
using NKF.Domain.SharedKernels.ValueObjects;

namespace NKF.Domain.Tests.Base;

public abstract class GenericStringValueObjectTests<T> where T : GenericStringValueObject
{
    public abstract Func<string?, Result<T>> CreateFunc { get; }
    public abstract int MinLength { get; }
    public abstract int MaxLength { get; }

    [Fact(DisplayName = "وقتی طول نام بین حداقل و حداکثر مجاز باشد، مقدار با موفقیت ساخته می‌شود")]
    public void Should_Create_When_ValueLengthIsBetweenMinAndMax()
    {
        Result<T> value = CreateFunc(CreateValue(MinLength));

        value.IsSuccess.Should().BeTrue();
        value.Value.Should().NotBeNull();
    }

    [Theory(DisplayName = "وقتی نام خالی، نال یا فقط فاصله باشد، خطای الزامی بودن برمی‌گردد")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Should_Fail_With_Required_Error_When_ValueIsNullOrWhiteSpace(string? invalidValue)
    {
        Result<T> value = CreateFunc(invalidValue);

        value.IsFailed.Should().BeTrue();
        value.Errors.Should().ContainSingle();
        value.Errors[0].Metadata["Type"].Should().Be("Required");
    }

    [Fact(DisplayName = "وقتی طول نام کمتر از حداقل مجاز باشد، خطای کوتاه بودن برمی‌گردد")]
    public void Should_Fail_With_TooShort_Error_When_ValueLengthIsLessThanMinLength()
    {
        Result<T> value = CreateFunc(CreateValue(MinLength - 1));

        value.IsFailed.Should().BeTrue();
        value.Errors.Should().ContainSingle();
        value.Errors[0].Metadata["Type"].Should().Be("TooShort");
    }

    [Fact(DisplayName = "وقتی طول نام بیشتر از حداکثر مجاز باشد، خطای بلند بودن برمی‌گردد")]
    public void Should_Fail_With_TooLong_Error_When_ValueLengthIsGreaterThanMaxLength()
    {
        Result<T> value = CreateFunc(CreateValue(MaxLength + 1));

        value.IsFailed.Should().BeTrue();
        value.Errors.Should().ContainSingle();
        value.Errors[0].Metadata["Type"].Should().Be("TooLong");
    }

    [Fact(DisplayName = "وقتی طول نام برابر حداقل مجاز باشد، مقدار با موفقیت ساخته می‌شود")]
    public void Should_Create_When_ValueLengthEqualsMinLength()
    {
        Result<T> value = CreateFunc(CreateValue(MinLength));

        value.IsSuccess.Should().BeTrue();
        value.Value.Value.Should().Be(CreateValue(MinLength));
    }

    [Fact(DisplayName = "وقتی طول نام برابر حداکثر مجاز باشد، مقدار با موفقیت ساخته می‌شود")]
    public void Should_Create_When_ValueLengthEqualsMaxLength()
    {
        Result<T> value = CreateFunc(CreateValue(MaxLength));

        value.IsSuccess.Should().BeTrue();
        value.Value.Value.Should().Be(CreateValue(MaxLength));
    }

    [Fact(DisplayName = "وقتی نام معتبر باشد، مقدار با فرمت اصلاح‌شده ذخیره می‌شود")]
    public void Should_Store_FixedFormat_Value_When_ValueIsValid()
    {
        Result<T> value = CreateFunc("  Valid   StringValueObject  ");

        value.IsSuccess.Should().BeTrue();
        value.Value.Value.Should().Be("Valid StringValueObject");
    }

    [Fact(DisplayName = "وقتی مقدار دو نام یکسان باشد، دو value object برابر هستند")]
    public void Should_Be_Equal_When_ValuesAreSame()
    {
        T first = CreateFunc(CreateValue(MinLength)).Value;
        T second = CreateFunc(CreateValue(MinLength)).Value;

        first.Should().Be(second);
        first.GetHashCode().Should().Be(second.GetHashCode());
    }

    [Fact(DisplayName = "وقتی مقدار دو نام متفاوت باشد، دو value object برابر نیستند")]
    public void Should_Not_Be_Equal_When_ValuesAreDifferent()
    {
        T first = CreateFunc(CreateValue(MinLength, 'a')).Value;
        T second = CreateFunc(CreateValue(MinLength, 'b')).Value;

        first.Should().NotBe(second);
    }

    private static string CreateValue(int length, char character = 'a')
    {
        length.Should().BePositive();
        return new string(character, length);
    }
}
