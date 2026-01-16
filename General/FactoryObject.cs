using Godot;
using System;
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
        private set { }
    }
    protected bool _isValid;
    [Export]
    public MeshInstance2D MeshObject;
    protected PlacingController PlacingController;
    public override void _Ready()
    {
        PlacingController = GetParent<PlacingController>();
    }
    protected virtual bool isValid()
    {
        if (PlacingController.PlacedFactoryObjects.ContainsKey(Position))
        {
            _isValid = false;
        }
        return _isValid;
    }

}
