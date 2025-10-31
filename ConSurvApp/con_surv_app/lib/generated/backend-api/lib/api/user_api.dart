//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//
// @dart=2.18

// ignore_for_file: unused_element, unused_import
// ignore_for_file: always_put_required_named_parameters_first
// ignore_for_file: constant_identifier_names
// ignore_for_file: lines_longer_than_80_chars

part of openapi.api;


class UserApi {
  UserApi([ApiClient? apiClient]) : apiClient = apiClient ?? defaultApiClient;

  final ApiClient apiClient;

  /// Performs an HTTP 'PUT /API/v1/UserController/CreateUser' operation and returns the [Response].
  /// Parameters:
  ///
  /// * [String] xAccessToken (required):
  ///   Access Token
  ///
  /// * [String] user:
  ///
  /// * [String] password:
  Future<Response> aPIV1UserControllerCreateUserPutWithHttpInfo(String xAccessToken, { String? user, String? password, }) async {
    // ignore: prefer_const_declarations
    final path = r'/API/v1/UserController/CreateUser';

    // ignore: prefer_final_locals
    Object? postBody;

    final queryParams = <QueryParam>[];
    final headerParams = <String, String>{};
    final formParams = <String, String>{};

    if (user != null) {
      headerParams[r'user'] = parameterToString(user);
    }
    if (password != null) {
      headerParams[r'password'] = parameterToString(password);
    }
    headerParams[r'X-AccessToken'] = parameterToString(xAccessToken);

    const contentTypes = <String>[];


    return apiClient.invokeAPI(
      path,
      'PUT',
      queryParams,
      postBody,
      headerParams,
      formParams,
      contentTypes.isEmpty ? null : contentTypes.first,
    );
  }

  /// Parameters:
  ///
  /// * [String] xAccessToken (required):
  ///   Access Token
  ///
  /// * [String] user:
  ///
  /// * [String] password:
  Future<void> aPIV1UserControllerCreateUserPut(String xAccessToken, { String? user, String? password, }) async {
    final response = await aPIV1UserControllerCreateUserPutWithHttpInfo(xAccessToken,  user: user, password: password, );
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
  }

  /// Performs an HTTP 'PUT /API/v1/UserController/GetRoles' operation and returns the [Response].
  /// Parameters:
  ///
  /// * [String] xAccessToken (required):
  ///   Access Token
  Future<Response> aPIV1UserControllerGetRolesPutWithHttpInfo(String xAccessToken,) async {
    // ignore: prefer_const_declarations
    final path = r'/API/v1/UserController/GetRoles';

    // ignore: prefer_final_locals
    Object? postBody;

    final queryParams = <QueryParam>[];
    final headerParams = <String, String>{};
    final formParams = <String, String>{};

    headerParams[r'X-AccessToken'] = parameterToString(xAccessToken);

    const contentTypes = <String>[];


    return apiClient.invokeAPI(
      path,
      'PUT',
      queryParams,
      postBody,
      headerParams,
      formParams,
      contentTypes.isEmpty ? null : contentTypes.first,
    );
  }

  /// Parameters:
  ///
  /// * [String] xAccessToken (required):
  ///   Access Token
  Future<List<String>?> aPIV1UserControllerGetRolesPut(String xAccessToken,) async {
    final response = await aPIV1UserControllerGetRolesPutWithHttpInfo(xAccessToken,);
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
    // When a remote server returns no body with a status of 204, we shall not decode it.
    // At the time of writing this, `dart:convert` will throw an "Unexpected end of input"
    // FormatException when trying to decode an empty string.
    if (response.body.isNotEmpty && response.statusCode != HttpStatus.noContent) {
      final responseBody = await _decodeBodyBytes(response);
      return (await apiClient.deserializeAsync(responseBody, 'List<String>') as List)
        .cast<String>()
        .toList(growable: false);

    }
    return null;
  }

  /// Performs an HTTP 'GET /API/v1/UserController/GetUserInformation' operation and returns the [Response].
  /// Parameters:
  ///
  /// * [String] xAccessToken (required):
  ///   Access Token
  Future<Response> aPIV1UserControllerGetUserInformationGetWithHttpInfo(String xAccessToken,) async {
    // ignore: prefer_const_declarations
    final path = r'/API/v1/UserController/GetUserInformation';

    // ignore: prefer_final_locals
    Object? postBody;

    final queryParams = <QueryParam>[];
    final headerParams = <String, String>{};
    final formParams = <String, String>{};

    headerParams[r'X-AccessToken'] = parameterToString(xAccessToken);

    const contentTypes = <String>[];


    return apiClient.invokeAPI(
      path,
      'GET',
      queryParams,
      postBody,
      headerParams,
      formParams,
      contentTypes.isEmpty ? null : contentTypes.first,
    );
  }

  /// Parameters:
  ///
  /// * [String] xAccessToken (required):
  ///   Access Token
  Future<UserInformationDTO?> aPIV1UserControllerGetUserInformationGet(String xAccessToken,) async {
    final response = await aPIV1UserControllerGetUserInformationGetWithHttpInfo(xAccessToken,);
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
    // When a remote server returns no body with a status of 204, we shall not decode it.
    // At the time of writing this, `dart:convert` will throw an "Unexpected end of input"
    // FormatException when trying to decode an empty string.
    if (response.body.isNotEmpty && response.statusCode != HttpStatus.noContent) {
      return await apiClient.deserializeAsync(await _decodeBodyBytes(response), 'UserInformationDTO',) as UserInformationDTO;
    
    }
    return null;
  }

  /// Performs an HTTP 'PUT /API/v1/UserController/Login' operation and returns the [Response].
  /// Parameters:
  ///
  /// * [String] xUser:
  ///
  /// * [String] xPassword:
  Future<Response> aPIV1UserControllerLoginPutWithHttpInfo({ String? xUser, String? xPassword, }) async {
    // ignore: prefer_const_declarations
    final path = r'/API/v1/UserController/Login';

    // ignore: prefer_final_locals
    Object? postBody;

    final queryParams = <QueryParam>[];
    final headerParams = <String, String>{};
    final formParams = <String, String>{};

    if (xUser != null) {
      headerParams[r'x-user'] = parameterToString(xUser);
    }
    if (xPassword != null) {
      headerParams[r'x-password'] = parameterToString(xPassword);
    }

    const contentTypes = <String>[];


    return apiClient.invokeAPI(
      path,
      'PUT',
      queryParams,
      postBody,
      headerParams,
      formParams,
      contentTypes.isEmpty ? null : contentTypes.first,
    );
  }

  /// Parameters:
  ///
  /// * [String] xUser:
  ///
  /// * [String] xPassword:
  Future<AccessToken?> aPIV1UserControllerLoginPut({ String? xUser, String? xPassword, }) async {
    final response = await aPIV1UserControllerLoginPutWithHttpInfo( xUser: xUser, xPassword: xPassword, );
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
    // When a remote server returns no body with a status of 204, we shall not decode it.
    // At the time of writing this, `dart:convert` will throw an "Unexpected end of input"
    // FormatException when trying to decode an empty string.
    if (response.body.isNotEmpty && response.statusCode != HttpStatus.noContent) {
      return await apiClient.deserializeAsync(await _decodeBodyBytes(response), 'AccessToken',) as AccessToken;
    
    }
    return null;
  }

  /// Performs an HTTP 'PUT /API/v1/UserController/Logout' operation and returns the [Response].
  /// Parameters:
  ///
  /// * [String] xAccessToken (required):
  ///   Access Token
  Future<Response> aPIV1UserControllerLogoutPutWithHttpInfo(String xAccessToken,) async {
    // ignore: prefer_const_declarations
    final path = r'/API/v1/UserController/Logout';

    // ignore: prefer_final_locals
    Object? postBody;

    final queryParams = <QueryParam>[];
    final headerParams = <String, String>{};
    final formParams = <String, String>{};

    headerParams[r'X-AccessToken'] = parameterToString(xAccessToken);

    const contentTypes = <String>[];


    return apiClient.invokeAPI(
      path,
      'PUT',
      queryParams,
      postBody,
      headerParams,
      formParams,
      contentTypes.isEmpty ? null : contentTypes.first,
    );
  }

  /// Parameters:
  ///
  /// * [String] xAccessToken (required):
  ///   Access Token
  Future<void> aPIV1UserControllerLogoutPut(String xAccessToken,) async {
    final response = await aPIV1UserControllerLogoutPutWithHttpInfo(xAccessToken,);
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
  }

  /// Performs an HTTP 'GET /API/v1/UserController/TokenIsValid' operation and returns the [Response].
  /// Parameters:
  ///
  /// * [String] accessToken:
  Future<Response> aPIV1UserControllerTokenIsValidGetWithHttpInfo({ String? accessToken, }) async {
    // ignore: prefer_const_declarations
    final path = r'/API/v1/UserController/TokenIsValid';

    // ignore: prefer_final_locals
    Object? postBody;

    final queryParams = <QueryParam>[];
    final headerParams = <String, String>{};
    final formParams = <String, String>{};

    if (accessToken != null) {
      headerParams[r'accessToken'] = parameterToString(accessToken);
    }

    const contentTypes = <String>[];


    return apiClient.invokeAPI(
      path,
      'GET',
      queryParams,
      postBody,
      headerParams,
      formParams,
      contentTypes.isEmpty ? null : contentTypes.first,
    );
  }

  /// Parameters:
  ///
  /// * [String] accessToken:
  Future<bool?> aPIV1UserControllerTokenIsValidGet({ String? accessToken, }) async {
    final response = await aPIV1UserControllerTokenIsValidGetWithHttpInfo( accessToken: accessToken, );
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
    // When a remote server returns no body with a status of 204, we shall not decode it.
    // At the time of writing this, `dart:convert` will throw an "Unexpected end of input"
    // FormatException when trying to decode an empty string.
    if (response.body.isNotEmpty && response.statusCode != HttpStatus.noContent) {
      return await apiClient.deserializeAsync(await _decodeBodyBytes(response), 'bool',) as bool;
    
    }
    return null;
  }
}
