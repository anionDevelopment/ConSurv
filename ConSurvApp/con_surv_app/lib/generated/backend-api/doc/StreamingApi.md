# openapi.api.StreamingApi

## Load the API package
```dart
import 'package:openapi/api.dart';
```

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**aPIV3StreamingControllerStreamStreamIdFilenameGet**](StreamingApi.md#apiv3streamingcontrollerstreamstreamidfilenameget) | **GET** /API/v3/StreamingController/Stream/{streamId}/{filename} | Serves an HLS stream segment or playlist file for the specified stream.  Validates the filename format, resolves the file from the camera's fragment folder,  and returns it with the appropriate MIME type (`application/vnd.apple.mpegurl` for  `.m3u8` playlists, `video/MP2T` for `.ts` segments).


# **aPIV3StreamingControllerStreamStreamIdFilenameGet**
> aPIV3StreamingControllerStreamStreamIdFilenameGet(streamId, filename, xAccessToken)

Serves an HLS stream segment or playlist file for the specified stream.  Validates the filename format, resolves the file from the camera's fragment folder,  and returns it with the appropriate MIME type (`application/vnd.apple.mpegurl` for  `.m3u8` playlists, `video/MP2T` for `.ts` segments).

### Example
```dart
import 'package:openapi/api.dart';

final api_instance = StreamingApi();
final streamId = streamId_example; // String | The identifier of the stream (typically a camera ID).
final filename = filename_example; // String | The HLS segment or playlist filename to serve (must match `^[0-9A-Za-z_]+\\.[0-9A-Za-z]+$`).
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
 **streamId** | **String**| The identifier of the stream (typically a camera ID). | 
 **filename** | **String**| The HLS segment or playlist filename to serve (must match `^[0-9A-Za-z_]+\\.[0-9A-Za-z]+$`). | 
 **xAccessToken** | **String**| Access Token | 

### Return type

void (empty response body)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: Not defined

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

