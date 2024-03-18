using Godot;
using System;

public partial class ConnectScreen : Control
{
    [Signal]
    public delegate void ConnectButtonPressedEventHandler(string address, int port);

    private LineEdit addressLineEdit;
    private LineEdit portLineEdit;

    private Label errorLabel;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        addressLineEdit = GetNode<LineEdit>("%AddressValue");
        portLineEdit = GetNode<LineEdit>("%PortValue");
        errorLabel = GetNode<Label>("%ErrorLabel");

        Button connectButton = GetNode<Button>("%ConnectButton");
        connectButton.Pressed += OnConnectButtonPressed;
    }

    private void OnConnectButtonPressed()
    {
        EmitSignal(SignalName.ConnectButtonPressed, addressLineEdit.Text, int.Parse(portLineEdit.Text));
    }

    public void ShowError(string message)
    {
        errorLabel.Text = message;
    }
}
