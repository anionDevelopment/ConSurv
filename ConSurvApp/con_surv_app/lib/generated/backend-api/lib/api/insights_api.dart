//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//
// @dart=2.18

// ignore_for_file: unused_element, unused_import
// ignore_for_file: always_put_required_named_parameters_first
// ignore_for_file: constant_identifier_names
// ignore_for_file: lines_longer_than_80_chars

part of openapi.api;


class InsightsApi {
  InsightsApi([ApiClient? apiClient]) : apiClient = apiClient ?? defaultApiClient;

  final ApiClient apiClient;

  /// Performs an HTTP 'GET /API/v1/InsightsController/GetMediaMTXConfiguration/cameraId' operation and returns the [Response].
  /// Parameters:
  ///
  /// * [String] xAccessToken (required):
  ///   Access Token
  ///
  /// * [String] cameraId:
  Future<Response> aPIV1InsightsControllerGetMediaMTXConfigurationCameraIdGetWithHttpInfo(String xAccessToken, { String? cameraId, }) async {
    // ignore: prefer_const_declarations
    final path = r'/API/v1/InsightsController/GetMediaMTXConfiguration/cameraId';

    // ignore: prefer_final_locals
    Object? postBody;

    final queryParams = <QueryParam>[];
    final headerParams = <String, String>{};
    final formParams = <String, String>{};

    if (cameraId != null) {
      queryParams.addAll(_queryParams('', 'cameraId', cameraId));
    }

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
  ///
  /// * [String] cameraId:
  Future<CameraDTO?> aPIV1InsightsControllerGetMediaMTXConfigurationCameraIdGet(String xAccessToken, { String? cameraId, }) async {
    final response = await aPIV1InsightsControllerGetMediaMTXConfigurationCameraIdGetWithHttpInfo(xAccessToken,  cameraId: cameraId, );
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
    // When a remote server returns no body with a status of 204, we shall not decode it.
    // At the time of writing this, `dart:convert` will throw an "Unexpected end of input"
    // FormatException when trying to decode an empty string.
    if (response.body.isNotEmpty && response.statusCode != HttpStatus.noContent) {
      return await apiClient.deserializeAsync(await _decodeBodyBytes(response), 'CameraDTO',) as CameraDTO;
    
    }
    return null;
  }

  /// Performs an HTTP 'GET /API/v1/InsightsController/GetRunningProcesses' operation and returns the [Response].
  /// Parameters:
  ///
  /// * [String] xAccessToken (required):
  ///   Access Token
  Future<Response> aPIV1InsightsControllerGetRunningProcessesGetWithHttpInfo(String xAccessToken,) async {
    // ignore: prefer_const_declarations
    final path = r'/API/v1/InsightsController/GetRunningProcesses';

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
  Future<CameraDTO?> aPIV1InsightsControllerGetRunningProcessesGet(String xAccessToken,) async {
    final response = await aPIV1InsightsControllerGetRunningProcessesGetWithHttpInfo(xAccessToken,);
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
    // When a remote server returns no body with a status of 204, we shall not decode it.
    // At the time of writing this, `dart:convert` will throw an "Unexpected end of input"
    // FormatException when trying to decode an empty string.
    if (response.body.isNotEmpty && response.statusCode != HttpStatus.noContent) {
      return await apiClient.deserializeAsync(await _decodeBodyBytes(response), 'CameraDTO',) as CameraDTO;
    
    }
    return null;
  }
}
