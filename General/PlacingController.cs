using Godot;
using System;

public partial class PlacingController : Node2D
{
    [Export]
    public Node2D Target;

    public override void _Process(double delta)
    {
        if (Target != null)
        {
            Placing();    
        }
    }
    private void Placing()
    {
        Vector2 MousePos = GetGlobalMousePosition();
        Vector2 SnappedMousePos = MousePos.Snapped(new Vector2(16, 16));
        Target.Position = SnappedMousePos;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("click"))
        {
            Target = null;
        }
    }
}
