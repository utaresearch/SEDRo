# SEDRo - Simulated Environment for Developmental Robotics

[![Screenshot](https://github.com/utaresearch/SEDRo/blob/master/Figures/environment.png)](https://github.com/utaresearch/SEDRo/blob/master)

**SEDRo** is a simulated environment, developed with the Unity engine to facilitate human-infant-like experineces from the fetus stage to 12-months, for the purpose of developing intelligence in non-verbal agents. To evaluate the intellectual progress of the agent, **SEDRo** has the provision to run a series of developmental psychology experiments.

## Getting Started

### Installing SEDRo in your local machine

To lauch **SEDRo** in your local machine, follow the steps below:

1. Clone SEDRo repository into your local machine.
    * Use "Clone with SSH" option to avoid credential failure issues.
    * If you do not have git-lfs enabled, do so to avoid invalid image files.
2. Clone the Unity ML-agents from [here](https://github.com/Unity-Technologies/ml-agents/tree/master).
    * Checkout "release-0.14.0".
3. Launch unity hub by the command "./UnityHub.AppImage".
    * If you do not have unity hub installed in your machine, follow this [link](https://docs.unity3d.com/Manual/GettingStartedInstallingHub.html)
4. Using "ADD" option from the unity hub, navigate to the clonned SEDRo directory and open.
    * As current version of SEDRo uses Unity Version: 2018.4.18f1, download and install that from the Unity Hub.
5. While opening the project, an error might be shown mentioning "invalid path at ../Packages/manifest.json". Ignore the message and press continue.
6. Once the project is loaded into Unity Editor, got to Window > Package Manager. And remove the existing ML Agents package. Then, follow this [link](https://github.com/Unity-Technologies/ml-agents/blob/release_2_docs/docs/Installation.md#advanced-local-installation-for-development-1) to re-add the ML Agents packeage. This will fix the error mentioned in step 5.
7. Now, inside the Unity Editor, within "Scenes" folder existing scenes can be opened and interacted with. For example, opening the scene "MainScene" will look similar to this image-

[![Screenshot](https://github.com/utaresearch/SEDRo/blob/master/Figures/preview.png)](https://github.com/utaresearch/SEDRo/blob/master)

### Tutorials

## License
