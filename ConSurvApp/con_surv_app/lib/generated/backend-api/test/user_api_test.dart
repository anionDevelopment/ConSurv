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


/// tests for UserApi
void main() {
  // final instance = UserApi();

  group('tests for UserApi', () {
    // Creates a new user account with the given credentials and assigns the admin role to it.  Requires the caller to hold the admin role.
    //
    //Future aPIV3UserControllerCreateUserPut(String xAccessToken, { String user, String password }) async
    test('test aPIV3UserControllerCreateUserPut', () async {
      // TODO
    });

    // Returns the list of role names assigned to the currently authenticated user.
    //
    //Future<List<String>> aPIV3UserControllerGetRolesPut(String xAccessToken) async
    test('test aPIV3UserControllerGetRolesPut', () async {
      // TODO
    });

    // Returns profile information (e.g., username, roles) for the currently authenticated user.
    //
    //Future<UserInformationDTO> aPIV3UserControllerGetUserInformationGet(String xAccessToken) async
    test('test aPIV3UserControllerGetUserInformationGet', () async {
      // TODO
    });

    // Authenticates a user with the supplied credentials and returns a new access token on success.
    //
    //Future<AccessToken> aPIV3UserControllerLoginPut({ String xUser, String xPassword }) async
    test('test aPIV3UserControllerLoginPut', () async {
      // TODO
    });

    // Invalidates the access token of the currently authenticated user, effectively logging them out.
    //
    //Future aPIV3UserControllerLogoutPut(String xAccessToken) async
    test('test aPIV3UserControllerLogoutPut', () async {
      // TODO
    });

    // Checks whether the supplied access token is still valid (not expired and not revoked).
    //
    //Future<bool> aPIV3UserControllerTokenIsValidGet({ String accessToken }) async
    test('test aPIV3UserControllerTokenIsValidGet', () async {
      // TODO
    });

  });
}
