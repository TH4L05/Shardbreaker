Frozen Tundra Environment Pack Readme

Package from Asset Store will only import the base meshes and textures. After initial import, navigate to Frozen Tundra Environment Pack folder to find FTEP_HD.unitypackage, FTEP_URP.unitypackage, and FTEP_SD.unitypackage. Import the package appropriate for the render pipeline in your project.

Example scenes in Standard and Universal Render Pipeline versions of package are only partially supported due to limitations of the render pipelines, and there may be issues with reflection probes and post processing. Use HDRP with HD version of package for best quality.

For all render pipelines, in Player settings, set Color Space to Linear, and disable Static Batching.

If using HDRP version, you will need to manually add the diffusion profiles located in FTEP_HDRP>Diffusion Profiles to your High Definition Render Pipeline Asset.

If using URP version, under pipeline asset, enable HDR and set Grading Mode to High Dynamic Range for best quality.

If using Standard version, install Post Processing via the Package Manager. If there are issues with post processing in example scene, remove the Post-process Layer component from Camera and then add it again.