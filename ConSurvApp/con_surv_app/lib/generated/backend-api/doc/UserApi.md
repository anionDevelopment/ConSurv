# openapi.api.UserApi

## Load the API package
```dart
import 'package:openapi/api.dart';
```

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**aPIV3UserControllerCreateUserPut**](UserApi.md#apiv3usercontrollercreateuserput) | **PUT** /API/v3/UserController/CreateUser | Creates a new user account with the given credentials and assigns the admin role to it.  Requires the caller to hold the admin role.
[**aPIV3UserControllerGetRolesPut**](UserApi.md#apiv3usercontrollergetrolesput) | **PUT** /API/v3/UserController/GetRoles | Returns the list of role names assigned to the currently authenticated user.
[**aPIV3UserControllerGetUserInformationGet**](UserApi.md#apiv3usercontrollergetuserinformationget) | **GET** /API/v3/UserController/GetUserInformation | Returns profile information (e.g., username, roles) for the currently authenticated user.
[**aPIV3UserControllerLoginPut**](UserApi.md#apiv3usercontrollerloginput) | **PUT** /API/v3/UserController/Login | Authenticates a user with the supplied credentials and returns a new access token on success.
[**aPIV3UserControllerLogoutPut**](UserApi.md#apiv3usercontrollerlogoutput) | **PUT** /API/v3/UserController/Logout | Invalidates the access token of the currently authenticated user, effectively logging them out.
[**aPIV3UserControllerTokenIsValidGet**](UserApi.md#apiv3usercontrollertokenisvalidget) | **GET** /API/v3/UserController/TokenIsValid | Checks whether the supplied access token is still valid (not expired and not revoked).


# **aPIV3UserControllerCreateUserPut**
> aPIV3UserControllerCreateUserPut(xAccessToken, user, password)

Creates a new user account with the given credentials and assigns the admin role to it.  Requires the caller to hold the admin role.

### Example
```dart
import 'package:openapi/api.dart';

final api_instance = UserApi();
final xAccessToken = xAccessToken_example; // String | Access Token
final user = user_example; // String | The desired username, provided via request header.
final password = password_example; // String | The plain-text password that will be hashed before storage, provided via request header.

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
 **user** | **String**| The desired username, provided via request header. | [optional] 
 **password** | **String**| The plain-text password that will be hashed before storage, provided via request header. | [optional] 

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

Returns the list of role names assigned to the currently authenticated user.

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

Returns profile information (e.g., username, roles) for the currently authenticated user.

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

Authenticates a user with the supplied credentials and returns a new access token on success.

### Example
```dart
import 'package:openapi/api.dart';

final api_instance = UserApi();
final xUser = xUser_example; // String | The username, provided via the `x-user` HTTP header.
final xPassword = xPassword_example; // String | The plain-text password, provided via the `x-password` HTTP header.

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
 **xUser** | **String**| The username, provided via the `x-user` HTTP header. | [optional] 
 **xPassword** | **String**| The plain-text password, provided via the `x-password` HTTP header. | [optional] 

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

Invalidates the access token of the currently authenticated user, effectively logging them out.

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

Checks whether the supplied access token is still valid (not expired and not revoked).

### Example
```dart
import 'package:openapi/api.dart';

final api_instance = UserApi();
final accessToken = accessToken_example; // String | The access token to validate, provided via request header.

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
 **accessToken** | **String**| The access token to validate, provided via request header. | [optional] 

### Return type

**bool**

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

