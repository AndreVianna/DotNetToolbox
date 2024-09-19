namespace DotNetToolbox.Data.Repositories;

public partial class InMemoryDataSourceTests {
    [Fact]
    public void Update_BaseStrategy_ShouldThrow() {
        var action = () => _dummyDataSource.Update(_ => true, new(""));
        action.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public async Task UpdateAsync_BaseStrategy_ShouldThrow() {
        var action = () => _dummyDataSource.UpdateAsync(_ => true, new(""));
        await action.Should().ThrowAsync<NotImplementedException>();
    }

    [Fact]
    public void Update_WithEntity_BaseStrategy_ShouldThrow() {
        var action = () => _dummyDataSource.Update(new(""));
        action.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public async Task UpdateAsync_WithEntity_BaseStrategy_ShouldThrow() {
        var action = () => _dummyDataSource.UpdateAsync(new(""));
        await action.Should().ThrowAsync<NotImplementedException>();
    }

    [Fact]
    public void Update_UpdatedItem() {
        _updatableRepo.Update(s => s.Name == "BB", new("Z"));
        _updatableRepo.Count().Should().Be(3);
        _updatableRepo.FirstOrDefault(s => s.Name == "BB").Should().BeNull();
        _updatableRepo.FirstOrDefault(s => s.Name == "Z").Should().NotBeNull();
    }

    [Fact]
    public void Update_ForInvalidItem_UpdatedItem() {
        _updatableRepo.Update(s => s.Name == "ZZ", new("K"));
        _updatableRepo.Count().Should().Be(3);
        _updatableRepo.FirstOrDefault(s => s.Name == "K").Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_UpdatesItem() {
        await _updatableRepo.UpdateAsync(s => s.Name == "BB", new("Z"));
        _updatableRepo.Count().Should().Be(3);
        _updatableRepo.FirstOrDefault(s => s.Name == "BB").Should().BeNull();
        _updatableRepo.FirstOrDefault(s => s.Name == "Z").Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateAsync_ForInvalidItem_UpdatesItem() {
        await _updatableRepo.UpdateAsync(s => s.Name == "ZZ", new("K"));
        _updatableRepo.Count().Should().Be(3);
        _updatableRepo.FirstOrDefault(s => s.Name == "K").Should().BeNull();
    }

    [Fact]
    public void Update_WithEntity_UpdatedItem() {
        _updatableRepo.Update(new("Z") { Id = 2 });
        _updatableRepo.Count().Should().Be(3);
        _updatableRepo.FirstOrDefault(s => s.Name == "BB").Should().BeNull();
        _updatableRepo.FirstOrDefault(s => s.Name == "Z").Should().NotBeNull();
    }

    [Fact]
    public void Update_WithEntity_ForInvalidItem_UpdatedItem() {
        _updatableRepo.Update(new("K") { Id = 99 });
        _updatableRepo.Count().Should().Be(3);
        _updatableRepo.FirstOrDefault(s => s.Name == "K").Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_WithEntity_UpdatesItem() {
        await _updatableRepo.UpdateAsync(new("Z") { Id = 2 });
        _updatableRepo.Count().Should().Be(3);
        _updatableRepo.FirstOrDefault(s => s.Name == "BB").Should().BeNull();
        _updatableRepo.FirstOrDefault(s => s.Name == "Z").Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateAsync_WithEntity_ForInvalidItem_UpdatesItem() {
        await _updatableRepo.UpdateAsync(new("K") { Id = 99 });
        _updatableRepo.Count().Should().Be(3);
        _updatableRepo.FirstOrDefault(s => s.Name == "K").Should().BeNull();
    }
}
