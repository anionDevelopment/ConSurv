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

  /// Retrieves the full configuration details of a specific camera.
  ///
  /// Note: This method returns the HTTP [Response].
  ///
  /// Parameters:
  ///
  /// * [String] cameraId (required):
  ///   The unique identifier of the camera to retrieve.
  ///
  /// * [String] xAccessToken (required):
  ///   Access Token
  Future<Response> aPIV3CameraControllerCameraCameraIdGetWithHttpInfo(String cameraId, String xAccessToken,) async {
    // ignore: prefer_const_declarations
    final path = r'/API/v3/CameraController/Camera/{cameraId}'
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

  /// Retrieves the full configuration details of a specific camera.
  ///
  /// Parameters:
  ///
  /// * [String] cameraId (required):
  ///   The unique identifier of the camera to retrieve.
  ///
  /// * [String] xAccessToken (required):
  ///   Access Token
  Future<CameraDTO?> aPIV3CameraControllerCameraCameraIdGet(String cameraId, String xAccessToken,) async {
    final response = await aPIV3CameraControllerCameraCameraIdGetWithHttpInfo(cameraId, xAccessToken,);
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

  /// Returns the list of all configured cameras visible to the current user.
  ///
  /// Note: This method returns the HTTP [Response].
  ///
  /// Parameters:
  ///
  /// * [String] xAccessToken (required):
  ///   Access Token
  Future<Response> aPIV3CameraControllerCamerasGetWithHttpInfo(String xAccessToken,) async {
    // ignore: prefer_const_declarations
    final path = r'/API/v3/CameraController/Cameras';

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

  /// Returns the list of all configured cameras visible to the current user.
  ///
  /// Parameters:
  ///
  /// * [String] xAccessToken (required):
  ///   Access Token
  Future<List<CameraDTO>?> aPIV3CameraControllerCamerasGet(String xAccessToken,) async {
    final response = await aPIV3CameraControllerCamerasGetWithHttpInfo(xAccessToken,);
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

  /// Creates a new camera with default name and RTSP address and returns its generated identifier.
  ///
  /// Note: This method returns the HTTP [Response].
  ///
  /// Parameters:
  ///
  /// * [String] xAccessToken (required):
  ///   Access Token
  Future<Response> aPIV3CameraControllerCreateCameraPostWithHttpInfo(String xAccessToken,) async {
    // ignore: prefer_const_declarations
    final path = r'/API/v3/CameraController/CreateCamera';

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

  /// Creates a new camera with default name and RTSP address and returns its generated identifier.
  ///
  /// Parameters:
  ///
  /// * [String] xAccessToken (required):
  ///   Access Token
  Future<String?> aPIV3CameraControllerCreateCameraPost(String xAccessToken,) async {
    final response = await aPIV3CameraControllerCreateCameraPostWithHttpInfo(xAccessToken,);
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

  /// Downloads the raw bytes of a specific recorded video file for a given camera.
  ///
  /// Note: This method returns the HTTP [Response].
  ///
  /// Parameters:
  ///
  /// * [String] cameraId (required):
  ///   The unique identifier of the camera that owns the video.
  ///
  /// * [String] filename (required):
  ///   The filename of the recorded video to download.
  Future<Response> aPIV3CameraControllerDownloadVideoCameraIdFilenameGetWithHttpInfo(String cameraId, String filename,) async {
    // ignore: prefer_const_declarations
    final path = r'/API/v3/CameraController/DownloadVideo/{cameraId}/{filename}'
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

  /// Downloads the raw bytes of a specific recorded video file for a given camera.
  ///
  /// Parameters:
  ///
  /// * [String] cameraId (required):
  ///   The unique identifier of the camera that owns the video.
  ///
  /// * [String] filename (required):
  ///   The filename of the recorded video to download.
  Future<void> aPIV3CameraControllerDownloadVideoCameraIdFilenameGet(String cameraId, String filename,) async {
    final response = await aPIV3CameraControllerDownloadVideoCameraIdFilenameGetWithHttpInfo(cameraId, filename,);
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
  }

  /// Returns the latest preview image (as raw bytes) for the specified camera.
  ///
  /// Note: This method returns the HTTP [Response].
  ///
  /// Parameters:
  ///
  /// * [String] cameraId (required):
  ///   The unique identifier of the camera whose preview should be retrieved.
  ///
  /// * [String] xAccessToken (required):
  ///   Access Token
  Future<Response> aPIV3CameraControllerGetPreviewCameraIdGetWithHttpInfo(String cameraId, String xAccessToken,) async {
    // ignore: prefer_const_declarations
    final path = r'/API/v3/CameraController/GetPreview/{cameraId}'
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

  /// Returns the latest preview image (as raw bytes) for the specified camera.
  ///
  /// Parameters:
  ///
  /// * [String] cameraId (required):
  ///   The unique identifier of the camera whose preview should be retrieved.
  ///
  /// * [String] xAccessToken (required):
  ///   Access Token
  Future<String?> aPIV3CameraControllerGetPreviewCameraIdGet(String cameraId, String xAccessToken,) async {
    final response = await aPIV3CameraControllerGetPreviewCameraIdGetWithHttpInfo(cameraId, xAccessToken,);
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

  /// Returns a preview thumbnail image for the specified recorded video file of a camera.
  ///
  /// Note: This method returns the HTTP [Response].
  ///
  /// Parameters:
  ///
  /// * [String] cameraId (required):
  ///   The unique identifier of the camera that owns the video.
  ///
  /// * [String] filename (required):
  ///   The filename of the recorded video for which the preview is requested.
  Future<Response> aPIV3CameraControllerGetPreviewOfVideocameraIdFilenameGetWithHttpInfo(String cameraId, String filename,) async {
    // ignore: prefer_const_declarations
    final path = r'/API/v3/CameraController/GetPreviewOfVideo{cameraId}/{filename}'
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

  /// Returns a preview thumbnail image for the specified recorded video file of a camera.
  ///
  /// Parameters:
  ///
  /// * [String] cameraId (required):
  ///   The unique identifier of the camera that owns the video.
  ///
  /// * [String] filename (required):
  ///   The filename of the recorded video for which the preview is requested.
  Future<void> aPIV3CameraControllerGetPreviewOfVideocameraIdFilenameGet(String cameraId, String filename,) async {
    final response = await aPIV3CameraControllerGetPreviewOfVideocameraIdFilenameGetWithHttpInfo(cameraId, filename,);
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
  }

  /// Returns a dictionary mapping each camera identifier to the list of recorded video filenames available for that camera.
  ///
  /// Note: This method returns the HTTP [Response].
  Future<Response> aPIV3CameraControllerListVideosGetWithHttpInfo() async {
    // ignore: prefer_const_declarations
    final path = r'/API/v3/CameraController/ListVideos';

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

  /// Returns a dictionary mapping each camera identifier to the list of recorded video filenames available for that camera.
  Future<void> aPIV3CameraControllerListVideosGet() async {
    final response = await aPIV3CameraControllerListVideosGetWithHttpInfo();
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
  }

  /// Permanently removes the specified camera and all associated data.
  ///
  /// Note: This method returns the HTTP [Response].
  ///
  /// Parameters:
  ///
  /// * [String] cameraId (required):
  ///   The unique identifier of the camera to remove.
  ///
  /// * [String] xAccessToken (required):
  ///   Access Token
  Future<Response> aPIV3CameraControllerRemoveCameraCameraIdDeleteWithHttpInfo(String cameraId, String xAccessToken,) async {
    // ignore: prefer_const_declarations
    final path = r'/API/v3/CameraController/RemoveCamera/{cameraId}'
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

  /// Permanently removes the specified camera and all associated data.
  ///
  /// Parameters:
  ///
  /// * [String] cameraId (required):
  ///   The unique identifier of the camera to remove.
  ///
  /// * [String] xAccessToken (required):
  ///   Access Token
  Future<void> aPIV3CameraControllerRemoveCameraCameraIdDelete(String cameraId, String xAccessToken,) async {
    final response = await aPIV3CameraControllerRemoveCameraCameraIdDeleteWithHttpInfo(cameraId, xAccessToken,);
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
  }

  /// Permanently deletes a specific recorded video file belonging to the given camera.
  ///
  /// Note: This method returns the HTTP [Response].
  ///
  /// Parameters:
  ///
  /// * [String] cameraId (required):
  ///   The unique identifier of the camera that owns the video.
  ///
  /// * [String] filename (required):
  ///   The filename of the recorded video to delete.
  Future<Response> aPIV3CameraControllerRemoveVideoCameraIdFilenameDeleteWithHttpInfo(String cameraId, String filename,) async {
    // ignore: prefer_const_declarations
    final path = r'/API/v3/CameraController/RemoveVideo/{cameraId}/{filename}'
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

  /// Permanently deletes a specific recorded video file belonging to the given camera.
  ///
  /// Parameters:
  ///
  /// * [String] cameraId (required):
  ///   The unique identifier of the camera that owns the video.
  ///
  /// * [String] filename (required):
  ///   The filename of the recorded video to delete.
  Future<void> aPIV3CameraControllerRemoveVideoCameraIdFilenameDelete(String cameraId, String filename,) async {
    final response = await aPIV3CameraControllerRemoveVideoCameraIdFilenameDeleteWithHttpInfo(cameraId, filename,);
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
  }

  /// Executes an ONVIF command on the specified camera (e.g., PTZ control, preset recall).
  ///
  /// Note: This method returns the HTTP [Response].
  ///
  /// Parameters:
  ///
  /// * [String] cameraId (required):
  ///   The unique identifier of the target camera.
  ///
  /// * [ONVIFCommandDTO] oNVIFCommandDTO:
  ///   The ONVIF command to execute, including its type and parameters.
  Future<Response> aPIV3CameraControllerRunONVIFCommandCameraIdPostWithHttpInfo(String cameraId, { ONVIFCommandDTO? oNVIFCommandDTO, }) async {
    // ignore: prefer_const_declarations
    final path = r'/API/v3/CameraController/RunONVIFCommand/{cameraId}'
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

  /// Executes an ONVIF command on the specified camera (e.g., PTZ control, preset recall).
  ///
  /// Parameters:
  ///
  /// * [String] cameraId (required):
  ///   The unique identifier of the target camera.
  ///
  /// * [ONVIFCommandDTO] oNVIFCommandDTO:
  ///   The ONVIF command to execute, including its type and parameters.
  Future<void> aPIV3CameraControllerRunONVIFCommandCameraIdPost(String cameraId, { ONVIFCommandDTO? oNVIFCommandDTO, }) async {
    final response = await aPIV3CameraControllerRunONVIFCommandCameraIdPostWithHttpInfo(cameraId,  oNVIFCommandDTO: oNVIFCommandDTO, );
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
  }

  /// Updates the properties of an existing camera using the values provided in the request body.
  ///
  /// Note: This method returns the HTTP [Response].
  ///
  /// Parameters:
  ///
  /// * [String] xAccessToken (required):
  ///   Access Token
  ///
  /// * [UpdateCameraDTO] updateCameraDTO:
  ///   The DTO containing the updated camera properties.
  Future<Response> aPIV3CameraControllerUpdateCameraPutWithHttpInfo(String xAccessToken, { UpdateCameraDTO? updateCameraDTO, }) async {
    // ignore: prefer_const_declarations
    final path = r'/API/v3/CameraController/UpdateCamera';

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

  /// Updates the properties of an existing camera using the values provided in the request body.
  ///
  /// Parameters:
  ///
  /// * [String] xAccessToken (required):
  ///   Access Token
  ///
  /// * [UpdateCameraDTO] updateCameraDTO:
  ///   The DTO containing the updated camera properties.
  Future<void> aPIV3CameraControllerUpdateCameraPut(String xAccessToken, { UpdateCameraDTO? updateCameraDTO, }) async {
    final response = await aPIV3CameraControllerUpdateCameraPutWithHttpInfo(xAccessToken,  updateCameraDTO: updateCameraDTO, );
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
  }
}
