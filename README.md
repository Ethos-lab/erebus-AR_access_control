# EREBUS: USENIX Security'23 Artifact


## Camera-ready version
See attached `paper.pdf`.


## Appendix Abstract

The core components of Erebus can be separated into 3 separate modules:
1. Policy Engine that generates Erebus policies from Natural Language inputs.
2. Language Transpiler that converts the intermediate erebus language into the 
target platform code (in this implementation C#).
3. Native library implementation (in C#) that is used to build Android APKs for 
testing.


For reproducibility, we separate our implementation into these separate components
and provide instructions on how to reproduce our results.
In this Artifact, we provide instructions to reproduce
the policy engine and policy generation of Erebus (components 1 and 2). We also
open-source our implementation of the Erebus framework itself, developed using Unity 
and C#. However due to the complexity of setup required, and the number of 
platform-specific dependencies required to recompile the Android
APK files, we skip these components for reproducibility and provide all
necessary information to be used as reference.


## Prerequisites

Please ensure you have the following environments setup with all the dependencies
to be able to reproduce the artifacts in the paper.

System: Ubuntu 20.04
Python 3.8.10
Dotnet 6.0

*Install miniconda*

```
sudo apt update
sudo apt upgrade

wget https://repo.anaconda.com/miniconda/Miniconda3-py38_23.3.1-0-Linux-x86_64.sh 

bash Miniconda3-py38_23.3.1-0-Linux-x86_64.sh
```

Make sure to accept the terms and conditions for installing Miniconda and 
JupyterLab (next step).


*Create a Conda environment and install Jupyterlab*

```
conda create --name nlp python=3.8
conda install -c conda-forge jupyterlab
```

*Activate environment and install python packages*

```
conda activate nlp
pip install -r requirements.txt
```

*Install dotnet SDK and runtime*

```
wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo apt update
sudo apt install -y dotnet-sdk-6.0
sudo apt install -y dotnet-runtime-6.0
```

Check dotnet version using `dotnet --version`.

*Install OpenJDK. Our recommended version is OpenJDK 11.0.19*
```
sudo apt update
sudo apt install default-jdk
```

Check openjdk version using `java -version`.


## Organization of the Artifact

1. `erebus` folder contains our framework implementation in C#, that we 
open-source for developers and researchers to use as a reference model.

2. `policy_gen` folder contains the policy engine implementation which takes
generates Erebus policies from natural language input.

3. `policy_transpiler` folder contains the C# implementation of our transpiler
developed using ANTLR4, which converts the intermediate policy code to the 
target platform code.

4. `prototype_apps` contains the sample code for the prototype applications we 
developed for evaluation.

5. `survey` contains the app and device surveys we conducted to inform the design
of Erebus. Tables [1,2,3] in the paper summarize the results from these surveys.



## Instructions for Reproducibility


### Reproducing Policy Engine

Refer to Figure 4 and Listing 3 in the paper for included example.

For the convenience of evaluators, we provide a Jupyter notebook file that
can be directly used to reproduce and validate our results.

The actual implementation, along with the trained models and our training data
used are all included in the folder `erebus_policy_gen`.

1.  ` cd policy_gen `

2. Make sure you have the conda environment we created earlier
running in the current shell.
` conda activate nlp `

3.  Start a Jupyter Notebook Server
` jupyter notebook `

4. Open the Jupyter notebook console in your browser with the provided URL.
It will be something like `http://127.0.0.1:8888/?token=<tokenID>`

5. Open the `nl_policy_generation.ipynb` notebook file in the browser. You should
be in the `policy_gen` folder when you start the notebook server, else navigate
to the notebook file and open it on Jupyter Notebook.

6. Delete the file `resources/input.el`. This file stores the generated policies
by the policy engine, so make sure to remove this file before every run (else
the file just gets overwritten with new content).

7. Run the cells [1,2] in sequential order. It should load the models included
in the subdirectories and parse the input to create the policy required.


#### Sample policies that can be tested

The following set of sample policy descriptions are provided for verification.
Update the `text` field in Cell 2 of the Jupyter notebook with any one of these
statements and check the output generated in `resources/input.el`.

+ Allow image detection if I am Home and it is after 9:00pm
+ Allow this app to detect objects only for QR codes and only during evening
+ Allow this app to work only if I am at Work
+ Allow only when I am Home and during the permitted hours on weekends
+ Only allow Keyboards to be detected by the camera
+ Deny image tracking if I am at Home and it is after 10:00pm
+ If this app uses plane detection only allow if I am at Home
+ Deny location access if Batman is Home
+ If this app tracks location deny access if Superman is using the app at Work
+ If Batman is playing this game at Home allow plane detection

#### Example input & output

Input command passed in the `text` field of `nl_policy_generation.ipynb` notebook file : 
` Deny location access if Batman is Home `

Output file generated to `resources/input.el` :
```
function GetGPSLocation()
{
	let curLoc = GetCurrentLocation();
	let trustedLoc = GetTrustedLocation("Home");
	let curFace = GetCurrentFaceId();
	let trustedFaces = GetTrustedFaceId("Batman");
	if ( curLoc.within(trustedLoc) )
	{
		if ( curFace.matches(trustedFaces) )
		{
			Deny;
		}
	}
}
```


### Reproducing Policy Transpiler

The policy transpiler module acts as the target code generator. Refer to Table 4
in the paper which shows the language grammar we have defined for our intermediate
policy code. We developed a transpiler using this language to generate
target code in C# using the the policy code obtained in the earlier step.

Once you have generated a policy code using the Policy Engine in the previous
step, copy the `input.el` file into the Policy transpiler folder to be used as 
input for target code generation.

1. `cd policy_transpiler/erebus.Core`

2. Copy the `input.el` file generated by policy_gen in the previous step to 
`Resources/input.el`, or just use the one already included.

` cp ../../policy_gen/resources/input.el Resources/input.el `

3. Run the dotnet app. Ensure you are in the `erebus.Core` subdirectory.

` dotnet run `

4. This will generate the target code based on `input.el` file into the 
`Resources/output.c1s` file.


> Note: Other dependencies like ANTLR4 is already pacakaged into this application.
> If there are any permission errors for installation, make sure the 
> `Scripts/Grammar/antlr-4.10.1-complete.jar` has executable permissions.

#### Example input & output

Input command from `input.el` Erebus language file :
```
function GetGPSLocation()
{
	let curLoc = GetCurrentLocation();
	let trustedLoc = GetTrustedLocation("Home");
	let curFace = GetCurrentFaceId();
	let trustedFaces = GetTrustedFaceId("Batman");
	if ( curLoc.within(trustedLoc) )
	{
		if ( curFace.matches(trustedFaces) )
		{
			Deny;
		}
	}
}
```

Output C# file generated to `output.c1s` :
```
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Linq;
using System;
using UnityEngine;
using DetectionResult;
namespace Erebus
{
	namespace AccessControl
	{
		public class ErebusAccessControl : BaseAssemblyEntryPoint
		{
			public ErebusAccessControl(Assembly baseAssembly, string baseProgramName) : base(baseAssembly, baseProgramName)
			{ }
			public bool CheckGetGPSLocation()
			{
				var curLoc = GetCurrentLocation();
				var trustedLoc = GetTrustedLocation("Home");
				var curFace = GetCurrentFaceId();
				var trustedFaces = GetTrustedFaceId("Batman");
				if ( curLoc.Within(trustedLoc) )
				{
					if ( curFace.Matches(trustedFaces) )
					{
						return false;
					}
					}
					return false;
				}

			}
		}
}
```
