using Godot;
using System;

public partial class PlacingController : Node2D
{
    [Export]
    public FactoryObject Target;
    [Export]
    private Vector2 SnapSize = new Vector2(64, 64);
    
    
    // Debugging??
    [Export]
    private PackedScene FactoryObjectScene;

    public override void _Process(double delta)
    {
        if (Target != null)
        {
            Placing();    
        }
        else
        {
            Target = FactoryObjectScene.Instantiate<FactoryObject>();
            Target.MeshObject.Modulate = new Color(new Random().NextSingle(), new Random().NextSingle(), new Random().NextSingle(), 1);
            AddChild(Target);
        }
    }
    private void Placing()
    {
        Vector2 MousePos = GetGlobalMousePosition();
        Vector2 SnappedMousePos = MousePos.Snapped(SnapSize);
        Target.Position = SnappedMousePos;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("click") && Target.IsValid)
        {
            Target = null;
        }
    }
}
