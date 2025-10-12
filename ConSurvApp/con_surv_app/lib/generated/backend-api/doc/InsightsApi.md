# openapi.api.InsightsApi

## Load the API package
```dart
import 'package:openapi/api.dart';
```

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**aPIV1InsightsControllerGetMediaMTXConfigurationCameraIdGet**](InsightsApi.md#apiv1insightscontrollergetmediamtxconfigurationcameraidget) | **GET** /API/v1/InsightsController/GetMediaMTXConfiguration/cameraId | 
[**aPIV1InsightsControllerGetRunningProcessesGet**](InsightsApi.md#apiv1insightscontrollergetrunningprocessesget) | **GET** /API/v1/InsightsController/GetRunningProcesses | 


# **aPIV1InsightsControllerGetMediaMTXConfigurationCameraIdGet**
> CameraDTO aPIV1InsightsControllerGetMediaMTXConfigurationCameraIdGet(xAccessToken, cameraId)



### Example
```dart
import 'package:openapi/api.dart';

final api_instance = InsightsApi();
final xAccessToken = xAccessToken_example; // String | Access Token
final cameraId = cameraId_example; // String | 

try {
    final result = api_instance.aPIV1InsightsControllerGetMediaMTXConfigurationCameraIdGet(xAccessToken, cameraId);
    print(result);
} catch (e) {
    print('Exception when calling InsightsApi->aPIV1InsightsControllerGetMediaMTXConfigurationCameraIdGet: $e\n');
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **xAccessToken** | **String**| Access Token | 
 **cameraId** | **String**|  | [optional] 

### Return type

[**CameraDTO**](CameraDTO.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

# **aPIV1InsightsControllerGetRunningProcessesGet**
> CameraDTO aPIV1InsightsControllerGetRunningProcessesGet(xAccessToken)



### Example
```dart
import 'package:openapi/api.dart';

final api_instance = InsightsApi();
final xAccessToken = xAccessToken_example; // String | Access Token

try {
    final result = api_instance.aPIV1InsightsControllerGetRunningProcessesGet(xAccessToken);
    print(result);
} catch (e) {
    print('Exception when calling InsightsApi->aPIV1InsightsControllerGetRunningProcessesGet: $e\n');
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **xAccessToken** | **String**| Access Token | 

### Return type

[**CameraDTO**](CameraDTO.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

