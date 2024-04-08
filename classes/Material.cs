using Godot;
using System;

public partial class Material : StaticBody2D
{
    public virtual string MaterialType { get; set; } = "";
    protected MultiplayerSynchronizer synchronizer = null;
    protected SceneReplicationConfig sceneReplicationConfig = null;
    public Material()
    {
        sceneReplicationConfig = new SceneReplicationConfig();
        AddReplicationProperty(".:position", true, SceneReplicationConfig.ReplicationMode.OnChange);

        synchronizer = new MultiplayerSynchronizer();
        synchronizer.Name = "Synchronizer";
        synchronizer.DeltaInterval = 0.1f;
        synchronizer.ReplicationConfig = sceneReplicationConfig;
        AddChild(synchronizer);
    }
    public void AddReplicationProperty(string property, bool spawn = true, SceneReplicationConfig.ReplicationMode mode = SceneReplicationConfig.ReplicationMode.OnChange)
    {
        sceneReplicationConfig.AddProperty(property);
        sceneReplicationConfig.PropertySetSpawn(property, spawn);
        sceneReplicationConfig.PropertySetReplicationMode(property, mode);
    }
}
