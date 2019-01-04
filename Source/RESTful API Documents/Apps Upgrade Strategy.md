# Apps Upgrade Strategy

## 1. Gets available app metadata from the server

- `GET v2/apps`

- Response: `List<AppMetadata>`

  ```json
  [
      {
      	"id": "mongodb_id",
      	"name": "Accelerider.Windows.Modules.NetDisk",
      	"version": "0.0.1",
      	"url": "https://api.accelerider.com/v2/apps/{file_name}",
      	"moduleType": "{AssemblyQualifiedName}",
      	"dependsOn": []
      },
      {
          "id": "mongodb_id",
          "name": "Accelerider.Windows.Modules.NetDisk",
          "version": "0.0.1",
          "url": "https://api.accelerider.com/v2/apps/{file_name}",
          "moduleType": "{AssemblyQualifiedName}",
          "dependsOn": ["Accelerider.Windows.Modules.NetDisk"]
      }
  ]
  ```
