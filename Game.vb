Public Class Game

#If VersionType = "Beta" Then
    Public Const VersionTN As String = "1"
#ElseIf VersionType = "Release" Then
    Public Const VersionTN As String = "0"
#ElseIf VersionType = "Debug" Then
    Public Const VersionTN As String = "2"
#End If

    Public Const VersionNumber As String = "1.0.0302." + VersionTN + "000"

    Public Shared ToAddWaitlist As New List(Of GameObject)
    Public Shared Player As Player
    Public Shared ViewOffsetX As Double
    Public Shared ViewOffsetY As Double

    Public Shared World As World
    Public Shared Options As Options
    Public Shared Resources As New Resources
    Public Shared UserInterface As UserInterface

    Private Buffer As BufferedGraphics
    Private GameRunning As Boolean
    Private ScreenWidth As Integer
    Private ScreenHeight As Integer

    Public ReadOnly Property MaxTick As Double
        Get
            Return 1 / Options.Preferences("MaxFPS")
        End Get
    End Property

    Public ReadOnly Property PlayerRoom As Room
        Get
            Return World.RoomAt(Player.Middle.X + Player.Room.XOffset, Player.Middle.Y + Player.Room.YOffset)
        End Get
    End Property

    Public Sub New(Buffer As BufferedGraphics, ScreenWidth As Integer, ScreenHeight As Integer)
        Me.Buffer = Buffer
        Me.ScreenHeight = ScreenHeight
        Me.ScreenWidth = ScreenWidth
    End Sub

    Public Sub Start()
        Init()
        GameLoop()
    End Sub

    Private Sub Init()
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
        Dim TestObject2 = New GameObject(My.Resources.Telepad, PlayerRoom, New Vector3(100, 100, 0), 100, {GameObject.GameObjectProps.CastsShadow, GameObject.GameObjectProps.FloorObject, GameObject.GameObjectProps.Invulnerable})

        World.Rooms(0).AddGameObject(Player)
        World.Rooms(0).AddGameObject(TestObject1)
        World.Rooms(0).AddGameObject(TestObject2)

        Resources = New Resources()
        UserInterface = New UserInterface(ScreenWidth, ScreenHeight)

        GameRunning = True
        Watch = New Stopwatch()
        Watch.Start()
    End Sub

    Private Sub LoadFiles()
        Options = New Options()
    End Sub

    Public Sub StopGame()
        Options.SaveOptions()
        GameRunning = False
    End Sub

    Private Sub GameLoop()
        While GameRunning
            If (Not Options.OIStatus("Pause")) Then
                UpdateWorld(MaxTick)
            Else
                UpdateWorld(MaxTick / 25) 'Slow down time like a boss [For testing purposes] (maybe an added feature?)
            End If
            DrawWorld()

            Dim sleepSeconds As Double = MaxTick - Watch.Elapsed.TotalSeconds
            If (sleepSeconds > 0) Then
                Threading.Thread.Sleep(CInt(Math.Floor(sleepSeconds * 1000)))
            End If

            _tick = Watch.Elapsed.TotalSeconds
            Watch.Restart()
        End While
    End Sub

    Private Sub DrawWorld()
        ' Take this out when we figure out how to draw only the things that
        ' actually need to be drawn
        Buffer.Graphics.Clear(Color.Black)

        For Each r As Room In World.Rooms
            r.Redraw()
            Buffer.Graphics.DrawImage(r.GraphicsMap, New Point(r.XOffset - ViewOffsetX, r.YOffset - ViewOffsetY))
        Next

#If Not VersionType = "Release" Then
        Buffer.Graphics.FillRectangle(Resources.ShadeBrush, New Rectangle(0, 0, 200, 70))
        Buffer.Graphics.DrawString("Version: " + VersionNumber, SystemFonts.CaptionFont, Brushes.Red, 0, 0)
        Buffer.Graphics.DrawString(CInt(1 / Tick), SystemFonts.CaptionFont, Brushes.Red, 0, 10)
        Buffer.Graphics.DrawString(Player.Properties("Health"), SystemFonts.CaptionFont, Brushes.Red, 0, 20)
        Buffer.Graphics.DrawString(Player.Properties("CurrentStamina"), SystemFonts.CaptionFont, Brushes.Red, 0, 30)
        Buffer.Graphics.DrawString(Options.MouseWheel, SystemFonts.CaptionFont, Brushes.Red, 0, 40)
        Buffer.Graphics.DrawString(Player.Direction, SystemFonts.CaptionFont, Brushes.Red, 0, 50)
#End If

        'Buffer.Graphics.DrawImage(UserInterface.GraphicsMap, 0, 0)
        If GameRunning Then
            Buffer.Render()
        End If
    End Sub

    Private Watch As Stopwatch
    Private _tick As Double = 0.5

    Public ReadOnly Property Tick As Double
        Get
            Return _tick
        End Get
    End Property

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
                        If TypeOf O Is Actor Then
                            CType(O, Actor).Hit(other)
                        End If
                        If TypeOf other Is InventoryItem And TypeOf O Is Player Then
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
        For iWaitList As Integer = ToAddWaitlist.Count - 1 To 0 Step -1
            Dim CurGameObject As GameObject = ToAddWaitlist(iWaitList)
            If (CurGameObject.Room.Equals(r)) Then
                If CurGameObject.Position.X < 0 OrElse
                    CurGameObject.Position.Y + 16 < 0 OrElse
                    CurGameObject.Position.X + CurGameObject.HitBox.Width > r.Width OrElse
                    CurGameObject.Position.Y + CurGameObject.HitBox.Height > r.Height Then
                    ToAddWaitlist.Remove(CurGameObject)
                    Continue For
                End If

                Dim IsValidLocation As Boolean = True
                For Each o As GameObject In r.GameObjects
                    If o.CollidesWith(CurGameObject, CurGameObject.Position) Then
                        IsValidLocation = False
                        Exit For
                    End If
                Next

                If IsValidLocation Then
                    r.AddGameObject(CurGameObject)
                End If

                ToAddWaitlist.Remove(CurGameObject)
            End If
        Next

        'Delete all items flagged for deletion
        For value As Integer = r.GameObjects.Count - 1 To 0 Step -1
            If r.GameObjects(value).Dead Then
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
    End Sub

    Public Sub KeyDown(e As KeyEventArgs)
        For Each key As Keys In Options.OIMap.Keys
            If (e.KeyCode = key) Then
                Options.OIStatus(Options.OIMap(key)) = True
            End If
        Next
    End Sub

    Public Sub KeyUp(e As KeyEventArgs)
        For Each key As Keys In Options.OIMap.Keys
            If (e.KeyCode = key) Then
                Options.OIStatus(Options.OIMap(key)) = False
            End If
        Next
    End Sub

    Public Sub MouseDown(e As MouseEventArgs)
        For Each key As Keys In Options.OIMap.Keys
            If (e.Button = key) Then
                Options.OIStatus(Options.OIMap(key)) = True
            End If
        Next
    End Sub

    Public Sub MouseUp(e As MouseEventArgs)
        For Each key As Keys In Options.OIMap.Keys
            If (e.Button = key) Then
                Options.OIStatus(Options.OIMap(key)) = False
            End If
        Next
    End Sub

    Public Sub MouseMove(e As MouseEventArgs)
        'Dim direction As Double = Player.getDirectionTo(e.Location + New Size(ViewOffsetX, ViewOffsetY) - New Size(Player.Room.XOffset, Player.Room.YOffset))
        'Player.Direction = Actor.ToActorDirection(direction)
        'Mouse = e.Location + New Size(ViewOffsetX, ViewOffsetY) - New Size(Player.Room.XOffset, Player.Room.YOffset)
    End Sub

    Public Sub MouseWheel(e As MouseEventArgs)
        Options.MouseWheel = e.Delta / 120
    End Sub
End Class
