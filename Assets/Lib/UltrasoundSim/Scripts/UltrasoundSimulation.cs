using mKit;
using System;
using UnityEngine;

/// <summary>
/// This script calculates the ultrasound simulation. It is necessary to attach it to a gameobject.
/// In the inspector, the "ultrasoundComputeShader.compute" must be added to this script.
/// It is necessary to attach a RawImage to this script, where the texture can be displayed on.
/// It is possible to activate and to deactivate all parameters and to set the coefficients like 
/// frequency and gain with 'UltrasoundSimulation.Instance.xxx = yyy;'.
/// To use this script for ultrasound simulation, the mKit is needed. Write in the function where you    
/// activate ultrasound the line 'volume.UltrasoundRenderer = UltrasoundSimulation.Instance;' and following 
/// line in a Awake-Method: 'Config.UltrasoundGenerateInRenderTexture = true;'
/// 
/// Written by Fenja Bruns, licensed under the MIT License
/// 
/// MS ChangeLog 2020:
/// - Interpolated texture sampling 
/// - Curved rendering
/// MS ChangeLog 2022:
/// - multi-volume sampling
/// </summary>

public class UltrasoundSimulation : MonoBehaviour, IUltrasoundRenderer
{
    public static UltrasoundSimulation Instance;

    public Texture voxel;
    public Texture labels;

    /// <summary>Gain coefficient. Default: 300.</summary>
    [Range(1.0f, 10000.0f), Header("Ultrasound settings")]
    public float gainCoef = 300;

    /// <summary>Frequency in MHz. Default: 10.</summary>
    [Range(2.0f, 15.0f)]
    public float frequency = 10;

    /// <summary>If true, ultrasound gel will be simulated. Default: true.</summary>
    public bool SimulateGel = true;

    /// <summary>If true, input data is treated as normalized CT data, and acoustic impedance will be calculated. 
    /// If false, the input data will be directly used as normalized impedance value. 
    /// Default: true.</summary>
    [Header("Simulation options")]
    public bool Impedance = true;

    /// <summary>If true, reflection will be calculated. Default: true.</summary>
    public bool Reflection = true;

    /// <summary>If true, transmission will be calculated. Default: true.</summary>
    public bool Transmission = true;

    /// <summary>If true, absorption will be calculated. Default: true.</summary>
    public bool Absorption = true;

    /// <summary>If true, noise on output image will be simulated.</summary>
    public bool NoiseImage;

    /// <summary>Output image noise, scattering range (normalized [0..1]) Default: 0.05f</summary>
    public float NoiseImageRange = 0.05f;

    /// <summary>If true, noise on impedance input will be simulated. Default: true.</summary>
    //internal bool NoiseImpedance;

    /// <summary>
    /// If true, the alpha channel of input data is sampled. Otherwise the red channel.
    /// </summary>
    public bool bSampleAlpha;

    /// <summary>
    /// Normalized impedance range
    /// </summary>
    [Range(5,10)]
    public float maxImpedance = 7.8f; // 7.8f = bone

    /// CT intensity scale factor (use e.g. 0.79f for frozen Visible Human)
    [Range(0.5f,2.02f)]
    public float pBase = 1;

    /// <summary>
    /// Maximum index of impedance array. This is 15 for ARGB4444, or 255 for 8-bit input.
    /// </summary>
    public int maxIndexImpedanceArray = 15;

    [Header("Limits")]
    public float tgcFactor = 0;
    [Range(0, 1)]
    public float intensityMin = 0f; // 0.1
    [Range(0, 1)]
    public float transmissionMin = 0f; // 0.1

    [Range(0, 1)]
    public float absorptionMax = 1; // 0.95

    [Header("Sector parameters")]
    [Range(20, 80)]
    public float sectorAngleDeg = 40;
    public float sectorVerticalOffset = 0;
    public float sectorHorizontalStretch = 1;

    [Range(0f, 1f)]
    public float sectorVerticalStretch = 1;

    [Header("Indexed Impedance ")]
    public int impedanceMode; // 0:CT, 1:CT+label-Index; 2:indexed
    public float[] ImpedanceArray;

    // shader and RawImage, which must be attached
    [Header("GPU simulation")]
    internal ComputeShader shader;

    // volume
    private Volume volume;
    private Vm2 vm2;
    private readonly ToolgroupState ts;

    // variables for calculating on CPU and to sed to GPU
    private Vector4[] directionX, directionY, directionZ, slicePosition;
    private Vector3 worldSize;
    private Vector3 dataSize;
    private Vector3 dataSizeTexture;
    private Vector3 voxelPerMillimeter;

    //Vector3 scale;

    private Vector4[] originS;
    private float[] stepW;
    private Vector4 voxelpmm;

    // index of the different kernels
    private int kernelImpedance8x8;
    private int kernelUltrasoundLinear;
    private int kernelUltrasoundCurved;

    // used RenderTextures
    private RenderTexture resultTexture;

    private int shadowThreshold; // necessary for interface

    private ComputeBuffer bufferBooleanOptions;
    private int[] booleanOptions;
    private ComputeBuffer bufferIndexedImpedance;
    private readonly RenderTextureFormat floatTextureFormat = RenderTextureFormat.RFloat;
    private readonly int maxVolumes = 8;
    private int volumeCount;

    private int h, w;

    RenderTexture impedanceTexture;
    RenderTexture usTexture;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        //shader = MKitManager.Instance.GetComputeShader("ultrasoundComputeShader");
        shader = Resources.Load("ultrasoundComputeShader") as ComputeShader;

        // US boolean options
        booleanOptions = new int[5];
        bufferBooleanOptions = new ComputeBuffer(booleanOptions.Length, sizeof(int));

        // Indexed Impedance
        if (ImpedanceArray.Length == 0)
        {
            ImpedanceArray = new float[16];
        }

        bufferIndexedImpedance = new ComputeBuffer(ImpedanceArray.Length, sizeof(float));

        originS = new Vector4[maxVolumes];
        directionX = new Vector4[maxVolumes];
        directionY = new Vector4[maxVolumes];
        directionZ = new Vector4[maxVolumes];
        slicePosition = new Vector4[maxVolumes];
    }

    public void Init(Volume volume)
    {
        if (volume == null || volume.Vm2 == null)
        {
            return;
        }

        MultiVolumeSampling = false;

        // default values for the parameter
        Impedance = true;
        Reflection = true;
        Transmission = true;
        SimulateGel = true;
        Absorption = true;

        //GainCoef = 300;
        //Freq = 10;

        // enable rendering in RT
        volume.UltrasoundUseRendertexture = true;

        // volume
        this.volume = volume;
        vm2 = this.volume.Vm2;
        //ts = this.volume.CurrentToolgroupState;

        voxel = volume.VolumeTexture;

        bSampleAlpha = vm2.Header.FormatNumber == Vm2.FormatEnum.ARTIFICIAL_RGBA4444;
        shader.SetBool("bSampleAlpha", bSampleAlpha);

        maxIndexImpedanceArray = vm2.Header.FormatNumber == Vm2.FormatEnum.ARTIFICIAL_RGBA4444 ? 15 : 255;
        shader.SetInt("indexMax", maxIndexImpedanceArray);

        if (maxIndexImpedanceArray +1 != ImpedanceArray.Length)
        {
            Debug.LogWarning("ImpedanceArray size error (" + ImpedanceArray.Length + "), expected size: " + (maxIndexImpedanceArray + 1));
        }

        // Extents
        worldSize = vm2.Header.SizeInMeters; // Volume extent in world size
        dataSize = vm2.Header.VoxelDataDimensions; // data extent in voxel    
        voxelPerMillimeter = new Vector3(dataSize.x / worldSize.x, dataSize.y / worldSize.y, dataSize.z / worldSize.z);

        // GPU texture size usually differs from actual size
        dataSizeTexture = dataSize;

        if (mKit.Config.CreateTexturesPOT)
        {
            Debug.Log("Turn off pot textures");
            dataSizeTexture.x = Mathf.NextPowerOfTwo((int)dataSizeTexture.x);
            dataSizeTexture.y = Mathf.NextPowerOfTwo((int)dataSizeTexture.y);
            dataSizeTexture.z = Mathf.NextPowerOfTwo((int)dataSizeTexture.z);
        }

        // data dimensions vor trinlinear sampling
        shader.SetVector("voxelDim", dataSizeTexture);

        // variables for sending data to GPU
        stepW = new float[2];
        voxelpmm = new Vector4();

        voxelpmm = voxelPerMillimeter;
        //scale = this.volume.ScaleNonUniform;

        // ids of all kernels of the compute shader
        kernelImpedance8x8 = shader.FindKernel("csImpedanceImage");

        kernelUltrasoundLinear = shader.FindKernel("csUltrasoundLinear");
        kernelUltrasoundCurved = shader.FindKernel("csUltrasoundCurved");
    }

    public Volume CurrentVolume { get => volume; }

    public void Generate (RenderTexture output, float sliceWidth, float sliceHeight, Vector3 slicePos , Vector3 sliceNormal, Vector3 sliceRight, Vector3 sliceUp)
    {
        SetStepWidth(sliceWidth, sliceHeight);

        directionZ[0] = sliceNormal;
        directionX[0] = sliceRight;
        directionY[0] = sliceUp;
        slicePosition[0] = slicePos + 0.5f * volume.VolumeSize;

        GenerateUltrasoundImage(output);
    }

    /// <summary>
    /// mKit interface API to generate Ultrasound image.
    /// </summary>
    /// <param name="sliceIndex"></param>
    public void Generate(int sliceIndex)
    {
        var sliceSize = volume.CurrentToolgroupState.sliceSize[sliceIndex];
        SetStepWidth(sliceSize.x, sliceSize.y);

        // slice start position relative to volumes
        if (!MultiVolumeSampling)
        {
            var ts = volume.CurrentToolgroupState;
            directionZ[0] = ts.sliceNormal1[sliceIndex]; // slice normal
            directionX[0] = ts.sliceNormal2[sliceIndex];
            directionY[0] = ts.sliceNormal3[sliceIndex]; // add * -1 for Mkit Version 20190515 (fixed in 2019813)
            slicePosition[0] = (Vector3)ts.slicePosition1[sliceIndex] + 0.5f * volume.VolumeSize;
        }
        else
        {
            int volumeCount = 0;

            for (int i = 0; i < maxVolumes; i++)
            {
                if (volumeCount < Volume.Volumes.Count)
                {
                    var vol = Volume.Volumes[volumeCount];
                    if (vol != null && vol.VolumeState == VolumeStateEnum.READY)
                    {
                        var ts = vol.CurrentToolgroupState;
                        directionZ[volumeCount] = ts.sliceNormal1[sliceIndex]; // slice normal
                        directionX[volumeCount] = ts.sliceNormal2[sliceIndex];
                        directionY[volumeCount] = ts.sliceNormal3[sliceIndex]; // add * -1 for Mkit Version 20190515 (fixed in 2019813)
                        slicePosition[volumeCount] = (Vector3)ts.slicePosition1[sliceIndex] + 0.5f * volume.VolumeSize;

                        volumeCount++;
                    }
                }
            }

        }

        resultTexture = this.volume.GetUltrasoundRendertexture(0);
        GenerateUltrasoundImage(resultTexture);
    }

    void SetStepWidth(float sliceWidthMeters, float sliceHeightMeters)
    {
        // calculate resolution
        float lamda = 1.54f / frequency; // speed of sound / frequency
        float axialRes = lamda * 2;
        float lateralRes = lamda * 4;
        axialRes = axialRes / 1000; // mm in m
        lateralRes = lateralRes / 1000;

        float stepWidthY = axialRes;
        float stepWidthX = lateralRes;

        // width and height for temporary texture, in relation to resolution
        h = Mathf.RoundToInt(sliceHeightMeters / stepWidthY); // shader disptach with 8x8 or 16x16, so pixel must be something with 8 or 16, depends on highEnd-bool
        w = Mathf.RoundToInt(sliceWidthMeters / stepWidthX);

        h += (h % 4) > 0 ? 4 : 0;
        w += (w % 4) > 0 ? 4 : 0;

        stepW[0] = sliceWidthMeters / w;
        stepW[1] = sliceHeightMeters / h;

        shader.SetFloats("stepW", stepW);
    }


    public void GenerateUltrasoundImage(RenderTexture output)
    {
        //Debug.Log("US frame #" + Time.frameCount);

        if (volume.VolumeTexture == null)
            return;

        bool sector = volume.UltrasoundScannerType != UltrasoundScannerTypeEnum.LINEAR;

        // Extents
        worldSize = vm2.Header.SizeInMeters; // Volume extent in world size
        dataSize = vm2.Header.VoxelDataDimensions; // data extent in voxel    
        voxelPerMillimeter = new Vector3(dataSize.x / worldSize.x, dataSize.y / worldSize.y, dataSize.z / worldSize.z);
        voxelpmm = voxelPerMillimeter;

        //// slice size in world units 
        //float sliceWidthMeters = volume.CurrentToolgroupState.sliceSize[sliceIndex].x;
        //float sliceHeightMeters = volume.CurrentToolgroupState.sliceSize[sliceIndex].y;

        //// calculate resolution
        //float lamda = 1.54f / frequency; // speed of sound / frequency
        //float axialRes = lamda * 2;
        //float lateralRes = lamda * 4;
        //axialRes = axialRes / 1000; // mm in m
        //lateralRes = lateralRes / 1000;

        //float stepWidthY = axialRes;
        //float stepWidthX = lateralRes;

        //// width and height for temporary texture, in relation to resolution
        //int h = Mathf.RoundToInt(sliceHeightMeters / stepWidthY); // shader disptach with 8x8 or 16x16, so pixel must be something with 8 or 16, depends on highEnd-bool
        //int w = Mathf.RoundToInt(sliceWidthMeters / stepWidthX);

        //h += (h % 4) > 0 ? 4 : 0;
        //w += (w % 4) > 0 ? 4 : 0;

        //stepW[0] = sliceWidthMeters / w;
        //stepW[1] = sliceHeightMeters / h;

        volumeCount = 0;

        if (impedanceMode == 1)
        {
            if (labels == null)
                return;
            else
                shader.SetTexture(kernelImpedance8x8, "labelVolume", labels);
        }

        // default textures
        for (int i = 1; i < maxVolumes; i++)
        {
            shader.SetTexture(kernelImpedance8x8, "Volume" + i, volume.VolumeTexture);
        }

        // slice start position relative to volumes
        if (!MultiVolumeSampling)
        {
            SetupVolumeInShader(volumeCount, volume);
            volumeCount++;
        }
        else
        {
            for (int i = 0; i < maxVolumes; i++)
            {
                if (volumeCount < Volume.Volumes.Count)
                {
                    var vol = Volume.Volumes[volumeCount];
                    if (vol != null && vol.VolumeState == VolumeStateEnum.READY)
                    {
                        SetupVolumeInShader(volumeCount, vol);

                        volumeCount++;
                    }
                }
            }
        }

        // set array of relative slice start positions 
        shader.SetVectorArray("originS", originS);
        shader.SetVectorArray("directionX", directionX);
        shader.SetVectorArray("directionY", directionY);
        shader.SetVectorArray("directionZ", directionZ);

        // sector rendering
        if (sector)
        {
            sectorVerticalOffset = w * (1.0f / Mathf.Cos(sectorAngleDeg));
        }

        // temporary texture for the ct-data and the impedance
        if (impedanceTexture == null || impedanceTexture.width != w || impedanceTexture.height != h)
        {
            if (impedanceTexture != null) RenderTexture.ReleaseTemporary(impedanceTexture);

            impedanceTexture = RenderTexture.GetTemporary(w, h, 0, floatTextureFormat, RenderTextureReadWrite.Linear);
            impedanceTexture.enableRandomWrite = true;
            if (!impedanceTexture.Create())
            {
                Debug.LogError("RT.Create failed");
            }
        }

        if (usTexture == null || usTexture.width != w || usTexture.height != h)
        {
            if (usTexture != null) RenderTexture.ReleaseTemporary(usTexture);

            usTexture = RenderTexture.GetTemporary(w, h, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
            usTexture.enableRandomWrite = true;
            if (!usTexture.Create())
            {
                Debug.LogError("RT.Create failed");
            }
        }          

        if (sector)
        {
            // clear RT
            RenderTexture rt = RenderTexture.active;
            RenderTexture.active = usTexture;
            GL.Clear(true, true, Color.black);
            RenderTexture.active = rt;
        }

        // volume count to sample
        //Debug.Log(volumeCount);
        shader.SetInt("volumeCount", volumeCount);

        // CT specific intensity scaling factor
        shader.SetFloat("pBase", pBase);

        // start kernel for getting ct-data and calculating the impedance
        shader.SetTexture(kernelImpedance8x8, "Impedance2D", impedanceTexture);
        shader.SetTexture(kernelImpedance8x8, "labelVolume", labels != null ? labels : voxel);
        //shader.SetTexture(kernelImpedance, "Result", textureResult);
        //shader.SetTexture(kernelImpedance8x8, "Voxel", volume.VolumeTexture);
        //shader.SetVector("directionX", directionX);
        //shader.SetVector("directionY", directionY);
        //shader.SetVector("directionZ", directionZ);
        //shader.SetVectorArray ("originS", originS);
        //shader.SetFloats("stepW", stepW);
        shader.SetVector("voxelpmm", voxelpmm);
        shader.SetBool("bImp", Impedance);

        shader.SetInt("impedanceMode", impedanceMode);
        shader.SetBool("bNoiseImage", NoiseImage);
        shader.SetFloat("NoiseImageRange", NoiseImageRange);

        // limits
        shader.SetFloat("absorptionMax", absorptionMax); // 0.95
        shader.SetFloat("tgcFactor", tgcFactor); // 10
        shader.SetFloat("intensityMin", intensityMin); // 0.1
        shader.SetFloat("transmissionMin", transmissionMin); // 0.1
        //shader.SetFloat("reflectionMax", reflectionMax);

        // indexed impedance    
        shader.SetFloat("maxImpedance", maxImpedance);
        bufferIndexedImpedance.SetData(ImpedanceArray);
        shader.SetBuffer(kernelImpedance8x8, "impedanceArray", bufferIndexedImpedance);

        shader.GetKernelThreadGroupSizes(kernelImpedance8x8, out uint tgs_x, out uint tgs_y, out uint tgs_z);
        shader.Dispatch(kernelImpedance8x8, impedanceTexture.width / (int)tgs_x, impedanceTexture.height / (int)tgs_y, 1);

        // booleans, which parameters should be calculated
        //int[] booleanOptions = new int[5];
        booleanOptions[0] = Convert.ToInt32(Reflection);
        booleanOptions[1] = Convert.ToInt32(Transmission);
        booleanOptions[2] = Convert.ToInt32(Absorption);
        booleanOptions[3] = Convert.ToInt32(SimulateGel);

        // computebuffer for the booleans
        //ComputeBuffer buffer = new ComputeBuffer(booleans.Length, sizeof(int)); // moved to Awake()
        bufferBooleanOptions.SetData(booleanOptions);

        shader.SetFloat("random", UnityEngine.Random.Range(78f, 79f));
        shader.SetFloat("gainCoef", gainCoef);
        shader.SetFloat("freq", frequency);

        shader.SetFloat("ResultWidth", usTexture.width);
        shader.SetFloat("ResultHeight", usTexture.height);

        // curved params
        float[] curvedParams = new float[4];
        curvedParams[0] = sectorAngleDeg;
        curvedParams[1] = sectorVerticalOffset;
        curvedParams[2] = sectorHorizontalStretch;
        curvedParams[3] = sectorVerticalStretch;
        shader.SetFloats("curvedParams", curvedParams);

        // linear or sector?
        shader.SetInt("scannerType", (int)volume.UltrasoundScannerType); // unused
        int kernelUltrasound = sector ? kernelUltrasoundCurved : kernelUltrasoundLinear;

        //kernelUltrasound = kernelUltrasoundLinear; // testing

        // start kernel for calculating ultrasound
        shader.SetBuffer(kernelUltrasound, "parameter", bufferBooleanOptions);

        shader.SetTexture(kernelUltrasound, "Impedance2D", impedanceTexture);
        shader.SetTexture(kernelUltrasound, "Result", usTexture);

        shader.GetKernelThreadGroupSizes(kernelUltrasound, out tgs_x, out tgs_y, out tgs_z);
        shader.Dispatch(kernelUltrasound, impedanceTexture.width / (int)tgs_x, 1, 1);

        Graphics.Blit(usTexture, output); // , blitScale, Vector2.zero); // copy texture1 to textureResult
    }

    private void SetupVolumeInShader(int volumeIndex, Volume vol)
    {
        // set volume texture #n
        shader.SetTexture(kernelImpedance8x8, "Volume" + volumeIndex, vol.VolumeTexture);
        originS[volumeIndex] = slicePosition[volumeIndex] - 0.5f * (stepW[0] * w - stepW[0] * 0.5f) * directionX[volumeIndex] - 0.5f * (stepW[1] * h - stepW[1] * 0.5f) * directionY[volumeIndex];
    }

    private void OnApplicationQuit()
    {
        // release resources

        if (bufferBooleanOptions != null)
            bufferBooleanOptions.Release();

        if (bufferIndexedImpedance != null)
            bufferIndexedImpedance.Release();

        if (usTexture != null)
            RenderTexture.ReleaseTemporary(usTexture);

        if (impedanceTexture != null)
            RenderTexture.ReleaseTemporary(impedanceTexture);
    }

    public bool MultiVolumeSampling { get; set; }

    // necessary for interface
    public bool IsReady
    {
        get
        {
            return true;
        }
    }

    public int ShadowThreshold
    {
        get
        {
            return shadowThreshold;
        }

        set
        {
            shadowThreshold = value;
        }
    }

    /// <summary>
    /// Set threshold default for volume type.
    /// </summary>
    /// <param name="volumeType"></param>
    /// <returns></returns>
    public byte SetDefaultThreshold(VolumeTypeEnum volumeType)
    {
        // throw new System.NotImplementedException();
        return 0;
    }

    /// <summary>
    /// Set parameters configuring the shape of the sector
    /// </summary>
    public void SetSectorParameters(float sectorAngleDeg, float sectorVerticalOffset, float sectorHorizontalStretch, float sectorVerticalStretch)
    {
        this.sectorAngleDeg = sectorAngleDeg;
        this.sectorVerticalOffset = sectorVerticalOffset;
        this.sectorHorizontalStretch = sectorHorizontalStretch;
        this.sectorVerticalStretch = sectorVerticalStretch;
    }

    public float Impedance0
    {
        get { return ImpedanceArray[0]; }
        set { ImpedanceArray[0] = value; }
    }

    public float Impedance6
    {
        get
        {
            return ImpedanceArray[6];
        }

        set
        {
            ImpedanceArray[6] = value;
        }
    }

    public float Impedance9
    {
        get
        {
            return ImpedanceArray[9];
        }

        set
        {
            ImpedanceArray[9] = value;
        }
    }

    public float Impedance12
    {
        get
        {
            return ImpedanceArray[12];
        }

        set
        {
            ImpedanceArray[12] = value;
        }
    }

    public float Impedance1
    {
        get { return ImpedanceArray[1]; }
        set { ImpedanceArray[1] = value; }
    }

    public float Impedance7
    {
        get { return ImpedanceArray[7]; }
        set { ImpedanceArray[7] = value; }
    }

    public float Impedance10
    {
        get { return ImpedanceArray[10]; }
        set { ImpedanceArray[10] = value; }
    }

    public float Impedance13
    {
        get { return ImpedanceArray[13]; }
        set { ImpedanceArray[13] = value; }
    }

    public float Gain { get => gainCoef; set => gainCoef = value; }
    public float Frequency { get => frequency; set => frequency = value; }
}
