# openapi.api.CameraApi

## Load the API package
```dart
import 'package:openapi/api.dart';
```

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**aPIV1CameraControllerCameraCameraIdGet**](CameraApi.md#apiv1cameracontrollercameracameraidget) | **GET** /API/v1/CameraController/Camera/{cameraId} | 
[**aPIV1CameraControllerCamerasGet**](CameraApi.md#apiv1cameracontrollercamerasget) | **GET** /API/v1/CameraController/Cameras | 
[**aPIV1CameraControllerCreateCameraPost**](CameraApi.md#apiv1cameracontrollercreatecamerapost) | **POST** /API/v1/CameraController/CreateCamera | 
[**aPIV1CameraControllerDownloadVideoCameraIdFilenameGet**](CameraApi.md#apiv1cameracontrollerdownloadvideocameraidfilenameget) | **GET** /API/v1/CameraController/DownloadVideo/{cameraId}/{filename} | 
[**aPIV1CameraControllerGetPreviewCameraIdGet**](CameraApi.md#apiv1cameracontrollergetpreviewcameraidget) | **GET** /API/v1/CameraController/GetPreview/{cameraId} | 
[**aPIV1CameraControllerGetPreviewOfVideocameraIdFilenameGet**](CameraApi.md#apiv1cameracontrollergetpreviewofvideocameraidfilenameget) | **GET** /API/v1/CameraController/GetPreviewOfVideo{cameraId}/{filename} | 
[**aPIV1CameraControllerListVideosGet**](CameraApi.md#apiv1cameracontrollerlistvideosget) | **GET** /API/v1/CameraController/ListVideos | 
[**aPIV1CameraControllerRemoveCameraCameraIdDelete**](CameraApi.md#apiv1cameracontrollerremovecameracameraiddelete) | **DELETE** /API/v1/CameraController/RemoveCamera/{cameraId} | 
[**aPIV1CameraControllerRemoveVideoCameraIdFilenameDelete**](CameraApi.md#apiv1cameracontrollerremovevideocameraidfilenamedelete) | **DELETE** /API/v1/CameraController/RemoveVideo/{cameraId}/{filename} | 
[**aPIV1CameraControllerRunONVIFCommandCameraIdPost**](CameraApi.md#apiv1cameracontrollerrunonvifcommandcameraidpost) | **POST** /API/v1/CameraController/RunONVIFCommand/{cameraId} | 
[**aPIV1CameraControllerUpdateCameraPut**](CameraApi.md#apiv1cameracontrollerupdatecameraput) | **PUT** /API/v1/CameraController/UpdateCamera | 


# **aPIV1CameraControllerCameraCameraIdGet**
> CameraDTO aPIV1CameraControllerCameraCameraIdGet(cameraId, xAccessToken)



### Example
```dart
import 'package:openapi/api.dart';

final api_instance = CameraApi();
final cameraId = cameraId_example; // String | 
final xAccessToken = xAccessToken_example; // String | Access Token

try {
    final result = api_instance.aPIV1CameraControllerCameraCameraIdGet(cameraId, xAccessToken);
    print(result);
} catch (e) {
    print('Exception when calling CameraApi->aPIV1CameraControllerCameraCameraIdGet: $e\n');
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **cameraId** | **String**|  | 
 **xAccessToken** | **String**| Access Token | 

### Return type

[**CameraDTO**](CameraDTO.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

# **aPIV1CameraControllerCamerasGet**
> List<CameraDTO> aPIV1CameraControllerCamerasGet(xAccessToken)



### Example
```dart
import 'package:openapi/api.dart';

final api_instance = CameraApi();
final xAccessToken = xAccessToken_example; // String | Access Token

try {
    final result = api_instance.aPIV1CameraControllerCamerasGet(xAccessToken);
    print(result);
} catch (e) {
    print('Exception when calling CameraApi->aPIV1CameraControllerCamerasGet: $e\n');
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

# **aPIV1CameraControllerCreateCameraPost**
> String aPIV1CameraControllerCreateCameraPost(xAccessToken)



### Example
```dart
import 'package:openapi/api.dart';

final api_instance = CameraApi();
final xAccessToken = xAccessToken_example; // String | Access Token

try {
    final result = api_instance.aPIV1CameraControllerCreateCameraPost(xAccessToken);
    print(result);
} catch (e) {
    print('Exception when calling CameraApi->aPIV1CameraControllerCreateCameraPost: $e\n');
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

# **aPIV1CameraControllerDownloadVideoCameraIdFilenameGet**
> aPIV1CameraControllerDownloadVideoCameraIdFilenameGet(cameraId, filename)



### Example
```dart
import 'package:openapi/api.dart';

final api_instance = CameraApi();
final cameraId = cameraId_example; // String | 
final filename = filename_example; // String | 

try {
    api_instance.aPIV1CameraControllerDownloadVideoCameraIdFilenameGet(cameraId, filename);
} catch (e) {
    print('Exception when calling CameraApi->aPIV1CameraControllerDownloadVideoCameraIdFilenameGet: $e\n');
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **cameraId** | **String**|  | 
 **filename** | **String**|  | 

### Return type

void (empty response body)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: Not defined

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

# **aPIV1CameraControllerGetPreviewCameraIdGet**
> String aPIV1CameraControllerGetPreviewCameraIdGet(cameraId, xAccessToken)



### Example
```dart
import 'package:openapi/api.dart';

final api_instance = CameraApi();
final cameraId = cameraId_example; // String | 
final xAccessToken = xAccessToken_example; // String | Access Token

try {
    final result = api_instance.aPIV1CameraControllerGetPreviewCameraIdGet(cameraId, xAccessToken);
    print(result);
} catch (e) {
    print('Exception when calling CameraApi->aPIV1CameraControllerGetPreviewCameraIdGet: $e\n');
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **cameraId** | **String**|  | 
 **xAccessToken** | **String**| Access Token | 

### Return type

**String**

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

# **aPIV1CameraControllerGetPreviewOfVideocameraIdFilenameGet**
> aPIV1CameraControllerGetPreviewOfVideocameraIdFilenameGet(cameraId, filename)



### Example
```dart
import 'package:openapi/api.dart';

final api_instance = CameraApi();
final cameraId = cameraId_example; // String | 
final filename = filename_example; // String | 

try {
    api_instance.aPIV1CameraControllerGetPreviewOfVideocameraIdFilenameGet(cameraId, filename);
} catch (e) {
    print('Exception when calling CameraApi->aPIV1CameraControllerGetPreviewOfVideocameraIdFilenameGet: $e\n');
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **cameraId** | **String**|  | 
 **filename** | **String**|  | 

### Return type

void (empty response body)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: Not defined

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

# **aPIV1CameraControllerListVideosGet**
> aPIV1CameraControllerListVideosGet()



### Example
```dart
import 'package:openapi/api.dart';

final api_instance = CameraApi();

try {
    api_instance.aPIV1CameraControllerListVideosGet();
} catch (e) {
    print('Exception when calling CameraApi->aPIV1CameraControllerListVideosGet: $e\n');
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

# **aPIV1CameraControllerRemoveCameraCameraIdDelete**
> aPIV1CameraControllerRemoveCameraCameraIdDelete(cameraId, xAccessToken)



### Example
```dart
import 'package:openapi/api.dart';

final api_instance = CameraApi();
final cameraId = cameraId_example; // String | 
final xAccessToken = xAccessToken_example; // String | Access Token

try {
    api_instance.aPIV1CameraControllerRemoveCameraCameraIdDelete(cameraId, xAccessToken);
} catch (e) {
    print('Exception when calling CameraApi->aPIV1CameraControllerRemoveCameraCameraIdDelete: $e\n');
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **cameraId** | **String**|  | 
 **xAccessToken** | **String**| Access Token | 

### Return type

void (empty response body)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: Not defined

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

# **aPIV1CameraControllerRemoveVideoCameraIdFilenameDelete**
> aPIV1CameraControllerRemoveVideoCameraIdFilenameDelete(cameraId, filename)



### Example
```dart
import 'package:openapi/api.dart';

final api_instance = CameraApi();
final cameraId = cameraId_example; // String | 
final filename = filename_example; // String | 

try {
    api_instance.aPIV1CameraControllerRemoveVideoCameraIdFilenameDelete(cameraId, filename);
} catch (e) {
    print('Exception when calling CameraApi->aPIV1CameraControllerRemoveVideoCameraIdFilenameDelete: $e\n');
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **cameraId** | **String**|  | 
 **filename** | **String**|  | 

### Return type

void (empty response body)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: Not defined

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

# **aPIV1CameraControllerRunONVIFCommandCameraIdPost**
> aPIV1CameraControllerRunONVIFCommandCameraIdPost(cameraId, oNVIFCommandDTO)



### Example
```dart
import 'package:openapi/api.dart';

final api_instance = CameraApi();
final cameraId = cameraId_example; // String | 
final oNVIFCommandDTO = ONVIFCommandDTO(); // ONVIFCommandDTO | 

try {
    api_instance.aPIV1CameraControllerRunONVIFCommandCameraIdPost(cameraId, oNVIFCommandDTO);
} catch (e) {
    print('Exception when calling CameraApi->aPIV1CameraControllerRunONVIFCommandCameraIdPost: $e\n');
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **cameraId** | **String**|  | 
 **oNVIFCommandDTO** | [**ONVIFCommandDTO**](ONVIFCommandDTO.md)|  | [optional] 

### Return type

void (empty response body)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json, text/json, application/*+json
 - **Accept**: Not defined

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

# **aPIV1CameraControllerUpdateCameraPut**
> aPIV1CameraControllerUpdateCameraPut(xAccessToken, updateCameraDTO)



### Example
```dart
import 'package:openapi/api.dart';

final api_instance = CameraApi();
final xAccessToken = xAccessToken_example; // String | Access Token
final updateCameraDTO = UpdateCameraDTO(); // UpdateCameraDTO | 

try {
    api_instance.aPIV1CameraControllerUpdateCameraPut(xAccessToken, updateCameraDTO);
} catch (e) {
    print('Exception when calling CameraApi->aPIV1CameraControllerUpdateCameraPut: $e\n');
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **xAccessToken** | **String**| Access Token | 
 **updateCameraDTO** | [**UpdateCameraDTO**](UpdateCameraDTO.md)|  | [optional] 

### Return type

void (empty response body)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json, text/json, application/*+json
 - **Accept**: Not defined

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

