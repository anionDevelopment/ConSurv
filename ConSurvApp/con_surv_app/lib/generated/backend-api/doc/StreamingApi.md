# openapi.api.StreamingApi

## Load the API package
```dart
import 'package:openapi/api.dart';
```

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**aPIV2StreamingControllerStreamStreamIdFilenameGet**](StreamingApi.md#apiv2streamingcontrollerstreamstreamidfilenameget) | **GET** /API/v2/StreamingController/Stream/{streamId}/{filename} | 


# **aPIV2StreamingControllerStreamStreamIdFilenameGet**
> aPIV2StreamingControllerStreamStreamIdFilenameGet(streamId, filename, xAccessToken)



### Example
```dart
import 'package:openapi/api.dart';

final api_instance = StreamingApi();
final streamId = streamId_example; // String | 
final filename = filename_example; // String | 
final xAccessToken = xAccessToken_example; // String | Access Token

try {
    api_instance.aPIV2StreamingControllerStreamStreamIdFilenameGet(streamId, filename, xAccessToken);
} catch (e) {
    print('Exception when calling StreamingApi->aPIV2StreamingControllerStreamStreamIdFilenameGet: $e\n');
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **streamId** | **String**|  | 
 **filename** | **String**|  | 
 **xAccessToken** | **String**| Access Token | 

### Return type

void (empty response body)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: Not defined

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

