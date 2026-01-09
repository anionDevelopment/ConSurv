# openapi.api.UserApi

## Load the API package
```dart
import 'package:openapi/api.dart';
```

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**aPIV3UserControllerCreateUserPut**](UserApi.md#apiv3usercontrollercreateuserput) | **PUT** /API/v3/UserController/CreateUser | 
[**aPIV3UserControllerGetRolesPut**](UserApi.md#apiv3usercontrollergetrolesput) | **PUT** /API/v3/UserController/GetRoles | 
[**aPIV3UserControllerGetUserInformationGet**](UserApi.md#apiv3usercontrollergetuserinformationget) | **GET** /API/v3/UserController/GetUserInformation | 
[**aPIV3UserControllerLoginPut**](UserApi.md#apiv3usercontrollerloginput) | **PUT** /API/v3/UserController/Login | 
[**aPIV3UserControllerLogoutPut**](UserApi.md#apiv3usercontrollerlogoutput) | **PUT** /API/v3/UserController/Logout | 
[**aPIV3UserControllerTokenIsValidGet**](UserApi.md#apiv3usercontrollertokenisvalidget) | **GET** /API/v3/UserController/TokenIsValid | 


# **aPIV3UserControllerCreateUserPut**
> aPIV3UserControllerCreateUserPut(xAccessToken, user, password)



### Example
```dart
import 'package:openapi/api.dart';

final api_instance = UserApi();
final xAccessToken = xAccessToken_example; // String | Access Token
final user = user_example; // String | 
final password = password_example; // String | 

try {
    api_instance.aPIV3UserControllerCreateUserPut(xAccessToken, user, password);
} catch (e) {
    print('Exception when calling UserApi->aPIV3UserControllerCreateUserPut: $e\n');
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

# **aPIV3UserControllerGetRolesPut**
> List<String> aPIV3UserControllerGetRolesPut(xAccessToken)



### Example
```dart
import 'package:openapi/api.dart';

final api_instance = UserApi();
final xAccessToken = xAccessToken_example; // String | Access Token

try {
    final result = api_instance.aPIV3UserControllerGetRolesPut(xAccessToken);
    print(result);
} catch (e) {
    print('Exception when calling UserApi->aPIV3UserControllerGetRolesPut: $e\n');
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

# **aPIV3UserControllerGetUserInformationGet**
> UserInformationDTO aPIV3UserControllerGetUserInformationGet(xAccessToken)



### Example
```dart
import 'package:openapi/api.dart';

final api_instance = UserApi();
final xAccessToken = xAccessToken_example; // String | Access Token

try {
    final result = api_instance.aPIV3UserControllerGetUserInformationGet(xAccessToken);
    print(result);
} catch (e) {
    print('Exception when calling UserApi->aPIV3UserControllerGetUserInformationGet: $e\n');
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

# **aPIV3UserControllerLoginPut**
> AccessToken aPIV3UserControllerLoginPut(xUser, xPassword)



### Example
```dart
import 'package:openapi/api.dart';

final api_instance = UserApi();
final xUser = xUser_example; // String | 
final xPassword = xPassword_example; // String | 

try {
    final result = api_instance.aPIV3UserControllerLoginPut(xUser, xPassword);
    print(result);
} catch (e) {
    print('Exception when calling UserApi->aPIV3UserControllerLoginPut: $e\n');
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

# **aPIV3UserControllerLogoutPut**
> aPIV3UserControllerLogoutPut(xAccessToken)



### Example
```dart
import 'package:openapi/api.dart';

final api_instance = UserApi();
final xAccessToken = xAccessToken_example; // String | Access Token

try {
    api_instance.aPIV3UserControllerLogoutPut(xAccessToken);
} catch (e) {
    print('Exception when calling UserApi->aPIV3UserControllerLogoutPut: $e\n');
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

# **aPIV3UserControllerTokenIsValidGet**
> bool aPIV3UserControllerTokenIsValidGet(accessToken)



### Example
```dart
import 'package:openapi/api.dart';

final api_instance = UserApi();
final accessToken = accessToken_example; // String | 

try {
    final result = api_instance.aPIV3UserControllerTokenIsValidGet(accessToken);
    print(result);
} catch (e) {
    print('Exception when calling UserApi->aPIV3UserControllerTokenIsValidGet: $e\n');
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

