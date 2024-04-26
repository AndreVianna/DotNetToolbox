namespace DotNetToolbox.Data.Repositories;

public partial class AsyncRepositoryTests {
    [Fact]
    public async Task ContainsAsync_WithExistingItem_ReturnsTrue() {
        var item = new TestEntity("B");
        var result = await _set1.ContainsAsync(item);
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ContainsAsync_WithInvalidItem_ReturnsFalse() {
        var item = new TestEntity("K");
        var result = await _set1.ContainsAsync(item);
        result.Should().BeFalse();
    }
}