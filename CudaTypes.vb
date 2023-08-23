Imports System.Runtime.InteropServices

Namespace CudaForVb

    <StructLayout(LayoutKind.Sequential)>
    Public Structure UInt3

        Public ReadOnly X As UInteger
        Public ReadOnly Y As UInteger
        Public ReadOnly Z As UInteger

    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Public Structure Dim3

        Public ReadOnly X As UInteger
        Public ReadOnly Y As UInteger
        Public ReadOnly Z As UInteger

        Public Sub New(Optional vx As UInteger = 1, Optional vy As UInteger = 1, Optional vz As UInteger = 1)
            X = vx
            Y = vy
            Z = vz
        End Sub

        Public Sub New(v As Uint3)
            X = v.x
            Y = v.y
            Z = v.z
        End Sub

    End Structure

    ''' <summary>
    ''' Количество ядер на SM (Streaming Multiprocessor).
    ''' </summary>
    Friend Structure SmToCores

        ''' <summary>
        ''' ID потокового мультипроцессора.
        ''' </summary>
        ''' <remarks>
        ''' 0xMm (hexadecimal notation).
        ''' M = SM Major version, and m = SM minor version.
        ''' </remarks>
        Public ReadOnly SM As Integer
        ''' <summary>
        ''' Число ядер.
        ''' </summary>
        Public ReadOnly Cores As Integer

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="sm">0xMm: M = major version, m = minor version.</param>
        ''' <param name="cores"></param>
        Public Sub New(sm As Integer, cores As Integer)
            Me.SM = sm
            Me.Cores = cores
        End Sub

    End Structure

    ''' <summary>
    ''' Коды результатов выполнения вызовов CUDA API.
    ''' </summary>
    ''' <remarks>
    ''' Все члены перечисления имели префикс "CUDA_".
    ''' </remarks>
    Public Enum CuResult As Integer

        ''' <summary>
        '''  The API call returned with no errors. 
        '''  In the case of query calls this also means that the operation being queried is complete (see :cuEventQuery() and :cuStreamQuery()).
        ''' </summary>
        SUCCESS = 0

        ''' <summary>
        ''' This indicates that one or more of the parameters passed to the API call is not within an acceptable range of values.
        ''' </summary>
        ERROR_INVALID_VALUE = 1

        ''' <summary>
        ''' The API call failed because it was unable to allocate enough memory to perform the requested operation.
        ''' </summary>
        ERROR_OUT_OF_MEMORY = 2

        ''' <summary>
        ''' This indicates that the CUDA driver has not been initialized with :cuInit() or that initialization has failed.
        ''' </summary>
        ERROR_NOT_INITIALIZED = 3

        ''' <summary>
        ''' This indicates that the CUDA driver is in the process of shutting down.
        ''' </summary>
        ERROR_DEINITIALIZED = 4

        ''' <summary>
        ''' This indicates profiler is not initialized for this run. 
        ''' This can happen when the application is running with external profiling tools Like visual profiler.
        ''' </summary>
        ERROR_PROFILER_DISABLED = 5

        ''' <summary>
        ''' This error return is deprecated as of CUDA 5.0. 
        ''' It is no longer an error to attempt to enable/disable the profiling via :cuProfilerStart or :cuProfilerStop without initialization.
        ''' </summary>
        <Obsolete()>
        ERROR_PROFILER_NOT_INITIALIZED = 6

        ''' <summary>
        ''' This error return is deprecated as of CUDA 5.0. 
        ''' It is no longer an error to call cuProfilerStart() when profiling is already enabled.
        ''' </summary>
        <Obsolete()>
        ERROR_PROFILER_ALREADY_STARTED = 7

        ''' <summary>
        ''' This error return is deprecated as of CUDA 5.0. 
        ''' It is no longer an error to call cuProfilerStop() when profiling is already disabled.
        ''' </summary>
        <Obsolete()>
        ERROR_PROFILER_ALREADY_STOPPED = 8


        ''' <summary>
        ''' This indicates that the CUDA driver that the application has loaded is a stub library. 
        ''' Applications that run with the stub rather than a real driver loaded will result in CUDA API returning this error.
        ''' </summary>
        ERROR_STUB_LIBRARY = 34

        ''' <summary>
        ''' This indicates that no CUDA-capable devices were detected by the installed CUDA driver.
        ''' </summary>
        ERROR_NO_DEVICE = 100

        ''' <summary>
        '''This indicates that the device ordinal supplied by the user does not correspond to a valid CUDA device 
        '''or that the action requested is invalid for the specified device.
        ''' </summary>
        ERROR_INVALID_DEVICE = 101

        ''' <summary>
        ''' This error indicates that the Grid license is not applied.
        ''' </summary>
        ERROR_DEVICE_NOT_LICENSED = 102

        ''' <summary>
        ''' This indicates that the device kernel image is invalid. 
        ''' This can also indicate an invalid CUDA module.
        ''' </summary>
        ERROR_INVALID_IMAGE = 200

        ''' <summary>
        ''' This most frequently indicates that there is no context bound to the current thread. 
        ''' This can also be returned if the context passed to an API call is not a valid handle 
        ''' (such as a context that has had :cuCtxDestroy() invoked on it). 
        ''' This can also be returned if a user mixes different API versions (i.e. 3010 context with 3020 API calls).
        ''' See :cuCtxGetApiVersion() for more details.
        ''' </summary>
        ERROR_INVALID_CONTEXT = 201

        ''' <summary>
        ''' This indicated that the context being supplied as a parameter to the API call was already the active context.
        ''' This error return is deprecated as of CUDA 3.2. 
        ''' It is no longer an error to attempt to push the active context via :cuCtxPushCurrent().
        ''' </summary>
        <Obsolete()>
        ERROR_CONTEXT_ALREADY_CURRENT = 202

        ''' <summary>
        ''' This indicates that a map or register operation has failed.
        ''' </summary>
        ERROR_MAP_FAILED = 205

        ''' <summary>
        ''' This indicates that an unmap or unregister operation has failed.
        ''' </summary>
        ERROR_UNMAP_FAILED = 206

        ''' <summary>
        ''' This indicates that the specified array is currently mapped and thus cannot be destroyed.
        ''' </summary>
        ERROR_ARRAY_IS_MAPPED = 207

        ''' <summary>
        ''' This indicates that the resource is already mapped.
        ''' </summary>
        ERROR_ALREADY_MAPPED = 208

        ''' <summary>
        ''' This indicates that there is no kernel image available that is suitable
        ''' for the device. This can occur when a user specifies code generation
        ''' options for a particular CUDA source file that do not include the
        ''' corresponding device configuration.
        ''' </summary>
        ERROR_NO_BINARY_FOR_GPU = 209

        ''' <summary>
        ''' This indicates that a resource has already been acquired.
        ''' </summary>
        ERROR_ALREADY_ACQUIRED = 210

        ''' <summary>
        ''' This indicates that a resource is not mapped.
        ''' </summary>
        ERROR_NOT_MAPPED = 211

        ''' <summary>
        ''' This indicates that a mapped resource is not available for access as an array.
        ''' </summary>
        ERROR_NOT_MAPPED_AS_ARRAY = 212

        ''' <summary>
        ''' This indicates that a mapped resource is not available for access as a pointer.
        ''' </summary>
        ERROR_NOT_MAPPED_AS_POINTER = 213

        ''' <summary>
        ''' This indicates that an uncorrectable ECC error was detected during execution.
        ''' </summary>
        ERROR_ECC_UNCORRECTABLE = 214

        ''' <summary>
        ''' This indicates that the :CUlimit passed To the API Call is not supported by the active device.
        ''' </summary>
        ERROR_UNSUPPORTED_LIMIT = 215

        ''' <summary>
        ''' This indicates that the :CUcontext passed to the API call can only be bound 
        ''' to a single CPU thread at a time but is already bound to a CPU thread.
        ''' </summary>
        ERROR_CONTEXT_ALREADY_IN_USE = 216

        ''' <summary>
        ''' This indicates that peer access is not supported across the given devices.
        ''' </summary>
        ERROR_PEER_ACCESS_UNSUPPORTED = 217

        ''' <summary>
        ''' This indicates that a PTX JIT compilation failed.
        ''' </summary>
        ERROR_INVALID_PTX = 218

        ''' <summary>
        ''' This indicates an error with OpenGL or DirectX context.
        ''' </summary>
        ERROR_INVALID_GRAPHICS_CONTEXT = 219

        ''' <summary>
        ''' This indicates that an uncorrectable NVLink error was detected during the execution.
        ''' </summary>
        ERROR_NVLINK_UNCORRECTABLE = 220

        ''' <summary>
        ''' This indicates that the PTX JIT compiler library was not found.
        ''' </summary>
        ERROR_JIT_COMPILER_NOT_FOUND = 221

        ''' <summary>
        ''' This indicates that the provided PTX was compiled with an unsupported toolchain.
        ''' </summary>
        ERROR_UNSUPPORTED_PTX_VERSION = 222

        ''' <summary>
        ''' This indicates that the PTX JIT compilation was disabled.
        ''' </summary>
        ERROR_JIT_COMPILATION_DISABLED = 223

        ''' <summary>
        ''' This indicates that the :CUexecAffinityType passed to the API call is not supported by the active device.
        ''' </summary>
        ERROR_UNSUPPORTED_EXEC_AFFINITY = 224

        ''' <summary>
        ''' This indicates that the device kernel source is invalid. 
        ''' This includes compilation/linker errors encountered in device code or user error.
        ''' </summary>
        ERROR_INVALID_SOURCE = 300

        ''' <summary>
        ''' This indicates that the file specified was not found.
        ''' </summary>
        ERROR_FILE_NOT_FOUND = 301

        ''' <summary>
        ''' This indicates that a link to a shared object failed to resolve.
        ''' </summary>
        ERROR_SHARED_OBJECT_SYMBOL_NOT_FOUND = 302

        ''' <summary>
        ''' This indicates that initialization of a shared object failed.
        ''' </summary>
        ERROR_SHARED_OBJECT_INIT_FAILED = 303

        ''' <summary>
        ''' This indicates that an OS call failed.
        ''' </summary>
        ERROR_OPERATING_SYSTEM = 304

        ''' <summary>
        ''' This indicates that a resource handle passed to the API call was not valid. 
        ''' Resource handles are opaque types Like :CUstream and :CUevent.
        ''' </summary>
        ERROR_INVALID_HANDLE = 400

        ''' <summary>
        ''' This indicates that a resource required by the API call is not in a valid state to perform the requested operation.
        ''' </summary>
        ERROR_ILLEGAL_STATE = 401

        ''' <summary>
        ''' This indicates that a named symbol was not found. 
        ''' Examples of symbols are global/constant variable names driver function names texture names and surface names.
        ''' </summary>
        ERROR_NOT_FOUND = 500

        ''' <summary>
        ''' This indicates that asynchronous operations issued previously have not completed yet. 
        ''' This result is not actually an error but must be indicated differently than :SUCCESS (which indicates completion). 
        ''' Calls that may return this value include :cuEventQuery() and :cuStreamQuery().
        ''' </summary>
        ERROR_NOT_READY = 600

        ''' <summary>
        ''' While executing a kernel the device encountered a load or store instruction on an invalid memory address.
        ''' This leaves the process in an inconsistent state and any further CUDA work will return the same error. 
        ''' To continue using CUDA the process must be terminated and relaunched.
        ''' </summary>
        ERROR_ILLEGAL_ADDRESS = 700

        ''' <summary>
        ''' This indicates that a launch did not occur because it did not have appropriate resources. 
        ''' This error usually indicates that the user has attempted to pass too many arguments 
        ''' to the device kernel or the kernel launch specifies too many threads for the kernel's register count. 
        ''' Passing arguments of the wrong size (i.e. a 64-bit pointer when a 32-bit int is expected) 
        ''' is equivalent to passing too many arguments and can also result in this error.
        ''' </summary>
        ERROR_LAUNCH_OUT_OF_RESOURCES = 701

        ''' <summary>
        ''' This indicates that the device kernel took too long to execute. 
        ''' This can only occur if timeouts are enabled - see the device attribute
        ''' :CU_DEVICE_ATTRIBUTE_KERNEL_EXEC_TIMEOUT for more information.
        ''' This leaves the process in an inconsistent state and any further CUDA work will return the same error. 
        ''' To continue using CUDA the process must be terminated and relaunched.
        ''' </summary>
        ERROR_LAUNCH_TIMEOUT = 702

        ''' <summary>
        ''' This error indicates a kernel launch that uses an incompatible texturing mode.
        ''' </summary>
        ERROR_LAUNCH_INCOMPATIBLE_TEXTURING = 703

        ''' <summary>
        ''' This error indicates that a call to :cuCtxEnablePeerAccess() is trying to re-enable peer access 
        ''' to a context which has already had peer access to it enabled.
        ''' </summary>
        ERROR_PEER_ACCESS_ALREADY_ENABLED = 704

        ''' <summary>
        ''' This error indicates that :cuCtxDisablePeerAccess() is trying to disable 
        ''' peer access which has not been enabled yet via :cuCtxEnablePeerAccess().
        ''' </summary>
        ERROR_PEER_ACCESS_NOT_ENABLED = 705

        ''' <summary>
        ''' This error indicates that the primary context for the specified device has already been initialized.
        ''' </summary>
        ERROR_PRIMARY_CONTEXT_ACTIVE = 708

        ''' <summary>
        ''' This error indicates that the context current to the calling thread has been destroyed using :cuCtxDestroy 
        ''' or is a primary context which has not yet been initialized.
        ''' </summary>
        ERROR_CONTEXT_IS_DESTROYED = 709

        ''' <summary>
        ''' A device-side assert triggered during kernel execution. 
        ''' The context cannot be used anymore and must be destroyed. 
        ''' All existing device memory allocations from this context are invalid and must be reconstructed if the program is to continue using CUDA.
        ''' </summary>
        ERROR_ASSERT = 710

        ''' <summary>
        ''' This error indicates that the hardware resources required to enable peer access have been exhausted 
        ''' for one or more of the devices passed to :cuCtxEnablePeerAccess().
        ''' </summary>
        ERROR_TOO_MANY_PEERS = 711

        ''' <summary>
        ''' This error indicates that the memory range passed to :cuMemHostRegister() has already been registered.
        ''' </summary>
        ERROR_HOST_MEMORY_ALREADY_REGISTERED = 712

        ''' <summary>
        ''' This error indicates that the pointer passed to :cuMemHostUnregister() does not correspond to any currently registered memory region.
        ''' </summary>
        ERROR_HOST_MEMORY_NOT_REGISTERED = 713

        ''' <summary>
        ''' While executing a kernel the device encountered a stack error.
        ''' This can be due to stack corruption or exceeding the stack size limit.
        ''' This leaves the process in an inconsistent state and any further CUDA work will return the same error. 
        ''' To continue using CUDA the process must be terminated and relaunched.
        ''' </summary>
        ERROR_HARDWARE_STACK_ERROR = 714

        ''' <summary>
        ''' While executing a kernel the device encountered an illegal instruction.
        ''' This leaves the process in an inconsistent state and any further CUDA work will return the same error. 
        ''' To continue using CUDA the process must be terminated and relaunched.
        ''' </summary>
        ERROR_ILLEGAL_INSTRUCTION = 715

        ''' <summary>
        ''' While executing a kernel the device encountered a load or store instruction on a memory address which is not aligned.
        ''' This leaves the process in an inconsistent state and any further CUDA work will return the same error. 
        ''' To continue using CUDA the process must be terminated and relaunched.
        ''' </summary>
        ERROR_MISALIGNED_ADDRESS = 716

        ''' <summary>
        ''' While executing a kernel the device encountered an instruction
        ''' which can only operate on memory locations in certain address spaces
        ''' (global shared or local) but was supplied a memory address not belonging to an allowed address space.
        ''' This leaves the process in an inconsistent state and any further CUDA work will return the same error. 
        ''' To continue using CUDA the process must be terminated and relaunched.
        ''' </summary>
        ERROR_INVALID_ADDRESS_SPACE = 717

        ''' <summary>
        ''' While executing a kernel the device program counter wrapped its address space.
        ''' This leaves the process in an inconsistent state and any further CUDA work will return the same error. 
        ''' To continue using CUDA the process must be terminated and relaunched.
        ''' </summary>
        ERROR_INVALID_PC = 718

        ''' <summary>
        ''' An exception occurred on the device while executing a kernel.
        ''' Common causes include dereferencing an invalid device pointer and accessing out of bounds shared memory. 
        ''' Less common cases can be system specific - more information about these cases can be found in the system specific user guide.
        ''' This leaves the process in an inconsistent state and any further CUDA work will return the same error. 
        ''' To continue using CUDA the process must be terminated and relaunched.
        ''' </summary>
        ERROR_LAUNCH_FAILED = 719

        ''' <summary>
        ''' This error indicates that the number of blocks launched per grid for a kernel that was
        ''' launched via either :cuLaunchCooperativeKernel or :cuLaunchCooperativeKernelMultiDevice
        ''' exceeds the maximum number of blocks as allowed by :cuOccupancyMaxActiveBlocksPerMultiprocessor
        ''' or :cuOccupancyMaxActiveBlocksPerMultiprocessorWithFlags times the number Of multiprocessors
        ''' as specified by the device attribute :CU_DEVICE_ATTRIBUTE_MULTIPROCESSOR_COUNT.
        ''' </summary>
        ERROR_COOPERATIVE_LAUNCH_TOO_LARGE = 720

        ''' <summary>
        ''' This error indicates that the attempted operation is not permitted.
        ''' </summary>
        ERROR_NOT_PERMITTED = 800

        ''' <summary>
        ''' This error indicates that the attempted operation is not supported on the current system or device.
        ''' </summary>
        ERROR_NOT_SUPPORTED = 801

        ''' <summary>
        ''' This error indicates that the system is not yet ready to start any CUDA work. 
        ''' To continue using CUDA verify the system configuration is in a valid state and all required driver daemons are actively running.
        ''' More information about this error can be found in the system specific user guide.
        ''' </summary>
        ERROR_SYSTEM_NOT_READY = 802

        ''' <summary>
        ''' This error indicates that there is a mismatch between the versions of the display driver and the CUDA driver. 
        ''' Refer to the compatibility documentation for supported versions.
        ''' </summary>
        ERROR_SYSTEM_DRIVER_MISMATCH = 803

        ''' <summary>
        ''' This error indicates that the system was upgraded to run with forward compatibility
        ''' but the visible hardware detected by CUDA does not support this configuration.
        ''' Refer to the compatibility documentation for the supported hardware matrix or ensure
        ''' that only supported hardware is visible during initialization via the VISIBLE_DEVICES
        ''' environment variable.
        ''' </summary>
        ERROR_COMPAT_NOT_SUPPORTED_ON_DEVICE = 804

        ''' <summary>
        ''' This error indicates that the MPS client failed to connect to the MPS control daemon or the MPS server.
        ''' </summary>
        ERROR_MPS_CONNECTION_FAILED = 805

        ''' <summary>
        ''' This error indicates that the remote procedural call between the MPS server and the MPS client failed.
        ''' </summary>
        ERROR_MPS_RPC_FAILURE = 806

        ''' <summary>
        ''' This error indicates that the MPS server is not ready to accept new MPS client requests.
        ''' This error can be returned when the MPS server is in the process of recovering from a fatal failure.
        ''' </summary>
        ERROR_MPS_SERVER_NOT_READY = 807

        ''' <summary>
        ''' This error indicates that the hardware resources required to create MPS client have been exhausted.
        ''' </summary>
        ERROR_MPS_MAX_CLIENTS_REACHED = 808

        ''' <summary>
        ''' This error indicates the the hardware resources required to support device connections have been exhausted.
        ''' </summary>
        ERROR_MPS_MAX_CONNECTIONS_REACHED = 809

        ''' <summary>
        ''' This error indicates that the operation is not permitted when the stream is capturing.
        ''' </summary>
        ERROR_STREAM_CAPTURE_UNSUPPORTED = 900

        ''' <summary>
        ''' This error indicates that the current capture sequence on the stream has been invalidated due to a previous error.
        ''' </summary>
        ERROR_STREAM_CAPTURE_INVALIDATED = 901

        ''' <summary>
        ''' This error indicates that the operation would have resulted in a merge of two independent capture sequences.
        ''' </summary>
        ERROR_STREAM_CAPTURE_MERGE = 902

        ''' <summary>
        ''' This error indicates that the capture was not initiated in this stream.
        ''' </summary>
        ERROR_STREAM_CAPTURE_UNMATCHED = 903

        ''' <summary>
        ''' This error indicates that the capture sequence contains a fork that was not joined to the primary stream.
        ''' </summary>
        ERROR_STREAM_CAPTURE_UNJOINED = 904

        ''' <summary>
        ''' This error indicates that a dependency would have been created which crosses the capture sequence boundary. 
        ''' Only implicit in-stream ordering dependencies are allowed to cross the boundary.
        ''' </summary>
        ERROR_STREAM_CAPTURE_ISOLATION = 905

        ''' <summary>
        ''' This error indicates a disallowed implicit dependency on a current capture sequence from cudaStreamLegacy.
        ''' </summary>
        ERROR_STREAM_CAPTURE_IMPLICIT = 906

        ''' <summary>
        ''' This error indicates that the operation is not permitted on an event which was last recorded in a capturing stream.
        ''' </summary>
        ERROR_CAPTURED_EVENT = 907

        ''' <summary>
        ''' A stream capture sequence not initiated with the :CU_STREAM_CAPTURE_MODE_RELAXED
        ''' argument to :cuStreamBeginCapture was passed To :cuStreamEndCapture in a different thread.
        ''' </summary>
        ERROR_STREAM_CAPTURE_WRONG_THREAD = 908

        ''' <summary>
        ''' This error indicates that the timeout specified for the wait operation has lapsed.
        ''' </summary>
        ERROR_TIMEOUT = 909

        ''' <summary>
        ''' This error indicates that the graph update was not performed because it included 
        ''' changes which violated constraints specific to instantiated graph update.
        ''' </summary>
        ERROR_GRAPH_EXEC_UPDATE_FAILURE = 910

        ''' <summary>
        ''' This indicates that an async error has occurred in a device outside of CUDA.
        ''' If CUDA was waiting for an external device's signal before consuming shared data
        ''' the external device signaled an error indicating that the data is not valid for consumption. 
        ''' This leaves the process in an inconsistent state and any further CUDA work will return the same error. 
        ''' To continue using CUDA the process must be terminated and relaunched.
        ''' </summary>
        ERROR_EXTERNAL_DEVICE = 911

        ''' <summary>
        ''' This indicates that an unknown internal error has occurred.
        ''' </summary>
        ERROR_UNKNOWN = 999
    End Enum

    ''' <summary>
    ''' Режимы вычислений.
    ''' </summary>
    ''' <remarks>
    ''' Все члены перечисления имели префикс "CU_COMPUTEMODE_".
    ''' </remarks>
    Public Enum CuComputeMode As Integer
        ''' <summary>
        ''' Default compute mode (Multiple contexts allowed per device).
        ''' </summary>
        [DEFAULT] = 0
        ''' <summary>
        ''' Exclusive mode.
        ''' </summary>
        EXCLUSIVE = 1
        ''' <summary>
        ''' Compute-prohibited mode (no contexts can be created on this device at this time).
        ''' </summary>
        PROHIBITED = 2
        ''' <summary>
        ''' Compute-exclusive-process mode (only one context used by a single process can be present on this device at a time).
        ''' </summary>
        EXCLUSIVE_PROCESS = 3
    End Enum

    ''' <summary>
    ''' Атрибуты устройства GPU.
    ''' </summary>
    ''' <remarks>
    ''' Все члены перечисления имели префикс "CU_DEVICE_ATTRIBUTE_".
    ''' </remarks>
    Public Enum CuDeviceAttribute As Integer
        ''' <summary>
        ''' Maximum number of threads per block.
        ''' </summary>
        MAX_THREADS_PER_BLOCK = 1
        ''' <summary>
        ''' Maximum block dimension X.
        ''' </summary>
        MAX_BLOCK_DIM_X = 2
        ''' <summary>
        ''' Maximum block dimension Y.
        ''' </summary>
        MAX_BLOCK_DIM_Y = 3
        ''' <summary>
        ''' Maximum block dimension Z.
        ''' </summary>
        MAX_BLOCK_DIM_Z = 4
        ''' <summary>
        ''' Maximum grid dimension X.
        ''' </summary>
        MAX_GRID_DIM_X = 5
        ''' <summary>
        ''' Maximum grid dimension Y.
        ''' </summary>
        MAX_GRID_DIM_Y = 6
        ''' <summary>
        ''' Maximum grid dimension Z.
        ''' </summary>
        MAX_GRID_DIM_Z = 7
        ''' <summary>
        ''' Maximum shared memory available per block in bytes
        ''' </summary>
        MAX_SHARED_MEMORY_PER_BLOCK = 8
        <Obsolete("Deprecated, use CU_DEVICE_ATTRIBUTE_MAX_SHARED_MEMORY_PER_BLOCK")>
        SHARED_MEMORY_PER_BLOCK = 8
        ''' <summary>
        ''' Memory available on device for <code>__constant__</code> variables in a CUDA C kernel in bytes.
        ''' </summary>
        TOTAL_CONSTANT_MEMORY = 9
        ''' <summary>
        ''' Warp size in threads.
        ''' </summary>
        WARP_SIZE = 10
        ''' <summary>
        ''' Maximum pitch in bytes allowed by memory copies.
        ''' </summary>
        MAX_PITCH = 11
        ''' <summary>
        ''' Maximum number of 32-bit registers available per block.
        ''' </summary>
        MAX_REGISTERS_PER_BLOCK = 12
        <Obsolete("Use CU_DEVICE_ATTRIBUTE_MAX_REGISTERS_PER_BLOCK")>
        REGISTERS_PER_BLOCK = 12
        ''' <summary>
        ''' Typical clock frequency in kilohertz.
        ''' </summary>
        CLOCK_RATE = 13
        ''' <summary>
        ''' Alignment requirement for textures.
        ''' </summary>
        TEXTURE_ALIGNMENT = 14
        ''' <summary>
        ''' Device can possibly copy memory and execute a kernel concurrently. 
        ''' </summary>
        <Obsolete("Use instead CU_DEVICE_ATTRIBUTE_ASYNC_ENGINE_COUNT.")>
        GPU_OVERLAP = 15
        ''' <summary>
        ''' Number of multiprocessors on device.
        ''' </summary>
        MULTIPROCESSOR_COUNT = 16
        ''' <summary>
        ''' Specifies whether there is a run time limit on kernels.
        ''' </summary>
        KERNEL_EXEC_TIMEOUT = 17
        ''' <summary>
        ''' Device is integrated with host memory.
        ''' </summary>
        INTEGRATED = 18
        ''' <summary>
        ''' Device can map host memory into CUDA address space.
        ''' </summary>
        CAN_MAP_HOST_MEMORY = 19
        ''' <summary>
        ''' Compute mode (See :CUcomputemode for details).
        ''' </summary>
        COMPUTE_MODE = 20
        ''' <summary>
        ''' Maximum 1D texture width.
        ''' </summary>
        MAXIMUM_TEXTURE1D_WIDTH = 21
        ''' <summary>
        ''' Maximum 2D texture width.
        ''' </summary>
        MAXIMUM_TEXTURE2D_WIDTH = 22
        ''' <summary>
        ''' Maximum 2D texture height.
        ''' </summary>
        MAXIMUM_TEXTURE2D_HEIGHT = 23
        ''' <summary>
        ''' Maximum 3D texture width.
        ''' </summary>
        MAXIMUM_TEXTURE3D_WIDTH = 24
        ''' <summary>
        ''' Maximum 3D texture height.
        ''' </summary>
        MAXIMUM_TEXTURE3D_HEIGHT = 25
        ''' <summary>
        ''' Maximum 3D texture depth.
        ''' </summary>
        MAXIMUM_TEXTURE3D_DEPTH = 26
        ''' <summary>
        ''' Maximum 2D layered texture width.
        ''' </summary>
        MAXIMUM_TEXTURE2D_LAYERED_WIDTH = 27
        ''' <summary>
        ''' Maximum 2D layered texture height.
        ''' </summary>
        MAXIMUM_TEXTURE2D_LAYERED_HEIGHT = 28
        ''' <summary>
        ''' Maximum layers in a 2D layered texture.
        ''' </summary>
        MAXIMUM_TEXTURE2D_LAYERED_LAYERS = 29
        <Obsolete("Deprecated, use CU_DEVICE_ATTRIBUTE_MAXIMUM_TEXTURE2D_LAYERED_WIDTH")>
        MAXIMUM_TEXTURE2D_ARRAY_WIDTH = 27
        <Obsolete("Deprecated, use CU_DEVICE_ATTRIBUTE_MAXIMUM_TEXTURE2D_LAYERED_HEIGHT")>
        MAXIMUM_TEXTURE2D_ARRAY_HEIGHT = 28
        <Obsolete("Deprecated, use CU_DEVICE_ATTRIBUTE_MAXIMUM_TEXTURE2D_LAYERED_LAYERS")>
        MAXIMUM_TEXTURE2D_ARRAY_NUMSLICES = 29
        ''' <summary>
        ''' Alignment requirement for surfaces.
        ''' </summary>
        SURFACE_ALIGNMENT = 30
        ''' <summary>
        ''' Device can possibly execute multiple kernels concurrently.
        ''' </summary>
        CONCURRENT_KERNELS = 31
        ''' <summary>
        ''' Device has ECC support enabled.
        ''' </summary>
        ECC_ENABLED = 32
        ''' <summary>
        ''' PCI bus ID of the device.
        ''' </summary>
        PCI_BUS_ID = 33
        ''' <summary>
        ''' PCI device ID of the device.
        ''' </summary>
        PCI_DEVICE_ID = 34
        ''' <summary>
        ''' Device is using TCC driver model.
        ''' </summary>
        TCC_DRIVER = 35
        ''' <summary>
        ''' Peak memory clock frequency in kilohertz.
        ''' </summary>
        MEMORY_CLOCK_RATE = 36
        ''' <summary>
        ''' Global memory bus width in bits.
        ''' </summary>
        GLOBAL_MEMORY_BUS_WIDTH = 37
        ''' <summary>
        ''' Size of L2 cache in bytes.
        ''' </summary>
        L2_CACHE_SIZE = 38
        ''' <summary>
        ''' Maximum resident threads per multiprocessor.
        ''' </summary>
        MAX_THREADS_PER_MULTIPROCESSOR = 39
        ''' <summary>
        ''' Number of asynchronous engines.
        ''' </summary>
        ASYNC_ENGINE_COUNT = 40
        ''' <summary>
        ''' Device shares a unified address space with the host.
        ''' </summary>
        UNIFIED_ADDRESSING = 41
        ''' <summary>
        ''' Maximum 1D layered texture width.
        ''' </summary>
        MAXIMUM_TEXTURE1D_LAYERED_WIDTH = 42
        ''' <summary>
        ''' Maximum layers in a 1D layered texture.
        ''' </summary>
        MAXIMUM_TEXTURE1D_LAYERED_LAYERS = 43
        <Obsolete("Deprecated, do not use.")>
        CAN_TEX2D_GATHER = 44
        ''' <summary>
        ''' Maximum 2D texture width if CUDA_ARRAY3D_TEXTURE_GATHER is set.
        ''' </summary>
        MAXIMUM_TEXTURE2D_GATHER_WIDTH = 45
        ''' <summary>
        ''' Maximum 2D texture height if CUDA_ARRAY3D_TEXTURE_GATHER is set.
        ''' </summary>
        MAXIMUM_TEXTURE2D_GATHER_HEIGHT = 46
        ''' <summary>
        ''' Alternate maximum 3D texture width.
        ''' </summary>
        MAXIMUM_TEXTURE3D_WIDTH_ALTERNATE = 47
        ''' <summary>
        ''' Alternate maximum 3D texture height.
        ''' </summary>
        MAXIMUM_TEXTURE3D_HEIGHT_ALTERNATE = 48
        ''' <summary>
        ''' Alternate maximum 3D texture depth.
        ''' </summary>
        MAXIMUM_TEXTURE3D_DEPTH_ALTERNATE = 49
        ''' <summary>
        ''' PCI domain ID of the device.
        ''' </summary>
        PCI_DOMAIN_ID = 50
        ''' <summary>
        ''' Pitch alignment requirement for textures.
        ''' </summary>
        TEXTURE_PITCH_ALIGNMENT = 51
        ''' <summary>
        ''' Maximum cubemap texture width/height.
        ''' </summary>
        MAXIMUM_TEXTURECUBEMAP_WIDTH = 52
        ''' <summary>
        ''' Maximum cubemap layered texture width/height.
        ''' </summary>
        MAXIMUM_TEXTURECUBEMAP_LAYERED_WIDTH = 53
        ''' <summary>
        ''' Maximum layers in a cubemap layered texture.
        ''' </summary>
        MAXIMUM_TEXTURECUBEMAP_LAYERED_LAYERS = 54
        ''' <summary>
        ''' Maximum 1D surface width.
        ''' </summary>
        MAXIMUM_SURFACE1D_WIDTH = 55
        ''' <summary>
        ''' Maximum 2D surface width.
        ''' </summary>
        MAXIMUM_SURFACE2D_WIDTH = 56
        ''' <summary>
        ''' Maximum 2D surface height.
        ''' </summary>
        MAXIMUM_SURFACE2D_HEIGHT = 57
        ''' <summary>
        ''' Maximum 3D surface width.
        ''' </summary>
        MAXIMUM_SURFACE3D_WIDTH = 58
        ''' <summary>
        ''' Maximum 3D surface height.
        ''' </summary>
        MAXIMUM_SURFACE3D_HEIGHT = 59
        ''' <summary>
        ''' Maximum 3D surface depth.
        ''' </summary>
        MAXIMUM_SURFACE3D_DEPTH = 60
        ''' <summary>
        ''' Maximum 1D layered surface width.
        ''' </summary>
        MAXIMUM_SURFACE1D_LAYERED_WIDTH = 61
        ''' <summary>
        ''' Maximum layers in a 1D layered surface.
        ''' </summary>
        MAXIMUM_SURFACE1D_LAYERED_LAYERS = 62
        ''' <summary>
        ''' Maximum 2D layered surface width.
        ''' </summary>
        MAXIMUM_SURFACE2D_LAYERED_WIDTH = 63
        ''' <summary>
        ''' Maximum 2D layered surface height.
        ''' </summary>
        MAXIMUM_SURFACE2D_LAYERED_HEIGHT = 64
        ''' <summary>
        ''' Maximum layers in a 2D layered surface.
        ''' </summary>
        MAXIMUM_SURFACE2D_LAYERED_LAYERS = 65
        ''' <summary>
        ''' Maximum cubemap surface width.
        ''' </summary>
        MAXIMUM_SURFACECUBEMAP_WIDTH = 66
        ''' <summary>
        ''' Maximum cubemap layered surface width.
        ''' </summary>
        MAXIMUM_SURFACECUBEMAP_LAYERED_WIDTH = 67
        ''' <summary>
        ''' Maximum layers in a cubemap layered surface.
        ''' </summary>
        MAXIMUM_SURFACECUBEMAP_LAYERED_LAYERS = 68
        <Obsolete("Do not use. Use cudaDeviceGetTexture1DLinearMaxWidth() or cuDeviceGetTexture1DLinearMaxWidth() instead.")>
        MAXIMUM_TEXTURE1D_LINEAR_WIDTH = 69
        ''' <summary>
        ''' Maximum 2D linear texture width.
        ''' </summary>
        MAXIMUM_TEXTURE2D_LINEAR_WIDTH = 70
        ''' <summary>
        ''' Maximum 2D linear texture height.
        ''' </summary>
        MAXIMUM_TEXTURE2D_LINEAR_HEIGHT = 71
        ''' <summary>
        ''' Maximum 2D linear texture pitch in bytes.
        ''' </summary>
        MAXIMUM_TEXTURE2D_LINEAR_PITCH = 72
        ''' <summary>
        ''' Maximum mipmapped 2D texture width.
        ''' </summary>
        MAXIMUM_TEXTURE2D_MIPMAPPED_WIDTH = 73
        ''' <summary>
        ''' Maximum mipmapped 2D texture height.
        ''' </summary>
        MAXIMUM_TEXTURE2D_MIPMAPPED_HEIGHT = 74
        ''' <summary>
        ''' Major compute capability version number.
        ''' </summary>
        COMPUTE_CAPABILITY_MAJOR = 75
        ''' <summary>
        ''' Minor compute capability version number.
        ''' </summary>
        COMPUTE_CAPABILITY_MINOR = 76
        ''' <summary>
        ''' Maximum mipmapped 1D texture width.
        ''' </summary>
        MAXIMUM_TEXTURE1D_MIPMAPPED_WIDTH = 77
        ''' <summary>
        ''' Device supports stream priorities.
        ''' </summary>
        STREAM_PRIORITIES_SUPPORTED = 78
        ''' <summary>
        ''' Device supports caching globals in L1.
        ''' </summary>
        GLOBAL_L1_CACHE_SUPPORTED = 79
        ''' <summary>
        ''' Device supports caching locals in L1.
        ''' </summary>
        LOCAL_L1_CACHE_SUPPORTED = 80
        ''' <summary>
        ''' Maximum shared memory available per multiprocessor in bytes.
        ''' </summary>
        MAX_SHARED_MEMORY_PER_MULTIPROCESSOR = 81
        ''' <summary>
        ''' Maximum number of 32-bit registers available per multiprocessor.
        ''' </summary>
        MAX_REGISTERS_PER_MULTIPROCESSOR = 82
        ''' <summary>
        ''' Device can allocate managed memory on this system.
        ''' </summary>
        MANAGED_MEMORY = 83
        ''' <summary>
        ''' Device is on a multi-GPU board.
        ''' </summary>
        MULTI_GPU_BOARD = 84
        ''' <summary>
        ''' Unique id for a group of devices on the same multi-GPU board.
        ''' </summary>
        MULTI_GPU_BOARD_GROUP_ID = 85
        ''' <summary>
        ''' Link between the device and the host supports native atomic operations (this is a placeholder attribute, and is not supported on any current hardware).
        ''' </summary>
        HOST_NATIVE_ATOMIC_SUPPORTED = 86
        ''' <summary>
        ''' Ratio of single precision performance (in floating-point operations per second) to double precision performance.
        ''' </summary>
        SINGLE_TO_DOUBLE_PRECISION_PERF_RATIO = 87
        ''' <summary>
        ''' Device supports coherently accessing pageable memory without calling cudaHostRegister on it.
        ''' </summary>
        PAGEABLE_MEMORY_ACCESS = 88
        ''' <summary>
        ''' Device can coherently access managed memory concurrently with the CPU.
        ''' </summary>
        CONCURRENT_MANAGED_ACCESS = 89
        ''' <summary>
        ''' Device supports compute preemption.
        ''' </summary>
        COMPUTE_PREEMPTION_SUPPORTED = 90
        ''' <summary>
        ''' Device can access host registered memory at the same virtual address as the CPU.
        ''' </summary>
        CAN_USE_HOST_POINTER_FOR_REGISTERED_MEM = 91
        ''' <summary>
        ''' :cuStreamBatchMemOp and related APIs are supported.
        ''' </summary>
        CAN_USE_STREAM_MEM_OPS = 92
        ''' <summary>
        ''' 64-bit operations are supported in :cuStreamBatchMemOp and related APIs.
        ''' </summary>
        CAN_USE_64_BIT_STREAM_MEM_OPS = 93
        ''' <summary>
        ''' :CU_STREAM_WAIT_VALUE_NOR is supported.
        ''' </summary>
        CAN_USE_STREAM_WAIT_VALUE_NOR = 94
        ''' <summary>
        ''' Device supports launching cooperative kernels via :cuLaunchCooperativeKernel.
        ''' </summary>
        COOPERATIVE_LAUNCH = 95
        <Obsolete("Deprecated, :cuLaunchCooperativeKernelMultiDevice is deprecated.")>
        COOPERATIVE_MULTI_DEVICE_LAUNCH = 96
        ''' <summary>
        ''' Maximum optin shared memory per block.
        ''' </summary>
        MAX_SHARED_MEMORY_PER_BLOCK_OPTIN = 97
        ''' <summary>
        ''' The :CU_STREAM_WAIT_VALUE_FLUSH flag and the :CU_STREAM_MEM_OP_FLUSH_REMOTE_WRITES MemOp are supported on the device. 
        ''' See \ref CUDA_MEMOP for additional details.
        ''' </summary>
        CAN_FLUSH_REMOTE_WRITES = 98
        ''' <summary>
        ''' Device supports host memory registration via :cudaHostRegister.
        ''' </summary>
        HOST_REGISTER_SUPPORTED = 99
        ''' <summary>
        ''' Device accesses pageable memory via the host's page tables.
        ''' </summary>
        PAGEABLE_MEMORY_ACCESS_USES_HOST_PAGE_TABLES = 100
        ''' <summary>
        ''' The host can directly access managed memory on the device without migration.
        ''' </summary>
        DIRECT_MANAGED_MEM_ACCESS_FROM_HOST = 101
        <Obsolete("Deprecated, use CU_DEVICE_ATTRIBUTE_VIRTUAL_MEMORY_MANAGEMENT_SUPPORTED.")>
        VIRTUAL_ADDRESS_MANAGEMENT_SUPPORTED = 102
        ''' <summary>
        ''' Device supports virtual memory management APIs like :cuMemAddressReserve, :cuMemCreate, :cuMemMap and related APIs.
        ''' </summary>
        VIRTUAL_MEMORY_MANAGEMENT_SUPPORTED = 102
        ''' <summary>
        ''' Device supports exporting memory to a posix file descriptor with :cuMemExportToShareableHandle, if requested via :cuMemCreate.
        ''' </summary>
        HANDLE_TYPE_POSIX_FILE_DESCRIPTOR_SUPPORTED = 103
        ''' <summary>
        ''' Device supports exporting memory to a Win32 NT handle with :cuMemExportToShareableHandle, if requested via :cuMemCreate.
        ''' </summary>
        HANDLE_TYPE_WIN32_HANDLE_SUPPORTED = 104
        ''' <summary>
        ''' Device supports exporting memory to a Win32 KMT handle with :cuMemExportToShareableHandle, if requested via :cuMemCreate.
        ''' </summary>
        HANDLE_TYPE_WIN32_KMT_HANDLE_SUPPORTED = 105
        ''' <summary>
        ''' Maximum number of blocks per multiprocessor.
        ''' </summary>
        MAX_BLOCKS_PER_MULTIPROCESSOR = 106
        ''' <summary>
        ''' Device supports compression of memory.
        ''' </summary>
        GENERIC_COMPRESSION_SUPPORTED = 107
        ''' <summary>
        ''' Maximum L2 persisting lines capacity setting in bytes.
        ''' </summary>
        MAX_PERSISTING_L2_CACHE_SIZE = 108
        ''' <summary>
        ''' Maximum value of CUaccessPolicyWindow:num_bytes.
        ''' </summary>
        MAX_ACCESS_POLICY_WINDOW_SIZE = 109
        ''' <summary>
        ''' Device supports specifying the GPUDirect RDMA flag with :cuMemCreate.
        ''' </summary>
        GPU_DIRECT_RDMA_WITH_CUDA_VMM_SUPPORTED = 110
        ''' <summary>
        ''' Shared memory reserved by CUDA driver per block in bytes.
        ''' </summary>
        RESERVED_SHARED_MEMORY_PER_BLOCK = 111
        ''' <summary>
        ''' Device supports sparse CUDA arrays and sparse CUDA mipmapped arrays.
        ''' </summary>
        SPARSE_CUDA_ARRAY_SUPPORTED = 112
        ''' <summary>
        ''' Device supports using the :cuMemHostRegister flag :CU_MEMHOSTERGISTER_READ_ONLY to register memory that must be mapped as read-only to the GPU.
        ''' </summary>
        READ_ONLY_HOST_REGISTER_SUPPORTED = 113
        ''' <summary>
        ''' External timeline semaphore interop is supported on the device.
        ''' </summary>
        TIMELINE_SEMAPHORE_INTEROP_SUPPORTED = 114
        ''' <summary>
        ''' Device supports using the :cuMemAllocAsync and :cuMemPool family of APIs.
        ''' </summary>
        MEMORY_POOLS_SUPPORTED = 115
        ''' <summary>
        ''' Device supports GPUDirect RDMA APIs, like nvidia_p2p_get_pages (see https://docs.nvidia.com/cuda/gpudirect-rdma for more information).
        ''' </summary>
        GPU_DIRECT_RDMA_SUPPORTED = 116
        ''' <summary>
        ''' The returned attribute shall be interpreted as a bitmask, where the individual bits are described by the :CUflushGPUDirectRDMAWritesOptions enum.
        ''' </summary>
        GPU_DIRECT_RDMA_FLUSH_WRITES_OPTIONS = 117
        ''' <summary>
        ''' GPUDirect RDMA writes to the device do not need to be flushed For consumers within the scope indicated by the returned attribute. 
        ''' See :CUGPUDirectRDMAWritesOrdering for the numerical values returned here.
        ''' </summary>
        GPU_DIRECT_RDMA_WRITES_ORDERING = 118
        ''' <summary>
        ''' Handle types supported with mempool based IPC.
        ''' </summary>
        MEMPOOL_SUPPORTED_HANDLE_TYPES = 119
        ''' <summary>
        ''' 
        ''' </summary>
        MAX
    End Enum

    ''' <summary>
    ''' Коды результатов выполнения вызовов NVRTC.
    ''' </summary>
    ''' <remarks>
    ''' Все члены перечисления имели префикс "NVRTC_".
    ''' </remarks>
    Public Enum NvrtcResult As Integer
        SUCCESS = 0
        ERROR_OUT_OF_MEMORY = 1
        ERROR_PROGRAM_CREATION_FAILURE = 2
        ERROR_INVALID_INPUT = 3
        ERROR_INVALID_PROGRAM = 4
        ERROR_INVALID_OPTION = 5
        ERROR_COMPILATION = 6
        ERROR_BUILTIN_OPERATION_FAILURE = 7
        ERROR_NO_NAME_EXPRESSIONS_AFTER_COMPILATION = 8
        ERROR_NO_LOWERED_NAMES_BEFORE_COMPILATION = 9
        ERROR_NAME_EXPRESSION_NOT_VALID = 10
        ERROR_INTERNAL_ERROR = 11
    End Enum

End Namespace