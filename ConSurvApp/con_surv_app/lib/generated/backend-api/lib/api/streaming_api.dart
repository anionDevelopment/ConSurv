//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//
// @dart=2.18

// ignore_for_file: unused_element, unused_import
// ignore_for_file: always_put_required_named_parameters_first
// ignore_for_file: constant_identifier_names
// ignore_for_file: lines_longer_than_80_chars

part of openapi.api;


class StreamingApi {
  StreamingApi([ApiClient? apiClient]) : apiClient = apiClient ?? defaultApiClient;

  final ApiClient apiClient;

  /// Performs an HTTP 'GET /API/v3/StreamingController/Stream/{streamId}/{filename}' operation and returns the [Response].
  /// Parameters:
  ///
  /// * [String] streamId (required):
  ///
  /// * [String] filename (required):
  ///
  /// * [String] xAccessToken (required):
  ///   Access Token
  Future<Response> aPIV3StreamingControllerStreamStreamIdFilenameGetWithHttpInfo(String streamId, String filename, String xAccessToken,) async {
    // ignore: prefer_const_declarations
    final path = r'/API/v3/StreamingController/Stream/{streamId}/{filename}'
      .replaceAll('{streamId}', streamId)
      .replaceAll('{filename}', filename);

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
  /// * [String] streamId (required):
  ///
  /// * [String] filename (required):
  ///
  /// * [String] xAccessToken (required):
  ///   Access Token
  Future<void> aPIV3StreamingControllerStreamStreamIdFilenameGet(String streamId, String filename, String xAccessToken,) async {
    final response = await aPIV3StreamingControllerStreamStreamIdFilenameGetWithHttpInfo(streamId, filename, xAccessToken,);
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
  }
}
