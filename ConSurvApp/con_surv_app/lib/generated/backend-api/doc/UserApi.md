# openapi.api.UserApi

## Load the API package
```dart
import 'package:openapi/api.dart';
```

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**aPIV2UserControllerCreateUserPut**](UserApi.md#apiv2usercontrollercreateuserput) | **PUT** /API/v2/UserController/CreateUser | 
[**aPIV2UserControllerGetRolesPut**](UserApi.md#apiv2usercontrollergetrolesput) | **PUT** /API/v2/UserController/GetRoles | 
[**aPIV2UserControllerGetUserInformationGet**](UserApi.md#apiv2usercontrollergetuserinformationget) | **GET** /API/v2/UserController/GetUserInformation | 
[**aPIV2UserControllerLoginPut**](UserApi.md#apiv2usercontrollerloginput) | **PUT** /API/v2/UserController/Login | 
[**aPIV2UserControllerLogoutPut**](UserApi.md#apiv2usercontrollerlogoutput) | **PUT** /API/v2/UserController/Logout | 
[**aPIV2UserControllerTokenIsValidGet**](UserApi.md#apiv2usercontrollertokenisvalidget) | **GET** /API/v2/UserController/TokenIsValid | 


# **aPIV2UserControllerCreateUserPut**
> aPIV2UserControllerCreateUserPut(xAccessToken, user, password)



### Example
```dart
import 'package:openapi/api.dart';

final api_instance = UserApi();
final xAccessToken = xAccessToken_example; // String | Access Token
final user = user_example; // String | 
final password = password_example; // String | 

try {
    api_instance.aPIV2UserControllerCreateUserPut(xAccessToken, user, password);
} catch (e) {
    print('Exception when calling UserApi->aPIV2UserControllerCreateUserPut: $e\n');
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **xAccessToken** | **String**| Access Token | 
 **user** | **String**|  | [optional] 
 **password** | **String**|  | [optional] 

### Return type

void (empty response body)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: Not defined

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

# **aPIV2UserControllerGetRolesPut**
> List<String> aPIV2UserControllerGetRolesPut(xAccessToken)



### Example
```dart
import 'package:openapi/api.dart';

final api_instance = UserApi();
final xAccessToken = xAccessToken_example; // String | Access Token

try {
    final result = api_instance.aPIV2UserControllerGetRolesPut(xAccessToken);
    print(result);
} catch (e) {
    print('Exception when calling UserApi->aPIV2UserControllerGetRolesPut: $e\n');
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **xAccessToken** | **String**| Access Token | 

### Return type

**List<String>**

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

# **aPIV2UserControllerGetUserInformationGet**
> UserInformationDTO aPIV2UserControllerGetUserInformationGet(xAccessToken)



### Example
```dart
import 'package:openapi/api.dart';

final api_instance = UserApi();
final xAccessToken = xAccessToken_example; // String | Access Token

try {
    final result = api_instance.aPIV2UserControllerGetUserInformationGet(xAccessToken);
    print(result);
} catch (e) {
    print('Exception when calling UserApi->aPIV2UserControllerGetUserInformationGet: $e\n');
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **xAccessToken** | **String**| Access Token | 

### Return type

[**UserInformationDTO**](UserInformationDTO.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

# **aPIV2UserControllerLoginPut**
> AccessToken aPIV2UserControllerLoginPut(xUser, xPassword)



### Example
```dart
import 'package:openapi/api.dart';

final api_instance = UserApi();
final xUser = xUser_example; // String | 
final xPassword = xPassword_example; // String | 

try {
    final result = api_instance.aPIV2UserControllerLoginPut(xUser, xPassword);
    print(result);
} catch (e) {
    print('Exception when calling UserApi->aPIV2UserControllerLoginPut: $e\n');
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **xUser** | **String**|  | [optional] 
 **xPassword** | **String**|  | [optional] 

### Return type

[**AccessToken**](AccessToken.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

# **aPIV2UserControllerLogoutPut**
> aPIV2UserControllerLogoutPut(xAccessToken)



### Example
```dart
import 'package:openapi/api.dart';

final api_instance = UserApi();
final xAccessToken = xAccessToken_example; // String | Access Token

try {
    api_instance.aPIV2UserControllerLogoutPut(xAccessToken);
} catch (e) {
    print('Exception when calling UserApi->aPIV2UserControllerLogoutPut: $e\n');
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **xAccessToken** | **String**| Access Token | 

### Return type

void (empty response body)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: Not defined

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

# **aPIV2UserControllerTokenIsValidGet**
> bool aPIV2UserControllerTokenIsValidGet(accessToken)



### Example
```dart
import 'package:openapi/api.dart';

final api_instance = UserApi();
final accessToken = accessToken_example; // String | 

try {
    final result = api_instance.aPIV2UserControllerTokenIsValidGet(accessToken);
    print(result);
} catch (e) {
    print('Exception when calling UserApi->aPIV2UserControllerTokenIsValidGet: $e\n');
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **accessToken** | **String**|  | [optional] 

### Return type

**bool**

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

