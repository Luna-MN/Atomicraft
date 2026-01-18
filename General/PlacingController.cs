using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class PlacingController : Node2D
{
    [Export]
    public FactoryObject Target;

    [Export] private Vector2 snapSize = new Vector2(64, 64);
    public Vector2 SnapSize => snapSize;
    
    public Dictionary<Vector2, FactoryObject> PlacedFactoryObjects = new Dictionary<Vector2, FactoryObject>(); // <Position, FactoryObject>

    private bool IsDraging;
    private bool DragStarted;
    private bool Vertical;
    private Vector2 EndOfDragObjects;
    public List<FactoryObject> Ghosts = new List<FactoryObject>();
    // Debugging??
    [Export]
    private PackedScene FactoryObjectScene;

    public override void _Process(double delta)
    {
        if (Target != null && !IsDraging)
        {
            Placing();    
        }
        else if (Target != null && IsDraging)
        {
            if ((GetGlobalMousePosition().Snapped(SnapSize) != EndOfDragObjects && GetGlobalMousePosition().Snapped(SnapSize) != Target.Position) && !DragStarted)
            {
                StartDrag();
            }

            if (DragStarted)
            {
                Dragging();    
            }
            
        }
        else
        {
            Target = CreateNewGhost(new Vector2(0, 0)); // (0, 0)
            Target.MeshObject.Modulate = new Color(new Random().NextSingle(), new Random().NextSingle(), new Random().NextSingle(), 0.5f);
        }

    }
    private void Placing()
    {
        Vector2 MousePos = GetGlobalMousePosition();
        Vector2 SnappedMousePos = MousePos.Snapped(SnapSize);
        Target.Position = SnappedMousePos;
    }

    private void StartDrag()
    {
        Vector2 MousePos = GetGlobalMousePosition();
        Vector2 SnappedMousePos = MousePos.Snapped(SnapSize);
        if (EndOfDragObjects == Vector2.Zero)
        {
            EndOfDragObjects = Target.Position;
        }
        float XDif = Mathf.Abs(SnappedMousePos.X - EndOfDragObjects.X);
        float YDif = Mathf.Abs(SnappedMousePos.Y - EndOfDragObjects.Y);
        GD.Print($"X {XDif}, Y{YDif}");
        Vertical = XDif < YDif;
        DragStarted = true;
    }
    private void Dragging()
    {
        Vector2 MousePos = GetGlobalMousePosition();
        Vector2 SnappedMousePos = MousePos.Snapped(SnapSize);
        if (EndOfDragObjects == Vector2.Zero)
        {
            EndOfDragObjects = Target.Position;
        }
        
        bool CreateNewObject = CompareCreate(EndOfDragObjects, SnappedMousePos, Vertical);

        if (CreateNewObject)
        {
            FactoryObject ghost = CreateNewGhost(Vertical ? new Vector2(EndOfDragObjects.X, SnappedMousePos.Y) : new Vector2(SnappedMousePos.X, EndOfDragObjects.Y));
            if (!ghost.IsValid)
            {
                Ghosts.Remove(ghost);
                ghost.QueueFree();
            }
        }
        
        // compare target pos to end of drag objects, if mouse pos is not in end of drag object and its not creating then destroy
        else if(CompareDestroy(EndOfDragObjects, SnappedMousePos, Vertical))
        {
            // destroy the last in row
        }
    }
    private bool CompareCreate(Vector2 obj, Vector2 mousePos, bool vertical) // vertical == true => compare Y
    {
        if (vertical)
        {
            float maxTileY = float.NegativeInfinity;
            float minTileY = float.PositiveInfinity;
            foreach (var t in Target.Tiles)
            {
                if (t.Y > maxTileY) maxTileY = t.Y;
                if (t.Y < minTileY) minTileY = t.Y;
            }

            float worldMaxY = obj.Y + maxTileY * SnapSize.Y;
            float worldMinY = obj.Y + minTileY * SnapSize.Y;
            if (mousePos.Y < worldMinY) return true;
            if (mousePos.Y > worldMaxY) return true;
        }
        else
        {
            float maxTileX = float.NegativeInfinity;
            float minTileX = float.PositiveInfinity;
            foreach (var t in Target.Tiles)
            {
                if (t.X < minTileX) minTileX = t.X;
                if (t.X > maxTileX) maxTileX = t.X;
            }

            float worldMaxX = obj.X + maxTileX * SnapSize.X;
            float worldMinX = obj.X + minTileX * SnapSize.X;
            if (mousePos.X < worldMinX) return true;
            if (mousePos.X > worldMaxX) return true;
        }
        return false;
    }

    public bool CompareDestroy(Vector2 obj, Vector2 mousePos, bool vertical)
    {

        if (vertical)
        {
            float diff = Mathf.Abs(mousePos.Y - obj.Y);
            if (diff > SnapSize.Y / 2)
            {
                float distMouse = Mathf.Abs(mousePos.Y - Target.Position.Y);
                float distObj = Mathf.Abs(Target.Position.Y - obj.Y);
                if (distMouse < distObj)
                {
                    return true;
                }
            }
        }
        else
        {
            float diff = Mathf.Abs(mousePos.X - obj.X);
            if (diff > SnapSize.X / 2)
            {
                float distMouse = Mathf.Abs(mousePos.X - Target.Position.X);
                float distObj = Mathf.Abs(Target.Position.X - obj.X);
                if (distMouse < distObj)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("click"))
        {
            // drag multi placing
            IsDraging = true;
        }
        else if (@event.IsActionReleased("click"))
        {
            IsDraging = false;
            EndOfDragObjects = Vector2.Zero;
            DragStarted = false;
            placeObjects();
            Ghosts.Clear();
        }
    }
    public void placeObjects()
    {

        foreach (FactoryObject ghost in Ghosts.ToList())
        {
            Color TargColor = Target.MeshObject.Modulate;
            ghost.MeshObject.Modulate = new Color(TargColor.R, TargColor.G, TargColor.B);
            if (ghost.IsValid)
            {
                Ghosts.Remove(ghost);
                foreach (Vector2 tile in ghost.Tiles)
                {
                    PlacedFactoryObjects.Add(ghost.Position + (tile * SnapSize), ghost);
                }
            }
            else
            {
                Ghosts.Remove(ghost);
                ghost.QueueFree();
            }
        }

        Target = null;
    }

    public FactoryObject CreateNewGhost(Vector2 position)
    {
        FactoryObject newGhost = FactoryObjectScene.Instantiate<FactoryObject>();
        if (Target != null)
        {
            newGhost.MeshObject.Modulate = Target.MeshObject.Modulate;
        }
        newGhost.Position = position;
        AddChild(newGhost);
        Ghosts.Add(newGhost);
        return newGhost;
    }
}
