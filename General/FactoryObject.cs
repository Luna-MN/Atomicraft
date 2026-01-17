using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class FactoryObject : Node2D
{
    [Export]
    public bool IsValid
    {
        get
        {
            _isValid = true;
            return isValid();
        }
        private set => _isValid = value;
    }
    protected bool _isValid;
    [Export]
    public MeshInstance2D MeshObject;
    [Export]
    public Vector2[] Tiles; // = position + (List[i] * gridsize)
    protected PlacingController PlacingController;

    public override void _Ready()
    {
        PlacingController = GetParent<PlacingController>();
    }
    protected virtual bool isValid()
    {
        foreach (Vector2 tile in Tiles)
        {
            if (PlacingController.PlacedFactoryObjects.ContainsKey(Position + (tile * PlacingController.SnapSize)))
            {
                _isValid = false;
            }
        }

        return _isValid;
    }

}
