# openapi.api.StreamingApi

## Load the API package
```dart
import 'package:openapi/api.dart';
```

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**aPIV3StreamingControllerStreamStreamIdFilenameGet**](StreamingApi.md#apiv3streamingcontrollerstreamstreamidfilenameget) | **GET** /API/v3/StreamingController/Stream/{streamId}/{filename} | 


# **aPIV3StreamingControllerStreamStreamIdFilenameGet**
> aPIV3StreamingControllerStreamStreamIdFilenameGet(streamId, filename, xAccessToken)



### Example
```dart
import 'package:openapi/api.dart';

final api_instance = StreamingApi();
final streamId = streamId_example; // String | 
final filename = filename_example; // String | 
final xAccessToken = xAccessToken_example; // String | Access Token

try {
    api_instance.aPIV3StreamingControllerStreamStreamIdFilenameGet(streamId, filename, xAccessToken);
} catch (e) {
    print('Exception when calling StreamingApi->aPIV3StreamingControllerStreamStreamIdFilenameGet: $e\n');
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

