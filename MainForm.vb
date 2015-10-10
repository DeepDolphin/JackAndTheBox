Public Class MainForm
    Public ReadOnly Property Version As String
        Get
            Return "Version 1.0.0_04 Beta"
        End Get
    End Property

    Public UpPressed As Boolean
    Public DownPressed As Boolean
    Public RightPressed As Boolean
    Public LeftPressed As Boolean
    Public ControlPressed As Boolean

    Public Player As Person
    Public GroundBrush As TextureBrush
    Public WallBrush As TextureBrush
    Public Random As New Random(0)
    Public ViewOffsetX As Double
    Public ViewOffsetY As Double
    Public World As World
    Public ReadOnly Property ScreenWidth As Integer
        Get
            Return ClientSize.Width
        End Get
    End Property
    Public ReadOnly Property ScreenHeight As Integer
        Get
            Return ClientSize.Height
        End Get
    End Property

    Public ReadOnly Property PlayerRoom As Room
        Get
            Return World.RoomAt(Player.X + Player.Room.XOffset, Player.Y + Player.Room.YOffset)
        End Get
    End Property

    Public Loaded As Boolean = False

    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.DoubleBuffered = True

        'Environment.Add(New RectangleF(-1, 0, 1, ScreenHeight))
        'Environment.Add(New RectangleF(0, -1, ScreenWidth, 1))
        'Environment.Add(New RectangleF(ScreenWidth, -1, 1, ScreenHeight))
        'Environment.Add(New RectangleF(-1, ScreenHeight, ScreenWidth, -1))

        GroundBrush = New TextureBrush(My.Resources.FloorTile)
        WallBrush = New TextureBrush(My.Resources.WallStrip)

        ' Load the rooms that we have.
        Dim rooms As New List(Of Room)
        For Each s As String In IO.Directory.EnumerateFiles("Rooms\")
            If IO.Path.GetFileNameWithoutExtension(s) = "up" OrElse
                    IO.Path.GetFileNameWithoutExtension(s) = "down" OrElse
                    IO.Path.GetFileNameWithoutExtension(s) = "left" OrElse
                    IO.Path.GetFileNameWithoutExtension(s) = "right" Then
                Continue For
            End If
            Dim r As New Room(s)
            rooms.Add(r)
        Next

        ' Generate the world to play in
        World = New World("DavidAndBen", rooms)

        ' Load the player and testing stuff
        Player = New Person(World.RoomAt(150, 150), 150, 150, 1)
        Dim TestObject2 = New NormalEnemy(PlayerRoom, 100, 100)
        World.Rooms(0).AddGameObject(Player)
        World.Rooms(0).AddGameObject(TestObject2)


        Loaded = True ' Keep the timer from firing until the game is done loading.
    End Sub

    Private Sub MainForm_Paint(sender As Object, e As PaintEventArgs) Handles Me.Paint
        For Each r As Room In World.Rooms
            e.Graphics.FillRectangle(WallBrush, CSng(-ViewOffsetX + r.XOffset), CSng(-ViewOffsetY + r.YOffset - 32), r.Bounds.Width, 32)
            e.Graphics.FillRectangle(GroundBrush, CSng(-ViewOffsetX + r.XOffset), CSng(-ViewOffsetY + r.YOffset), r.Bounds.Width, r.Bounds.Height)
            e.Graphics.DrawImage(My.Resources.GradientLeft, CSng(-ViewOffsetX + r.XOffset), CSng(-ViewOffsetY + r.YOffset - 32), 64, 32)
            e.Graphics.DrawImage(My.Resources.GradientRight, CSng(-ViewOffsetX + r.XOffset + r.Width - 63), CSng(-ViewOffsetY + r.YOffset - 32), 64, 32)
        Next
        For Each r As Room In World.Rooms
            For Each o As GameObject In r.GameObjects
                If o.CastsShadow Then e.Graphics.DrawImage(My.Resources.Shadow, CSng(o.X - ViewOffsetX + r.XOffset), CSng(o.Y + o.Image.Height - 7 - ViewOffsetY + r.YOffset), o.Image.Width, 10)
            Next
            For Each O As GameObject In r.GameObjects
                e.Graphics.DrawImage(O.Image, CSng(O.X - ViewOffsetX + r.XOffset), CSng(O.Y + O.Z * (10 / 16) - ViewOffsetY + r.YOffset), O.Image.Width, O.Image.Height)
            Next
            e.Graphics.DrawString(IO.Path.GetFileName(r.Filename), SystemFonts.CaptionFont, Brushes.Red, CSng(-ViewOffsetX + r.XOffset), CSng(-ViewOffsetY + r.YOffset))
        Next
    End Sub

    Private Sub Timer_Tick(sender As Object, e As EventArgs) Handles Timer.Tick
        If Loaded = False Then Exit Sub
        Invalidate()
        UpdateWorld()
    End Sub

    Public Sub UpdateWorld()
        If ControlPressed Then
            Player.Speed = 3
        Else
            Player.Speed = 2
        End If

        For Each r As Room In World.Rooms
            For Each O As GameObject In r.GameObjects
                Dim newx As Double = O.X + O.XSpeed
                Dim newy As Double = O.Y + O.YSpeed
                If (O.Equals(Player)) Then
                    If UpPressed Then
                        newy -= Player.Speed
                        Player.Direction = Person.PersonDirection.Up
                    End If
                    If DownPressed Then
                        newy += Player.Speed
                        Player.Direction = Person.PersonDirection.Down
                    End If
                    If RightPressed Then
                        newx += Player.Speed
                        Player.Direction = Person.PersonDirection.Right
                    End If
                    If LeftPressed Then
                        newx -= Player.Speed
                        Player.Direction = Person.PersonDirection.Left
                    End If
                End If
                Dim good As Boolean = True
                For Each other As GameObject In r.GameObjects
                    If other.Equals(O) Then Continue For
                    If other.CollidesWith(O, newx, newy) Then
                        good = False
                        Exit For
                    End If
                Next
                If New RectangleF(0, 0, r.Width, r.Height).Contains(New RectangleF(newx + O.HitBox.X, newy + O.HitBox.Y, O.HitBox.Width, O.HitBox.Height)) = False Then good = False
                If good Then
                    O.X = newx
                    O.Y = newy
                End If
                O.Update()
            Next
            r.ResortGameObjects()
        Next

        If IsNothing(PlayerRoom) = False Then
            If Player.Room.Equals(PlayerRoom) = False Then
                Dim oldroom As Room = Player.Room
                Dim newroom As Room = PlayerRoom
                Player.X += (oldroom.XOffset - newroom.XOffset)
                Player.Y += (oldroom.YOffset - newroom.YOffset)
                oldroom.GameObjects.Remove(Player)
                newroom.GameObjects.Add(Player)
                Player.Room = newroom
            End If
        End If

        ViewOffsetX = Player.X + Player.Room.XOffset - (ScreenWidth / 2 - Player.HitBox.Width / 2)
        ViewOffsetY = Player.Y + Player.Room.YOffset - (ScreenHeight / 2 - Player.HitBox.Height / 2)
        GroundBrush.ResetTransform()
        GroundBrush.TranslateTransform(-Player.X Mod My.Resources.FloorTile.Width, -Player.Y Mod My.Resources.FloorTile.Height)
        WallBrush.ResetTransform()
        WallBrush.TranslateTransform(-Player.X Mod My.Resources.WallStrip.Width - 4, -Player.Y Mod My.Resources.WallStrip.Height + 2)
    End Sub

    Private Sub MainForm_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown

        Select Case e.KeyCode
            Case Keys.Up
                UpPressed = True
            Case Keys.Down
                DownPressed = True
            Case Keys.Left
                LeftPressed = True
            Case Keys.Right
                RightPressed = True
            Case Keys.ControlKey
                ControlPressed = True
            Case Keys.Space
                Dim X As Integer
                Dim Y As Integer
                Select Case Player.Direction
                    Case Person.PersonDirection.Down
                        X = Player.X
                        Y = Player.Y - My.Resources.Crate.Height + 20
                    Case Person.PersonDirection.Up
                        X = Player.X
                        Y = Player.Y + Player.Image.Height + 1
                    Case Person.PersonDirection.Left
                        X = Player.X + Player.Image.Width
                        Y = Player.Y + Player.Image.Height - My.Resources.Crate.Height
                    Case Person.PersonDirection.Right
                        X = Player.X - My.Resources.Crate.Width - 1
                        Y = Player.Y + Player.Image.Height - My.Resources.Crate.Height
                End Select
                Dim newcrate As New GameObject(My.Resources.Crate, Player.Room, X, Y)
                Dim good As Boolean = True

                For Each o As GameObject In Player.Room.GameObjects
                    If o.CollidesWith(newcrate, X, Y) Then
                        good = False
                        Exit For
                    End If
                Next
                If good Then
                    Player.Room.AddGameObject(newcrate)
                End If
        End Select
    End Sub

    Private Sub MainForm_KeyUp(sender As Object, e As KeyEventArgs) Handles Me.KeyUp

        Select Case e.KeyCode
            Case Keys.Up
                UpPressed = False
            Case Keys.Down
                DownPressed = False
            Case Keys.Left
                LeftPressed = False
            Case Keys.Right
                RightPressed = False
            Case Keys.ControlKey
                ControlPressed = False
        End Select
    End Sub

    Protected Overrides Function IsInputKey(
        ByVal keyData As System.Windows.Forms.Keys) As Boolean
        Return True

    End Function

End Class
