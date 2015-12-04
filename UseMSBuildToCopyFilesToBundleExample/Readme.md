UseMSBuildToCopyFilesToBundleExample
==========

This sample shows you how to extend your build with msbuild to copy arbitrary files into your bundle.

This is better the custom commands in your projects settings, as they will fire even when building via xbuild. They also fire before code signing, so they may invalidate your signature and require resigning.

You will need to edit your csproj in a text editor and add a line similar to:

```
<Import Project="CustomBuildActions.targets" />
```

for the logic to apply during your build.