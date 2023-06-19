
####  *The hierarchy/structure is NOT as same as the original. Uploaded only the core scripts of the projects*

## 1. Erebus Access Controller (Language UI app)
> Language : Unity C# 

#### 1.  Unity configuration
- Android platform
- **MUST** be Mono scripting  (IL2CPP use AOT compilation -> No runtime code compilation)
- .NET 4.0
- Reconfigure 'Company Name,' 'Product Name,' if you download the Unity project file and set Minimum Android API (25) 
- (Optional) Place output.txt (Mock C# code) to Resources folder
  - ***Temporarily, input is NOT coming from the Erebus Language, but directly from output.txt (CSharp file) from the Resources folder***
  - Language doesn't support the conversion of custom defined (Contains/IncludedIn/Within) functions. 
  - 'Output.txt' is the expected C# code that is converted from Erebus language/code
- Place the runtime code base/parent class ('BaseAssemblyEntryPoint.cs') under StreamingAssets folder
- Place the default (Placeholder code) erebus language code of the four AR apps and name them
input1.erebus, input2.erebus, input3.erebus, input4.erebus, respectively
- Add runtime library assets (System.Collections.dll, DetectionResult.dll, etc) using Roslyn Libray Reference Asset Manager and link all the additional MetaAssets to the CodeController.cs of the UI app
- Place all the necessary files of ANTLR Transpiler, C# Converter to the Scripts folder

#### 2.  External dependencies
- BetterStreamingAssets : For easy I/O access in Android
- RosylnCSharp : For assembly code byte conversion + Runtime compilation
- NuGet for Unity : To import NuGest in Unity
- ANTLR4 (Runtime.Standard) : To convert Erebus language to C# (**MUST** download this version : "Antlr4.Runtime.Standard.4.10.1" thru Nuget)
- NetMQ : For networking with the main ErebusARManager Unity app
- OpenCV for Unity : For saving face signatures to the DB
- Custom DLL (DetectionResult; Managed C#) : C# Interface for DetectionResult/Metadata Class 
(Sharing types over runtime compiled code & Erebus AR Manager's code)

## 2. Example AR apps using Erebus (AR Toy Car)

> Language : Unity C# 

#### 1.  Unity configuration
- Android platform
- **MUST** be Mono scripting (IL2CPP use AOT compilation -> No runtime code compilation)
- .NET 4.0
- Reconfigure 'Company Name,' 'Product Name,' if you download the Unity project file and set Minimum Android API (25) 
- Place frozen weight (ONNX) files under StreamingAssets folder for face det/recognition 
('face_detection_yunet_2021dec.onnx' and 'face_recognition_sface_2021dec')
- Connect prefabs of PopUpMenu and DataManager to the Erebus AR Manager
- Must include 'FaceIdCheck.scene' to the second row of the list of scenes
- Input the correct IP addr/port of the Local processing server

#### 2.  External dependencies
- BetterStreamingAssets : For easy I/O access in Android
- NuGet for Unity : To import NuGest in Unity
- ANTLR4 (Runtime.Standard) : To convert Erebus language to C#
- NetMQ : For networking with the main ErebusARManager Unity app
- OpenCV for Unity : For loading saved face signatures + Face recognition algorithm + OpenCV operations
- Custom DLL (DetectionResult; Managed C#) : C# Interface for DetectionResult/Metadata Class 
(Sharing types over runtime compiled code & Erebus AR Manager's code)
- MessagePack for Unity : Byte converter (Faster than built-in utf-8 encoding or Newtonsoft Json)
- Unity.IO.Compression (.NET gzip) : Used to decompress the bytes sent from the Local server (Python)

## 3. Local processing server
> Language : Python (Remote node)

#### 1.  Python configuration
- Either run the ***actual server*** or use lightweight ***mock-up version*** (Returns mock-up json-formatted values)

#### 2.  External dependencies
- pyzmq
- msgpack
- gzip
- etc (Object detector/recognizer/etc)
