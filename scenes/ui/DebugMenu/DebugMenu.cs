using Godot;
using System;

public partial class DebugMenu : Control
{
    public Player Player { get; set; } = null;
    public Map Map { get; set; } = null;
    private bool selected = false;
    private Button workerButton = null;
    private PanelContainer previewPanel = null;
    private Label previewLabel = null;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        workerButton = GetNode<Button>("%WorkerButton");
        workerButton.Pressed += OnWorkerButtonPressed;

        previewPanel = GetNode<PanelContainer>("%PreviewPanel");
        previewLabel = GetNode<Label>("%PreviewLabel");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (selected)
        {
            previewPanel.Position = GetLocalMousePosition();
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustReleased("left_click"))
        {
            // TODO: only reset when not above UI
            // ResetSelected();
        }
        else if (Input.IsActionJustReleased("right_click"))
        {
            HandleSelected();
            ResetSelected();
        }
    }

    private void HandleSelected()
    {
        if (!selected)
        {
            return;
        }

        switch (previewLabel.Text)
        {
            case "Worker":
                Map.RpcId(1, Map.MethodName.CreateWorkerRPC, Player.GetGlobalMousePosition());
                return;
            default:
                return;
        }
    }

    private void ResetSelected()
    {
        if (!selected)
        {
            return;
        }

        previewPanel.Hide();
        previewLabel.Text = "";
        selected = false;
    }

    private void OnWorkerButtonPressed()
    {
        if (!selected)
        {
            previewPanel.Show();
            previewLabel.Text = "Worker";
            selected = true;
        }
    }
}
