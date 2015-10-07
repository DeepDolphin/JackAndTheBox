Public Class MainForm
    Public GameObjects As New List(Of GameObject)
    Public Environment As New List(Of RectangleF)
    Public UpPressed As Boolean
    Public DownPressed As Boolean
    Public RightPressed As Boolean
    Public LeftPressed As Boolean

    Public Player As Person
    Public GroundBrush As Brush
    Public Random As New Random(0)
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

    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.DoubleBuffered = True
        Player = New Person(ScreenWidth / 2, ScreenHeight / 2)
        Dim TestObject2 = New Person(100, 100)
        AddGameObject(Player)
        AddGameObject(TestObject2)

        Environment.Add(New RectangleF(-1, 0, 1, ScreenHeight))
        Environment.Add(New RectangleF(0, -1, ScreenWidth, 1))
        Environment.Add(New RectangleF(ScreenWidth, -1, 1, ScreenHeight))
        Environment.Add(New RectangleF(-1, ScreenHeight, ScreenWidth, -1))

        For i As Integer = 0 To 10
            Dim l As New GameObject(My.Resources.CeilingLight, Random.Next(ScreenWidth), Random.Next(ScreenHeight))
            l.CastsShadow = False
            l.Z = 10
            AddGameObject(l)
        Next

        GroundBrush = New TextureBrush(My.Resources.FloorTile)
    End Sub

    Private Sub MainForm_Paint(sender As Object, e As PaintEventArgs) Handles Me.Paint
        e.Graphics.FillRectangle(GroundBrush, 0, 0, ScreenWidth, ScreenHeight)
        For Each o As GameObject In GameObjects
            If o.CastsShadow Then e.Graphics.DrawImage(My.Resources.Shadow, CSng(o.X), CSng(o.Y + o.Image.Height - 7), o.Image.Width, 10)
        Next
        For Each O As GameObject In GameObjects
            e.Graphics.DrawImage(O.Image, CSng(O.X), CSng(O.Y + O.Z * (10 / 16)), O.Image.Width, O.Image.Height)
        Next
    End Sub

    Private Sub Timer_Tick(sender As Object, e As EventArgs) Handles Timer.Tick
        Invalidate()
        UpdateWorld()
    End Sub

    Public Sub UpdateWorld()
        For Each O As GameObject In GameObjects
            Dim newx As Double = O.X + O.XSpeed
            Dim newy As Double = O.Y + O.YSpeed
            If (O.Equals(Player)) Then
                If UpPressed Then
                    newy -= 2
                    Player.Direction = Person.PersonDirection.Up
                End If
                If DownPressed Then
                    newy += 2
                    Player.Direction = Person.PersonDirection.Down
                End If
                If RightPressed Then
                    newx += 2
                    Player.Direction = Person.PersonDirection.Right
                End If
                If LeftPressed Then
                    newx -= 2
                    Player.Direction = Person.PersonDirection.Left
                End If
            End If
            Dim good As Boolean = True
            For Each other As GameObject In GameObjects
                If other.Equals(O) Then Continue For
                If other.CollidesWith(O, newx, newy) Then
                    good = False
                    Exit For
                End If
            Next
            For Each rectangle As RectangleF In Environment
                Dim otherhitbox As RectangleF = O.HitBox
                otherhitbox.X = newx
                otherhitbox.Y = newy
                If (otherhitbox.IntersectsWith(rectangle)) Then
                    good = False
                    Exit For
                End If
            Next
            If good Then
                O.X = newx
                O.Y = newy
            End If
            O.Update()
        Next
        ResortGameObjects()
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
            Case Keys.Space
                Dim X As Integer
                Dim Y As Integer
                Select Case Player.Direction
                    Case Person.PersonDirection.Up
                        X = Player.X
                        Y = Player.Y - My.Resources.Crate.Height
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
                For Each o As GameObject In GameObjects
                    If o.CollidesWith(newcrate, X, Y) Then
                        good = False
                        Exit For
                    End If
                Next
                If good Then
                    AddGameObject(newcrate)
                End If
        End Select
    End Sub

    Public Sub AddGameObject(O As GameObject)
        GameObjects.Add(O)
        ResortGameObjects()
    End Sub

    Public Sub RemoveGameObject(O As GameObject)
        GameObjects.Remove(O)
        ResortGameObjects()
    End Sub

    Public Sub RemoveGameObject(index As Integer)
        GameObjects.RemoveAt(index)
        ResortGameObjects()
    End Sub

    Public Sub ResortGameObjects()
        Dim l As New List(Of GameObject)
        While GameObjects.Count > 0
            ' find the smallest y
            Dim b As New List(Of GameObject)
            Dim record As Integer = Integer.MaxValue
            For Each o As GameObject In GameObjects
                If o.Y - o.HitBox.Height + o.Z * (10 / 16) < record Then
                    b.Clear()
                    record = o.Y - o.HitBox.Height + o.Z * (10 / 16)
                    b.Add(o)
                ElseIf o.Y - o.HitBox.Height + o.Z * (10 / 16) = record
                    b.Add(o)
                End If
            Next
            For Each o As GameObject In b
                GameObjects.Remove(o)
                l.Add(o)
            Next
        End While

        GameObjects = l
    End Sub

    Private Sub Form1_KeyUp(sender As Object, e As KeyEventArgs) Handles Me.KeyUp
        Select Case e.KeyCode
            Case Keys.Up
                UpPressed = False
            Case Keys.Down
                DownPressed = False
            Case Keys.Left
                LeftPressed = False
            Case Keys.Right
                RightPressed = False
        End Select
    End Sub
End Class
