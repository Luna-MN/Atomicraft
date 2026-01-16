using Godot;
using System;
using System.Collections.Generic;

public partial class PlacingController : Node2D
{
    [Export]
    public FactoryObject Target;

    [Export] private Vector2 snapSize = new Vector2(64, 64);
    public Vector2 SnapSize => snapSize;
    
    public Dictionary<Vector2, FactoryObject> PlacedFactoryObjects = new Dictionary<Vector2, FactoryObject>(); // <Position, FactoryObject>
    
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
            Target.MeshObject.Modulate = new Color(new Random().NextSingle(), new Random().NextSingle(), new Random().NextSingle(), 0.5f);
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
            Color TargColor = Target.MeshObject.Modulate;
            Target.MeshObject.Modulate = new Color(TargColor.R, TargColor.G, TargColor.B);
            foreach (Vector2 tile in Target.Tiles)
            {
                PlacedFactoryObjects.Add(Target.Position + (tile * SnapSize), Target);
            }
            Target = null;
        }
    }
}
