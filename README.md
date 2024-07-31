![logo](https://github.com/yetanotherchris/CloudFileStore/raw/master/icon.png)

# CloudFileStore
A .NET Standard multi-cloud file storage library (S3, Google Cloud buckets, Azure blobs).

[![NuGet](https://img.shields.io/nuget/v/CloudFileStore.svg)](https://www.nuget.org/packages/CloudFileStore/)  

## About

CloudFileStore is a .NET Core library to try to unify or speed up file management when you're talking to multiple cloud providers. You know...ones apart from AWS! It's designed just for file storage: S3, Google Cloud, Azure Blobs and hopefully more as it develops.

It's not intended to cover every scenario each SDK offers, for example it won't give you metadata. It also bundles every file storage SDK of every cloud provider - it's designed this way (for now at least) on purpose, for applications that need multi-cloud file operations. So in terms of extra DLLs and file size, it's fairly hefty.

Two projects that currently use it are [Letmein](https://github.com/yetanotherchris/letmein) and [Roadkill](https://github.com/roadkillwiki/roadkill).

## Usage

Right now, the library only supports text file loading and saving, but it will grow over time. Contributions are welcome, all that is asked is you follow the [.NET framework guidelines](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions) e.g. implicit typed local variables.

```
var config = new S3Configuration()
{
    BucketName = "some bucket",
    SecretKey = "secret key",
    AccessKey = "access key",
    Region = "eu-west-1"
};
var provider = new S3StorageProvider(config);
await provider.SaveTextFileAsync("myfile.txt", "content here");

string content = await provider.LoadTextFileAsync("myfile.txt");
```

Full examples can be found in the [integration tests](https://github.com/yetanotherchris/CloudFileStore/tree/master/src/CloudFileStore.Tests/Integration).
