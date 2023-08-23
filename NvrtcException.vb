Namespace CudaForVb

    Public Class NvrtcException
        Inherits Exception

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(message As String, result As NvrtcResult)
            MyBase.New(message)
            Source = "NVRTC"
            HResult = result
        End Sub

        Public Overrides Function ToString() As String
            Return $"{Source}: {Message} [ {HResult} ]"
        End Function

    End Class

End Namespace