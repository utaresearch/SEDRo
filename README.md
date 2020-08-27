# SEDRo - Simulated Environment for Developmental Robotics

[![Screenshot](https://github.com/utaresearch/SEDRo/blob/master/Figures/environment.png)](https://github.com/utaresearch/SEDRo/blob/master)

**SEDRo** is a simulated environment, developed with the Unity engine to facilitate human-infant-like experineces from the fetus stage to 12-months, for the purpose of developing intelligence in non-verbal agents. To evaluate the intellectual progress of the agent, **SEDRo** has the provision to run a series of developmental psychology experiments.

## Getting Started

### Requirements
- Unity 2020.1.2f1+
- Unity ML agents Release 3 

### Installing SEDRo in your local machine

To lauch **SEDRo** in your local machine, follow the steps below:

1. Clone SEDRo repository into your local machine.
    * Use "Clone with SSH" option to avoid credential failure issues.
    * You can use GitHub Desktop app to avoid credential failure issues. 
    * If you do not have git-lfs enabled, do so to avoid invalid image files.
2. Clone the Unity ML-agents from [here](https://github.com/Unity-Technologies/ml-agents/tree/master).
    * Checkout "release_3_branch".
3. Launch unity hub by the command "./UnityHub.AppImage".
    * If you do not have unity hub installed in your machine, follow this [link](https://docs.unity3d.com/Manual/GettingStartedInstallingHub.html)
4. Using "ADD" option from the unity hub, navigate to the clonned SEDRo directory and open.
    * As current version of SEDRo uses Unity Version: 2018.4.18f1, download and install that from the Unity Hub.
5. While opening the project, an error might be shown mentioning "invalid path at ../Packages/manifest.json". Ignore the message and press continue.
6. Once the project is loaded into Unity Editor, got to Window > Package Manager. And remove the existing ML Agents package. Then, follow this [link](https://github.com/Unity-Technologies/ml-agents/blob/release_2_docs/docs/Installation.md#advanced-local-installation-for-development-1) to re-add the ML Agents packeage. This will fix the error mentioned in step 5.
7. Now, inside the Unity Editor, within "Scenes" folder existing scenes can be opened and interacted with. For example, opening the scene "MainScene" will look similar to this image-

[![Screenshot](https://github.com/utaresearch/SEDRo/blob/master/Figures/preview.png)](https://github.com/utaresearch/SEDRo/blob/master)

### Frequently Asked Questions (FAQs)

1. I do not have unity installed, what can I do?
    * Please follow this [link](https://docs.unity3d.com/Manual/GettingStartedInstallingHub.html) to install unity in your local machine.
2. I do not have ML-agents installed, how can I get that?
    * Clone the Unity ML-agents from [here](https://github.com/Unity-Technologies/ml-agents/tree/master). Then, checkout "release_3_branch".
3. I have clonned the repository, but the .png images seem to be corrupted, what can I do?
    * To save the time and bandwidth for "pushing" updates, we have enabled git-lfs for the image files (as they are large in size). You need to have git-lfs enabled to clone the actual image files rather than the placeholder files.
4. While loading project SEDRo in UnityHub, it is throwing an error about "invalid path at ../Packages/manifest.json", what should I do?
    * This error is occuring due to the local path mismatch of the ML-agents. To solve it, ignore the error message and press continue to load SEDRo. Once the project is loaded into Unity Editor, got to Window > Package Manager. And remove the existing ML Agents package. Then, follow this [link](https://github.com/Unity-Technologies/ml-agents/blob/release_2_docs/docs/Installation.md#advanced-local-installation-for-development-1) to re-add the ML Agents packeage.
5. I installed the latest version of ML-agents, but still getting namespace error like "namespace MLAgents can not be found". How can I solve this?
    * The current version of SEDRo is implemented with "release_3_branch" of the ML-agents. Please checkout "release_3_branch" to resolve the namespace error.

### Tutorials

## License
