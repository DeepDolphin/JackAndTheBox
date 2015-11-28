Public Class MainForm

#If VersionType = "Beta" Then
    Public Const VersionTN As String = "1"
#ElseIf VersionType = "Release" Then
    Public Const VersionTN As String = "0"
#ElseIf VersionType = "Debug" Then
    Public Const VersionTN As String = "2"
#End If

    Public Const VersionNumber As String = "1.0.0223." + VersionTN + "000"

    Public ToAddWaitlist As New List(Of GameObject)
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

        ' Load the rooms that we have.
        Dim rooms As New List(Of Room)
#If Not VersionType = "Debug" Then
        For Each s As String In IO.Directory.EnumerateFiles("Rooms\")
            If IO.Path.GetFileNameWithoutExtension(s) = "up" OrElse
                    IO.Path.GetFileNameWithoutExtension(s) = "down" OrElse
                    IO.Path.GetFileNameWithoutExtension(s) = "left" OrElse
                    IO.Path.GetFileNameWithoutExtension(s) = "right" OrElse
                    IO.Path.GetFileNameWithoutExtension(s) = "debug" Then
                Continue For
            End If
            Dim r As New Room(s)
            rooms.Add(r)
        Next
#Else
        rooms.Add(New Room("Rooms\debug.xml"))
#End If
        LoadFiles()

        ' Generate the world to play in
        World = New World("DavidAndBen", rooms)

        ' Load the player and testing stuff
        Player = New Player(World.RoomAt(150, 150))
        Dim TestObject1 = New EXPOrb(100, PlayerRoom, New Vector2(200, 200))
        Dim TestObject2 = New GameObject(My.Resources.Telepad, PlayerRoom, New Vector3(100, 100, 0), {GameObject.GameObjectProps.CastsShadow, GameObject.GameObjectProps.FloorObject})

        World.Rooms(0).AddGameObject(Player)
        World.Rooms(0).AddGameObject(TestObject1)
        World.Rooms(0).AddGameObject(TestObject2)

        GroundBrush = World.GroundBrush
        WallBrush = World.WallBrush
        ShadeBrush = New SolidBrush(Color.FromArgb(175, 0, 0, 0))


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

    Private Function CIntFloor(vals() As Double) As Integer
        Dim sum As Integer
        For Each val As Double In vals
            sum += CInt(Math.Floor(val))
        Next
        Return sum
    End Function

    Private Sub MainForm_Paint(sender As Object, e As PaintEventArgs) Handles Me.Paint
        For Each r As Room In World.Rooms
            e.Graphics.FillRectangle(WallBrush, CIntFloor({-ViewOffsetX, r.XOffset}), CIntFloor({-ViewOffsetY, r.YOffset - 32}), r.Bounds.Width, 32)
            e.Graphics.FillRectangle(GroundBrush, CIntFloor({-ViewOffsetX, r.XOffset}), CIntFloor({-ViewOffsetY, r.YOffset}), r.Bounds.Width, r.Bounds.Height)
            e.Graphics.DrawImage(My.Resources.GradientLeft, CIntFloor({-ViewOffsetX, r.XOffset}), CIntFloor({-ViewOffsetY, r.YOffset - 32}), 64, 32)
            e.Graphics.DrawImage(My.Resources.GradientRight, CIntFloor({-ViewOffsetX, r.XOffset, r.Width - 63}), CIntFloor({-ViewOffsetY, r.YOffset - 32}), 64, 32)
            If r.Equals(Player.Room) Then
                ' Draw shadows first
                For Each O As GameObject In r.GameObjects
                    If O.CastsShadow Then e.Graphics.DrawImage(My.Resources.Shadow,
                                                               CIntFloor({O.Position.X, -ViewOffsetX, r.XOffset}),
                                                               CIntFloor({O.Position.Y, O.Sprite.Height - 7, -ViewOffsetY, r.YOffset}), O.Sprite.Width, 10)
                Next

                ' Draw the rest of the game objects
                For Each O As GameObject In r.GameObjects
                    Try
                        e.Graphics.DrawImage(O.Sprite.CurrentFrame,
                                             CIntFloor({O.Position.X, -ViewOffsetX, r.XOffset}),
                                             CIntFloor({O.Position.Y, O.Position.Z * (10 / 16), -ViewOffsetY, r.YOffset}),
                                             O.Sprite.Width,
                                             O.Sprite.Height)
#If VersionType = "Debug" Then
                        e.Graphics.DrawRectangle(Pens.Red,
                                                 CIntFloor({O.Position.X, -ViewOffsetX, r.XOffset, O.HitBox.X}),
                                                 CIntFloor({O.Position.Y, O.Position.Z * (10 / 16), -ViewOffsetY, r.YOffset, O.HitBox.Y}),
                                                 O.HitBox.Width,
                                                 O.HitBox.Height)
#End If
                    Catch ex As Exception
                        Stop
                    End Try
                Next
            Else
                e.Graphics.FillRectangle(ShadeBrush, CIntFloor({-ViewOffsetX, r.XOffset}), CIntFloor({-ViewOffsetY, r.YOffset - 32}), r.Bounds.Width, r.Bounds.Height + 32)
            End If
#If Not VersionType = "Release" Then
            e.Graphics.DrawString(IO.Path.GetFileName(r.Filename), SystemFonts.CaptionFont, Brushes.Red, CSng(-ViewOffsetX + r.XOffset), CSng(-ViewOffsetY + r.YOffset))
#End If
        Next
#If Not VersionType = "Release" Then
        e.Graphics.DrawString("Version: " + VersionNumber, SystemFonts.CaptionFont, Brushes.Red, 0, 0)
        e.Graphics.DrawString(CInt(1 / Tick), SystemFonts.CaptionFont, Brushes.Red, 0, 10)
        e.Graphics.DrawString(Player.Properties("Health"), SystemFonts.CaptionFont, Brushes.Red, 0, 20)
        e.Graphics.DrawString(Player.Properties("CurrentStamina"), SystemFonts.CaptionFont, Brushes.Red, 0, 30)
        e.Graphics.DrawString(Options.MouseWheel, SystemFonts.CaptionFont, Brushes.Red, 0, 40)
        e.Graphics.DrawString(Player.Direction, SystemFonts.CaptionFont, Brushes.Red, 0, 50)
#End If
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
                        If (O.Flags.Contains("Actor")) Then
                            CType(O, Actor).Hit(other)
                        End If
                        If (other.Flags.Contains("InventoryItem") And O.Equals(Player)) Then
                            Player.Inventory.AddItem(other)
                        End If
                        Exit For
                    End If
                Next

                If New RectangleF(0, 0, r.Width, r.Height).Contains(New RectangleF(newx + O.HitBox.X, newy + O.HitBox.Y, O.HitBox.Width, O.HitBox.Height)) = False Then
                    good = False
                End If

                If good Then
                        O.Position.X = newx
                        O.Position.Y = newy
                    End If
                End If
            O.Update(t)
        Next

        'Add all objects waiting to be added
        For value As Integer = ToAddWaitlist.Count - 1 To 0 Step -1
            Dim GameObject As GameObject = ToAddWaitlist(value)
            If (GameObject.Room.Equals(r)) Then
                If GameObject.Position.X < 0 OrElse
                    GameObject.Position.Y + 16 < 0 OrElse
                    GameObject.Position.X + GameObject.HitBox.Width > r.Width OrElse
                    GameObject.Position.Y + GameObject.HitBox.Height > r.Height Then
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

        r.GameObjects.Sort()

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
        'Dim direction As Double = Player.getDirectionTo(e.Location + New Size(ViewOffsetX, ViewOffsetY) - New Size(Player.Room.XOffset, Player.Room.YOffset))
        'Player.Direction = Actor.ToActorDirection(direction)
        'Mouse = e.Location + New Size(ViewOffsetX, ViewOffsetY) - New Size(Player.Room.XOffset, Player.Room.YOffset)
    End Sub

    Private Sub MainForm_MouseWheel(sender As Object, e As MouseEventArgs) Handles Me.MouseWheel
        Options.MouseWheel = e.Delta / 120
    End Sub


End Class
