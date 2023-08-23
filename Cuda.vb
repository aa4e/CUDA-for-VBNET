Imports System.Runtime.InteropServices

Namespace CudaForVb

    ''' <summary>
    ''' 
    ''' </summary>
    Public Module Cuda

#Region "ИНИЦИАЛИЗАЦИЯ"

        ''' <summary>
        ''' Инициализирует CUDA.
        ''' </summary>
        ''' <param name="flags"></param>
        Public Sub Initialize(flags As UInteger)
            Dim result As CuResult = cuInit(flags)
            If (result <> CuResult.SUCCESS) Then
                Throw New CudaException($"{NameOf(cuInit)} failed with code [{result}].", result)
            End If
            CheckDlls()
        End Sub

        ''' <summary>
        ''' Возвращает индекс наиболее производительного GPU.
        ''' </summary>
        Public Function GetMaxGflopsDeviceId() As Integer

            Dim result As CuResult = cuInit(0)
            If (result <> CuResult.SUCCESS) Then
                Throw New CudaException($"{NameOf(cuInit)} failed with code [{result}].", result)
            End If

            Dim deviceCount As Integer = 0
            result = cuDeviceGetCount(deviceCount)
            If (result <> CuResult.SUCCESS) Then
                Throw New CudaException($"{NameOf(cuDeviceGetCount)} failed with code [{result}].", result)
            End If
            If (deviceCount = 0) Then
                Throw New Exception("No devices support CUDA.")
            End If

            ' Find the best CUDA capable GPU device
            Dim currentDevice As Integer = 0
            Dim maxPerfDevice As Integer = 0
            Dim maxComputePerf As ULong = 0
            Dim devicesProhibited As Integer = 0

            Do While (currentDevice < deviceCount)

                Dim multiProcessorCount As Integer = 0
                result = cuDeviceGetAttribute(multiProcessorCount, CuDeviceAttribute.MULTIPROCESSOR_COUNT, currentDevice)
                If (result <> CuResult.SUCCESS) Then
                    Throw New CudaException($"{NameOf(cuDeviceGetAttribute)} failed with code [{result}].", result)
                End If

                Dim clockRate As Integer = 0
                result = cuDeviceGetAttribute(clockRate, CuDeviceAttribute.CLOCK_RATE, currentDevice)
                If (result <> CuResult.SUCCESS) Then
                    Throw New CudaException($"{NameOf(cuDeviceGetAttribute)} failed with code [{result}].", result)
                End If

                Dim major As Integer = 0
                result = cuDeviceGetAttribute(major, CuDeviceAttribute.COMPUTE_CAPABILITY_MAJOR, currentDevice)
                If (result <> CuResult.SUCCESS) Then
                    Throw New CudaException($"{NameOf(cuDeviceGetAttribute)} failed with code [{result}].", result)
                End If

                Dim minor As Integer = 0
                result = cuDeviceGetAttribute(minor, CuDeviceAttribute.COMPUTE_CAPABILITY_MINOR, currentDevice)
                If (result <> CuResult.SUCCESS) Then
                    Throw New CudaException($"{NameOf(cuDeviceGetAttribute)} failed with code [{result}].", result)
                End If

                Dim computeMode As Integer = 0
                cuDeviceGetAttribute(computeMode, CuDeviceAttribute.COMPUTE_MODE, currentDevice)

                If (computeMode <> CuComputeMode.PROHIBITED) Then

                    Dim sm_per_multiproc As Integer = 0
                    If (major = 9999) AndAlso (minor = 9999) Then
                        sm_per_multiproc = 1
                    Else
                        sm_per_multiproc = ConvertSMVer2CoresDRV(major, minor)
                    End If

                    Dim a As Double = multiProcessorCount * sm_per_multiproc
                    Dim b As Double = a * clockRate
                    Dim compute_perf As ULong = CULng(b) 'multiProcessorCount * sm_per_multiproc * clockRate NOTE Тут происходит OverflowException 
                    If (compute_perf > maxComputePerf) Then
                        maxComputePerf = compute_perf
                        maxPerfDevice = currentDevice
                    End If
                Else
                    devicesProhibited += 1
                End If

                currentDevice += 1
            Loop

            If (devicesProhibited = deviceCount) Then
                Throw New Exception($"{NameOf(GetMaxGflopsDeviceId)} error: all devices have compute mode prohibitied.")
            End If

            Return maxPerfDevice
        End Function

#End Region '/ИНИЦИАЛИЗАЦИЯ

#Region "ИНФО"

        ''' <summary>
        ''' Возвращает число устройств CUDA в системе.
        ''' </summary>
        Public Function DeviceGetCount() As Integer
            Dim count As Integer = 0
            Dim result As CuResult = cuDeviceGetCount(count)
            If (result <> CuResult.SUCCESS) Then
                Throw New CudaException($"{NameOf(cuDeviceGetCount)} failed with code [{result}].", result)
            End If
            Return count
        End Function

        ''' <summary>
        ''' Возвращает значение заданного атрибута <paramref name="attrib"/> устройства <paramref name="deviceId"/>.
        ''' </summary>
        Public Function DeviceGetAttribute(attrib As CuDeviceAttribute, deviceId As Integer) As Integer
            Dim pi As Integer = 0
            Dim result As CuResult = cuDeviceGetAttribute(pi, attrib, deviceId)
            If (result <> CuResult.SUCCESS) Then
                Throw New CudaException($"{NameOf(cuDeviceGetAttribute)} failed with code [{result}].", result)
            End If
            Return pi
        End Function

        ''' <summary>
        ''' Возвращает название устройства <paramref name="deviceId"/>.
        ''' </summary>
        Public Function DeviceGetName(deviceId As Integer) As String
            Dim buf(255) As Byte
            Dim result As CuResult = cuDeviceGetName(buf, 256, deviceId)
            If (result <> CuResult.SUCCESS) Then
                Throw New CudaException($"{NameOf(cuDeviceGetName)} failed with code [{result}].", result)
            End If
            Dim name As String = System.Text.Encoding.ASCII.GetString(buf).Trim({CChar(vbNullChar)})
            Return name
        End Function

        ''' <summary>
        ''' Возвращает версию драйвера.
        ''' </summary>
        Public Function DriverGetVersion() As Integer
            Dim ver As Integer = 0
            Dim result As CuResult = cuDriverGetVersion(ver)
            If (result <> CuResult.SUCCESS) Then
                Throw New CudaException($"{NameOf(cuDriverGetVersion)} failed with code [{result}].", result)
            End If
            Return ver
        End Function

        ''' <summary>
        ''' Возвращает полный объём памяти заданного устройства <paramref name="deviceId"/>.
        ''' </summary>
        Public Function DeviceTotalMem(deviceId As Integer) As ULong
            Dim totalMem As ULong = 0
            Dim result As CuResult = cuDeviceTotalMem_v2(totalMem, deviceId)
            If (result <> CuResult.SUCCESS) Then
                Throw New CudaException($"{NameOf(cuDeviceTotalMem_v2)} failed with code [{result}].", result)
            End If
            Return totalMem
        End Function

        ''' <summary>
        ''' Получает полный объём памяти <paramref name="totalBytes"/> и объём свободной памяти <paramref name="freeBytes"/> устройства <paramref name="deviceId"/>.
        ''' </summary>
        ''' <param name="deviceId">Может быть NULL, если контекст уже создан.</param>
        ''' <param name="freeBytes">Объём свободной памяти GPU, байт.</param>
        ''' <param name="totalBytes">Полный объём памяти GPU, байт.</param>
        Public Sub MemoryInfo(deviceId As Integer?, ByRef freeBytes As ULong, ByRef totalBytes As ULong)
            Dim ctxId As ULong = 0
            If (deviceId IsNot Nothing) Then
                ctxId = ContextCreateId(0, If(deviceId, 0))
            End If

            Dim result As CuResult = cuMemGetInfo(freeBytes, totalBytes)
            If (deviceId IsNot Nothing) Then
                ContextDestroy(ctxId)
            End If
            If (result <> CuResult.SUCCESS) Then
                Throw New CudaException($"{NameOf(cuMemGetInfo)} failed with code [{result}].", result)
            End If
        End Sub

        ''' <summary>
        ''' Имеет ли устройство <paramref name="deviceId"/> доступ к пиру <paramref name="peerDeviceId"/>.
        ''' </summary>
        ''' <param name="deviceId"></param>
        ''' <param name="peerDeviceId"></param>
        Public Function DeviceCanAccessPeer(deviceId As Integer, peerDeviceId As Integer) As Boolean
            Dim canAccess As Integer = 0
            Dim result As CuResult = cuDeviceCanAccessPeer(canAccess, deviceId, peerDeviceId)
            If (result <> CuResult.SUCCESS) Then
                Throw New CudaException($"{NameOf(cuDeviceCanAccessPeer)} failed with code [{result}].", result)
            End If
            Return (canAccess <> 0)
        End Function

#End Region '/ИНФО

#Region "КОМПИЛЯЦИЯ В CUDA PTX"

        ''' <summary>
        ''' Компилирует файл с исходным кодом <paramref name="filename"/> в PTX CUDA бинарный код.
        ''' </summary>
        ''' <param name="gpuDeviceId">Может быть NULL; тогда будет выбрано самое производительное устройство.</param>
        ''' <param name="filename">Путь к файлу с сиходным кодом на CUDA C.</param>
        ''' <param name="cuBinResult">Скомпилированный массив с исполняемым кодом для GPU.</param>
        ''' <param name="cuBinResultSize">Размер результирующего бинарного кода.</param>
        Public Sub CompileFileToCUBIN(gpuDeviceId As Integer?, filename As String, ByRef cuBinResult As Byte(), ByRef cuBinResultSize As ULong)
            CompileSourceCodeToCUBIN(gpuDeviceId, IO.File.ReadAllText(filename), filename, cuBinResult, cuBinResultSize)
        End Sub

        ''' <summary>
        ''' Компилирует исходный код <paramref name="sourceCode"/> в PTX CUDA бинарный код.
        ''' </summary>
        ''' <param name="gpuDeviceId">Может быть NULL; тогда будет выбрано самое производительное устройство.</param>
        ''' <param name="sourceCode">Текст сиходного кода на CUDA C.</param>
        ''' <param name="fileName">Имя файла, под которым будет сохранён исходный код на CUDA C.</param>
        ''' <param name="cuBinResult">Скомпилированный массив с исполняемым кодом для GPU.</param>
        ''' <param name="cuBinResultSize">Размер результирующего бинарного кода.</param>
        Public Sub CompileSourceCodeToCUBIN(gpuDeviceId As Integer?, sourceCode As String, fileName As String,
                                            ByRef cuBinResult As Byte(), ByRef cuBinResultSize As ULong)

            Dim cuDevice As Integer = If(gpuDeviceId, GetMaxGflopsDeviceId())

            ' Get compute capabilities and the device name:
            Dim major As Integer = DeviceGetAttribute(CuDeviceAttribute.COMPUTE_CAPABILITY_MAJOR, cuDevice)
            Dim minor As Integer = DeviceGetAttribute(CuDeviceAttribute.COMPUTE_CAPABILITY_MINOR, cuDevice)

            ' compile
            Dim program As ULong = 0
            Dim headers As ULong = 0
            Dim includeNames As ULong = 0

            Dim nvrtcRes As NvrtcResult = nvrtcCreateProgram(program, sourceCode, fileName, 0, headers, includeNames)
            If (nvrtcRes <> NvrtcResult.SUCCESS) Then
                Throw New NvrtcException($"{NameOf(nvrtcCreateProgram)} failed with code [{nvrtcRes}].", nvrtcRes)
            End If

            ' Compile cubin for the GPU arch on which are going to run cuda kernel:
            Dim numCompileOptions As Integer = 0
            Dim compileParams As String() = {
                $"--gpu-architecture=sm_{major}{minor}",
                ""
            }

            numCompileOptions += 1
            nvrtcRes = nvrtcCompileProgram(program, numCompileOptions, compileParams)
            If (nvrtcRes <> NvrtcResult.SUCCESS) Then
                Throw New NvrtcException($"{NameOf(nvrtcCompileProgram)} failed with code [{nvrtcRes}].", nvrtcRes)
            End If

            Dim codeSize As ULong = 0
            nvrtcRes = nvrtcGetCUBINSize(program, codeSize)
            If (nvrtcRes <> NvrtcResult.SUCCESS) Then
                Throw New NvrtcException($"{NameOf(nvrtcGetCUBINSize)} failed with code [{nvrtcRes}].", nvrtcRes)
            End If

            Dim code(CInt(codeSize - 1)) As Byte
            nvrtcRes = nvrtcGetCUBIN(program, code)
            If (nvrtcRes <> NvrtcResult.SUCCESS) Then
                Throw New NvrtcException($"{NameOf(nvrtcGetCUBIN)} failed with code [{nvrtcRes}].", nvrtcRes)
            End If

            cuBinResult = code
            cuBinResultSize = codeSize
        End Sub

        ''' <summary>
        ''' Загружает CUBIN и возвращает адрес модуля.
        ''' </summary>
        ''' <param name="gpuDeviceId">Может быть NULL.</param>
        ''' <param name="cubin"></param>
        Public Function LoadCUBIN(gpuDeviceId As Integer?, cuBin As Byte()) As ULong

            Dim cuDevice As Integer = If(gpuDeviceId, GetMaxGflopsDeviceId())

            Dim result As CuResult = cuInit(0)
            If (result <> CuResult.SUCCESS) Then
                Throw New CudaException($"{NameOf(cuInit)} failed with code [{result}].", result)
            End If

            Dim context As ULong = 0
            result = cuCtxCreate(context, 0, cuDevice)
            If (result <> CuResult.SUCCESS) Then
                Throw New CudaException($"{NameOf(cuCtxCreate)} failed with code [{result}].", result)
            End If

            Dim [module] As ULong = 0
            result = cuModuleLoadData([module], cuBin)
            If (result <> CuResult.SUCCESS) Then
                Throw New CudaException($"{NameOf(cuModuleLoadData)} failed with code [{result}].", result)
            End If

            Return [module]
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="hfunc"></param>
        ''' <param name="[module]"></param>
        ''' <param name="name"></param>
        Public Sub GetModuleFunction(ByRef hfunc As IntPtr, [module] As ULong, name As String)
            Dim result As CuResult = cuModuleGetFunction(hfunc, [module], name)
            If (result <> CuResult.SUCCESS) Then
                Throw New CudaException($"{NameOf(cuModuleGetFunction)} failed with code [{result}].", result)
            End If
        End Sub

        ''' <summary>
        ''' Вызов функции на GPU.
        ''' </summary>
        ''' <param name="func"></param>
        ''' <param name="gridDimX"></param>
        ''' <param name="gridDimY"></param>
        ''' <param name="gridDimZ"></param>
        ''' <param name="blockDimX"></param>
        ''' <param name="blockDimY"></param>
        ''' <param name="blockDimZ"></param>
        ''' <param name="sharedMemBytes"></param>
        ''' <param name="hStream"></param>
        ''' <param name="kernelParams"></param>
        ''' <param name="extra"></param>
        Public Sub LaunchKernel(func As IntPtr,
                                gridDimX As UInteger, gridDimY As UInteger, gridDimZ As UInteger,
                                blockDimX As UInteger, blockDimY As UInteger, blockDimZ As UInteger,
                                sharedMemBytes As UInteger,
                                hStream As IntPtr,
                                kernelParams As Object(),
                                extra As IntPtr())

            Dim count As Integer = kernelParams.Length
            Dim arr(count - 1) As IntPtr
            Dim gCHandleList(count - 1) As GCHandle

            'Get pointers to kernel parameters
            For i As Integer = 0 To count - 1
                gCHandleList(i) = GCHandle.Alloc(kernelParams(i), GCHandleType.Pinned)
                arr(i) = gCHandleList(i).AddrOfPinnedObject()
            Next

            Dim result As CuResult = cuLaunchKernel(func, gridDimX, gridDimY, gridDimZ, blockDimX, blockDimY, blockDimZ, sharedMemBytes, hStream, arr, extra)
            If (result <> CuResult.SUCCESS) Then
                Throw New CudaException($"{NameOf(cuLaunchKernel)} failed with code [{result}].", result)
            End If
        End Sub

#End Region '/КОМПИЛЯЦИЯ В CUDA PTX

#Region "ВЫДЕЛЕНИЕ И ОСВОБОЖДЕНИЕ ПАМЯТИ"

        ''' <summary>
        ''' Выделяеть память на GPU.
        ''' </summary>
        ''' <param name="size"></param>
        Public Function MemAlloc(size As ULong) As ULong
            Dim devicePtr As ULong = 0
            Dim result As CuResult = cuMemAlloc(devicePtr, size)
            If (result = CuResult.ERROR_OUT_OF_MEMORY) Then
                Throw New Exception("Out of device memory.")
            ElseIf (result <> CuResult.SUCCESS) Then
                Throw New CudaException($"{NameOf(cuMemAlloc)} failed with code [{result}].", result)
            End If
            Return devicePtr
        End Function

        ''' <summary>
        ''' Освобождает память на GPU.
        ''' </summary>
        ''' <param name="devicePtr"></param>
        Public Sub MemFree(devicePtr As ULong)
            Dim result As CuResult = cuMemFree(devicePtr)
            If (result <> CuResult.SUCCESS) Then
                Throw New CudaException($"{NameOf(cuMemFree)} failed with code [{result}].", result)
            End If
        End Sub

#End Region '/ВЫДЕЛЕНИЕ И ОСВОБОЖДЕНИЕ ПАМЯТИ

#Region "КОПИРОВАНИЕ ДАННЫХ НА И С GPU"

        Public Sub MemcpyHostToDev(destDevice As ULong, srcHost As Byte(), size As ULong)
            Dim result As CuResult = cuMemcpyHtoD(destDevice, srcHost, size)
            If (result <> CuResult.SUCCESS) Then
                Throw New CudaException($"{NameOf(cuMemcpyHtoD)} failed with code [{result}].", result)
            End If
        End Sub

        Public Sub MemcpyHostToDev(destDevice As ULong, srcHost As UShort(), size As ULong)
            Dim result As CuResult = cuMemcpyHtoD(destDevice, srcHost, size)
            If (result <> CuResult.SUCCESS) Then
                Throw New CudaException($"{NameOf(cuMemcpyHtoD)} failed with code [{result}].", result)
            End If
        End Sub

        Public Sub MemcpyHostToDev(destDevice As ULong, srcHost As UInteger(), size As ULong)
            Dim result As CuResult = cuMemcpyHtoD(destDevice, srcHost, size)
            If (result <> CuResult.SUCCESS) Then
                Throw New CudaException($"{NameOf(cuMemcpyHtoD)} failed with code [{result}].", result)
            End If
        End Sub

        Public Sub MemcpyHostToDev(destDevice As ULong, srcHost As ULong(), size As ULong)
            Dim result As CuResult = cuMemcpyHtoD(destDevice, srcHost, size)
            If (result <> CuResult.SUCCESS) Then
                Throw New CudaException($"{NameOf(cuMemcpyHtoD)} failed with code [{result}].", result)
            End If
        End Sub

        Public Sub MemcpyHostToDev(destDevice As ULong, srcHost As Short(), size As ULong)
            Dim result As CuResult = cuMemcpyHtoD(destDevice, srcHost, size)
            If (result <> CuResult.SUCCESS) Then
                Throw New CudaException($"{NameOf(cuMemcpyHtoD)} failed with code [{result}].", result)
            End If
        End Sub

        Public Sub MemcpyHostToDev(destDevice As ULong, srcHost As Integer(), size As ULong)
            Dim result As CuResult = cuMemcpyHtoD(destDevice, srcHost, size)
            If (result <> CuResult.SUCCESS) Then
                Throw New CudaException($"{NameOf(cuMemcpyHtoD)} failed with code [{result}].", result)
            End If
        End Sub

        Public Sub MemcpyHostToDev(destDevice As ULong, srcHost As Long(), size As ULong)
            Dim result As CuResult = cuMemcpyHtoD(destDevice, srcHost, size)
            If (result <> CuResult.SUCCESS) Then
                Throw New CudaException($"{NameOf(cuMemcpyHtoD)} failed with code [{result}].", result)
            End If
        End Sub

        Public Sub MemcpyHostToDev(destDevice As ULong, srcHost As Single(), size As ULong)
            Dim result As CuResult = cuMemcpyHtoD(destDevice, srcHost, size)
            If (result <> CuResult.SUCCESS) Then
                Throw New CudaException($"{NameOf(cuMemcpyHtoD)} failed with code [{result}].", result)
            End If
        End Sub

        Public Sub MemcpyHostToDev(destDevice As ULong, srcHost As Double(), size As ULong)
            Dim result As CuResult = cuMemcpyHtoD(destDevice, srcHost, size)
            If (result <> CuResult.SUCCESS) Then
                Throw New CudaException($"{NameOf(cuMemcpyHtoD)} failed with code [{result}].", result)
            End If
        End Sub

        Public Sub MemcpyDevToHost(destHost As Byte(), srcDevice As ULong, size As ULong)
            Dim result As CuResult = cuMemcpyDtoH(destHost, srcDevice, size)
            If (result <> CuResult.SUCCESS) Then
                Throw New CudaException($"{NameOf(cuMemcpyDtoH)} failed with code [{result}].", result)
            End If
        End Sub

        Public Sub MemcpyDevToHost(destHost As UShort(), srcDevice As ULong, size As ULong)
            Dim result As CuResult = cuMemcpyDtoH(destHost, srcDevice, size)
            If (result <> CuResult.SUCCESS) Then
                Throw New CudaException($"{NameOf(cuMemcpyDtoH)} failed with code [{result}].", result)
            End If
        End Sub

        Public Sub MemcpyDevToHost(destHost As UInteger(), srcDevice As ULong, size As ULong)
            Dim result As CuResult = cuMemcpyDtoH(destHost, srcDevice, size)
            If (result <> CuResult.SUCCESS) Then
                Throw New CudaException($"{NameOf(cuMemcpyDtoH)} failed with code [{result}].", result)
            End If
        End Sub

        Public Sub MemcpyDevToHost(destHost As ULong(), srcDevice As ULong, size As ULong)
            Dim result As CuResult = cuMemcpyDtoH(destHost, srcDevice, size)
            If (result <> CuResult.SUCCESS) Then
                Throw New CudaException($"{NameOf(cuMemcpyDtoH)} failed with code [{result}].", result)
            End If
        End Sub

        Public Sub MemcpyDevToHost(destHost As Short(), srcDevice As ULong, size As ULong)
            Dim result As CuResult = cuMemcpyDtoH(destHost, srcDevice, size)
            If (result <> CuResult.SUCCESS) Then
                Throw New CudaException($"{NameOf(cuMemcpyDtoH)} failed with code [{result}].", result)
            End If
        End Sub

        Public Sub MemcpyDevToHost(destHost As Integer(), srcDevice As ULong, size As ULong)
            Dim result As CuResult = cuMemcpyDtoH(destHost, srcDevice, size)
            If (result <> CuResult.SUCCESS) Then
                Throw New CudaException($"{NameOf(cuMemcpyDtoH)} failed with code [{result}].", result)
            End If
        End Sub

        Public Sub MemcpyDevToHost(destHost As Long(), srcDevice As ULong, size As ULong)
            Dim result As CuResult = cuMemcpyDtoH(destHost, srcDevice, size)
            If (result <> CuResult.SUCCESS) Then
                Throw New CudaException($"{NameOf(cuMemcpyDtoH)} failed with code [{result}].", result)
            End If
        End Sub

        Public Sub MemcpyDevToHost(destHost As Single(), srcDevice As ULong, size As ULong)
            Dim result As CuResult = cuMemcpyDtoH(destHost, srcDevice, size)
            If (result <> CuResult.SUCCESS) Then
                Throw New CudaException($"{NameOf(cuMemcpyDtoH)} failed with code [{result}].", result)
            End If
        End Sub

        Public Sub MemcpyDevToHost(destHost As Double(), srcDevice As ULong, size As ULong)
            Dim result As CuResult = cuMemcpyDtoH(destHost, srcDevice, size)
            If (result <> CuResult.SUCCESS) Then
                Throw New CudaException($"{NameOf(cuMemcpyDtoH)} failed with code [{result}].", result)
            End If
        End Sub

        Public Sub MemcpyDevToDev(destDevice As ULong, srcDevice As ULong, size As ULong)
            Dim result As CuResult = cuMemcpyDtoD(destDevice, srcDevice, size)
            If (result <> CuResult.SUCCESS) Then
                Throw New CudaException($"{NameOf(cuMemcpyDtoD)} failed with code [{result}].", result)
            End If
        End Sub

#End Region '/КОПИРОВАНИЕ ДАННЫХ НА И С GPU

#Region "УПРАВЛЕНИЕ КОНТЕКСТОМ"

        Public Function ContextCreateId(flags As UInteger, deviceId As Integer) As ULong
            Dim ctx As ULong = 0
            Dim result As CuResult = cuCtxCreate(ctx, flags, deviceId)
            If (result <> CuResult.SUCCESS) Then
                Throw New CudaException($"{NameOf(cuCtxCreate)} failed with code [{result}].", result)
            End If
            Return ctx
        End Function

        Public Sub ContextSynchronize()
            Dim result As CuResult = cuCtxSynchronize()
            If (result <> CuResult.SUCCESS) Then
                Throw New CudaException($"{NameOf(cuCtxSynchronize)} failed with code [{result}].", result)
            End If
        End Sub

        Public Sub ContextDestroy(contextId As ULong)
            Dim result As CuResult = cuCtxDestroy(contextId)
            If (result <> CuResult.SUCCESS) Then
                Throw New CudaException($"{NameOf(cuCtxDestroy)} failed with code [{result}].", result)
            End If
        End Sub

#End Region '/УПРАВЛЕНИЕ КОНТЕКСТОМ

#Region "ВСПОМОГАТЕЛЬНЫЕ"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="ordinal"></param>
        Public Function DeviceGet(ordinal As Integer) As Integer
            Dim dev As Integer = 0
            Dim result As CuResult = cuDeviceGet(dev, ordinal)
            If (result <> CuResult.SUCCESS) Then
                Throw New CudaException($"{NameOf(cuDeviceGet)} failed with code [{result}].", result)
            End If
            Return dev
        End Function

        ''' <summary>
        ''' Возвращает количество ядер на один SM (Streaming Multiprocessors) по известной версии драйвера.
        ''' </summary>
        Public Function ConvertSMVer2CoresDRV(major As Integer, minor As Integer) As Integer
            ' Defines for GPU Architecture types (using the SM version to determine the # of cores per SM):
            Dim nGpuArchCoresPerSM As SmToCores() = {
                New SmToCores(&H30, 192),
                New SmToCores(&H32, 192),
                New SmToCores(&H35, 192),
                New SmToCores(&H37, 192),
                New SmToCores(&H50, 128),
                New SmToCores(&H52, 128),
                New SmToCores(&H53, 128),
                New SmToCores(&H60, 64),
                New SmToCores(&H61, 128),
                New SmToCores(&H62, 128),
                New SmToCores(&H70, 64),
                New SmToCores(&H72, 64),
                New SmToCores(&H75, 64),
                New SmToCores(&H80, 64),
                New SmToCores(&H86, 128),
                New SmToCores(&H87, 128),
                New SmToCores(-1, -1)
            }

            Dim index As Integer = 0
            Do While (nGpuArchCoresPerSM(index).SM <> -1)
                If (nGpuArchCoresPerSM(index).SM = ((major << 4) + minor)) Then
                    Return nGpuArchCoresPerSM(index).Cores
                End If
                index += 1
            Loop

            'If we don't find the values, we default use the previous one to run properly:
            Return nGpuArchCoresPerSM(index - 1).Cores
        End Function

        ''' <summary>
        ''' Проверяет наличие необходимых DLL в текущей директории и при отсутствии создаёт.
        ''' </summary>
        Private Sub CheckDlls()
            Dim dlls As New Dictionary(Of String, Byte()) From {
                {"nvrtc64_112_0.dll", My.Resources.nvrtc64_112_0_dll},
                {"nvrtc-builtins64_115.dll", My.Resources.nvrtc_builtins64_115_dll}
            }
            For Each dll In dlls
                If (Not IO.File.Exists(dll.Key)) Then
                    Using compressed As New IO.MemoryStream(dll.Value),
                        gzStm As New IO.Compression.GZipStream(compressed, IO.Compression.CompressionMode.Decompress),
                        decompressed As IO.FileStream = IO.File.Create(dll.Key)

                        gzStm.CopyTo(decompressed)

                    End Using
                End If
            Next
        End Sub

#End Region '/ВСПОМОГАТЕЛЬНЫЕ

#Region "NATIVE"

        'Информационные методы:

        Private Const NvCudaLibName As String = "nvcuda"

        <DllImport(NvCudaLibName, CallingConvention:=CallingConvention.StdCall)>
        Private Function cuInit(flags As UInteger) As CuResult
        End Function

        <DllImport(NvCudaLibName, CallingConvention:=CallingConvention.StdCall)>
        Private Function cuMemGetInfo(ByRef free As ULong, ByRef total As ULong) As CuResult
        End Function

        <DllImport(NvCudaLibName, CallingConvention:=CallingConvention.StdCall)>
        Private Function cuDeviceGetCount(ByRef count As Integer) As CuResult
        End Function

        <DllImport(NvCudaLibName, CallingConvention:=CallingConvention.StdCall)>
        Private Function cuDeviceGetAttribute(ByRef pi As Integer, attrib As CuDeviceAttribute, dev As Integer) As CuResult
        End Function

        <DllImport(NvCudaLibName, CallingConvention:=CallingConvention.StdCall)>
        Private Function cuDeviceGetName(name As Byte(), len As Integer, dev As Integer) As CuResult
        End Function

        <DllImport(NvCudaLibName, CallingConvention:=CallingConvention.StdCall)>
        Private Function cuDriverGetVersion(ByRef driverVersion As Integer) As CuResult
        End Function

        <DllImport(NvCudaLibName, CallingConvention:=CallingConvention.StdCall)>
        Private Function cuDeviceTotalMem_v2(ByRef bytes As ULong, dev As Integer) As CuResult
        End Function

        <DllImport(NvCudaLibName, CallingConvention:=CallingConvention.StdCall)>
        Private Function cuDeviceCanAccessPeer(ByRef canAccessPeer As Integer, dev As Integer, peerDev As Integer) As CuResult
        End Function

        'Методы для выделения и освобождения памяти на устройстве:

        <DllImport(NvCudaLibName, CallingConvention:=CallingConvention.StdCall)>
        Private Function cuMemAlloc(ByRef dptr As ULong, bytesize As ULong) As CuResult
        End Function

        <DllImport(NvCudaLibName, CallingConvention:=CallingConvention.StdCall)>
        Private Function cuMemFree(dptr As ULong) As CuResult
        End Function

        'Методы для копирования данных с хоста на устройство:

        <DllImport(NvCudaLibName, CallingConvention:=CallingConvention.StdCall)>
        Private Function cuMemcpyHtoD(dstDevice As ULong, srcHost As Byte(), byteCount As ULong) As CuResult
        End Function

        <DllImport(NvCudaLibName, CallingConvention:=CallingConvention.StdCall)>
        Private Function cuMemcpyHtoD(dstDevice As ULong, srcHost As UShort(), byteCount As ULong) As CuResult
        End Function

        <DllImport(NvCudaLibName, CallingConvention:=CallingConvention.StdCall)>
        Private Function cuMemcpyHtoD(dstDevice As ULong, srcHost As UInteger(), byteCount As ULong) As CuResult
        End Function

        <DllImport(NvCudaLibName, CallingConvention:=CallingConvention.StdCall)>
        Private Function cuMemcpyHtoD(dstDevice As ULong, srcHost As ULong(), byteCount As ULong) As CuResult
        End Function

        <DllImport(NvCudaLibName, CallingConvention:=CallingConvention.StdCall)>
        Private Function cuMemcpyHtoD(dstDevice As ULong, srcHost As Short(), byteCount As ULong) As CuResult
        End Function

        <DllImport(NvCudaLibName, CallingConvention:=CallingConvention.StdCall)>
        Private Function cuMemcpyHtoD(dstDevice As ULong, srcHost As Integer(), byteCount As ULong) As CuResult
        End Function

        <DllImport(NvCudaLibName, CallingConvention:=CallingConvention.StdCall)>
        Private Function cuMemcpyHtoD(dstDevice As ULong, srcHost As Long(), byteCount As ULong) As CuResult
        End Function

        <DllImport(NvCudaLibName, CallingConvention:=CallingConvention.StdCall)>
        Private Function cuMemcpyHtoD(dstDevice As ULong, srcHost As Single(), byteCount As ULong) As CuResult
        End Function

        <DllImport(NvCudaLibName, CallingConvention:=CallingConvention.StdCall)>
        Private Function cuMemcpyHtoD(dstDevice As ULong, srcHost As Double(), byteCount As ULong) As CuResult
        End Function

        'Методы для копирования данных с устройства на хост:

        <DllImport(NvCudaLibName, CallingConvention:=CallingConvention.StdCall)>
        Private Function cuMemcpyDtoH(dstHost As Byte(), srcDevice As ULong, byteCount As ULong) As CuResult
        End Function

        <DllImport(NvCudaLibName, CallingConvention:=CallingConvention.StdCall)>
        Private Function cuMemcpyDtoH(dstHost As UShort(), srcDevice As ULong, byteCount As ULong) As CuResult
        End Function

        <DllImport(NvCudaLibName, CallingConvention:=CallingConvention.StdCall)>
        Private Function cuMemcpyDtoH(dstHost As UInteger(), srcDevice As ULong, byteCount As ULong) As CuResult
        End Function

        <DllImport(NvCudaLibName, CallingConvention:=CallingConvention.StdCall)>
        Private Function cuMemcpyDtoH(dstHost As ULong(), srcDevice As ULong, byteCount As ULong) As CuResult
        End Function

        <DllImport(NvCudaLibName, CallingConvention:=CallingConvention.StdCall)>
        Private Function cuMemcpyDtoH(dstHost As Short(), srcDevice As ULong, byteCount As ULong) As CuResult
        End Function

        <DllImport(NvCudaLibName, CallingConvention:=CallingConvention.StdCall)>
        Private Function cuMemcpyDtoH(dstHost As Integer(), srcDevice As ULong, byteCount As ULong) As CuResult
        End Function

        <DllImport(NvCudaLibName, CallingConvention:=CallingConvention.StdCall)>
        Private Function cuMemcpyDtoH(dstHost As Long(), srcDevice As ULong, byteCount As ULong) As CuResult
        End Function

        <DllImport(NvCudaLibName, CallingConvention:=CallingConvention.StdCall)>
        Private Function cuMemcpyDtoH(dstHost As Single(), srcDevice As ULong, byteCount As ULong) As CuResult
        End Function

        <DllImport(NvCudaLibName, CallingConvention:=CallingConvention.StdCall)>
        Private Function cuMemcpyDtoH(dstHost As Double(), srcDevice As ULong, byteCount As ULong) As CuResult
        End Function

        'Метод для копирования с устройства на устройство:

        <DllImport(NvCudaLibName, CallingConvention:=CallingConvention.StdCall)>
        Private Function cuMemcpyDtoD(dstDevice As ULong, srcDevice As ULong, byteCount As ULong) As CuResult
        End Function

        'Разные:

        <DllImport(NvCudaLibName, CallingConvention:=CallingConvention.StdCall)>
        Private Function cuDeviceGet(ByRef device As Integer, ordinal As Integer) As CuResult
        End Function

        <DllImport(NvCudaLibName, CallingConvention:=CallingConvention.StdCall)>
        Private Function cuCtxCreate(ByRef pctx As ULong, flags As UInteger, dev As Integer) As CuResult
        End Function

        <DllImport(NvCudaLibName, CallingConvention:=CallingConvention.StdCall)>
        Private Function cuCtxSynchronize() As CuResult
        End Function

        <DllImport(NvCudaLibName, CallingConvention:=CallingConvention.StdCall)>
        Private Function cuCtxDestroy(ctx As ULong) As CuResult
        End Function

        'Работа с модулями:

        <DllImport(NvCudaLibName, CallingConvention:=CallingConvention.StdCall)>
        Private Function cuLaunchKernel(func As IntPtr, gridDimX As UInteger, gridDimY As UInteger, gridDimZ As UInteger, blockDimX As UInteger, blockDimY As UInteger, blockDimZ As UInteger, sharedMemBytes As UInteger, hStream As IntPtr, kernelParams As IntPtr(), extra As IntPtr()) As CuResult
        End Function

        <DllImport(NvCudaLibName, CallingConvention:=CallingConvention.StdCall)>
        Private Function cuModuleLoadData(ByRef [module] As ULong, image As Byte()) As CuResult
        End Function

        <DllImport(NvCudaLibName, CallingConvention:=CallingConvention.StdCall)>
        Private Function cuModuleGetFunction(ByRef hfunc As IntPtr, hmod As ULong, name As String) As CuResult
        End Function

        'Методы для компиляции CUDA C в CUBIN:

        Private Const NvRtcLibName As String = "nvrtc64_112_0"

        <DllImport(NvRtcLibName, CallingConvention:=CallingConvention.Cdecl)>
        Private Function nvrtcCreateProgram(ByRef program As ULong, sourceCode As String, fileName As String, numHeaders As Integer, ByRef headers As ULong, ByRef includeNames As ULong) As NvrtcResult
        End Function

        <DllImport(NvRtcLibName, CallingConvention:=CallingConvention.Cdecl)>
        Private Function nvrtcCompileProgram(prog As ULong, numOptions As Integer, options As String()) As NvrtcResult
        End Function

        <DllImport(NvRtcLibName, CallingConvention:=CallingConvention.Cdecl)>
        Private Function nvrtcGetCUBINSize(prog As ULong, ByRef cubinSizeRet As ULong) As NvrtcResult
        End Function

        <DllImport(NvRtcLibName, CallingConvention:=CallingConvention.Cdecl)>
        Private Function nvrtcGetCUBIN(prog As ULong, cubin As Byte()) As NvrtcResult
        End Function

#End Region '/NATIVE

    End Module

End Namespace