Public Class Enemy
    Inherits GameObject

    Public Sub New(x As Double, y As Double)
        MyBase.New(My.Resources.CharacterUp1, x, y) ' TODO: Write a decent constructor.
    End Sub

    Public Overrides Sub Update()
        MyBase.Update()
        Dim radians As Double = Math.Atan2(MainForm.Player.Y - Y, MainForm.Player.X - X)
        Dim deg As Double = (radians * 360) / (Math.PI * 2)
        XSpeed = 2 * Math.Cos(deg)
        YSpeed = 2 * Math.Sign(deg)
    End Sub
End Class
