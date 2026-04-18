//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//
// @dart=2.18

// ignore_for_file: unused_element, unused_import
// ignore_for_file: always_put_required_named_parameters_first
// ignore_for_file: constant_identifier_names
// ignore_for_file: lines_longer_than_80_chars

import 'package:openapi/api.dart';
import 'package:test/test.dart';


/// tests for CameraApi
void main() {
  // final instance = CameraApi();

  group('tests for CameraApi', () {
    // Retrieves the full configuration details of a specific camera.
    //
    //Future<CameraDTO> aPIV3CameraControllerCameraCameraIdGet(String cameraId, String xAccessToken) async
    test('test aPIV3CameraControllerCameraCameraIdGet', () async {
      // TODO
    });

    // Returns the list of all configured cameras visible to the current user.
    //
    //Future<List<CameraDTO>> aPIV3CameraControllerCamerasGet(String xAccessToken) async
    test('test aPIV3CameraControllerCamerasGet', () async {
      // TODO
    });

    // Creates a new camera with default name and RTSP address and returns its generated identifier.
    //
    //Future<String> aPIV3CameraControllerCreateCameraPost(String xAccessToken) async
    test('test aPIV3CameraControllerCreateCameraPost', () async {
      // TODO
    });

    // Downloads the raw bytes of a specific recorded video file for a given camera.
    //
    //Future aPIV3CameraControllerDownloadVideoCameraIdFilenameGet(String cameraId, String filename) async
    test('test aPIV3CameraControllerDownloadVideoCameraIdFilenameGet', () async {
      // TODO
    });

    // Returns the latest preview image (as raw bytes) for the specified camera.
    //
    //Future<String> aPIV3CameraControllerGetPreviewCameraIdGet(String cameraId, String xAccessToken) async
    test('test aPIV3CameraControllerGetPreviewCameraIdGet', () async {
      // TODO
    });

    // Returns a preview thumbnail image for the specified recorded video file of a camera.
    //
    //Future aPIV3CameraControllerGetPreviewOfVideocameraIdFilenameGet(String cameraId, String filename) async
    test('test aPIV3CameraControllerGetPreviewOfVideocameraIdFilenameGet', () async {
      // TODO
    });

    // Returns a dictionary mapping each camera identifier to the list of recorded video filenames available for that camera.
    //
    //Future aPIV3CameraControllerListVideosGet() async
    test('test aPIV3CameraControllerListVideosGet', () async {
      // TODO
    });

    // Permanently removes the specified camera and all associated data.
    //
    //Future aPIV3CameraControllerRemoveCameraCameraIdDelete(String cameraId, String xAccessToken) async
    test('test aPIV3CameraControllerRemoveCameraCameraIdDelete', () async {
      // TODO
    });

    // Permanently deletes a specific recorded video file belonging to the given camera.
    //
    //Future aPIV3CameraControllerRemoveVideoCameraIdFilenameDelete(String cameraId, String filename) async
    test('test aPIV3CameraControllerRemoveVideoCameraIdFilenameDelete', () async {
      // TODO
    });

    // Executes an ONVIF command on the specified camera (e.g., PTZ control, preset recall).
    //
    //Future aPIV3CameraControllerRunONVIFCommandCameraIdPost(String cameraId, { ONVIFCommandDTO oNVIFCommandDTO }) async
    test('test aPIV3CameraControllerRunONVIFCommandCameraIdPost', () async {
      // TODO
    });

    // Updates the properties of an existing camera using the values provided in the request body.
    //
    //Future aPIV3CameraControllerUpdateCameraPut(String xAccessToken, { UpdateCameraDTO updateCameraDTO }) async
    test('test aPIV3CameraControllerUpdateCameraPut', () async {
      // TODO
    });

  });
}
