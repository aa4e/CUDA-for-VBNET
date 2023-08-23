Imports System.IO
Imports System.Runtime.InteropServices
Imports Orion.CudaForVb

Module Program

    Sub Main(args As String())
        Println("ТЕСТ CUDA НА VB.NET", Color.Header)
        Try
            Dim deviceId As Integer = Init()
            Println($"Всего устройств: {Cuda.DeviceGetCount()}")
            Println($"Используется GPU {deviceId} [ {Cuda.DeviceGetName(deviceId)} ]")

            VectorAddTest(deviceId)

        Catch ex As Exception
            Println(ex.Message, Color.Error)
        End Try
        Console.ReadKey()
    End Sub

    ''' <summary>
    ''' Выбирает GPU для работы и возваращет его индекс.
    ''' </summary>
    Private Function Init() As Integer
        Cuda.Initialize(0)
        Dim deviceId As Integer = Cuda.GetMaxGflopsDeviceId()
        Return deviceId
    End Function

    Private ReadOnly ConsoleLockObj As New Object()

    Private Sub Println(text As String, Optional color As Color = Color.Normal)
        SyncLock ConsoleLockObj
            Console.ForegroundColor = CType(color, ConsoleColor)
            Console.WriteLine(text)
        End SyncLock
    End Sub

#Region "TEST"

    ''' <summary>
    ''' Складывает 2 вектора на CPU и на GPU.
    ''' </summary>
    Private Sub VectorAddTest(deviceId As Integer)
        'CUDA Kernel Device code:
        Dim vectorAdd_kernel_code As String = "
extern ""C"" __global__ void vectorAdd(const float *A, const float *B, float *C, int numElements) 
{
	int i = blockDim.x * blockIdx.x + threadIdx.x;
	if (i < numElements) 
    {
		C[i] = A[i] + B[i];
	}
}"
        Dim totMem = Cuda.DeviceTotalMem(deviceId)
		Println($"Полная память GPU={totMem / 1024 / 1024:F0} Мб")

        Dim freeMemory As ULong
        Dim totalMemory As ULong
        Cuda.MemoryInfo(deviceId, freeMemory, totalMemory)
        Println($"Доступная память GPU={totalMemory / 1024 / 1024:F0} Mb, свободно={freeMemory / 1024 / 1024:F0} Мб".Insert(0, Environment.NewLine))

        Dim kernelFile As String = IO.Path.Combine(Directory.GetCurrentDirectory(), "vectorAdd_kernel.cu")
        File.WriteAllText(kernelFile, vectorAdd_kernel_code)

        Dim cuBin As Byte() = New Byte() {}
        Dim cuBinSize As ULong
        Cuda.CompileFileToCUBIN(deviceId, kernelFile, cuBin, cuBinSize)

        Dim [module] As ULong = Cuda.LoadCUBIN(deviceId, cuBin)

        Dim kernelAddr As IntPtr
        Cuda.GetModuleFunction(kernelAddr, [module], "vectorAdd")

        ' Print the vector length to be used, and compute its size
        Const ALLOWED As Integer = 5 * 1024 * 1024 ' 5MB room allowed for GPU to execute code this may not always be enough!
        Dim numElements As Integer = CInt((freeMemory - ALLOWED) / Marshal.SizeOf(GetType(Single)) / 3)
        Dim size As ULong = CULng(numElements * Marshal.SizeOf(GetType(Single)))

        Println($"Инициализация {numElements} элементов...")

        Dim h_A(numElements - 1) As Single ' Allocate the host input vector A
        Dim h_B(numElements - 1) As Single ' Allocate the host input vector B
        Dim h_C(numElements - 1) As Single ' Allocate the host output vector C

        ' Initialize the host input vectors
        Dim rnd As New Random()
        For i As Integer = 0 To numElements - 1
            h_A(i) = CSng(rnd.NextDouble())
            h_B(i) = CSng(rnd.NextDouble())
        Next

        Println($"Сложение вектора из {numElements} элементов...")
        Println("На CPU...".Insert(0, Environment.NewLine))
        
		Dim sw As New Stopwatch()
        sw.Start()
        For i As Integer = 0 To numElements - 1
            h_C(i) = h_A(i) + h_B(i)
        Next
        Println($"{sw.Elapsed.TotalMilliseconds} мс")

        Dim d_A As ULong = Cuda.MemAlloc(size) ' Allocate the device input vector A
        Dim d_B As ULong = Cuda.MemAlloc(size) ' Allocate the device input vector B
        Dim d_C As ULong = Cuda.MemAlloc(size) ' Allocate the device output vector C

        ' Copy the host input vectors A and B in host memory to the device input vectors in device memory
        Cuda.MemcpyHostToDev(d_A, h_A, size)
        Cuda.MemcpyHostToDev(d_B, h_B, size)

        ' Launch the Vector Add CUDA Kernel
        Dim threadsPerBlock As UInteger = CUInt(Cuda.DeviceGetAttribute(CuDeviceAttribute.MAX_THREADS_PER_BLOCK, deviceId))
        Dim blocksPerGrid As UInteger = CUInt((numElements + threadsPerBlock - 1) / threadsPerBlock)
        Println($"Программа CUDA запущена на {blocksPerGrid} блоках по {threadsPerBlock} потоков")

        Dim cudaBlockSize As New Dim3(threadsPerBlock, 1, 1)
        Dim cudaGridSize As New Dim3(blocksPerGrid, 1, 1)

        Dim extra As IntPtr() = Nothing
        Dim parameters As Object() = {d_A, d_B, d_C, numElements}

        Println("На GPU...".Insert(0, Environment.NewLine))
        sw.Restart()
        Cuda.LaunchKernel(kernelAddr, cudaGridSize.X, cudaGridSize.Y, cudaGridSize.Z, cudaBlockSize.X, cudaBlockSize.Y, cudaBlockSize.Z, 0, New IntPtr(0), parameters, extra)
        Cuda.ContextSynchronize()
        Println($"{sw.Elapsed.TotalMilliseconds} мс")

        ' Copy the device result vector in device memory to the host result vector in host memory.
        Cuda.MemcpyDevToHost(h_C, d_C, size)

        ' Verify that the result vector is correct
        For i As Integer = 0 To numElements - 1
            If (h_A(i) + h_B(i) - h_C(i) <> 0) Then
                Println($"Результат проверки отрицательный на элементе {i}!", Color.Error)
                GoTo free_memory
            End If
        Next

        Println("Тест сложения векторов ПРОЙДЕН!" & Environment.NewLine, Color.Ok)

free_memory:

        ' Free device global memory
        Cuda.MemFree(d_C)
        Cuda.MemFree(d_B)
        Cuda.MemFree(d_A)
    End Sub

#End Region '/TEST

#Region "NESTED TYPES"

    Private Enum Color As Integer
        Normal = ConsoleColor.Gray
        Ok = ConsoleColor.Green
        Header = ConsoleColor.Cyan
        Found = ConsoleColor.Magenta
        [Error] = ConsoleColor.Red
    End Enum

#End Region '/NESTED TYPES

End Module
