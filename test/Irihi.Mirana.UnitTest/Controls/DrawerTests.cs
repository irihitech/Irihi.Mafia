using System.Linq;
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
    public void DrawerPlacement_HasAllExpectedValues()
    {
        // Arrange
        var expectedValues = new[] { 
            DrawerPlacement.Bottom, 
            DrawerPlacement.Top, 
            DrawerPlacement.Left, 
            DrawerPlacement.Right 
        };
        
        // Act
        var actualValues = Enum.GetValues(typeof(DrawerPlacement)).Cast<DrawerPlacement>().ToArray();
        
        // Assert
        Assert.Equal(4, actualValues.Length);
        Assert.Contains(DrawerPlacement.Bottom, actualValues);
        Assert.Contains(DrawerPlacement.Top, actualValues);
        Assert.Contains(DrawerPlacement.Left, actualValues);
        Assert.Contains(DrawerPlacement.Right, actualValues);
    }
}
