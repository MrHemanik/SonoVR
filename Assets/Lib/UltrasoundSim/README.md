# Ultrasound Simulation
Version 1.0 from 11.08.2019
As part of a master thesis, a new ultrasound simulation has been developed. Physical formulas are used to calculate ultrasound simulation based on CT-data.

## Getting Started
These instructions will help you running the project on your machine for development purposes.

### Prerequisites
It is necessary to install Unity. I used Unity 2018.3.14f1 with Windows 10.
Also the mKit of the Hochschule Flensburg is needed. If you making a new project, the mKit has to be moved in the folder of this project.
The mKit is already included in this project.

### Installing
It is necessary to attach the script *NewUltrasoundRender.cs* to a gameobject in the Unity scene. It can be an empty one. In the inspector, the *ultrasoundComputeShader.compute* must be added to this script. It is also necessary to attach a RawImage to this script, where the texture can be displayed on.

To use this script, the following line must be written in a Awake-Method:
```
Config.UltrasoundGenerateInRenderTexture = true;
```
Furthermore write in the function where you activate ultrasound 
```
ctrl.useUltrasound = true;
ctrl.UltrasoundRenderer = NewUltrasoundRender.Instance;
```

## Deployment
For changing the values use the following code
```
NewUltrasoundRender.Instance.xxx = yyy;
```
xxx can be different parameters, like frequency, gain or noise. yyy is the chosen value.

For testing the script in the editor of Unity with android as target platform, the graphics emulation need to be changed. You can find it under *Edit*. It must be set to *No Emulation*.

## Authors
These scripts are written by Fenja Bruns.

## License
This project is licensed under the MIT License - see at [OpenSourceInitiative](https://opensource.org/licenses/MIT) for details.

## Acknowledgments
Thanks to Matthias Süncksen for helping me understanding the features of the mKit.