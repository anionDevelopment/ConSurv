# openapi.api.CameraApi

## Load the API package
```dart
import 'package:openapi/api.dart';
```

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**aPIV3CameraControllerCameraCameraIdGet**](CameraApi.md#apiv3cameracontrollercameracameraidget) | **GET** /API/v3/CameraController/Camera/{cameraId} | 
[**aPIV3CameraControllerCamerasGet**](CameraApi.md#apiv3cameracontrollercamerasget) | **GET** /API/v3/CameraController/Cameras | 
[**aPIV3CameraControllerCreateCameraPost**](CameraApi.md#apiv3cameracontrollercreatecamerapost) | **POST** /API/v3/CameraController/CreateCamera | 
[**aPIV3CameraControllerDownloadVideoCameraIdFilenameGet**](CameraApi.md#apiv3cameracontrollerdownloadvideocameraidfilenameget) | **GET** /API/v3/CameraController/DownloadVideo/{cameraId}/{filename} | 
[**aPIV3CameraControllerGetPreviewCameraIdGet**](CameraApi.md#apiv3cameracontrollergetpreviewcameraidget) | **GET** /API/v3/CameraController/GetPreview/{cameraId} | 
[**aPIV3CameraControllerGetPreviewOfVideocameraIdFilenameGet**](CameraApi.md#apiv3cameracontrollergetpreviewofvideocameraidfilenameget) | **GET** /API/v3/CameraController/GetPreviewOfVideo{cameraId}/{filename} | 
[**aPIV3CameraControllerListVideosGet**](CameraApi.md#apiv3cameracontrollerlistvideosget) | **GET** /API/v3/CameraController/ListVideos | 
[**aPIV3CameraControllerRemoveCameraCameraIdDelete**](CameraApi.md#apiv3cameracontrollerremovecameracameraiddelete) | **DELETE** /API/v3/CameraController/RemoveCamera/{cameraId} | 
[**aPIV3CameraControllerRemoveVideoCameraIdFilenameDelete**](CameraApi.md#apiv3cameracontrollerremovevideocameraidfilenamedelete) | **DELETE** /API/v3/CameraController/RemoveVideo/{cameraId}/{filename} | 
[**aPIV3CameraControllerRunONVIFCommandCameraIdPost**](CameraApi.md#apiv3cameracontrollerrunonvifcommandcameraidpost) | **POST** /API/v3/CameraController/RunONVIFCommand/{cameraId} | 
[**aPIV3CameraControllerUpdateCameraPut**](CameraApi.md#apiv3cameracontrollerupdatecameraput) | **PUT** /API/v3/CameraController/UpdateCamera | 


# **aPIV3CameraControllerCameraCameraIdGet**
> CameraDTO aPIV3CameraControllerCameraCameraIdGet(cameraId, xAccessToken)



### Example
```dart
import 'package:openapi/api.dart';

final api_instance = CameraApi();
final cameraId = cameraId_example; // String | 
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

# **aPIV3CameraControllerCamerasGet**
> List<CameraDTO> aPIV3CameraControllerCamerasGet(xAccessToken)



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



### Example
```dart
import 'package:openapi/api.dart';

final api_instance = CameraApi();
final cameraId = cameraId_example; // String | 
final filename = filename_example; // String | 

try {
    api_instance.aPIV3CameraControllerDownloadVideoCameraIdFilenameGet(cameraId, filename);
} catch (e) {
    print('Exception when calling CameraApi->aPIV3CameraControllerDownloadVideoCameraIdFilenameGet: $e\n');
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

# **aPIV3CameraControllerGetPreviewCameraIdGet**
> String aPIV3CameraControllerGetPreviewCameraIdGet(cameraId, xAccessToken)



### Example
```dart
import 'package:openapi/api.dart';

final api_instance = CameraApi();
final cameraId = cameraId_example; // String | 
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

# **aPIV3CameraControllerGetPreviewOfVideocameraIdFilenameGet**
> aPIV3CameraControllerGetPreviewOfVideocameraIdFilenameGet(cameraId, filename)



### Example
```dart
import 'package:openapi/api.dart';

final api_instance = CameraApi();
final cameraId = cameraId_example; // String | 
final filename = filename_example; // String | 

try {
    api_instance.aPIV3CameraControllerGetPreviewOfVideocameraIdFilenameGet(cameraId, filename);
} catch (e) {
    print('Exception when calling CameraApi->aPIV3CameraControllerGetPreviewOfVideocameraIdFilenameGet: $e\n');
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

# **aPIV3CameraControllerListVideosGet**
> aPIV3CameraControllerListVideosGet()



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



### Example
```dart
import 'package:openapi/api.dart';

final api_instance = CameraApi();
final cameraId = cameraId_example; // String | 
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

# **aPIV3CameraControllerRemoveVideoCameraIdFilenameDelete**
> aPIV3CameraControllerRemoveVideoCameraIdFilenameDelete(cameraId, filename)



### Example
```dart
import 'package:openapi/api.dart';

final api_instance = CameraApi();
final cameraId = cameraId_example; // String | 
final filename = filename_example; // String | 

try {
    api_instance.aPIV3CameraControllerRemoveVideoCameraIdFilenameDelete(cameraId, filename);
} catch (e) {
    print('Exception when calling CameraApi->aPIV3CameraControllerRemoveVideoCameraIdFilenameDelete: $e\n');
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

# **aPIV3CameraControllerRunONVIFCommandCameraIdPost**
> aPIV3CameraControllerRunONVIFCommandCameraIdPost(cameraId, oNVIFCommandDTO)



### Example
```dart
import 'package:openapi/api.dart';

final api_instance = CameraApi();
final cameraId = cameraId_example; // String | 
final oNVIFCommandDTO = ONVIFCommandDTO(); // ONVIFCommandDTO | 

try {
    api_instance.aPIV3CameraControllerRunONVIFCommandCameraIdPost(cameraId, oNVIFCommandDTO);
} catch (e) {
    print('Exception when calling CameraApi->aPIV3CameraControllerRunONVIFCommandCameraIdPost: $e\n');
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

# **aPIV3CameraControllerUpdateCameraPut**
> aPIV3CameraControllerUpdateCameraPut(xAccessToken, updateCameraDTO)



### Example
```dart
import 'package:openapi/api.dart';

final api_instance = CameraApi();
final xAccessToken = xAccessToken_example; // String | Access Token
final updateCameraDTO = UpdateCameraDTO(); // UpdateCameraDTO | 

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
 **updateCameraDTO** | [**UpdateCameraDTO**](UpdateCameraDTO.md)|  | [optional] 

### Return type

void (empty response body)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json, text/json, application/*+json
 - **Accept**: Not defined

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

