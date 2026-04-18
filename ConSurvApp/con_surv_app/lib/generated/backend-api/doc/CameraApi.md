# openapi.api.CameraApi

## Load the API package
```dart
import 'package:openapi/api.dart';
```

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**aPIV3CameraControllerCameraCameraIdGet**](CameraApi.md#apiv3cameracontrollercameracameraidget) | **GET** /API/v3/CameraController/Camera/{cameraId} | Retrieves the full configuration details of a specific camera.
[**aPIV3CameraControllerCamerasGet**](CameraApi.md#apiv3cameracontrollercamerasget) | **GET** /API/v3/CameraController/Cameras | Returns the list of all configured cameras visible to the current user.
[**aPIV3CameraControllerCreateCameraPost**](CameraApi.md#apiv3cameracontrollercreatecamerapost) | **POST** /API/v3/CameraController/CreateCamera | Creates a new camera with default name and RTSP address and returns its generated identifier.
[**aPIV3CameraControllerDownloadVideoCameraIdFilenameGet**](CameraApi.md#apiv3cameracontrollerdownloadvideocameraidfilenameget) | **GET** /API/v3/CameraController/DownloadVideo/{cameraId}/{filename} | Downloads the raw bytes of a specific recorded video file for a given camera.
[**aPIV3CameraControllerGetPreviewCameraIdGet**](CameraApi.md#apiv3cameracontrollergetpreviewcameraidget) | **GET** /API/v3/CameraController/GetPreview/{cameraId} | Returns the latest preview image (as raw bytes) for the specified camera.
[**aPIV3CameraControllerGetPreviewOfVideocameraIdFilenameGet**](CameraApi.md#apiv3cameracontrollergetpreviewofvideocameraidfilenameget) | **GET** /API/v3/CameraController/GetPreviewOfVideo{cameraId}/{filename} | Returns a preview thumbnail image for the specified recorded video file of a camera.
[**aPIV3CameraControllerListVideosGet**](CameraApi.md#apiv3cameracontrollerlistvideosget) | **GET** /API/v3/CameraController/ListVideos | Returns a dictionary mapping each camera identifier to the list of recorded video filenames available for that camera.
[**aPIV3CameraControllerRemoveCameraCameraIdDelete**](CameraApi.md#apiv3cameracontrollerremovecameracameraiddelete) | **DELETE** /API/v3/CameraController/RemoveCamera/{cameraId} | Permanently removes the specified camera and all associated data.
[**aPIV3CameraControllerRemoveVideoCameraIdFilenameDelete**](CameraApi.md#apiv3cameracontrollerremovevideocameraidfilenamedelete) | **DELETE** /API/v3/CameraController/RemoveVideo/{cameraId}/{filename} | Permanently deletes a specific recorded video file belonging to the given camera.
[**aPIV3CameraControllerRunONVIFCommandCameraIdPost**](CameraApi.md#apiv3cameracontrollerrunonvifcommandcameraidpost) | **POST** /API/v3/CameraController/RunONVIFCommand/{cameraId} | Executes an ONVIF command on the specified camera (e.g., PTZ control, preset recall).
[**aPIV3CameraControllerUpdateCameraPut**](CameraApi.md#apiv3cameracontrollerupdatecameraput) | **PUT** /API/v3/CameraController/UpdateCamera | Updates the properties of an existing camera using the values provided in the request body.


# **aPIV3CameraControllerCameraCameraIdGet**
> CameraDTO aPIV3CameraControllerCameraCameraIdGet(cameraId, xAccessToken)

Retrieves the full configuration details of a specific camera.

### Example
```dart
import 'package:openapi/api.dart';

final api_instance = CameraApi();
final cameraId = cameraId_example; // String | The unique identifier of the camera to retrieve.
final xAccessToken = xAccessToken_example; // String | Access Token

try {
    final result = api_instance.aPIV3CameraControllerCameraCameraIdGet(cameraId, xAccessToken);
    print(result);
} catch (e) {
    print('Exception when calling CameraApi->aPIV3CameraControllerCameraCameraIdGet: $e\n');
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **cameraId** | **String**| The unique identifier of the camera to retrieve. | 
 **xAccessToken** | **String**| Access Token | 

### Return type

[**CameraDTO**](CameraDTO.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

# **aPIV3CameraControllerCamerasGet**
> List<CameraDTO> aPIV3CameraControllerCamerasGet(xAccessToken)

Returns the list of all configured cameras visible to the current user.

### Example
```dart
import 'package:openapi/api.dart';

final api_instance = CameraApi();
final xAccessToken = xAccessToken_example; // String | Access Token

try {
    final result = api_instance.aPIV3CameraControllerCamerasGet(xAccessToken);
    print(result);
} catch (e) {
    print('Exception when calling CameraApi->aPIV3CameraControllerCamerasGet: $e\n');
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **xAccessToken** | **String**| Access Token | 

### Return type

[**List<CameraDTO>**](CameraDTO.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

# **aPIV3CameraControllerCreateCameraPost**
> String aPIV3CameraControllerCreateCameraPost(xAccessToken)

Creates a new camera with default name and RTSP address and returns its generated identifier.

### Example
```dart
import 'package:openapi/api.dart';

final api_instance = CameraApi();
final xAccessToken = xAccessToken_example; // String | Access Token

try {
    final result = api_instance.aPIV3CameraControllerCreateCameraPost(xAccessToken);
    print(result);
} catch (e) {
    print('Exception when calling CameraApi->aPIV3CameraControllerCreateCameraPost: $e\n');
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **xAccessToken** | **String**| Access Token | 

### Return type

**String**

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

# **aPIV3CameraControllerDownloadVideoCameraIdFilenameGet**
> aPIV3CameraControllerDownloadVideoCameraIdFilenameGet(cameraId, filename)

Downloads the raw bytes of a specific recorded video file for a given camera.

### Example
```dart
import 'package:openapi/api.dart';

final api_instance = CameraApi();
final cameraId = cameraId_example; // String | The unique identifier of the camera that owns the video.
final filename = filename_example; // String | The filename of the recorded video to download.

try {
    api_instance.aPIV3CameraControllerDownloadVideoCameraIdFilenameGet(cameraId, filename);
} catch (e) {
    print('Exception when calling CameraApi->aPIV3CameraControllerDownloadVideoCameraIdFilenameGet: $e\n');
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **cameraId** | **String**| The unique identifier of the camera that owns the video. | 
 **filename** | **String**| The filename of the recorded video to download. | 

### Return type

void (empty response body)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: Not defined

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

# **aPIV3CameraControllerGetPreviewCameraIdGet**
> String aPIV3CameraControllerGetPreviewCameraIdGet(cameraId, xAccessToken)

Returns the latest preview image (as raw bytes) for the specified camera.

### Example
```dart
import 'package:openapi/api.dart';

final api_instance = CameraApi();
final cameraId = cameraId_example; // String | The unique identifier of the camera whose preview should be retrieved.
final xAccessToken = xAccessToken_example; // String | Access Token

try {
    final result = api_instance.aPIV3CameraControllerGetPreviewCameraIdGet(cameraId, xAccessToken);
    print(result);
} catch (e) {
    print('Exception when calling CameraApi->aPIV3CameraControllerGetPreviewCameraIdGet: $e\n');
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **cameraId** | **String**| The unique identifier of the camera whose preview should be retrieved. | 
 **xAccessToken** | **String**| Access Token | 

### Return type

**String**

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

# **aPIV3CameraControllerGetPreviewOfVideocameraIdFilenameGet**
> aPIV3CameraControllerGetPreviewOfVideocameraIdFilenameGet(cameraId, filename)

Returns a preview thumbnail image for the specified recorded video file of a camera.

### Example
```dart
import 'package:openapi/api.dart';

final api_instance = CameraApi();
final cameraId = cameraId_example; // String | The unique identifier of the camera that owns the video.
final filename = filename_example; // String | The filename of the recorded video for which the preview is requested.

try {
    api_instance.aPIV3CameraControllerGetPreviewOfVideocameraIdFilenameGet(cameraId, filename);
} catch (e) {
    print('Exception when calling CameraApi->aPIV3CameraControllerGetPreviewOfVideocameraIdFilenameGet: $e\n');
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **cameraId** | **String**| The unique identifier of the camera that owns the video. | 
 **filename** | **String**| The filename of the recorded video for which the preview is requested. | 

### Return type

void (empty response body)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: Not defined

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

# **aPIV3CameraControllerListVideosGet**
> aPIV3CameraControllerListVideosGet()

Returns a dictionary mapping each camera identifier to the list of recorded video filenames available for that camera.

### Example
```dart
import 'package:openapi/api.dart';

final api_instance = CameraApi();

try {
    api_instance.aPIV3CameraControllerListVideosGet();
} catch (e) {
    print('Exception when calling CameraApi->aPIV3CameraControllerListVideosGet: $e\n');
}
```

### Parameters
This endpoint does not need any parameter.

### Return type

void (empty response body)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: Not defined

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

# **aPIV3CameraControllerRemoveCameraCameraIdDelete**
> aPIV3CameraControllerRemoveCameraCameraIdDelete(cameraId, xAccessToken)

Permanently removes the specified camera and all associated data.

### Example
```dart
import 'package:openapi/api.dart';

final api_instance = CameraApi();
final cameraId = cameraId_example; // String | The unique identifier of the camera to remove.
final xAccessToken = xAccessToken_example; // String | Access Token

try {
    api_instance.aPIV3CameraControllerRemoveCameraCameraIdDelete(cameraId, xAccessToken);
} catch (e) {
    print('Exception when calling CameraApi->aPIV3CameraControllerRemoveCameraCameraIdDelete: $e\n');
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **cameraId** | **String**| The unique identifier of the camera to remove. | 
 **xAccessToken** | **String**| Access Token | 

### Return type

void (empty response body)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: Not defined

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

# **aPIV3CameraControllerRemoveVideoCameraIdFilenameDelete**
> aPIV3CameraControllerRemoveVideoCameraIdFilenameDelete(cameraId, filename)

Permanently deletes a specific recorded video file belonging to the given camera.

### Example
```dart
import 'package:openapi/api.dart';

final api_instance = CameraApi();
final cameraId = cameraId_example; // String | The unique identifier of the camera that owns the video.
final filename = filename_example; // String | The filename of the recorded video to delete.

try {
    api_instance.aPIV3CameraControllerRemoveVideoCameraIdFilenameDelete(cameraId, filename);
} catch (e) {
    print('Exception when calling CameraApi->aPIV3CameraControllerRemoveVideoCameraIdFilenameDelete: $e\n');
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **cameraId** | **String**| The unique identifier of the camera that owns the video. | 
 **filename** | **String**| The filename of the recorded video to delete. | 

### Return type

void (empty response body)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: Not defined

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

# **aPIV3CameraControllerRunONVIFCommandCameraIdPost**
> aPIV3CameraControllerRunONVIFCommandCameraIdPost(cameraId, oNVIFCommandDTO)

Executes an ONVIF command on the specified camera (e.g., PTZ control, preset recall).

### Example
```dart
import 'package:openapi/api.dart';

final api_instance = CameraApi();
final cameraId = cameraId_example; // String | The unique identifier of the target camera.
final oNVIFCommandDTO = ONVIFCommandDTO(); // ONVIFCommandDTO | The ONVIF command to execute, including its type and parameters.

try {
    api_instance.aPIV3CameraControllerRunONVIFCommandCameraIdPost(cameraId, oNVIFCommandDTO);
} catch (e) {
    print('Exception when calling CameraApi->aPIV3CameraControllerRunONVIFCommandCameraIdPost: $e\n');
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **cameraId** | **String**| The unique identifier of the target camera. | 
 **oNVIFCommandDTO** | [**ONVIFCommandDTO**](ONVIFCommandDTO.md)| The ONVIF command to execute, including its type and parameters. | [optional] 

### Return type

void (empty response body)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json, text/json, application/*+json
 - **Accept**: Not defined

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

# **aPIV3CameraControllerUpdateCameraPut**
> aPIV3CameraControllerUpdateCameraPut(xAccessToken, updateCameraDTO)

Updates the properties of an existing camera using the values provided in the request body.

### Example
```dart
import 'package:openapi/api.dart';

final api_instance = CameraApi();
final xAccessToken = xAccessToken_example; // String | Access Token
final updateCameraDTO = UpdateCameraDTO(); // UpdateCameraDTO | The DTO containing the updated camera properties.

try {
    api_instance.aPIV3CameraControllerUpdateCameraPut(xAccessToken, updateCameraDTO);
} catch (e) {
    print('Exception when calling CameraApi->aPIV3CameraControllerUpdateCameraPut: $e\n');
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **xAccessToken** | **String**| Access Token | 
 **updateCameraDTO** | [**UpdateCameraDTO**](UpdateCameraDTO.md)| The DTO containing the updated camera properties. | [optional] 

### Return type

void (empty response body)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json, text/json, application/*+json
 - **Accept**: Not defined

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

