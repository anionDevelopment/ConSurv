//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//
// @dart=2.18

// ignore_for_file: unused_element, unused_import
// ignore_for_file: always_put_required_named_parameters_first
// ignore_for_file: constant_identifier_names
// ignore_for_file: lines_longer_than_80_chars

part of openapi.api;

class CameraDTO {
  /// Returns a new [CameraDTO] instance.
  CameraDTO({
    this.cameraId,
    this.name,
    this.videoInformationDTO,
    this.recordModeDTO,
    this.recordStateDTO,
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

  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  RecordStateDTO? recordStateDTO;

  @override
  bool operator ==(Object other) => identical(this, other) || other is CameraDTO &&
    other.cameraId == cameraId &&
    other.name == name &&
    other.videoInformationDTO == videoInformationDTO &&
    other.recordModeDTO == recordModeDTO &&
    other.recordStateDTO == recordStateDTO;

  @override
  int get hashCode =>
    // ignore: unnecessary_parenthesis
    (cameraId == null ? 0 : cameraId!.hashCode) +
    (name == null ? 0 : name!.hashCode) +
    (videoInformationDTO == null ? 0 : videoInformationDTO!.hashCode) +
    (recordModeDTO == null ? 0 : recordModeDTO!.hashCode) +
    (recordStateDTO == null ? 0 : recordStateDTO!.hashCode);

  @override
  String toString() => 'CameraDTO[cameraId=$cameraId, name=$name, videoInformationDTO=$videoInformationDTO, recordModeDTO=$recordModeDTO, recordStateDTO=$recordStateDTO]';

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
    if (this.recordStateDTO != null) {
      json[r'recordStateDTO'] = this.recordStateDTO;
    } else {
      json[r'recordStateDTO'] = null;
    }
    return json;
  }

  /// Returns a new [CameraDTO] instance and imports its values from
  /// [value] if it's a [Map], null otherwise.
  // ignore: prefer_constructors_over_static_methods
  static CameraDTO? fromJson(dynamic value) {
    if (value is Map) {
      final json = value.cast<String, dynamic>();

      // Ensure that the map contains the required keys.
      // Note 1: the values aren't checked for validity beyond being non-null.
      // Note 2: this code is stripped in release mode!
      assert(() {
        requiredKeys.forEach((key) {
          assert(json.containsKey(key), 'Required key "CameraDTO[$key]" is missing from JSON.');
          assert(json[key] != null, 'Required key "CameraDTO[$key]" has a null value in JSON.');
        });
        return true;
      }());

      return CameraDTO(
        cameraId: mapValueOfType<String>(json, r'cameraId'),
        name: mapValueOfType<String>(json, r'name'),
        videoInformationDTO: VideoInformationDTO.fromJson(json[r'videoInformationDTO']),
        recordModeDTO: RecordModeDTO.fromJson(json[r'recordModeDTO']),
        recordStateDTO: RecordStateDTO.fromJson(json[r'recordStateDTO']),
      );
    }
    return null;
  }

  static List<CameraDTO> listFromJson(dynamic json, {bool growable = false,}) {
    final result = <CameraDTO>[];
    if (json is List && json.isNotEmpty) {
      for (final row in json) {
        final value = CameraDTO.fromJson(row);
        if (value != null) {
          result.add(value);
        }
      }
    }
    return result.toList(growable: growable);
  }

  static Map<String, CameraDTO> mapFromJson(dynamic json) {
    final map = <String, CameraDTO>{};
    if (json is Map && json.isNotEmpty) {
      json = json.cast<String, dynamic>(); // ignore: parameter_assignments
      for (final entry in json.entries) {
        final value = CameraDTO.fromJson(entry.value);
        if (value != null) {
          map[entry.key] = value;
        }
      }
    }
    return map;
  }

  // maps a json object with a list of CameraDTO-objects as value to a dart map
  static Map<String, List<CameraDTO>> mapListFromJson(dynamic json, {bool growable = false,}) {
    final map = <String, List<CameraDTO>>{};
    if (json is Map && json.isNotEmpty) {
      // ignore: parameter_assignments
      json = json.cast<String, dynamic>();
      for (final entry in json.entries) {
        map[entry.key] = CameraDTO.listFromJson(entry.value, growable: growable,);
      }
    }
    return map;
  }

  /// The list of required keys that must be present in a JSON.
  static const requiredKeys = <String>{
  };
}

