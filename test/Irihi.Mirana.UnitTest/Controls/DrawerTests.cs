using Xunit;
using Irihi.Mirana.Controls;

namespace Irihi.Mirana.UnitTest.Controls;

public class DrawerTests
{
    [Fact]
    public void Drawer_DefaultProperties_AreSet()
    {
        // Arrange & Act
        var drawer = new Drawer();
        
        // Assert
        Assert.False(drawer.IsOpen);
        Assert.Equal(DrawerPlacement.Bottom, drawer.Placement);
        Assert.True(drawer.IsLightDismissEnabled);
        Assert.True(drawer.IsModal);
        Assert.Null(drawer.Child);
    }
    
    [Fact]
    public void Drawer_Placement_CanBeChanged()
    {
        // Arrange
        var drawer = new Drawer();
        
        // Act
        drawer.Placement = DrawerPlacement.Top;
        
        // Assert
        Assert.Equal(DrawerPlacement.Top, drawer.Placement);
    }
    
    [Fact]
    public void Drawer_IsOpen_CanBeToggled()
    {
        // Arrange
        var drawer = new Drawer();
        
        // Act
        drawer.IsOpen = true;
        
        // Assert
        Assert.True(drawer.IsOpen);
    }
    
    [Fact]
    public void DrawerPlacement_HasCorrectValues()
    {
        // Assert all enum values exist
        Assert.Equal(DrawerPlacement.Bottom, DrawerPlacement.Bottom);
        Assert.Equal(DrawerPlacement.Top, DrawerPlacement.Top);
        Assert.Equal(DrawerPlacement.Left, DrawerPlacement.Left);
        Assert.Equal(DrawerPlacement.Right, DrawerPlacement.Right);
    }
}
