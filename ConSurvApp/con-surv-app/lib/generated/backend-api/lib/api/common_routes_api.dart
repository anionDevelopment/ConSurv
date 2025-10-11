//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//
// @dart=2.18

// ignore_for_file: unused_element, unused_import
// ignore_for_file: always_put_required_named_parameters_first
// ignore_for_file: constant_identifier_names
// ignore_for_file: lines_longer_than_80_chars

part of openapi.api;


class CommonRoutesApi {
  CommonRoutesApi([ApiClient? apiClient]) : apiClient = apiClient ?? defaultApiClient;

  final ApiClient apiClient;

  /// Performs an HTTP 'GET /API/Other/Resources/Information/Contact' operation and returns the [Response].
  Future<Response> aPIOtherResourcesInformationContactGetWithHttpInfo() async {
    // ignore: prefer_const_declarations
    final path = r'/API/Other/Resources/Information/Contact';

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

  Future<void> aPIOtherResourcesInformationContactGet() async {
    final response = await aPIOtherResourcesInformationContactGetWithHttpInfo();
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
  }

  /// Performs an HTTP 'GET /API/Other/Resources/Information/License' operation and returns the [Response].
  Future<Response> aPIOtherResourcesInformationLicenseGetWithHttpInfo() async {
    // ignore: prefer_const_declarations
    final path = r'/API/Other/Resources/Information/License';

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

  Future<void> aPIOtherResourcesInformationLicenseGet() async {
    final response = await aPIOtherResourcesInformationLicenseGetWithHttpInfo();
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
  }

  /// Performs an HTTP 'GET /API/Other/Resources/Information/TermsOfService' operation and returns the [Response].
  Future<Response> aPIOtherResourcesInformationTermsOfServiceGetWithHttpInfo() async {
    // ignore: prefer_const_declarations
    final path = r'/API/Other/Resources/Information/TermsOfService';

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

  Future<void> aPIOtherResourcesInformationTermsOfServiceGet() async {
    final response = await aPIOtherResourcesInformationTermsOfServiceGetWithHttpInfo();
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
  }
}
