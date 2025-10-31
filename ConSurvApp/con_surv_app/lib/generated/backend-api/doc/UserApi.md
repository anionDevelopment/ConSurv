# openapi.api.UserApi

## Load the API package
```dart
import 'package:openapi/api.dart';
```

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**aPIV1UserControllerCreateUserPut**](UserApi.md#apiv1usercontrollercreateuserput) | **PUT** /API/v1/UserController/CreateUser | 
[**aPIV1UserControllerGetRolesPut**](UserApi.md#apiv1usercontrollergetrolesput) | **PUT** /API/v1/UserController/GetRoles | 
[**aPIV1UserControllerGetUserInformationGet**](UserApi.md#apiv1usercontrollergetuserinformationget) | **GET** /API/v1/UserController/GetUserInformation | 
[**aPIV1UserControllerLoginPut**](UserApi.md#apiv1usercontrollerloginput) | **PUT** /API/v1/UserController/Login | 
[**aPIV1UserControllerLogoutPut**](UserApi.md#apiv1usercontrollerlogoutput) | **PUT** /API/v1/UserController/Logout | 
[**aPIV1UserControllerTokenIsValidGet**](UserApi.md#apiv1usercontrollertokenisvalidget) | **GET** /API/v1/UserController/TokenIsValid | 


# **aPIV1UserControllerCreateUserPut**
> aPIV1UserControllerCreateUserPut(xAccessToken, user, password)



### Example
```dart
import 'package:openapi/api.dart';

final api_instance = UserApi();
final xAccessToken = xAccessToken_example; // String | Access Token
final user = user_example; // String | 
final password = password_example; // String | 

try {
    api_instance.aPIV1UserControllerCreateUserPut(xAccessToken, user, password);
} catch (e) {
    print('Exception when calling UserApi->aPIV1UserControllerCreateUserPut: $e\n');
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

# **aPIV1UserControllerGetRolesPut**
> List<String> aPIV1UserControllerGetRolesPut(xAccessToken)



### Example
```dart
import 'package:openapi/api.dart';

final api_instance = UserApi();
final xAccessToken = xAccessToken_example; // String | Access Token

try {
    final result = api_instance.aPIV1UserControllerGetRolesPut(xAccessToken);
    print(result);
} catch (e) {
    print('Exception when calling UserApi->aPIV1UserControllerGetRolesPut: $e\n');
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

# **aPIV1UserControllerGetUserInformationGet**
> UserInformationDTO aPIV1UserControllerGetUserInformationGet(xAccessToken)



### Example
```dart
import 'package:openapi/api.dart';

final api_instance = UserApi();
final xAccessToken = xAccessToken_example; // String | Access Token

try {
    final result = api_instance.aPIV1UserControllerGetUserInformationGet(xAccessToken);
    print(result);
} catch (e) {
    print('Exception when calling UserApi->aPIV1UserControllerGetUserInformationGet: $e\n');
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

# **aPIV1UserControllerLoginPut**
> AccessToken aPIV1UserControllerLoginPut(xUser, xPassword)



### Example
```dart
import 'package:openapi/api.dart';

final api_instance = UserApi();
final xUser = xUser_example; // String | 
final xPassword = xPassword_example; // String | 

try {
    final result = api_instance.aPIV1UserControllerLoginPut(xUser, xPassword);
    print(result);
} catch (e) {
    print('Exception when calling UserApi->aPIV1UserControllerLoginPut: $e\n');
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

# **aPIV1UserControllerLogoutPut**
> aPIV1UserControllerLogoutPut(xAccessToken)



### Example
```dart
import 'package:openapi/api.dart';

final api_instance = UserApi();
final xAccessToken = xAccessToken_example; // String | Access Token

try {
    api_instance.aPIV1UserControllerLogoutPut(xAccessToken);
} catch (e) {
    print('Exception when calling UserApi->aPIV1UserControllerLogoutPut: $e\n');
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

# **aPIV1UserControllerTokenIsValidGet**
> bool aPIV1UserControllerTokenIsValidGet(accessToken)



### Example
```dart
import 'package:openapi/api.dart';

final api_instance = UserApi();
final accessToken = accessToken_example; // String | 

try {
    final result = api_instance.aPIV1UserControllerTokenIsValidGet(accessToken);
    print(result);
} catch (e) {
    print('Exception when calling UserApi->aPIV1UserControllerTokenIsValidGet: $e\n');
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

