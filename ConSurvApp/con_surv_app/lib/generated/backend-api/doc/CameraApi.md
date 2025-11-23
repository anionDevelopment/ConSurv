# openapi.api.CameraApi

## Load the API package
```dart
import 'package:openapi/api.dart';
```

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**aPIV2CameraControllerCameraCameraIdGet**](CameraApi.md#apiv2cameracontrollercameracameraidget) | **GET** /API/v2/CameraController/Camera/{cameraId} | 
[**aPIV2CameraControllerCamerasGet**](CameraApi.md#apiv2cameracontrollercamerasget) | **GET** /API/v2/CameraController/Cameras | 
[**aPIV2CameraControllerCreateCameraPost**](CameraApi.md#apiv2cameracontrollercreatecamerapost) | **POST** /API/v2/CameraController/CreateCamera | 
[**aPIV2CameraControllerDownloadVideoCameraIdFilenameGet**](CameraApi.md#apiv2cameracontrollerdownloadvideocameraidfilenameget) | **GET** /API/v2/CameraController/DownloadVideo/{cameraId}/{filename} | 
[**aPIV2CameraControllerGetPreviewCameraIdGet**](CameraApi.md#apiv2cameracontrollergetpreviewcameraidget) | **GET** /API/v2/CameraController/GetPreview/{cameraId} | 
[**aPIV2CameraControllerGetPreviewOfVideocameraIdFilenameGet**](CameraApi.md#apiv2cameracontrollergetpreviewofvideocameraidfilenameget) | **GET** /API/v2/CameraController/GetPreviewOfVideo{cameraId}/{filename} | 
[**aPIV2CameraControllerListVideosGet**](CameraApi.md#apiv2cameracontrollerlistvideosget) | **GET** /API/v2/CameraController/ListVideos | 
[**aPIV2CameraControllerRemoveCameraCameraIdDelete**](CameraApi.md#apiv2cameracontrollerremovecameracameraiddelete) | **DELETE** /API/v2/CameraController/RemoveCamera/{cameraId} | 
[**aPIV2CameraControllerRemoveVideoCameraIdFilenameDelete**](CameraApi.md#apiv2cameracontrollerremovevideocameraidfilenamedelete) | **DELETE** /API/v2/CameraController/RemoveVideo/{cameraId}/{filename} | 
[**aPIV2CameraControllerRunONVIFCommandCameraIdPost**](CameraApi.md#apiv2cameracontrollerrunonvifcommandcameraidpost) | **POST** /API/v2/CameraController/RunONVIFCommand/{cameraId} | 
[**aPIV2CameraControllerUpdateCameraPut**](CameraApi.md#apiv2cameracontrollerupdatecameraput) | **PUT** /API/v2/CameraController/UpdateCamera | 


# **aPIV2CameraControllerCameraCameraIdGet**
> CameraDTO aPIV2CameraControllerCameraCameraIdGet(cameraId, xAccessToken)



### Example
```dart
import 'package:openapi/api.dart';

final api_instance = CameraApi();
final cameraId = cameraId_example; // String | 
final xAccessToken = xAccessToken_example; // String | Access Token

try {
    final result = api_instance.aPIV2CameraControllerCameraCameraIdGet(cameraId, xAccessToken);
    print(result);
} catch (e) {
    print('Exception when calling CameraApi->aPIV2CameraControllerCameraCameraIdGet: $e\n');
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

# **aPIV2CameraControllerCamerasGet**
> List<CameraDTO> aPIV2CameraControllerCamerasGet(xAccessToken)



### Example
```dart
import 'package:openapi/api.dart';

final api_instance = CameraApi();
final xAccessToken = xAccessToken_example; // String | Access Token

try {
    final result = api_instance.aPIV2CameraControllerCamerasGet(xAccessToken);
    print(result);
} catch (e) {
    print('Exception when calling CameraApi->aPIV2CameraControllerCamerasGet: $e\n');
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

# **aPIV2CameraControllerCreateCameraPost**
> String aPIV2CameraControllerCreateCameraPost(xAccessToken)



### Example
```dart
import 'package:openapi/api.dart';

final api_instance = CameraApi();
final xAccessToken = xAccessToken_example; // String | Access Token

try {
    final result = api_instance.aPIV2CameraControllerCreateCameraPost(xAccessToken);
    print(result);
} catch (e) {
    print('Exception when calling CameraApi->aPIV2CameraControllerCreateCameraPost: $e\n');
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

# **aPIV2CameraControllerDownloadVideoCameraIdFilenameGet**
> aPIV2CameraControllerDownloadVideoCameraIdFilenameGet(cameraId, filename)



### Example
```dart
import 'package:openapi/api.dart';

final api_instance = CameraApi();
final cameraId = cameraId_example; // String | 
final filename = filename_example; // String | 

try {
    api_instance.aPIV2CameraControllerDownloadVideoCameraIdFilenameGet(cameraId, filename);
} catch (e) {
    print('Exception when calling CameraApi->aPIV2CameraControllerDownloadVideoCameraIdFilenameGet: $e\n');
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

# **aPIV2CameraControllerGetPreviewCameraIdGet**
> String aPIV2CameraControllerGetPreviewCameraIdGet(cameraId, xAccessToken)



### Example
```dart
import 'package:openapi/api.dart';

final api_instance = CameraApi();
final cameraId = cameraId_example; // String | 
final xAccessToken = xAccessToken_example; // String | Access Token

try {
    final result = api_instance.aPIV2CameraControllerGetPreviewCameraIdGet(cameraId, xAccessToken);
    print(result);
} catch (e) {
    print('Exception when calling CameraApi->aPIV2CameraControllerGetPreviewCameraIdGet: $e\n');
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

# **aPIV2CameraControllerGetPreviewOfVideocameraIdFilenameGet**
> aPIV2CameraControllerGetPreviewOfVideocameraIdFilenameGet(cameraId, filename)



### Example
```dart
import 'package:openapi/api.dart';

final api_instance = CameraApi();
final cameraId = cameraId_example; // String | 
final filename = filename_example; // String | 

try {
    api_instance.aPIV2CameraControllerGetPreviewOfVideocameraIdFilenameGet(cameraId, filename);
} catch (e) {
    print('Exception when calling CameraApi->aPIV2CameraControllerGetPreviewOfVideocameraIdFilenameGet: $e\n');
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

# **aPIV2CameraControllerListVideosGet**
> aPIV2CameraControllerListVideosGet()



### Example
```dart
import 'package:openapi/api.dart';

final api_instance = CameraApi();

try {
    api_instance.aPIV2CameraControllerListVideosGet();
} catch (e) {
    print('Exception when calling CameraApi->aPIV2CameraControllerListVideosGet: $e\n');
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

# **aPIV2CameraControllerRemoveCameraCameraIdDelete**
> aPIV2CameraControllerRemoveCameraCameraIdDelete(cameraId, xAccessToken)



### Example
```dart
import 'package:openapi/api.dart';

final api_instance = CameraApi();
final cameraId = cameraId_example; // String | 
final xAccessToken = xAccessToken_example; // String | Access Token

try {
    api_instance.aPIV2CameraControllerRemoveCameraCameraIdDelete(cameraId, xAccessToken);
} catch (e) {
    print('Exception when calling CameraApi->aPIV2CameraControllerRemoveCameraCameraIdDelete: $e\n');
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

# **aPIV2CameraControllerRemoveVideoCameraIdFilenameDelete**
> aPIV2CameraControllerRemoveVideoCameraIdFilenameDelete(cameraId, filename)



### Example
```dart
import 'package:openapi/api.dart';

final api_instance = CameraApi();
final cameraId = cameraId_example; // String | 
final filename = filename_example; // String | 

try {
    api_instance.aPIV2CameraControllerRemoveVideoCameraIdFilenameDelete(cameraId, filename);
} catch (e) {
    print('Exception when calling CameraApi->aPIV2CameraControllerRemoveVideoCameraIdFilenameDelete: $e\n');
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

# **aPIV2CameraControllerRunONVIFCommandCameraIdPost**
> aPIV2CameraControllerRunONVIFCommandCameraIdPost(cameraId, oNVIFCommandDTO)



### Example
```dart
import 'package:openapi/api.dart';

final api_instance = CameraApi();
final cameraId = cameraId_example; // String | 
final oNVIFCommandDTO = ONVIFCommandDTO(); // ONVIFCommandDTO | 

try {
    api_instance.aPIV2CameraControllerRunONVIFCommandCameraIdPost(cameraId, oNVIFCommandDTO);
} catch (e) {
    print('Exception when calling CameraApi->aPIV2CameraControllerRunONVIFCommandCameraIdPost: $e\n');
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

# **aPIV2CameraControllerUpdateCameraPut**
> aPIV2CameraControllerUpdateCameraPut(xAccessToken, updateCameraDTO)



### Example
```dart
import 'package:openapi/api.dart';

final api_instance = CameraApi();
final xAccessToken = xAccessToken_example; // String | Access Token
final updateCameraDTO = UpdateCameraDTO(); // UpdateCameraDTO | 

try {
    api_instance.aPIV2CameraControllerUpdateCameraPut(xAccessToken, updateCameraDTO);
} catch (e) {
    print('Exception when calling CameraApi->aPIV2CameraControllerUpdateCameraPut: $e\n');
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

