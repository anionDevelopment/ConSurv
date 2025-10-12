//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//
// @dart=2.18

// ignore_for_file: unused_element, unused_import
// ignore_for_file: always_put_required_named_parameters_first
// ignore_for_file: constant_identifier_names
// ignore_for_file: lines_longer_than_80_chars

part of openapi.api;


class CameraApi {
  CameraApi([ApiClient? apiClient]) : apiClient = apiClient ?? defaultApiClient;

  final ApiClient apiClient;

  /// Performs an HTTP 'GET /API/v1/CameraController/Camera/{cameraId}' operation and returns the [Response].
  /// Parameters:
  ///
  /// * [String] cameraId (required):
  ///
  /// * [String] xAccessToken (required):
  ///   Access Token
  Future<Response> aPIV1CameraControllerCameraCameraIdGetWithHttpInfo(String cameraId, String xAccessToken,) async {
    // ignore: prefer_const_declarations
    final path = r'/API/v1/CameraController/Camera/{cameraId}'
      .replaceAll('{cameraId}', cameraId);

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
  /// * [String] cameraId (required):
  ///
  /// * [String] xAccessToken (required):
  ///   Access Token
  Future<CameraDTO?> aPIV1CameraControllerCameraCameraIdGet(String cameraId, String xAccessToken,) async {
    final response = await aPIV1CameraControllerCameraCameraIdGetWithHttpInfo(cameraId, xAccessToken,);
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

  /// Performs an HTTP 'GET /API/v1/CameraController/Cameras' operation and returns the [Response].
  /// Parameters:
  ///
  /// * [String] xAccessToken (required):
  ///   Access Token
  Future<Response> aPIV1CameraControllerCamerasGetWithHttpInfo(String xAccessToken,) async {
    // ignore: prefer_const_declarations
    final path = r'/API/v1/CameraController/Cameras';

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
  Future<List<CameraDTO>?> aPIV1CameraControllerCamerasGet(String xAccessToken,) async {
    final response = await aPIV1CameraControllerCamerasGetWithHttpInfo(xAccessToken,);
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
    // When a remote server returns no body with a status of 204, we shall not decode it.
    // At the time of writing this, `dart:convert` will throw an "Unexpected end of input"
    // FormatException when trying to decode an empty string.
    if (response.body.isNotEmpty && response.statusCode != HttpStatus.noContent) {
      final responseBody = await _decodeBodyBytes(response);
      return (await apiClient.deserializeAsync(responseBody, 'List<CameraDTO>') as List)
        .cast<CameraDTO>()
        .toList(growable: false);

    }
    return null;
  }

  /// Performs an HTTP 'POST /API/v1/CameraController/CreateCamera' operation and returns the [Response].
  /// Parameters:
  ///
  /// * [String] xAccessToken (required):
  ///   Access Token
  Future<Response> aPIV1CameraControllerCreateCameraPostWithHttpInfo(String xAccessToken,) async {
    // ignore: prefer_const_declarations
    final path = r'/API/v1/CameraController/CreateCamera';

    // ignore: prefer_final_locals
    Object? postBody;

    final queryParams = <QueryParam>[];
    final headerParams = <String, String>{};
    final formParams = <String, String>{};

    headerParams[r'X-AccessToken'] = parameterToString(xAccessToken);

    const contentTypes = <String>[];


    return apiClient.invokeAPI(
      path,
      'POST',
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
  Future<String?> aPIV1CameraControllerCreateCameraPost(String xAccessToken,) async {
    final response = await aPIV1CameraControllerCreateCameraPostWithHttpInfo(xAccessToken,);
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
    // When a remote server returns no body with a status of 204, we shall not decode it.
    // At the time of writing this, `dart:convert` will throw an "Unexpected end of input"
    // FormatException when trying to decode an empty string.
    if (response.body.isNotEmpty && response.statusCode != HttpStatus.noContent) {
      return await apiClient.deserializeAsync(await _decodeBodyBytes(response), 'String',) as String;
    
    }
    return null;
  }

  /// Performs an HTTP 'GET /API/v1/CameraController/DownloadVideo/{cameraId}/{filename}' operation and returns the [Response].
  /// Parameters:
  ///
  /// * [String] cameraId (required):
  ///
  /// * [String] filename (required):
  Future<Response> aPIV1CameraControllerDownloadVideoCameraIdFilenameGetWithHttpInfo(String cameraId, String filename,) async {
    // ignore: prefer_const_declarations
    final path = r'/API/v1/CameraController/DownloadVideo/{cameraId}/{filename}'
      .replaceAll('{cameraId}', cameraId)
      .replaceAll('{filename}', filename);

    // ignore: prefer_final_locals
    Object? postBody;

    final queryParams = <QueryParam>[];
    final headerParams = <String, String>{};
    final formParams = <String, String>{};

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
  /// * [String] cameraId (required):
  ///
  /// * [String] filename (required):
  Future<void> aPIV1CameraControllerDownloadVideoCameraIdFilenameGet(String cameraId, String filename,) async {
    final response = await aPIV1CameraControllerDownloadVideoCameraIdFilenameGetWithHttpInfo(cameraId, filename,);
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
  }

  /// Performs an HTTP 'GET /API/v1/CameraController/GetPreview/{cameraId}/{maximalHeight}/{maximalWidth}' operation and returns the [Response].
  /// Parameters:
  ///
  /// * [String] cameraId (required):
  ///
  /// * [int] maximalHeight (required):
  ///
  /// * [int] maximalWidth (required):
  ///
  /// * [String] xAccessToken (required):
  ///   Access Token
  Future<Response> aPIV1CameraControllerGetPreviewCameraIdMaximalHeightMaximalWidthGetWithHttpInfo(String cameraId, int maximalHeight, int maximalWidth, String xAccessToken,) async {
    // ignore: prefer_const_declarations
    final path = r'/API/v1/CameraController/GetPreview/{cameraId}/{maximalHeight}/{maximalWidth}'
      .replaceAll('{cameraId}', cameraId)
      .replaceAll('{maximalHeight}', maximalHeight.toString())
      .replaceAll('{maximalWidth}', maximalWidth.toString());

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
  /// * [String] cameraId (required):
  ///
  /// * [int] maximalHeight (required):
  ///
  /// * [int] maximalWidth (required):
  ///
  /// * [String] xAccessToken (required):
  ///   Access Token
  Future<String?> aPIV1CameraControllerGetPreviewCameraIdMaximalHeightMaximalWidthGet(String cameraId, int maximalHeight, int maximalWidth, String xAccessToken,) async {
    final response = await aPIV1CameraControllerGetPreviewCameraIdMaximalHeightMaximalWidthGetWithHttpInfo(cameraId, maximalHeight, maximalWidth, xAccessToken,);
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
    // When a remote server returns no body with a status of 204, we shall not decode it.
    // At the time of writing this, `dart:convert` will throw an "Unexpected end of input"
    // FormatException when trying to decode an empty string.
    if (response.body.isNotEmpty && response.statusCode != HttpStatus.noContent) {
      return await apiClient.deserializeAsync(await _decodeBodyBytes(response), 'String',) as String;
    
    }
    return null;
  }

  /// Performs an HTTP 'GET /API/v1/CameraController/GetPreviewOfVideo{cameraId}/{filename}' operation and returns the [Response].
  /// Parameters:
  ///
  /// * [String] cameraId (required):
  ///
  /// * [String] filename (required):
  Future<Response> aPIV1CameraControllerGetPreviewOfVideocameraIdFilenameGetWithHttpInfo(String cameraId, String filename,) async {
    // ignore: prefer_const_declarations
    final path = r'/API/v1/CameraController/GetPreviewOfVideo{cameraId}/{filename}'
      .replaceAll('{cameraId}', cameraId)
      .replaceAll('{filename}', filename);

    // ignore: prefer_final_locals
    Object? postBody;

    final queryParams = <QueryParam>[];
    final headerParams = <String, String>{};
    final formParams = <String, String>{};

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
  /// * [String] cameraId (required):
  ///
  /// * [String] filename (required):
  Future<void> aPIV1CameraControllerGetPreviewOfVideocameraIdFilenameGet(String cameraId, String filename,) async {
    final response = await aPIV1CameraControllerGetPreviewOfVideocameraIdFilenameGetWithHttpInfo(cameraId, filename,);
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
  }

  /// Performs an HTTP 'GET /API/v1/CameraController/ListVideos' operation and returns the [Response].
  Future<Response> aPIV1CameraControllerListVideosGetWithHttpInfo() async {
    // ignore: prefer_const_declarations
    final path = r'/API/v1/CameraController/ListVideos';

    // ignore: prefer_final_locals
    Object? postBody;

    final queryParams = <QueryParam>[];
    final headerParams = <String, String>{};
    final formParams = <String, String>{};

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

  Future<void> aPIV1CameraControllerListVideosGet() async {
    final response = await aPIV1CameraControllerListVideosGetWithHttpInfo();
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
  }

  /// Performs an HTTP 'DELETE /API/v1/CameraController/RemoveCamera/{cameraId}' operation and returns the [Response].
  /// Parameters:
  ///
  /// * [String] cameraId (required):
  ///
  /// * [String] xAccessToken (required):
  ///   Access Token
  Future<Response> aPIV1CameraControllerRemoveCameraCameraIdDeleteWithHttpInfo(String cameraId, String xAccessToken,) async {
    // ignore: prefer_const_declarations
    final path = r'/API/v1/CameraController/RemoveCamera/{cameraId}'
      .replaceAll('{cameraId}', cameraId);

    // ignore: prefer_final_locals
    Object? postBody;

    final queryParams = <QueryParam>[];
    final headerParams = <String, String>{};
    final formParams = <String, String>{};

    headerParams[r'X-AccessToken'] = parameterToString(xAccessToken);

    const contentTypes = <String>[];


    return apiClient.invokeAPI(
      path,
      'DELETE',
      queryParams,
      postBody,
      headerParams,
      formParams,
      contentTypes.isEmpty ? null : contentTypes.first,
    );
  }

  /// Parameters:
  ///
  /// * [String] cameraId (required):
  ///
  /// * [String] xAccessToken (required):
  ///   Access Token
  Future<void> aPIV1CameraControllerRemoveCameraCameraIdDelete(String cameraId, String xAccessToken,) async {
    final response = await aPIV1CameraControllerRemoveCameraCameraIdDeleteWithHttpInfo(cameraId, xAccessToken,);
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
  }

  /// Performs an HTTP 'DELETE /API/v1/CameraController/RemoveVideo/{cameraId}/{filename}' operation and returns the [Response].
  /// Parameters:
  ///
  /// * [String] cameraId (required):
  ///
  /// * [String] filename (required):
  Future<Response> aPIV1CameraControllerRemoveVideoCameraIdFilenameDeleteWithHttpInfo(String cameraId, String filename,) async {
    // ignore: prefer_const_declarations
    final path = r'/API/v1/CameraController/RemoveVideo/{cameraId}/{filename}'
      .replaceAll('{cameraId}', cameraId)
      .replaceAll('{filename}', filename);

    // ignore: prefer_final_locals
    Object? postBody;

    final queryParams = <QueryParam>[];
    final headerParams = <String, String>{};
    final formParams = <String, String>{};

    const contentTypes = <String>[];


    return apiClient.invokeAPI(
      path,
      'DELETE',
      queryParams,
      postBody,
      headerParams,
      formParams,
      contentTypes.isEmpty ? null : contentTypes.first,
    );
  }

  /// Parameters:
  ///
  /// * [String] cameraId (required):
  ///
  /// * [String] filename (required):
  Future<void> aPIV1CameraControllerRemoveVideoCameraIdFilenameDelete(String cameraId, String filename,) async {
    final response = await aPIV1CameraControllerRemoveVideoCameraIdFilenameDeleteWithHttpInfo(cameraId, filename,);
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
  }

  /// Performs an HTTP 'POST /API/v1/CameraController/RunONVIFCommand/{cameraId}' operation and returns the [Response].
  /// Parameters:
  ///
  /// * [String] cameraId (required):
  ///
  /// * [ONVIFCommandDTO] oNVIFCommandDTO:
  Future<Response> aPIV1CameraControllerRunONVIFCommandCameraIdPostWithHttpInfo(String cameraId, { ONVIFCommandDTO? oNVIFCommandDTO, }) async {
    // ignore: prefer_const_declarations
    final path = r'/API/v1/CameraController/RunONVIFCommand/{cameraId}'
      .replaceAll('{cameraId}', cameraId);

    // ignore: prefer_final_locals
    Object? postBody = oNVIFCommandDTO;

    final queryParams = <QueryParam>[];
    final headerParams = <String, String>{};
    final formParams = <String, String>{};

    const contentTypes = <String>['application/json', 'text/json', 'application/*+json'];


    return apiClient.invokeAPI(
      path,
      'POST',
      queryParams,
      postBody,
      headerParams,
      formParams,
      contentTypes.isEmpty ? null : contentTypes.first,
    );
  }

  /// Parameters:
  ///
  /// * [String] cameraId (required):
  ///
  /// * [ONVIFCommandDTO] oNVIFCommandDTO:
  Future<void> aPIV1CameraControllerRunONVIFCommandCameraIdPost(String cameraId, { ONVIFCommandDTO? oNVIFCommandDTO, }) async {
    final response = await aPIV1CameraControllerRunONVIFCommandCameraIdPostWithHttpInfo(cameraId,  oNVIFCommandDTO: oNVIFCommandDTO, );
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
  }

  /// Performs an HTTP 'PUT /API/v1/CameraController/UpdateCamera' operation and returns the [Response].
  /// Parameters:
  ///
  /// * [String] xAccessToken (required):
  ///   Access Token
  ///
  /// * [UpdateCameraDTO] updateCameraDTO:
  Future<Response> aPIV1CameraControllerUpdateCameraPutWithHttpInfo(String xAccessToken, { UpdateCameraDTO? updateCameraDTO, }) async {
    // ignore: prefer_const_declarations
    final path = r'/API/v1/CameraController/UpdateCamera';

    // ignore: prefer_final_locals
    Object? postBody = updateCameraDTO;

    final queryParams = <QueryParam>[];
    final headerParams = <String, String>{};
    final formParams = <String, String>{};

    headerParams[r'X-AccessToken'] = parameterToString(xAccessToken);

    const contentTypes = <String>['application/json', 'text/json', 'application/*+json'];


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
  /// * [UpdateCameraDTO] updateCameraDTO:
  Future<void> aPIV1CameraControllerUpdateCameraPut(String xAccessToken, { UpdateCameraDTO? updateCameraDTO, }) async {
    final response = await aPIV1CameraControllerUpdateCameraPutWithHttpInfo(xAccessToken,  updateCameraDTO: updateCameraDTO, );
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
  }
}
