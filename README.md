# ObjectStorageSharp
OpenStack Object Storage Library https://developer.openstack.org/api-ref/object-store/ (for ConoHa https://www.conoha.jp/guide/objectstoragerestapi.php)

# Usage

## Authenticate

OpenStack Keystone

```csharp
var keystone = await KeyStone.Authenticate(
	"https://path.to/authenticate/service/",
	"tenantname",
	"username",
	"password"
	);
var os = ObjectStorage.FromKeyStone(keystone);
```

## Get Container List

```csharp
var list = await os.GetContainerList();
foreach(var c in list) {
	Console.WriteLine($"{c.name} : {c.bytes}byte, {c.count}files");
}
```

## Create Container

with Http Header

```csharp
var result = await os.CreateContainer("test-container", new Dictionary<string, string>() {
		{"X-Web-Mode", "true" },
		{"X-Container-Read", ".r:*" },
	});
```

## Delete Container

```csharp
var result = await os.DeleteContainer("test-delete-container");
```

## Get Object List

```csharp
var list = await os.GetObjectList("test", new Dictionary<string, string>() {
	});
foreach(var o in list) {
	Console.WriteLine($"{o.name} : {o.bytes}byte, {o.last_modified}");
}
```

## Download Object

and save to local

```csharp
var result = await os.GetObject("path/to-object");
System.IO.File.WriteAllBytes('path/to/save/file', await result.Content.ReadAsByteArrayAsync());
```

## Upload Object

```csharp
var result = await os.PutObject("containerName", "path/to/local-file", "object-name");
```

## Delete Object

```csharp
var result = await os.DeleteObject("containerName", "object-name");
```

