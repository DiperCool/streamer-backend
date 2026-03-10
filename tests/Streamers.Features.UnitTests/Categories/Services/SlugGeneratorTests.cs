using FluentAssertions;
using Streamers.Features.Categories.Services;
using Xunit;

namespace Streamers.Features.UnitTests.Categories.Services;

public class SlugGeneratorTests
{
    private readonly ISlugGenerator _slugGenerator = new SlugGenerator();

    [Theory]
    [InlineData("Hello World", "hello-world")]
    [InlineData("  leading and trailing spaces  ", "leading-and-trailing-spaces")]
    [InlineData("Multiple   Spaces", "multiple-spaces")]
    [InlineData("Special!@#$%^&*()_+=Chars", "specialchars")]
    [InlineData("áéíóú", "aeiou")]
    [InlineData("Some Diacritics like ñ and ü", "some-diacritics-like-n-and-u")]
    [InlineData("An Already-Valid-Slug", "an-already-valid-slug")]
    [InlineData("", "")]
    [InlineData("  ", "")]
    public void GenerateSlug_ShouldReturnCorrectSlug(string input, string expected)
    {
        // Act
        var result = _slugGenerator.GenerateSlug(input);

        // Assert
        result.Should().Be(expected);
    }
}
