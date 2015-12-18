Imports JackPhysics
Imports JackToolbox

Public Class Game
    Inherits IGame

#If VersionType = "Beta" Then
    Public Shadows Const VersionTN As String = "1"
#ElseIf VersionType = "Release" Then
    Public shadows Const VersionTN As String = "0"
#ElseIf VersionType = "Debug" Then
    Public shadows Const VersionTN As String = "2"
#End If

    Public Shadows Const VersionNumber As String = "1.0.0302." + VersionTN + "000"

    Public Sub New(Buffer As BufferedGraphics, ScreenWidth As Integer, ScreenHeight As Integer)
        MyBase.New(Buffer, ScreenWidth, ScreenHeight)
    End Sub

    Public Overrides Sub Start()
        Init()
        GameLoop()
    End Sub

    Protected Overrides Sub Init()
        ' Load the rooms that we have.

        Options.Import()
        Resources.Import()

        Dim rooms As New List(Of Room)
#If Not VersionType = "Debug" Then
        For Each s As String In IO.Directory.EnumerateFiles("resources\rooms\")
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
        rooms.Add(New Room("resources\rooms\debug.xml"))
#End If

        ' Generate the world to play in
        World = New World("DavidBenAndJonathan", rooms)

        ' Load the player and testing stuff
        Player = New Player(My.Resources.CharacterUp1, World.RoomAt(150, 150), New Vector3(World.RoomAt(150, 150).XOffset + World.RoomAt(150, 150).Width / 2, World.RoomAt(150, 150).YOffset + World.RoomAt(150, 150).Height / 2, 0), {100, 100, 100, 8, 2})
        Dim TestObject1 = New EXPOrb(100, PlayerRoom, New Vector2(200, 200))
        Dim TestObject2 = New GameObject(My.Resources.Telepad, PlayerRoom, New Vector3(100, 100, 0), {100}, {GameObjectProperties.FlagsEnum.CastsShadow, GameObjectProperties.FlagsEnum.FloorObject, GameObjectProperties.FlagsEnum.Invulnerable})

        World.Rooms(0).AddGameObject(Player)
        'World.Rooms(0).AddGameObject(TestObject1)
        'World.Rooms(0).AddGameObject(TestObject2)

        'UserInterface = New UserInterface(ScreenWidth, ScreenHeight)

        GameRunning = True
        Watch = New Stopwatch()
        Watch.Start()
    End Sub

    Public Overrides Sub StopGame()
        Options.SaveOptions()
        GameRunning = False
    End Sub

    Protected Overrides Sub GameLoop()
        Dim tick As Double
        If (Not Options.OIStatus("Pause")) Then
            tick = MaxTick
        Else
            tick = (MaxTick / 25) 'Slow down time like a boss [For testing purposes] (maybe an added feature?)
        End If

        Dim UpdateAction As Action(Of Double)
        UpdateAction = AddressOf UpdateWorld

        For Each r As Room In World.Rooms
            r.DrawBackground(New Rectangle(New Point(0, 0), r.GraphicsMap.Size))
        Next

        While GameRunning
            Dim DrawTask As Task = Task.Run(New Action(AddressOf DrawWorld))
            Dim UpdateTask As Task = Task.Run(New Action(
                                              Sub()
                                                  UpdateWorld(_tick)
                                              End Sub))

            UpdateTask.Wait()
            UpdateCompleteEvent.Set()
            DrawTask.Wait()

            Dim sleepSeconds As Double = MaxTick - Watch.Elapsed.TotalSeconds
            If (sleepSeconds > 0) Then
                Threading.Thread.Sleep(CInt(Math.Floor(sleepSeconds * 1000)))
            End If

            _tick = Watch.Elapsed.TotalSeconds
            Watch.Restart()
        End While
    End Sub

    Protected Overrides Sub DrawWorld()
        ' Take this out when we figure out how to draw only the things that
        ' actually need to be drawn
        Buffer.Graphics.Clear(Color.Black)

        UpdateCompleteEvent.WaitOne()

        For Each r As Room In World.Rooms
            r.Redraw(r.Equals(Game.Player.Room))
            Buffer.Graphics.DrawImage(r.GraphicsMap, New Point(r.XOffset - ViewOffsetX, r.YOffset - ViewOffsetY))
        Next

#If Not VersionType = "Release" Then
        Buffer.Graphics.FillRectangle(Resources.ShadeBrush, New Rectangle(0, 0, 200, 70))
        Buffer.Graphics.DrawString("Version: " + VersionNumber, SystemFonts.CaptionFont, Brushes.Red, 0, 0)
        Buffer.Graphics.DrawString(CInt(1 / Tick), SystemFonts.CaptionFont, Brushes.Red, 0, 10)
        Buffer.Graphics.DrawString(Player.Properties.Health, SystemFonts.CaptionFont, Brushes.Red, 0, 20)
        Buffer.Graphics.DrawString(Player.Properties.Stamina, SystemFonts.CaptionFont, Brushes.Red, 0, 30)
        Buffer.Graphics.DrawString(Options.MouseWheel, SystemFonts.CaptionFont, Brushes.Red, 0, 40)
        Buffer.Graphics.DrawString(Player.Direction, SystemFonts.CaptionFont, Brushes.Red, 0, 50)
#End If

        'Buffer.Graphics.DrawImage(UserInterface.GraphicsMap, 0, 0)
        If GameRunning Then
            Buffer.Render()
        End If
    End Sub

    Private Function CalcNewX(GameObject As GameObject, t As Double) As Double
        Dim retval As Double = GameObject.Position.X + (GameObject.Speed.X * t * If(GameObject.HitBox.Width > GameObject.HitBox.Height, GameObject.HitBox.Width, GameObject.HitBox.Height))

        Return retval
    End Function

    Private Function CalcNewY(GameObject As GameObject, t As Double) As Double
        Dim retval As Double = GameObject.Position.Y + (GameObject.Speed.Y * t * If(GameObject.HitBox.Width > GameObject.HitBox.Height, GameObject.HitBox.Width, GameObject.HitBox.Height))

        Return retval
    End Function

    Protected Overrides Sub UpdateWorld(t As Double)
        'Moving all objects
        Dim CurRoom As Room = Player.Room

        For Each CurGameObject As GameObject In CurRoom.GameObjects
            If (CurGameObject.Properties.IsMoving) Then
                Dim newx As Double = CurGameObject.Position.X + (CurGameObject.Speed.X * t * If(CurGameObject.HitBox.Width > CurGameObject.HitBox.Height, CurGameObject.HitBox.Width, CurGameObject.HitBox.Height))
                Dim newy As Double = CurGameObject.Position.Y + (CurGameObject.Speed.Y * t * If(CurGameObject.HitBox.Width > CurGameObject.HitBox.Height, CurGameObject.HitBox.Width, CurGameObject.HitBox.Height))
                Dim NewPosition As New Vector3(newx, newy, CurGameObject.Position.Z)
                Dim Collided As Boolean = False
                For Each other As GameObject In CurRoom.GameObjects
                    If other.Equals(CurGameObject) Then Continue For

                    ' Test Position Collision
                    If other.CollidesWith(CurGameObject, New Vector2(NewPosition.X, NewPosition.Y)) Then
                        Collided = True
                        CurGameObject.Properties.Collided = True
                        other.Properties.Collided = True
                        If TypeOf CurGameObject Is Actor Then
                            CType(CurGameObject, Actor).Hit(other)
                        End If
                        If TypeOf other Is InventoryItem And TypeOf CurGameObject Is Player Then
                            Player.Inventory.AddItem(other)
                        End If
                        Exit For
                    End If

                    ' Test Sprite Collision
                    If other.SpriteIntersects(CurGameObject) Then
                        other.Properties.Dirty = True

                        ' TODO: If we dirty one sprite, how do we handle sprites that *this* sprite
                        ' collides with?
                    End If
                Next

                If New RectangleF(0, 0, CurRoom.Width, CurRoom.Height).Contains(New RectangleF(NewPosition.X + CurGameObject.HitBox.X, NewPosition.Y + CurGameObject.HitBox.Y, CurGameObject.HitBox.Width, CurGameObject.HitBox.Height)) = False Then
                    Collided = True
                End If

                If Not Collided Then
                    CurGameObject.Position = NewPosition
                    CurGameObject.Properties.Dirty = True
                End If
            End If
            CurGameObject.Update(t)
        Next

        'Add all objects waiting to be added
        For iWaitList As Integer = ToAddWaitlist.Count - 1 To 0 Step -1
            Dim CurGameObject As GameObject = ToAddWaitlist(iWaitList)
            If (CurGameObject.Room.Equals(CurRoom)) Then
                If CurGameObject.Position.X < 0 OrElse
                    CurGameObject.Position.Y + 16 < 0 OrElse
                    CurGameObject.Position.X + CurGameObject.HitBox.Width > CurRoom.Width OrElse
                    CurGameObject.Position.Y + CurGameObject.HitBox.Height > CurRoom.Height Then
                    ToAddWaitlist.Remove(CurGameObject)
                    Continue For
                End If

                Dim IsValidLocation As Boolean = True
                For Each o As GameObject In CurRoom.GameObjects
                    If o.CollidesWith(CurGameObject, CurGameObject.Position) Then
                        IsValidLocation = False
                        Exit For
                    End If
                Next

                If IsValidLocation Then
                    CurRoom.AddGameObject(CurGameObject)
                End If

                ToAddWaitlist.Remove(CurGameObject)
            End If
        Next

        'Delete all items flagged for deletion
        For value As Integer = CurRoom.GameObjects.Count - 1 To 0 Step -1
            If CurRoom.GameObjects(value).Properties.Dead Then
                CurRoom.GameObjects.RemoveAt(value)
            End If
        Next

        For Each Objective As Objective In CurRoom.Objectives
            Objective.Update(t)
        Next

        CurRoom.GameObjects.Sort()

        If IsNothing(PlayerRoom) = False Then
            If Player.Room.Equals(PlayerRoom) = False Then
                Dim oldroom As Room = Player.Room
                Dim newroom As Room = PlayerRoom
                Dim NewPosition As New Vector3(
                    Player.Position.X + oldroom.XOffset - newroom.XOffset,
                    Player.Position.Y + oldroom.YOffset - newroom.YOffset,
                    Player.Position.Z)
                Player.Position = NewPosition
                oldroom.GameObjects.Remove(Player)
                newroom.GameObjects.Add(Player)
                Player.Room = newroom
            End If
        End If

        ViewOffsetX = Player.Position.X + Player.Room.XOffset - (ScreenWidth / 2 - Player.HitBox.Width / 2)
        ViewOffsetY = Player.Position.Y + Player.Room.YOffset - (ScreenHeight / 2 - Player.HitBox.Height / 2)
    End Sub
End Class
