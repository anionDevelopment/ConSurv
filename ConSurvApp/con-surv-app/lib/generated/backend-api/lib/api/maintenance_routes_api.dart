//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//
// @dart=2.18

// ignore_for_file: unused_element, unused_import
// ignore_for_file: always_put_required_named_parameters_first
// ignore_for_file: constant_identifier_names
// ignore_for_file: lines_longer_than_80_chars

part of openapi.api;


class MaintenanceRoutesApi {
  MaintenanceRoutesApi([ApiClient? apiClient]) : apiClient = apiClient ?? defaultApiClient;

  final ApiClient apiClient;

  /// Performs an HTTP 'GET /API/Other/Maintenance/AvailabilityCheck' operation and returns the [Response].
  Future<Response> aPIOtherMaintenanceAvailabilityCheckGetWithHttpInfo() async {
    // ignore: prefer_const_declarations
    final path = r'/API/Other/Maintenance/AvailabilityCheck';

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

  Future<void> aPIOtherMaintenanceAvailabilityCheckGet() async {
    final response = await aPIOtherMaintenanceAvailabilityCheckGetWithHttpInfo();
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
  }

  /// Performs an HTTP 'GET /API/Other/Maintenance/CurrentVersion' operation and returns the [Response].
  Future<Response> aPIOtherMaintenanceCurrentVersionGetWithHttpInfo() async {
    // ignore: prefer_const_declarations
    final path = r'/API/Other/Maintenance/CurrentVersion';

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

  Future<void> aPIOtherMaintenanceCurrentVersionGet() async {
    final response = await aPIOtherMaintenanceCurrentVersionGetWithHttpInfo();
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
  }

  /// Performs an HTTP 'GET /API/Other/Maintenance/HealthCheck' operation and returns the [Response].
  Future<Response> aPIOtherMaintenanceHealthCheckGetWithHttpInfo() async {
    // ignore: prefer_const_declarations
    final path = r'/API/Other/Maintenance/HealthCheck';

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

  Future<void> aPIOtherMaintenanceHealthCheckGet() async {
    final response = await aPIOtherMaintenanceHealthCheckGetWithHttpInfo();
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
  }

  /// Performs an HTTP 'GET /API/Other/Maintenance/Metrics' operation and returns the [Response].
  Future<Response> aPIOtherMaintenanceMetricsGetWithHttpInfo() async {
    // ignore: prefer_const_declarations
    final path = r'/API/Other/Maintenance/Metrics';

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

  Future<void> aPIOtherMaintenanceMetricsGet() async {
    final response = await aPIOtherMaintenanceMetricsGetWithHttpInfo();
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
  }

  /// Performs an HTTP 'GET /API/Other/Maintenance/ShowAllEndpoints' operation and returns the [Response].
  Future<Response> aPIOtherMaintenanceShowAllEndpointsGetWithHttpInfo() async {
    // ignore: prefer_const_declarations
    final path = r'/API/Other/Maintenance/ShowAllEndpoints';

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

  Future<void> aPIOtherMaintenanceShowAllEndpointsGet() async {
    final response = await aPIOtherMaintenanceShowAllEndpointsGetWithHttpInfo();
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
  }
}
