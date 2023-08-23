Namespace CudaForVb

    Public Class CudaException
        Inherits Exception

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(message As String, result As CuResult)
            MyBase.New(message)
            Source = "CUDA"
            HResult = result
        End Sub

        Public Overrides Function ToString() As String
            Return $"{Source}: {Message} [ {HResult} ]"
        End Function

    End Class

End Namespace