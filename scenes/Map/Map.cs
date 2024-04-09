using Godot;
using System;

public partial class Map : Node2D
{
    public const uint TileSize = 64;
    private NetworkManager networkManager = null;
    public NetworkManager NetworkManager
    {
        get
        {
            return networkManager;
        }
        set
        {
            networkManager = value;

            if (networkManager != null)
            {
                if (Multiplayer.IsServer())
                {
                    networkManager.ServerClientLoggedIn += OnServerClientLoggedIn;
                }
            }
        }
    }

    private Node2D units = null;
    private Node2D players = null;
    private Node2D materials = null;
    public AStarComponent AStarComponent = null;
    private PackedScene playerScene = GD.Load<PackedScene>("res://scenes/Player/Player.tscn");
    private PackedScene workerScene = GD.Load<PackedScene>("res://scenes/units/minions/Worker/Worker.tscn");
    private PackedScene townhallScene = GD.Load<PackedScene>("res://scenes/units/buildings/Townhall/Townhall.tscn");
    private PackedScene treeScene = GD.Load<PackedScene>("res://scenes/materials/Tree/Tree.tscn");
    private PackedScene berriesScene = GD.Load<PackedScene>("res://scenes/materials/Berries/Berries.tscn");
    private PackedScene constructionScene = GD.Load<PackedScene>("res://scenes/units/buildings/Construction/Construction.tscn");

    public override void _Ready()
    {
        units = GetNode<Node2D>("%Units");
        players = GetNode<Node2D>("%Players");
        materials = GetNode<Node2D>("%Materials");

        AStarComponent = GetNode<AStarComponent>("%AStarComponent");

        units.ChildEnteredTree += OnUnitsChildEnteredTree;
        players.ChildEnteredTree += OnPlayersChildEnteredTree;
    }

    public Unit GetUnit(string name)
    {
        return units.GetNodeOrNull<Unit>(name);
    }

    public Player GetPlayer(string name)
    {
        return players.GetNodeOrNull<Player>(name);
    }

    public Material GetMaterial(string name)
    {
        return materials.GetNodeOrNull<Material>(name);
    }

    public void ServerStart()
    {
        ServerGenerateTerrain(new Vector2(-32, -32), 64, 64, 100, 50);
        AStarComponent.NavigationMap = GetNode<TileMap>("%NavigationTileMap");
        AStarComponent.CreatePathfindingPoints();
    }

    public void ServerCreateEntity(Player player, string entityName, Vector2 position, bool regenPathfinding = true)
    {
        bool triggerRegenPathfinding = false;

        switch (entityName)
        {
            case "Worker":
                Worker worker = (Worker)workerScene.Instantiate();
                worker.PlayerName = player.Name;
                worker.Map = this;
                worker.Position = position;
                units.AddChild(worker, true);
                break;
            case "Townhall":
                Townhall townhall = (Townhall)townhallScene.Instantiate();
                townhall.PlayerName = player.Name;
                townhall.Map = this;
                townhall.Position = SnapToGrid(position);
                units.AddChild(townhall, true);
                triggerRegenPathfinding = true;
                break;
            case "Tree":
                Tree tree = (Tree)treeScene.Instantiate();
                tree.Position = SnapToGrid(position);
                materials.AddChild(tree, true);
                triggerRegenPathfinding = true;
                break;
            case "Berries":
                Berries berries = (Berries)berriesScene.Instantiate();
                berries.Position = SnapToGrid(position);
                materials.AddChild(berries, true);
                triggerRegenPathfinding = true;
                break;
            default:
                GD.Print("Unknown unit name: " + entityName);
                break;
        }

        if (regenPathfinding && triggerRegenPathfinding)
        {
            AStarComponent.CreatePathfindingPoints();
        }

    }
    public Unit GetClosestStorage(string playerName, Vector2 position)
    {
        Unit closestStorage = null;
        float closestDistance = float.MaxValue;

        foreach (Node node in units.GetChildren())
        {
            if (node is Unit unit && unit.IsStorage && unit.PlayerName == playerName)
            {
                float distance = position.DistanceTo(unit.Position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestStorage = unit;
                }
            }
        }

        return closestStorage;
    }

    public bool ServerPlaceConstruction(Player player, string buildingName, Vector2 position)
    {

        Construction construction = (Construction)constructionScene.Instantiate();
        construction.PlayerName = player.Name;
        construction.Position = position;
        construction.BuildingName = buildingName;
        units.AddChild(construction, true);

        return true;
    }
    private void ServerGenerateTerrain(Vector2 startPos, uint width, uint height, uint amountOfTrees = 100, uint amountOfBerries = 50)
    {
        for (uint i = 0; i < amountOfTrees; i++)
        {
            Vector2 position = new Vector2(startPos.X * TileSize + (float)GD.RandRange(0, width) * TileSize, startPos.Y * TileSize + (float)GD.RandRange(0, height) * TileSize);
            ServerCreateEntity(null, "Tree", position, false);
        }

        for (uint i = 0; i < amountOfBerries; i++)
        {
            Vector2 position = new Vector2(startPos.X * TileSize + (float)GD.RandRange(0, width) * TileSize, startPos.Y * TileSize + (float)GD.RandRange(0, height) * TileSize);
            ServerCreateEntity(null, "Berries", position, false);
        }
    }

    private void OnServerClientLoggedIn(Client client)
    {
        Player player = (Player)playerScene.Instantiate();
        player.Map = this;
        player.Username = client.Username;
        player.PeerID = client.PeerID;
        players.AddChild(player, true);

        client.Player = player;
    }

    private void OnUnitsChildEnteredTree(Node child)
    {
        if (child is Unit unit)
        {
            unit.Map = this;
        }
    }

    private void OnPlayersChildEnteredTree(Node child)
    {
        if (child is Player player)
        {
            player.Map = this;
        }
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    public void CreateEntityRPC(string entityName, Vector2 position)
    {
        if (!Multiplayer.IsServer())
        {
            GD.Print("CreateEntityRPC called on non-server node. Ignoring.");
            return;
        }

        Player player = networkManager.GetPlayerViaPeerID(Multiplayer.GetRemoteSenderId());
        if (player == null)
        {
            GD.Print("Player not found");
            return;
        }

        position = SnapToGrid(position);

        ServerCreateEntity(player, entityName, position);
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    public void PlaceConstructionRPC(string buildingName, Vector2 position)
    {
        if (!Multiplayer.IsServer())
        {
            GD.Print("PlaceConstructionRPC called on non-server node. Ignoring.");
            return;
        }

        Player player = networkManager.GetPlayerViaPeerID(Multiplayer.GetRemoteSenderId());
        if (player == null)
        {
            GD.Print("Player not found");
            return;
        }

        switch (buildingName)
        {
            case "Townhall":
                if (!player.RemoveMaterials(Townhall.Price["Gold"], Townhall.Price["Food"]))
                {
                    GD.Print("Not enough materials to place townhall");
                    return;
                }
                break;
            default:
                GD.Print("Unknown building name: " + buildingName);
                return;
        }

        position = SnapToGrid(position);

        ServerPlaceConstruction(player, buildingName, position);
    }

    static public Vector2 SnapToGrid(Vector2 position)
    {
        return new Vector2((int)position.X / TileSize * TileSize + TileSize / 2, (int)position.Y / TileSize * TileSize + TileSize / 2);
    }
}
