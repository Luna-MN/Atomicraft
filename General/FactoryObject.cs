using Godot;
using System;
[GlobalClass]
public partial class FactoryObject : Node2D
{
    [Export]
    public bool IsValid
    {
        get => isValid();
        private set { }
    }
    [Export]
    public MeshInstance2D MeshObject;
    protected virtual bool isValid()
    {
        return true;
    }
}
