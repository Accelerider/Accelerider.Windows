# Apps Upgrade Strategy

## 1. Get available app metadata list

- `GET v2/apps`

- Response: `List<AppMetadata>`

```json
[
    {
        "name": "Accelerider.Windows.Modules.NetDisk",
        "version": "0.0.1",
        "url": "https://api.accelerider.com/v2/apps/{file_name-version}.zip",
        "moduleType": "{AssemblyQualifiedName}",
        "updateNotes": "...",
        "dependsOn": []
    },
    {
        "name": "Accelerider.Windows.Modules.Group",
        "version": "0.0.1",
        "url": "https://api.accelerider.com/v2/apps/{file_name-version}.zip",
        "moduleType": "{AssemblyQualifiedName}",
        "updateNotes": "...",
        "dependsOn": ["Accelerider.Windows.Modules.NetDisk"]
    }
]
```

## 2. Publish a new version app (Authorize)

- `POST v2/apps`

- Response: `201`

- Post Body: `AppMetadata`

```json
    {
        "name": "Accelerider.Windows.Modules.Group",
        "version": "0.0.1",
        "url": null,
        "moduleType": "{AssemblyQualifiedName}",
        "updateNotes": "...",
        "dependsOn": ["Accelerider.Windows.Modules.NetDisk"]
    }
```
