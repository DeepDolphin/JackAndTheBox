Public Class MainForm
    Public UpPressed As Boolean
    Public DownPressed As Boolean
    Public RightPressed As Boolean
    Public LeftPressed As Boolean
    Public ControlPressed As Boolean

    Public Player As Person
    Public GroundBrush As TextureBrush
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

    Public Function RoomAt(X As Double, Y As Double) As Room
        For Each r As Room In World.Rooms
            If r.Bounds.Contains(X, Y) Then
                Return r
            End If
        Next
        Return Nothing
    End Function

    Public ReadOnly Property PlayerRoom As Room
        Get
            Return RoomAt(Player.X, Player.Y)
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

        ' Load the rooms that we have.
        Dim rooms As New List(Of Room)
        For Each s As String In IO.Directory.EnumerateFiles("Rooms\")
            Dim r As New Room(s)
            rooms.Add(r)
        Next

        ' Generate the world to play in
        World = New World("DavidAndBen", rooms, Nothing, Nothing, Nothing, Nothing)

        ' Load the player and testing stuff
        Player = New Person(ScreenWidth / 2, ScreenHeight / 2, 1)
        Dim TestObject2 = New Enemy(100, 100)
        World.Rooms(0).AddGameObject(Player)
        World.Rooms(0).AddGameObject(TestObject2)


        Loaded = True ' Keep the timer from firing until the game is done loading.
    End Sub

    Private Sub MainForm_Paint(sender As Object, e As PaintEventArgs) Handles Me.Paint
        For Each r As Room In World.Rooms
            e.Graphics.FillRectangle(GroundBrush, CSng(-ViewOffsetX + r.XOffset), CSng(-ViewOffsetY + r.YOffset), ScreenWidth, ScreenHeight)
            For Each o As GameObject In r.GameObjects
                If o.CastsShadow Then e.Graphics.DrawImage(My.Resources.Shadow, CSng(o.X - ViewOffsetX + r.XOffset), CSng(o.Y + o.Image.Height - 7 - ViewOffsetY + r.YOffset), o.Image.Width, 10)
            Next
            For Each O As GameObject In r.GameObjects
                e.Graphics.DrawImage(O.Image, CSng(O.X - ViewOffsetX + r.XOffset), CSng(O.Y + O.Z * (10 / 16) - ViewOffsetY + r.YOffset), O.Image.Width, O.Image.Height)
            Next
        Next
    End Sub

    Private Sub Timer_Tick(sender As Object, e As EventArgs) Handles Timer.Tick
        If Loaded = False Then Exit Sub
        Invalidate()
        UpdateWorld()
    End Sub

    Public Sub UpdateWorld()
        If ControlPressed Then
            Player.Speed = 5
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
                'For Each rectangle As RectangleF In Environment
                '    Dim otherhitbox As RectangleF = O.HitBox
                '    otherhitbox.X = newx
                '    otherhitbox.Y = newy
                '    If (otherhitbox.IntersectsWith(rectangle)) Then
                '        good = False
                '        Exit For
                '    End If
                'Next
                If good Then
                    O.X = newx
                    O.Y = newy
                End If
                O.Update()
            Next
            r.ResortGameObjects()
        Next

        ViewOffsetX = Player.X - (ScreenWidth / 2 - Player.HitBox.Width / 2)
        ViewOffsetY = Player.Y - (ScreenHeight / 2 - Player.HitBox.Height / 2)
        GroundBrush.ResetTransform()
        GroundBrush.TranslateTransform(-Player.X Mod My.Resources.FloorTile.Width, -Player.Y Mod My.Resources.FloorTile.Height)
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
                    Case Person.PersonDirection.Up
                        X = Player.X
                        Y = Player.Y - My.Resources.Crate.Height + 20
                    Case Person.PersonDirection.Down
                        X = Player.X
                        Y = Player.Y + Player.Image.Height + 1
                    Case Person.PersonDirection.Right
                        X = Player.X + Player.Image.Width
                        Y = Player.Y + Player.Image.Height - My.Resources.Crate.Height + 10
                    Case Person.PersonDirection.Left
                        X = Player.X - My.Resources.Crate.Width - 1
                        Y = Player.Y + Player.Image.Height - My.Resources.Crate.Height + 10
                End Select
                Dim newcrate As New GameObject(My.Resources.Crate, X, Y)
                Dim good As Boolean = True

                For Each o As GameObject In PlayerRoom.GameObjects
                    If o.CollidesWith(newcrate, X, Y) Then
                        good = False
                        Exit For
                    End If
                Next
                If good Then
                    PlayerRoom.AddGameObject(newcrate)
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
