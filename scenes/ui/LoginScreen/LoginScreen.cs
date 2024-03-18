using Godot;
using System;

public partial class LoginScreen : Control
{
    [Signal]
    public delegate void LoginButtonPressedEventHandler(string username, string password);

    private LineEdit usernameLineEdit;
    private LineEdit passwordLineEdit;
    private Label errorLabel;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        usernameLineEdit = GetNode<LineEdit>("%UsernameValue");
        passwordLineEdit = GetNode<LineEdit>("%PasswordValue");
        errorLabel = GetNode<Label>("%ErrorLabel");


        Button loginButton = GetNode<Button>("%LoginButton");
        loginButton.Pressed += OnLoginButtonPressed;
    }

    private void OnLoginButtonPressed()
    {
        EmitSignal(SignalName.LoginButtonPressed, usernameLineEdit.Text, passwordLineEdit.Text);
    }

    public void ShowError(string message)
    {
        errorLabel.Text = message;
    }
}
