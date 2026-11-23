using ModuleXmlReader.Presentation.Services;
using Xunit;
namespace ModuleXmlReader.CharacterizationTests;
public sealed class LightEnvelopeTests
{
    [Fact]
    public void LightRisesAndDecaysGraduallyWithoutJumpingWhenTargetChanges()
    {
        var light = new LightEnvelope { Target = 1 };
        light.Advance(0.016); Assert.InRange(light.Value, 0.1, 0.4);
        var first = light.Value; light.Advance(0.016); Assert.True(light.Value > first);
        var before = light.Value; light.Target = 0; Assert.Equal(before, light.Value);
        light.Advance(0.016); Assert.InRange(light.Value, 0.01, before);
        light.Advance(2); Assert.Equal(0, light.Value); Assert.True(light.IsSettled);
    }
    [Fact]
    public void EnvelopeDependsOnElapsedTimeRatherThanFrameRate()
    {
        var a = new LightEnvelope { Target = 1 }; var b = new LightEnvelope { Target = 1 };
        a.Advance(0.16); for(int i=0;i<10;i++) b.Advance(0.016);
        Assert.Equal(a.Value,b.Value,10);
        a.Target=b.Target=0; a.Advance(0.16); for(int i=0;i<10;i++) b.Advance(0.016);
        Assert.Equal(a.Value,b.Value,10);
        var before=a.Value; a.Advance(double.NaN); a.Advance(-1); Assert.Equal(before,a.Value);
    }
}
