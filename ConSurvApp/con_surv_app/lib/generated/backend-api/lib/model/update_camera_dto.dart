//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//
// @dart=2.18

// ignore_for_file: unused_element, unused_import
// ignore_for_file: always_put_required_named_parameters_first
// ignore_for_file: constant_identifier_names
// ignore_for_file: lines_longer_than_80_chars

part of openapi.api;

class UpdateCameraDTO {
  /// Returns a new [UpdateCameraDTO] instance.
  UpdateCameraDTO({
    this.cameraId,
    this.name,
    this.videoInformationDTO,
    this.recordModeDTO,
  });

  String? cameraId;

  String? name;

  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  VideoInformationDTO? videoInformationDTO;

  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  RecordModeDTO? recordModeDTO;

  @override
  bool operator ==(Object other) => identical(this, other) || other is UpdateCameraDTO &&
    other.cameraId == cameraId &&
    other.name == name &&
    other.videoInformationDTO == videoInformationDTO &&
    other.recordModeDTO == recordModeDTO;

  @override
  int get hashCode =>
    // ignore: unnecessary_parenthesis
    (cameraId == null ? 0 : cameraId!.hashCode) +
    (name == null ? 0 : name!.hashCode) +
    (videoInformationDTO == null ? 0 : videoInformationDTO!.hashCode) +
    (recordModeDTO == null ? 0 : recordModeDTO!.hashCode);

  @override
  String toString() => 'UpdateCameraDTO[cameraId=$cameraId, name=$name, videoInformationDTO=$videoInformationDTO, recordModeDTO=$recordModeDTO]';

  Map<String, dynamic> toJson() {
    final json = <String, dynamic>{};
    if (this.cameraId != null) {
      json[r'cameraId'] = this.cameraId;
    } else {
      json[r'cameraId'] = null;
    }
    if (this.name != null) {
      json[r'name'] = this.name;
    } else {
      json[r'name'] = null;
    }
    if (this.videoInformationDTO != null) {
      json[r'videoInformationDTO'] = this.videoInformationDTO;
    } else {
      json[r'videoInformationDTO'] = null;
    }
    if (this.recordModeDTO != null) {
      json[r'recordModeDTO'] = this.recordModeDTO;
    } else {
      json[r'recordModeDTO'] = null;
    }
    return json;
  }

  /// Returns a new [UpdateCameraDTO] instance and imports its values from
  /// [value] if it's a [Map], null otherwise.
  // ignore: prefer_constructors_over_static_methods
  static UpdateCameraDTO? fromJson(dynamic value) {
    if (value is Map) {
      final json = value.cast<String, dynamic>();

      // Ensure that the map contains the required keys.
      // Note 1: the values aren't checked for validity beyond being non-null.
      // Note 2: this code is stripped in release mode!
      assert(() {
        requiredKeys.forEach((key) {
          assert(json.containsKey(key), 'Required key "UpdateCameraDTO[$key]" is missing from JSON.');
          assert(json[key] != null, 'Required key "UpdateCameraDTO[$key]" has a null value in JSON.');
        });
        return true;
      }());

      return UpdateCameraDTO(
        cameraId: mapValueOfType<String>(json, r'cameraId'),
        name: mapValueOfType<String>(json, r'name'),
        videoInformationDTO: VideoInformationDTO.fromJson(json[r'videoInformationDTO']),
        recordModeDTO: RecordModeDTO.fromJson(json[r'recordModeDTO']),
      );
    }
    return null;
  }

  static List<UpdateCameraDTO> listFromJson(dynamic json, {bool growable = false,}) {
    final result = <UpdateCameraDTO>[];
    if (json is List && json.isNotEmpty) {
      for (final row in json) {
        final value = UpdateCameraDTO.fromJson(row);
        if (value != null) {
          result.add(value);
        }
      }
    }
    return result.toList(growable: growable);
  }

  static Map<String, UpdateCameraDTO> mapFromJson(dynamic json) {
    final map = <String, UpdateCameraDTO>{};
    if (json is Map && json.isNotEmpty) {
      json = json.cast<String, dynamic>(); // ignore: parameter_assignments
      for (final entry in json.entries) {
        final value = UpdateCameraDTO.fromJson(entry.value);
        if (value != null) {
          map[entry.key] = value;
        }
      }
    }
    return map;
  }

  // maps a json object with a list of UpdateCameraDTO-objects as value to a dart map
  static Map<String, List<UpdateCameraDTO>> mapListFromJson(dynamic json, {bool growable = false,}) {
    final map = <String, List<UpdateCameraDTO>>{};
    if (json is Map && json.isNotEmpty) {
      // ignore: parameter_assignments
      json = json.cast<String, dynamic>();
      for (final entry in json.entries) {
        map[entry.key] = UpdateCameraDTO.listFromJson(entry.value, growable: growable,);
      }
    }
    return map;
  }

  /// The list of required keys that must be present in a JSON.
  static const requiredKeys = <String>{
  };
}

