# openapi.api.StreamingApi

## Load the API package
```dart
import 'package:openapi/api.dart';
```

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**aPIV1StreamingControllerStreamStreamIdFilenameGet**](StreamingApi.md#apiv1streamingcontrollerstreamstreamidfilenameget) | **GET** /API/v1/StreamingController/Stream/{streamId}/{filename} | 


# **aPIV1StreamingControllerStreamStreamIdFilenameGet**
> aPIV1StreamingControllerStreamStreamIdFilenameGet(streamId, filename)



### Example
```dart
import 'package:openapi/api.dart';

final api_instance = StreamingApi();
final streamId = streamId_example; // String | 
final filename = filename_example; // String | 

try {
    api_instance.aPIV1StreamingControllerStreamStreamIdFilenameGet(streamId, filename);
} catch (e) {
    print('Exception when calling StreamingApi->aPIV1StreamingControllerStreamStreamIdFilenameGet: $e\n');
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **streamId** | **String**|  | 
 **filename** | **String**|  | 

### Return type

void (empty response body)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: Not defined

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

