# Accelerider RESTful API Document

## 1. Accelerider Account Methods

#### Summary

* **Base Url**: `http://api.usmusic.cn/v2`

| Method                        | HTTP Verb | API          | Description                                                                                          |
| ----------------------------- | --------- | ------------ | ---------------------------------------------------------------------------------------------------- |
| [Register](#register)         | `POST`    | `/signup`    | Registers a new Accelerider account.                                                                 |
| [Login](#login)               | `POST`    | `/login`     | Login your Accelerider account.                                                                      |
| [Sign out](#signout)          | `PATCH`   | `/{user id}` | Sign up your Accelerider account.                                                                    |
| [Update](#update)             | `PATCH`   | `/{user id}` | Update the information of your Accelerider account, i.e.: `username`, `password` or `headimage` etc. |
| [Get metadata](#getmetadata1) | `GET`     | `/{user id}` | Get the metadata of Accelerider account.                                                             |

#### <span id="register">Register</span>

#### <span id="login">Login</span>

#### <span id="signout">Sign out</span>

#### <span id="update">Update</span>

#### <span id="getmetadata1">Get metadata</span>

## 2. Net-Disk Service Methdos

* **Base Url**: `http://api.usmusic.cn/v2/{user id}`

### 2.1 Common Methods

#### Summary

| Method                               | HTTP Verb | API                      | Description                                                         |
| ------------------------------------ | --------- | ------------------------ | ------------------------------------------------------------------- |
| [Add a net-disk](#addanetdisk)       | `POST`    | `/{disk type}`           | Add a new net-disk with required authentication information.        |
| [Delete a net-disk](#deleteanetdisk) | `DELETE`  | `/{disk type}/{disk id}` | Delete a existed net-disk with disk id.                             |
| [Update metadata](#updatemetadata1)  | `PATCH`   | `/{disk type}/{disk id}` | Update the information of specified net-disk.                       |
| [Get metadata](#getmetadata2)        | `GET`     | `/{disk type}/{disk id}` | Get the metadata of specified net-disk.                             |
| [List net-disks](#listnetdisks)      | `GET`     | `/{disk type}/children`  | List all of existed net-disk, according to the specified disk type. |

#### <span id="addanetdisk">Add a net-disk</span>

#### <span id="deleteanetdisk">Delete a net-disk</span>

#### <span id="updatemetadata1">Update metadata</span>

#### <span id="getmetadata2">Get metadata</span>

#### <span id="listnetdisks">List net-disks</span>

### 2.2 Only Accelerider Cloud Methods

#### Summary

| Method                        | HTTP Verb | API                      | Description                        |
| ----------------------------- | --------- | ------------------------ | ---------------------------------- |
| [Add a task](#addatask)       | `POST`    | `/cloud/tasks`           | Add a new offline download task.   |
| [Delete a task](#deleteatask) | `DELETE`  | `/cloud/tasks/{task id}` | Delete the specified task.         |
| [Get metadata](#getmatadata3) | `GET`     | `/cloud/tasks/{task id}` | Get the metadata of specified task |
| [List tasks](#listtasks)      | `GET`     | `/cloud/tasks/children`  | Get all currently existing tasks.  |

#### <span id="addatask">Add a task</span>

#### <span id="deleteatask">Delete a task</span>

#### <span id="getmatadata3">Get metadata</span>

#### <span id="listtasks">List tasks</span>

## 3. Teams Service Methods

#### Summary

* **Base Url**: `http://api.usmusic.cn/v2/{user id}`

| Method                              | HTTP Verb | API                                  | Description                                                  |
| ----------------------------------- | --------- | ------------------------------------ | ------------------------------------------------------------ |
| [Create a team](#createateam)       | `POST`    | `/teams`                             | Create a new team in Teams.                                  |
| [List teams](#listteams)            | `GET`     | `/teams/children`                    | Get all currently existing teams.                            |
| [Get metadata](#getmetadata4)       | `GET`     | `/teams/{team id}`                   | Get the metadata of the specified team.                      |
| [Update metadata](#updatemetadata2) | `PATCH`   | `/teams/{team id}`                   | Update the metadata of the specified team.                   |
| [Delete a team](#deleteateam)       | `DELETE`  | `/teams/{team id}`                   | Dissolve a team, which requires the appropriate permissions. |
| [Add a member](#addamember)         | `POST`    | `/teams/{team id}/members`           | Join or add a new member to thes pecified team.              |
| [List members](#listmembers)        | `GET`     | `/teams/{team id}/members/children`  | Get all metadata of currently existing members in this team. |
| [Get metadata](#getmetadata5)       | `GET`     | `/teams/{team id}/members/{user id}` | Get the metadata of the specified member.                    |
| [Delete a member](#deleteamember)   | `DELETE`  | `/teams/{team id}/members/{user id}` | Quit the team, or delete a existed member by user id.        |
| [Add a comment](#addacomment)       | `POST`    | `/teams/{team id}/comments`          | Send a commont to the spicified team.                        |
| [List comments](#listcomments)      | `GET`     | `/teams/{team id}/comments/children` | Get all or part historical comment.                          |

#### <span id="createateam">Create a team</span>

#### <span id="listteams">List teams</span>

#### <span id="getmetadata4">Get metadata</span>

#### <span id="updatemetadata2">Update metadata</span>

#### <span id="deleteateam">Delete a team</span>

#### <span id="addamember">Add a member</span>

#### <span id="listmembers">List members</span>

#### <span id="getmetadata5">Get metadata</span>

#### <span id="deleteamember">Delete a member</span>

#### <span id="addacomment">Add a comment</span>

#### <span id="listcomments">List comments</span>

## 4. File Operation Methods

#### Summary

* **Base Url**: `http://api.usmusic.cn/v2/{user id}/{disk type}/{disk id}`
* **Base Url**: `http://api.usmusic.cn/v2/{user id}/teams/{team id}`

| Method                              | HTTP Verb | API                         | Description                                                                       |
| ----------------------------------- | --------- | --------------------------- | --------------------------------------------------------------------------------- |
| [Upload a file](#updateafile)       | `POST`    | `/files`                    | Update a new file to net-disk.                                                    |
| [Get files](#getfiles)              | `GET`     | `/files/{file type}`        | Get files by specified type. i.e.: `musics`, `vedios` or `pictures` etc.          |
| [Get meatdata](#getmetadata6)       | `GET`     | `/files/{file id}`          | Get the specified file.                                                           |
| [Update metadata](#updatemetadata7) | `PATCH`   | `/files/{file id}`          | Update the metadata of the specified file. i.e.: Rename, Move or Share operation. |
| [Delete a file](#deleteafile)       | `POST`    | `/files/{file id}`          | Delete a existed file with file id.                                               |
| [List files](#listfiles)            | `POST`    | `/files/{file id}/children` | List the children of the file, whose type must be folder.                         |
| [Download a file](#downloadafile)   | `POST`    | `/files/{file id}/content`  | Get download links of the specified file and necessary download information.      |

#### <span id="updateafile">Upload a file</span>

#### <span id="getfiles">Get files</span>

#### <span id="getmetadata6">Get meatdata</span>

#### <span id="updatemetadata7">Update metadata</span>

#### <span id="deleteafile">Delete a file</span>

#### <span id="listfiles">List files</span>

#### <span id="downloadafile">Download a file</span>
