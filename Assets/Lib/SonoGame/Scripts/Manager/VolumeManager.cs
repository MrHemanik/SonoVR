using mKit;
using mKit.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace SonoGame
{
    /// <summary>
    /// VolumeManager
    /// </summary>
    public class VolumeManager : MonoBehaviour, VolumeStateObserver
    {
        public static VolumeManager Instance;

        //[Header ("Runtime settings")]
        internal Vector3 workspaceSize;      

        internal IVolumeViewManager volumeViewManager;
        internal MaterialConfig materialConfig;

        private bool hideModelActive = false;
        private int shaderColorProperty; // _Color or _BaseColor

        private int visibleVolumeIndex = 0;

        private GameObject hideModel;

        private void Awake()
        {
            Instance = this;

            SetMkitConfig(); // set before InitMkit();
            shaderColorProperty = Shader.PropertyToID("_BaseColor"); // URP / SRP: "_Color"
        }

        private void Start()
        {
            /// see also : <see cref="GameManager.ResetAll"/>
            workspaceSize = AppConfig.current.artificialVolumeSize; // TODO: check wording
        }

        /// <summary>
        /// Sets some settings in the mKit-config BEFORE everything is loading
        /// </summary>
        private void SetMkitConfig()
        {
            // Events nicht unsubscriben, wenn Szene neugeladen wird
            mKit.Config.VolumeWorldSpaceDefaultRotation = Quaternion.identity;
            mKit.Config.UnsubscribeEventsOnSceneUnload = false;
            mKit.Config.DefaultWindowLevel = (0.26f);
            mKit.Config.DefaultWindowWidth = (0.11f);
            mKit.Config.DrawSliceNormal = false;

            mKit.Config.ESSLevels = 0; // enable for non-Tuebingen content

            mKit.Config.ToolCameraBelowSlice = false;
            mKit.Config.DefaultMprOutOfDataMode = mKit.MprOutOfDataMode.COLOR;
            mKit.Config.DefaultOutOfDataColor1 = Color.black;
            mKit.Config.DefaultOutOfDataColor2 = Color.black;

            //  mKit.Config.DebugMkitFlags = mKit.DebugMkitEnum.EVENTS;
            mKit.Config.LegacyUltrasoundCamera = true;

            mKit.Config.DefaultUltrasoundScannerType = mKit.UltrasoundScannerTypeEnum.CURVED;

            mKit.Config.DefaultUse4DTimeLerp = true;

            mKit.Config.DefaultSamplerateFactor = 0.6f;

            Config.AntialiasingSlice = 4;
            mKit.Config.DefaultRenderTextureWidth = 400;
            mKit.Config.sliceRenderTextureDepth = 1;

            //mKit.Config.UltrasoundAsyncRendering = true; // not for GPU US

            Config.AntialiasingSlice = 2;

            Config.WritableTexture3D = true;
            Config.ShapeGpuGenerationCopyToHost = false;
            Config.ShapeGpuGenerationBatchesPerFrame = 20;

            Config.TransparentVolumeDataCaching = false; // workaround for mKit bug, when loading of 1. volume , 2. GPU artificial, 3. volume again 

            Config.CreateTexturesPOT = false;

            //Debug.Log("mkit version " + Config.mKitVersion + " " + Config.CreateTexturesPOT);
        }



        /// <summary>
        /// Set volume visibility by adding the volume layer to the main camera's culling mask.
        /// </summary>
        /// <param name="volumeIndex"></param>
        /// <param name="clearFirst">if true, other volume layers will be disabled (default:true)</param>
        public void SetVisibleVolume(int volumeIndex, bool clearFirst = true)
        {
            if (clearFirst)
            {
                SetNoVolumeVisible();
            }

            visibleVolumeIndex = volumeIndex;

            if (Volume.Volumes[volumeIndex]!=null && Volume.Volumes[volumeIndex].VolumeProxy!=null)
            {
                volumeViewManager?.SetVisibleVolume(Volume.Volumes[volumeIndex].VolumeProxy.gameObject.layer);
            }                
        }

        /// <summary>
        /// Remove all volume layers from the main camera's culling mask.
        /// </summary>
        public void SetNoVolumeVisible()
        {
            volumeViewManager?.HideVolumes();
        }

        /// <summary>
        /// Capture texture from slice camera.
        /// To allow for multiple captures intra-frame, <see cref="Config.RenderPerCamera"/> is set to true before captrue, and to its previous state afterwards.
        /// </summary>
        /// <param name="volumeIndex"></param>
        /// <returns>texture. </returns>
        public Texture2D GetSliceCamCapture(Volume volume)
        {
            // save state
            Camera cam = volume.GetToolgroupCamera();
            bool renderPerCamera = volume.RenderPerCamera;
            bool camEnabled = cam.enabled;

            float saveGain = UltrasoundSimulation.Instance.Gain;

            //UltrasoundSimulation.Instance.Gain = 25000;

            // capture
            cam.enabled = false;
            volume.RenderPerCamera = true;
            Texture2D result = cam.GetComponent<CameraImage>().CaptureImage();


            // restore previous state
            UltrasoundSimulation.Instance.Gain = saveGain;
            volume.RenderPerCamera = renderPerCamera;
            cam.enabled = camEnabled;


            // name
            result.name = "CaptureImage volume #" + volume.ID;

            return result;
        }

        /// <summary>
        /// Resets the Toolgroup-Slice to default-values
        /// </summary>
        public void SliceReset()
        {
            Transform t = Volume.Volumes[0].GetToolTransform();

            if (t != null)
            {
                t.position = Vector3.zero;
                t.rotation = Quaternion.Euler(90f, 0f, 0f);
                t.localScale = Vector3.one;
            }
        }

        /// <summary>
        /// Sets the Volume-rotation to 90 Quaternion
        /// </summary>
        public void ResetVolumeRotation()
        {
            Transform t = Volume.Volumes[0].GetVolumeTransform();

            if (t != null)
            {
                t.rotation = Quaternion.Euler(90f, 0f, 0f);
            }
        }

        /// <summary>
        /// mKit Callback
        /// </summary>
        /// <param name="sender">das Ctrl von dem das Event kam</param>
        /// <param name="ea">Args</param>
        public void OnVolumeStateChanged(Volume sender, VolumeStateEventArgs ea)
        {
            //Debug.LogWarning("OnVolumeStateChanged #"+sender.ID+": "+ea.VolumeState);

            if (ea.VolumeState == VolumeStateEnum.EMPTY)
            {
                // cleanup any object models
                CleanupObjectModels(sender);
            }

            if (ea.VolumeState == VolumeStateEnum.READY)
            {
                sender.VolumeTextureFilterMode = FilterMode.Point;

                sender.VolumeTransform.gameObject.name = "mKitVolume #" + sender.ID + " (" + System.IO.Path.GetFileName(sender.Vm2.Filename) + ")";
                sender.ToolTransform.gameObject.name = "mKitToolgroup #" + sender.ID + " (" + System.IO.Path.GetFileName(sender.Vm2.Filename) + ")";

                // Toolgroup als Sektor
                sender.UltrasoundScannerType = UltrasoundScannerTypeEnum.CURVED;
                sender.UseSliceMasking = true;
                sender.SliceMaskingTexture = AppConfig.assets.ultrasoundMaskCurved;

                //SetUltrasoundScannerType(UltrasoundScannerTypeEnum.CURVED, sender);

                sender.UltrasoundRenderer = UltrasoundSimulation.Instance;

                sender.ToolTransform.GetChild(0).localRotation = Quaternion.identity;
                sender.ToolgroupIsLocal = false;

                sender.SetSliceEdgeColor(new Color(0, 0, 0, 0));
                
                VolumeManager.Instance.volumeViewManager?.OnVolumeReady(sender.VolumeProxy.gameObject.layer);
            }
        }

        /// <summary>
        /// Remove any child objects of the volume node
        /// </summary>
        /// <param name="volume"></param>
        public void CleanupObjectModels(Volume volume)
        {
            if (volume.VolumeTransform != null)
            {
                if (volume.VolumeTransform.childCount > 0)
                {
                    for (int i = 0; i < volume.VolumeTransform.childCount; i++)
                    {
                        Destroy(volume.VolumeTransform.GetChild(i).gameObject);
                    }
                }
            }
        }

        /// <summary>
        /// Add shape models below volume node and disable volume renderer. 
        /// </summary>
        /// <param name="volume"></param>
        /// <param name="shapeConfigList"></param>
        public List<Transform> AddObjectModels(Volume volume, ShapeConfigList shapeConfigList)
        {
            //List<ShapeConfig> list = shapeConfigList.shapeConfigList.ConvertAll(x => (ShapeConfig)x);
            return AddObjectModels(volume, shapeConfigList.shapeConfigList);
        }

        /// <summary>
        /// Add shape model below volume node and disable volume renderer. 
        /// Note: shapes with with Color.clear are ignored. 
        /// </summary>
        public List<Transform> AddObjectModels(Volume volume, ShapeConfig shapeConfig) =>
            AddObjectModels(volume, new List<ShapeConfig>() { shapeConfig });

        /// <summary>
        /// Add shape models below volume node and disable volume renderer. 
        /// Note: shapes with with Color.clear are ignored. 
        /// </summary>
        public List<Transform> AddObjectModels(Volume volume, List<ShapeConfig> shapes)
        {
            List<Transform> objectModels = new List<Transform>();

            if (shapes != null)
            {
                Vector4 dataDim = volume.Vm2.Header.VoxelDataDimensions;

                for (int i = 0; i < shapes.Count; i++)
                {
                    var shape = shapes[i];

                    // ignore transparent objects
                    if (shape.color.Equals(Color.clear))
                    {
                        continue;
                    }

                    var objectModel = AddObjectModel(volume, shape, dataDim);

                    if (objectModel != null)
                    {
                        objectModels.Add(objectModel);
                    }
                    else
                    {
                        Debug.LogWarning("AddObjectModels: shape is null at #" + i);
                    }
                }
            }

            return objectModels;
        }

        /// <summary>
        /// Add shape model below volume node and disable volume renderer. 
        /// Note: shapes with with "Color.clear" are ignored. 
        /// </summary>
        /// <param name="volume"></param>
        /// <param name="shape">shape definition</param>
        public Transform AddObjectModel(Volume volume, ShapeConfig shape, Vector4 dataDim)
        {
            int layer = volume.VolumeTransform.gameObject.layer;

            if (!(shape is ShapeConfigVoxel))
            {
                dataDim = new Vector4(1, 1, 1, dataDim.w); // override for normalized shapeconfig
            }

            Vector3 halfDataDim = shape.centeredCoordinates ? Vector3.zero : ((Vector3)dataDim) * 0.5f;

            Vector3 localScale = new Vector3(shape.size.x / dataDim.x, shape.size.y / dataDim.y, shape.size.z / dataDim.z);
            Vector3 localPosition = new Vector3((shape.shapeCenter.x - halfDataDim.x) / dataDim.x,
                                                    (shape.shapeCenter.y - halfDataDim.y) / dataDim.y,
                                                    (shape.shapeCenter.z - halfDataDim.z) / dataDim.z);

            Transform shapeGO = null;
            int matIndex = 0;

            Quaternion shapeRotation = shape.shapeRotation;

            float edgeWidthNorm = -1;

            switch (shape.type)
            {
                case ShapeType.CUBOID:
                    shapeGO = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
                    matIndex = 0;
                    break;
                case ShapeType.ELIPSOID:
                    shapeGO = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
                    matIndex = 1;
                    break;
                case ShapeType.TUBE_X:
                    localScale = new Vector3(localScale.y, localScale.x, localScale.z);
                    edgeWidthNorm = shape.edgeWidth / (0.5f * shape.size.y);
                    shapeRotation = shapeRotation * Quaternion.Euler(0, 0, 90);
                    goto case ShapeType.TUBE_Y;
                case ShapeType.TUBE_Z:
                    localScale = new Vector3(localScale.x, localScale.z, localScale.y);
                    edgeWidthNorm = shape.edgeWidth / (0.5f * shape.size.x);
                    shapeRotation = shapeRotation * Quaternion.Euler(90, 0, 0);
                    goto case ShapeType.TUBE_Y;
                case ShapeType.TUBE_Y: // tube prefab is TUBE_Y
                    if (edgeWidthNorm == -1)
                    {
                        edgeWidthNorm = shape.edgeWidth / (0.5f * shape.size.x);
                    }

                    shapeGO = GameObject.Instantiate(AppConfig.assets.tubePrefab).transform;
                    shapeGO.GetComponent<TubeScaler>().SetEdgeWidthNorm(edgeWidthNorm);
                    shapeGO.GetComponent<TubeScaler>().UpdateMesh();
                    matIndex = 2;
                    break;
                default:
                    Debug.LogError("Not implemented: " + shape.type);
                    break;
            }

            if (shapeGO != null)
            {
                // replicate volume object positions
                shapeGO.parent = volume.VolumeTransform;
                shapeGO.rotation = shapeRotation;
                shapeGO.localPosition = localPosition;

                shapeGO.localScale = localScale;
                shapeGO.gameObject.layer = layer;

                Renderer rend = shapeGO.GetComponent<Renderer>();
                rend.material = AppConfig.assets.artificialMat[1];
                rend.material = AppConfig.assets.artificialMat[matIndex]; // material

                rend.material.SetColor(shaderColorProperty, shape.color);
            }

            return shapeGO;
        }

        /// <summary>
        /// Add outline to volume. 
        /// For config, <see cref="AppConfig.cubeOutlineConfig"/>.
        /// </summary>
        /// <param name="volume"></param>
        public void AddVolumeOrientationHelp(Volume volume)
        {
            AddOrientationHelp(volume.VolumeTransform.gameObject);
        }

        /// <summary>
        /// Add orientation help for volume
        /// </summary>
        /// <param name="go"></param>
        private void AddOrientationHelp(GameObject go)
        {
            if (go != null)
            {
                CubeOutline outline = go.GetComponent<CubeOutline>();
                if (outline == null)
                {
                    outline = go.AddComponent<CubeOutline>();
                }

                outline.config = AppConfig.current.cubeOutlineConfig;
            }
        }

        /// <summary>
        /// Enable/disable volume background outline 
        /// </summary>
        /// <param name="volume"></param>
        /// <param name="state"></param>
        public void SetVolumeOutlineState(Volume volume, bool state)
        {
            CubeOutline outline = volume.VolumeTransform.gameObject.GetComponent<CubeOutline>();

            if (outline != null)
            {
                outline.enabled = state;
            }
            else
            {
                Debug.LogWarning("SetVolumeOutlineState: CubeOutline not present on volume #" + volume.ID);
            }
        }

        /// <summary>
        /// Register the DataNotify-Function to the Dataconverter
        /// </summary>
        public void SetupObserver()
        {
            Volume.OnVolumeStateChanged += OnVolumeStateChanged;
        }

        /// <summary>
        /// Temporarily toggle an otherwise active hide model.
        /// </summary>
        /// <param name="state">False: always reveals volume. True: hide volume if <see cref="hideModelActive"/> is true</param>
        public void ToggleHideGeometry(bool state)
        {
            bool actualHiding = hideModelActive && state;

            if (hideModel != null)
            {
                hideModel.SetActive(actualHiding);

                if (actualHiding)
                {
                    SetNoVolumeVisible();
                }
                else
                {
                    SetVisibleVolume(visibleVolumeIndex);
                }
            }
        }

        /// <summary>
        /// Set the general active state of the hide model.
        /// NOTE: Always hides the volume rendering.
        /// To temporarily show the volume rendering, use <see cref="ToggleHideGeometry(bool)"/>.
        /// </summary>
        /// <param name="activate"></param>
        public void ActivateHideGeometry(bool activate)
        {
            hideModelActive = activate;

            if (hideModel != null)
            {
                hideModel.SetActive(hideModelActive);
            }

            if (activate)
            {
                SetNoVolumeVisible();
            }
            else
            {
                SetVisibleVolume(visibleVolumeIndex);
            }
        }

        /// <summary>
        /// The model used for <see cref="ActivateHideGeometry(bool)"/>
        /// </summary>
        public GameObject HideModel
        {
            get
            {
                return hideModel;
            }

            set
            {
                hideModel = value;
            }
        }

        /// <summary>
        /// Hide true volume rotation
        /// </summary>
        /// <param name="volumeRotation"></param>
        public bool HideGeometryHideRotation
        {
            get => hideModel.GetComponent<MirrorVolume>().HideRotation;
            set => hideModel.GetComponent<MirrorVolume>().HideRotation = value;
        }


        /// <summary>
        /// Load a VM2 file from disk.
        /// </summary>
        /// <param name="filename">Vm2 filename</param>
        /// <param name="volumeSlot">the number of the volume in which the shape should be loaded</param>
        /// <param name="additional">is the shape in addition to the other, or should replace it another volume</param>
        internal IEnumerator LoadVolume(string filename, int volumeSlot, bool additional = false)
        {
            Volume volume = null;

            // Daten laden
            if (additional)
            {
                if (Volume.Volumes.Count == volumeSlot)
                {
                    volume = Volume.AddVolume(filename);
                }
                else
                {
                    volume = Volume.Volumes[volumeSlot];
                    volume.LoadFile(filename);
                }
            }
            else
            {
                volume = Volume.Volumes[volumeSlot];
                volume.LoadFile(filename);
            }

            // warten bis es fertig ist
            while (volume.VolumeState != VolumeStateEnum.READY)
            {
                yield return new WaitForEndOfFrame();
            }

            // Volume scale (temp)
            if (Volume.Volumes[0].VolumePhysicalSize.x > 150)
            {
                Volume.Volumes[0].Scale = 0.75f;
            }
        }

        /// <summary>
        /// Create geometry hiding the examined volume.  
        /// </summary>
        /// <param name="localScale"></param>
        /// <param name="orientationHelp">add orientation lines via <see cref="AddOrientationHelp"/></param>
        public void CreateHideGeometry(Vector3 localScale, HideGeometry hideGeometry = HideGeometry.Sphere, bool orientationHelp = true)
        {
            Destroy(hideModel);

            switch (hideGeometry)
            {
                case HideGeometry.None:
                    hideModel = new GameObject();
                    hideModel.name = "HideGeomtry.None";
                    break;
                case HideGeometry.Sphere:
                    hideModel = Instantiate(AppConfig.assets.hideSpherePrefab);
                    break;
                case HideGeometry.Cube:
                    hideModel = Instantiate(AppConfig.assets.hideCubePrefab);
                    break;
            }

            hideModel.transform.localScale = localScale;
            hideModel.transform.position = Vector3.zero;

            if (orientationHelp)
            {
                AddOrientationHelp(hideModel);
            }
        }

        /// <summary>
        /// Set a <see cref="MaterialConfig"/> for use with <see cref="GenerateArtificialVolume(List{ShapeConfig}, int, bool)"/>
        /// </summary>
        /// <param name="materialConfig"></param>
        public void SetMaterialConfig(MaterialConfig materialConfig)
        {
            if (materialConfig == null)
            {
                Debug.LogError("VolumeManager.SetMaterialConfig: materialConfig is null");
            }

            this.materialConfig = materialConfig;
        }

        public void UseMaterialConfigVisualization(Volume volume, EVisualization sliceVisualization, EVisualization volumeVisualization = EVisualization.Colored)
        {
            if (!materialConfig)
            {
                Debug.LogError("Use VolumeManager.SetMaterialConfig before calling UseMaterialConfigVisualization");
                return;
            }

            UseMaterialConfigVisualization(materialConfig, volume, sliceVisualization, volumeVisualization);
        }

        /// <summary>
        /// Apply settings from a <see cref="MaterialConfig"/> to a specific volume's slice.
        /// </summary>
        /// <param name="volume">volume instance</param>
        /// <param name="sliceVisualization">visualization type</param>
        public static void UseMaterialConfigVisualization(MaterialConfig materialConfig, Volume volume, EVisualization sliceVisualization, EVisualization volumeVisualization = EVisualization.Colored)
        {
            // slice
            switch (sliceVisualization)
            {
                case EVisualization.Colored:
                case EVisualization.Gray:
                    Texture2D sliceTF = CreateVolumeRenderingTransferFunction(materialConfig, sliceVisualization);
                    volume.UseUltrasound = false;
                    volume.UseTransferFunctionSlice = true;
                    volume.TransferFunctionTextureSlice = sliceTF;
                    break;
                case EVisualization.Ultrasound:
                    var usc = AppConfig.current.ultrasoundConfiguration;
                    usc.Apply(materialConfig);
                    volume.UseUltrasound = true;
                    break;
            }

            // volume
            Texture2D volumeTF = CreateVolumeRenderingTransferFunction(materialConfig, volumeVisualization);
            volume.UseTransferFunctionVolume = true;
            volume.TransferFunctionTextureVolume = volumeTF;

            // Out-of-data volocr: set color #0 also as out-of-data-color
            var mapping = materialConfig.map[0];
            var backgroundColor = Color.clear;
            var outOfDataMode = MprOutOfDataMode.CLIP;

            if (sliceVisualization != EVisualization.Ultrasound)
            {
                outOfDataMode = MprOutOfDataMode.COLOR;
                backgroundColor = sliceVisualization == EVisualization.Colored ? mapping.color : mapping.grayscale;
                backgroundColor.a = 1;
            }

            volume.OutOfDataColor1 = backgroundColor;
            volume.OutOfDataVisualMode = outOfDataMode;

            VolumeManager.Instance.volumeViewManager?.RenderVolumeView();
        }

        /// <summary>
        /// For volume rendering, create a transfer function mapping of voxels to color or grayscale.
        /// </summary>
        /// <param name="matConfig">material config</param>
        /// <param name="visualization">visualization type</param>
        /// <returns></returns>
        public static Texture2D CreateVolumeRenderingTransferFunction(MaterialConfig matConfig, EVisualization visualization)
        {
            if (visualization != EVisualization.Colored && visualization != EVisualization.Gray)
            {
                Debug.LogError("CreateTransferFunction applies only to color and grayscale modes");
                return null;
            }

            int mapItems = 256;

            var tfTex = new Texture2D(mapItems, 1);
            tfTex.filterMode = FilterMode.Point;
            tfTex.wrapMode = TextureWrapMode.Clamp;

            var texel32 = new Color32[mapItems];

            // reset to default Color(0,0,0,0)
            for (int i = 0; i < tfTex.width; i++)
            {
                texel32[i] = Color.clear;
            }

            // create TF
            matConfig.CreateTransferFunction(visualization, texel32);

            tfTex.SetPixels32(texel32);
            tfTex.Apply();

            return tfTex;
        }


        /// <summary>
        /// Edit an artifical volume previously generated by <see cref="GenerateArtificialVolume(List{ShapeConfig}, int, bool)"/>.
        /// </summary>
        /// <param name="shapeConfigList"></param>
        /// <returns>coroutine IEnumerator</returns>
        public IEnumerator EditArtificialVolume(ShapeConfig shapeConfig, int volumeSlot = 0)
            => EditArtificialVolume(new List<ShapeConfig>() { shapeConfig }, volumeSlot);

        /// <summary>
        /// Generate a random shape (artificial volume) on the GPU, using the default sizes for real (10 cm^3) and voxel data (200^3) size.
        /// </summary>
        /// <param name="path">the path of saved shapes</param>
        /// <param name="volumeSlot">the number of the volume in which the shape should be loaded</param>
        /// <param name="additional">is the volume in addition to the other, or should replace it another volume</param>
        /// <returns>coroutine IEnumerator</returns>
        public IEnumerator GenerateArtificialVolume(ShapeConfig shapeConfig, int volumeSlot, bool additional = false, bool addObjectModels = false)
            => GenerateArtificialVolume(new List<ShapeConfig>() { shapeConfig }, volumeSlot, additional, addObjectModels);


        /// <summary>
        /// Edit an artifical volume previously generated by <see cref="GenerateArtificialVolume(List{ShapeConfig}, int, bool)"/>.
        /// </summary>
        /// <param name="shapeConfigList"></param>
        /// <returns>coroutine IEnumerator</returns>
        public IEnumerator EditArtificialVolume(List<ShapeConfig> shapeConfigList, int volumeSlot = 0, bool addObjectModels = false)
        {
            if (materialConfig == null)
            {
                Debug.LogError("GenerateArtificialVolume: Use VolumeManager.SetMaterialConfig before GenerateArtificialVolume");
                yield break;
            }

            Volume volume = Volume.Volumes[volumeSlot];
            bool rendererState = volume.VolumeTransform.GetComponent<Renderer>().enabled;

            var renderList = materialConfig.GetRenderShapeConfigList(shapeConfigList);

            yield return volume.EditGpuAddArtificialShapes_Coroutine(renderList);

            // add object models
            if (addObjectModels)
            {
                AddObjectModels(volume, shapeConfigList);
            }

            volume.VolumeTransform.GetComponent<Renderer>().enabled = rendererState;
        }


        /// <summary>
        /// Generate a random shape (artificial volume) on the GPU, using the default sizes for real (10 cm^3) and voxel data (200^3) size.
        /// </summary>
        /// <param name="volumeSlot">the number of the volume in which the shape should be loaded</param>
        /// <param name="additional">is the volume in addition to the other, or should replace it another volume</param>
        /// <returns>coroutine IEnumerator</returns>
        public IEnumerator GenerateArtificialVolume(List<ShapeConfig> shapeConfigList, int volumeSlot, bool additional = false, bool addObjectModels = false)
        {
            Vector3Int volumeDataSize = AppConfig.current.artificialVolumeDataSize;
            Vector3 volumeRealSize = AppConfig.current.artificialVolumeSize;

            yield return GenerateArtificialVolumeWithSize(shapeConfigList, volumeSlot, volumeDataSize, volumeRealSize, additional, addObjectModels);
        }

        /// <summary>
        /// Generate a list of random shapes as "artificial" volume on the GPU, using custom sizes for real and voxel data size.
        /// </summary>
        /// <param name="volumeSlot">the number of the volume in which the shape should be loaded</param>
        /// <param name="dataSize">data size of the volume (voxels)</param>
        /// <param name="realSize">real physical size of the volume (meters)</param>
        /// <param name="additional">is the volume in addition to the other, or should replace it another volume</param>
        /// <returns>coroutine IEnumerator</returns>
        public IEnumerator GenerateArtificialVolumeWithSize(List<ShapeConfig> shapeConfigList, int volumeSlot, Vector3Int dataSize, Vector3 realSize, bool additional, bool addObjectModel)
        {
            if (materialConfig == null)
            {
                Debug.LogError("GenerateArtificialVolume: Use VolumeManager.SetMaterialConfig before GenerateArtificialVolume");
                yield break;
            }

            if (volumeSlot >= Volume.Volumes.Count)
            {
                if (volumeSlot > Volume.Volumes.Count)
                {
                    Debug.LogError("Volume slots must be used sequentially, using volumeSlot=" + volumeSlot);
                }

                additional = true;
                volumeSlot = Volume.Volumes.Count;
            }

            var renderList = materialConfig.GetRenderShapeConfigList(shapeConfigList);

            string filename = "Volume#" + volumeSlot;
            Vm2Header vm2header = new Vm2Header(Vm2.FormatEnum.CT8, dataSize, realSize * 1000, 1);

            Volume volume = null;

            if (additional && Volume.Volumes.Count == volumeSlot)
            {
                volume = Volume.AddGeneratedShapesGPU("ArtificialVolume.vm2", vm2header, renderList, null);
            }
            else
            {
                volume = Volume.Volumes[volumeSlot];
                //Debug.Log("GEN SLOT #" + volumeSlot +" SHAPES: "+shapeConfigList.Count);
                volume.GenerateShapesGPU("ArtificialVolume.vm2", vm2header, renderList, null);
            }// ---

            CleanupObjectModels(volume);

            while (volume.VolumeState != VolumeStateEnum.GPULOCK)
            {
                yield return null;
            }

            while (volume.VolumeState != VolumeStateEnum.READY)
            {
                yield return null;
            }

            // add background cube outline 
            AddVolumeOrientationHelp(volume);

            // set default vis
            UseMaterialConfigVisualization(volume, EVisualization.Colored);

            // add object models
            if (addObjectModel)
            {
                AddObjectModels(volume, shapeConfigList);
            }

            volume.VolumeTransform.GetComponent<Renderer>().enabled = false; // disable volume rendering

            //foreach (var shape in renderList)
            //    Debug.Log("renderList slot #"+volumeSlot+": " + shape.shapeCenter.ToString("F8"));
        }

        /// <summary>
        /// Generate a random shape as "artificial" volume on the GPU, using custom sizes for real and voxel data size.
        /// </summary>
        /// <param name="shapeConfig"></param>
        /// <param name="volumeSlot"></param>
        /// <param name="dataSize"></param>
        /// <param name="realSize"></param>
        /// <param name="additional"></param>
        /// <param name="addObjectModel"></param>
        /// <returns></returns>
        public IEnumerator GenerateArtificialVolumeWithSize(ShapeConfig shapeConfig, int volumeSlot, Vector3Int dataSize, Vector3 realSize, bool additional, bool addObjectModel)
            => GenerateArtificialVolumeWithSize(new List<ShapeConfig>() { shapeConfig }, volumeSlot, dataSize, realSize, additional, addObjectModel);

        /// <summary>
        /// Set the ultrasound scanner type
        /// </summary>
        /// <param name="scannerType"></param>
        /// <param name="usView"></param>
        public void SetUltrasoundScannerType(UltrasoundScannerTypeEnum scannerType, Volume volume = null)
        {
            //Debug.Log("<b>US: " + scannerType + "</b>");
            if (volume == null)
            {
                volume = Volume.Volumes[0];
            }

            switch (scannerType)
            {
                case UltrasoundScannerTypeEnum.LINEAR:
                    volume.UltrasoundScannerType = UltrasoundScannerTypeEnum.LINEAR;

                    volume.UseUltrasoundSliceMasking = false;
                    volume.UseSliceMasking = false;

                    volume.GetToolSliceMesh().GetComponent<MeshRenderer>().material.SetInt("uUseMask", 0);
                    break;

                case UltrasoundScannerTypeEnum.SECTOR:
                    volume.UltrasoundScannerType = UltrasoundScannerTypeEnum.SECTOR;

                    volume.SliceMaskingTexture = AppConfig.assets.ultrasoundMaskSector;
                    volume.UseSliceMasking = true;
                    volume.UseUltrasoundSliceMasking = true;
                    break;
                case UltrasoundScannerTypeEnum.CURVED:
                    volume.UltrasoundScannerType = UltrasoundScannerTypeEnum.CURVED;

                    volume.SliceMaskingTexture = AppConfig.assets.ultrasoundMaskCurved;
                    volume.UseSliceMasking = true;
                    volume.UseUltrasoundSliceMasking = true;
                    break;
                default:
                    volume.SliceMaskingTexture = null;
                    volume.UseSliceMasking = false;
                    volume.UseUltrasoundSliceMasking = false;
                    break;
            }

        }

        /// <summary>
        /// Set state of the volume rendering shader.
        /// </summary>
        /// <param name="volumeSlot"></param>
        /// <param name="state"></param>
        public void SetVolumeShaderState(int volumeSlot, bool state)
        {
            Volume.Volumes[volumeSlot].VolumeTransform.GetComponent<Renderer>().enabled = state;
            Volume.Volumes[volumeSlot].Threshold = 0.003f;
            Volume.Volumes[volumeSlot].SamplerateFactor = 2;
        }


        /// <summary>
        /// Query GPU memory for volume at worldspace position.
        /// </summary>
        /// <param name="volume"></param>
        /// <param name="worldspacePosition"></param>
        /// <returns>material index related to items of <see cref="MaterialConfig.map"/></returns>
        public int GetVoxelMaterialIndexAtPosition(Volume volume, Vector3 worldspacePosition)
        {
            Color32 gpuColor = volume.GetGpuVoxelRGB(worldspacePosition);
            return gpuColor.r;
        }

        public void ApplyScannerTypeToSliceView(UltrasoundScannerTypeEnum scannerType, RawImage usView, Texture texture)
        {
            switch (scannerType)
            {
                case UltrasoundScannerTypeEnum.LINEAR:
                    //usView.material = Instantiate(usViewRawImageLinearMaterial);
                    break;

                case UltrasoundScannerTypeEnum.SECTOR:
                    usView.material = Instantiate(AppConfig.assets.usViewRawImageSectorMaterial);
                    break;

                default:

                    usView.material = Instantiate(AppConfig.assets.usViewRawImageCurvedMaterial);
                    break;
            }

            usView.texture = null;
            usView.material.SetTexture("_MainTex", texture);
            usView.texture = texture;
        }

        public int GetActiveVolumeCount()
        {
            int result = 0;

            if (Volume.Volumes != null)
            {
                foreach (var v in Volume.Volumes)
                {
                    if (v != null && v.VolumeState != VolumeStateEnum.EMPTY)
                    {
                        result++;
                    }
                }
            }


            return result;
        }

    }
}