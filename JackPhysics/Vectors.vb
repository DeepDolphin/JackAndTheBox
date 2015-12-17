Imports System.Drawing

Public Structure Vector2
    Public X As Double
    Public Y As Double
    Public Property Length As Double
        Get
            Return Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2))
        End Get
        Set(value As Double)
            If (Not Length = 0.0) Then
                Normalize()
                X = X * value
                Y = Y * value
            Else
                Throw New InvalidOperationException("Cannot change length of a zero vector.")
            End If
        End Set
    End Property
    Public Property Direction As Double
        Get
            Return Math.Atan2(Y, X)
        End Get
        Set(value As Double)
            If (Not Length = 0.0) Then
                Dim l As Double = Length
                X = l * Math.Cos(value)
                Y = l * Math.Sin(value)
            Else
                Throw New InvalidOperationException("Cannot change direction of a zero vector.")
            End If
        End Set
    End Property
    Public Sub Normalize()
        If (Not Length = 0.0) Then
            Dim l As Double = Length
            X = X / l
            Y = Y / l
        Else
            Throw New InvalidOperationException("Cannot normalize a zero vector.")
        End If
    End Sub

    Public Sub New(X As Double, Y As Double)
        Me.X = X
        Me.Y = Y
    End Sub

    ' Operators
    Public Shared Operator +(left As Vector2, right As Vector2) As Vector2
        Return New Vector2(left.X + right.X, left.Y + right.Y)
    End Operator
    Public Shared Operator -(left As Vector2, right As Vector2) As Vector2
        Return New Vector2(left.X - right.X, left.Y - right.Y)
    End Operator
    Public Shared Operator *(left As Vector2, scalar As Double) As Vector2
        Return New Vector2(left.X * scalar, left.Y * scalar)
    End Operator
    Public Shared Operator /(left As Vector2, scalar As Double) As Vector2
        Return New Vector2(left.X / scalar, left.Y / scalar)
    End Operator

    ' Conversion
    Public Shared Widening Operator CType(input As PointF) As Vector2
        Return New Vector2(input.X, input.Y)
    End Operator
    Public Shared Narrowing Operator CType(input As Vector2) As PointF
        Return New PointF(input.X, input.Y)
    End Operator
End Structure

Public Structure Vector3
    Public X As Double
    Public Y As Double
    Public Z As Double
    Public Property XY As Vector2
        Get
            Return New Vector2(X, Y)
        End Get
        Set(value As Vector2)
            X = value.X
            Y = value.Y
        End Set
    End Property
    Public Property XZ As Vector2
        Get
            Return New Vector2(X, Z)
        End Get
        Set(value As Vector2)
            X = value.X
            Z = value.Y
        End Set
    End Property
    Public Property YZ As Vector2
        Get
            Return New Vector2(Y, Z)
        End Get
        Set(value As Vector2)
            Y = value.X
            Z = value.Y
        End Set
    End Property

    Public Property Length As Double
        Get
            Return Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2) + Math.Pow(Z, 2))
        End Get
        Set(value As Double)
            Normalize()
            X = X * value
            Y = Y * value
            Z = Z * value
        End Set
    End Property
    Public Sub Normalize()
        Dim l As Double = Length
        X = X / l
        Y = Y / l
        Z = Z / l
    End Sub

    Public Sub New(x As Double, y As Double, z As Double)
        Me.X = x
        Me.Y = y
        Me.Z = z
    End Sub

    'Operators
    Public Shared Operator +(left As Vector3, right As Vector3) As Vector3
        Return New Vector3(left.X + right.X, left.Y + right.Y, left.Z + right.Z)
    End Operator
    Public Shared Operator -(left As Vector3, right As Vector3) As Vector3
        Return New Vector3(left.X - right.X, left.Y - right.Y, left.Z - right.Z)
    End Operator
    Public Shared Operator *(left As Vector3, scalar As Double) As Vector3
        Return New Vector3(left.X * scalar, left.Y * scalar, left.Z * scalar)
    End Operator
    Public Shared Operator /(left As Vector3, scalar As Double) As Vector3
        Return New Vector3(left.X / scalar, left.Y / scalar, left.Z / scalar)
    End Operator
End Structure
