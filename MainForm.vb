Public Class MainForm
    Public Const Version As String = "Version 1.0.1_00 Beta"

    Public ToAddWaitlist As New Dictionary(Of GameObject, Room)
    Public ShadeBrush As SolidBrush
    Public Player As Player
    Public Mouse As PointF
    Public GroundBrush As TextureBrush
    Public WallBrush As TextureBrush
    Public Random As New Random(0)
    Public ViewOffsetX As Double
    Public ViewOffsetY As Double
    Public World As World
    Public Options As Options

    Public test As Double

    Public ReadOnly Property MaxTick As Double
        Get
            Return 1 / Options.Preferences("MaxFPS")
        End Get
    End Property
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
            Return World.RoomAt(Player.Middle.X + Player.Room.XOffset, Player.Middle.Y + Player.Room.YOffset)
        End Get
    End Property

    Public Loaded As Boolean = False

    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.DoubleBuffered = True

        GroundBrush = New TextureBrush(My.Resources.FloorTile)
        WallBrush = New TextureBrush(My.Resources.WallStrip)
        ShadeBrush = New SolidBrush(Color.FromArgb(175, 0, 0, 0))

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

        LoadFiles()

        ' Generate the world to play in
        World = New World("DavidAndBen", rooms)

        ' Load the player and testing stuff
        Player = New Player(World.RoomAt(150, 150))
        Dim TestObject1 = New EXPOrb(100, PlayerRoom, New Vector2(200, 200))
        Dim TestObject2 = New NormalEnemy(PlayerRoom, 100, 100)

        World.Rooms(0).AddGameObject(Player)
        World.Rooms(0).AddGameObject(TestObject1)
        'World.Rooms(0).AddGameObject(TestObject2)


        Loaded = True ' Keep the timer from firing until the game is done loading.
        Watch = New Stopwatch()
        Watch.Start()
    End Sub

    Private Sub LoadFiles()
        Options = New Options()
    End Sub

    Private Sub MainForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        Options.SaveOptions()
    End Sub

    Private Sub MainForm_Paint(sender As Object, e As PaintEventArgs) Handles Me.Paint
        For Each r As Room In World.Rooms
            e.Graphics.FillRectangle(WallBrush, CInt(-ViewOffsetX + r.XOffset), CInt(-ViewOffsetY + r.YOffset - 32), r.Bounds.Width, 32)
            e.Graphics.FillRectangle(GroundBrush, CInt(-ViewOffsetX + r.XOffset), CInt(-ViewOffsetY + r.YOffset), r.Bounds.Width, r.Bounds.Height)
            e.Graphics.DrawImage(My.Resources.GradientLeft, CInt(-ViewOffsetX + r.XOffset), CInt(-ViewOffsetY + r.YOffset - 32), 64, 32)
            e.Graphics.DrawImage(My.Resources.GradientRight, CInt(-ViewOffsetX + r.XOffset + r.Width - 63), CInt(-ViewOffsetY + r.YOffset - 32), 64, 32)
            If r.Equals(Player.Room) = False Then
                e.Graphics.FillRectangle(ShadeBrush, CInt(-ViewOffsetX + r.XOffset), CInt(-ViewOffsetY + r.YOffset - 32), r.Bounds.Width, r.Bounds.Height + 32)
            End If
        Next
        For Each r As Room In World.Rooms
            If r.Equals(Player.Room) Then
                For Each o As GameObject In r.GameObjects
                    If o.CastsShadow Then e.Graphics.DrawImage(My.Resources.Shadow, CInt(o.Position.X - ViewOffsetX + r.XOffset), CInt(o.Position.Y + o.Sprite.Height - 7 - ViewOffsetY + r.YOffset), o.Sprite.Width, 10)
                Next
                For Each O As GameObject In r.GameObjects
                    Try
                        e.Graphics.DrawImage(O.Sprite.CurrentFrame, CInt(O.Position.X - ViewOffsetX + r.XOffset), CInt(O.Position.Y + O.Position.Z * (10 / 16) - ViewOffsetY + r.YOffset), O.Sprite.Width, O.Sprite.Height)

                    Catch ex As Exception
                        Stop
                    End Try
                Next
            End If
            e.Graphics.DrawString(IO.Path.GetFileName(r.Filename), SystemFonts.CaptionFont, Brushes.Red, CSng(-ViewOffsetX + r.XOffset), CSng(-ViewOffsetY + r.YOffset))
        Next
        e.Graphics.DrawString(Version, SystemFonts.CaptionFont, Brushes.Red, 0, 0)
        e.Graphics.DrawString(CInt(1 / Tick), SystemFonts.CaptionFont, Brushes.Red, 0, 10)
        e.Graphics.DrawString(Player.Properties("Health"), SystemFonts.CaptionFont, Brushes.Red, 0, 20)
        e.Graphics.DrawString(Player.Properties("CurrentStamina"), SystemFonts.CaptionFont, Brushes.Red, 0, 30)
        e.Graphics.DrawString(test, SystemFonts.CaptionFont, Brushes.Red, 0, 40)
    End Sub

    Private Watch As Stopwatch
    Private _tick As Double = 0.5
    Public ReadOnly Property Tick As Double
        Get
            Return _tick
        End Get
    End Property
    Private Sub Timer_Tick(sender As Object, e As EventArgs) Handles Timer.Tick
        If Loaded = False Then Exit Sub
        Invalidate()
        While Watch.Elapsed.TotalSeconds < MaxTick
            Threading.Thread.Sleep(1)
        End While
        If (Not Options.OIStatus("Pause")) Then
            UpdateWorld(Watch.Elapsed.TotalSeconds)
        Else
            UpdateWorld(Watch.Elapsed.TotalSeconds / 25) 'Slow down time like a boss [For testing purposes] (maybe an added feature?)
        End If
        _tick = Watch.Elapsed.TotalSeconds
        Watch.Restart()
    End Sub

    Public Sub UpdateWorld(t As Double)
        'Moving all objects
        Dim r As Room = Player.Room

        For Each O As GameObject In r.GameObjects
                If (Not (O.Speed.X = 0 AndAlso O.Speed.Y = 0)) Then
                    Dim newx As Double = O.Position.X + (O.Speed.X * t * If(O.HitBox.Width > O.HitBox.Height, O.HitBox.Width, O.HitBox.Height))
                    Dim newy As Double = O.Position.Y + (O.Speed.Y * t * If(O.HitBox.Width > O.HitBox.Height, O.HitBox.Width, O.HitBox.Height))
                    Dim good As Boolean = True
                    For Each other As GameObject In r.GameObjects
                        If other.Equals(O) Then Continue For
                        If other.CollidesWith(O, New Vector2(newx, newy)) Then
                        good = False
                        If (O.Flags.Contains("Actor")) Then CType(O, Actor).Hit(other)
                        Exit For
                        End If
                    Next
                    If New RectangleF(0, 0, r.Width, r.Height).Contains(New RectangleF(newx + O.HitBox.X, newy + O.HitBox.Y, O.HitBox.Width, O.HitBox.Height)) = False Then good = False
                    If good Then
                        O.Position.X = newx
                        O.Position.Y = newy
                    End If
                End If
                O.Update(t)
            Next

        'Add all objects waiting to be added
        For value As Integer = ToAddWaitlist.Count - 1 To 0 Step -1
            Dim GameObject As GameObject = ToAddWaitlist.Keys(value)
            If (ToAddWaitlist(GameObject).Equals(r)) Then
                If GameObject.Position.X < 0 OrElse GameObject.Position.Y + 16 < 0 OrElse GameObject.Position.X + GameObject.HitBox.Width > r.Width OrElse GameObject.Position.Y + GameObject.HitBox.Height > r.Height Then
                    ToAddWaitlist.Remove(GameObject)
                    Continue For
                End If

                Dim good As Boolean = True
                    For Each o As GameObject In r.GameObjects
                    If o.CollidesWith(GameObject, GameObject.Position) Then
                        good = False
                        Exit For
                    End If
                Next
                    If good Then
                        r.AddGameObject(GameObject)
                        ToAddWaitlist.Remove(GameObject)
                    Else
                        ToAddWaitlist.Remove(GameObject)
                    End If
                End If
        Next

        'Delete all items flagged for deletion
        For value As Integer = r.GameObjects.Count - 1 To 0 Step -1
                If r.GameObjects(value).Flags.Contains("Delete") Then
                    r.GameObjects.RemoveAt(value)
                End If
            Next

            For Each Objective As Objective In r.Objectives
                Objective.Update(t)
            Next

        r.ResortGameObjects()

        If IsNothing(PlayerRoom) = False Then
            If Player.Room.Equals(PlayerRoom) = False Then
                Dim oldroom As Room = Player.Room
                Dim newroom As Room = PlayerRoom
                Player.Position.X += (oldroom.XOffset - newroom.XOffset)
                Player.Position.Y += (oldroom.YOffset - newroom.YOffset)
                oldroom.GameObjects.Remove(Player)
                newroom.GameObjects.Add(Player)
                Player.Room = newroom
            End If
        End If

        ViewOffsetX = Player.Position.X + Player.Room.XOffset - (ScreenWidth / 2 - Player.HitBox.Width / 2)
        ViewOffsetY = Player.Position.Y + Player.Room.YOffset - (ScreenHeight / 2 - Player.HitBox.Height / 2)
        GroundBrush.ResetTransform()
        GroundBrush.TranslateTransform(CInt(-Player.Position.X Mod My.Resources.FloorTile.Width), CInt(-Player.Position.Y Mod My.Resources.FloorTile.Height))
        WallBrush.ResetTransform()
        WallBrush.TranslateTransform(CInt(-Player.Position.X Mod My.Resources.WallStrip.Width - 4), CInt(-Player.Position.Y Mod My.Resources.WallStrip.Height + 2))

        test = 0
    End Sub

    Private Sub MainForm_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        For Each key As Keys In Options.OIMap.Keys
            If (e.KeyCode = key) Then
                Options.OIStatus(Options.OIMap(key)) = True
            End If
        Next
    End Sub

    Private Sub MainForm_KeyUp(sender As Object, e As KeyEventArgs) Handles Me.KeyUp
        For Each key As Keys In Options.OIMap.Keys
            If (e.KeyCode = key) Then
                Options.OIStatus(Options.OIMap(key)) = False
            End If
        Next
    End Sub

    Protected Overrides Function IsInputKey(
        ByVal keyData As Keys) As Boolean
        Return True
    End Function

    Private Sub MainForm_MouseDown(sender As Object, e As MouseEventArgs) Handles MyBase.MouseDown
        For Each key As Keys In Options.OIMap.Keys
            If (e.Button = key) Then
                Options.OIStatus(Options.OIMap(key)) = True
            End If
        Next
    End Sub

    Private Sub MainForm_MouseUp(sender As Object, e As MouseEventArgs) Handles Me.MouseUp
        For Each key As Keys In Options.OIMap.Keys
            If (e.Button = key) Then
                Options.OIStatus(Options.OIMap(key)) = False
            End If
        Next
    End Sub

    Private Sub MainForm_MouseMove(sender As Object, e As MouseEventArgs) Handles Me.MouseMove
        Dim direction As Double = Player.getDirectionTo(e.Location + New Size(ViewOffsetX, ViewOffsetY) - New Size(Player.Room.XOffset, Player.Room.YOffset))
        Player.Direction = Actor.ToActorDirection(direction)
        Mouse = e.Location + New Size(ViewOffsetX, ViewOffsetY) - New Size(Player.Room.XOffset, Player.Room.YOffset)
    End Sub

    Private Sub MainForm_MouseWheel(sender As Object, e As MouseEventArgs) Handles Me.MouseWheel
        test = e.Delta / 120
    End Sub


End Class
