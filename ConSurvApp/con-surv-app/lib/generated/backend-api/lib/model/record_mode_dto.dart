//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//
// @dart=2.18

// ignore_for_file: unused_element, unused_import
// ignore_for_file: always_put_required_named_parameters_first
// ignore_for_file: constant_identifier_names
// ignore_for_file: lines_longer_than_80_chars

part of openapi.api;

class RecordModeDTO {
  /// Returns a new [RecordModeDTO] instance.
  RecordModeDTO({
    this.recordMode,
  });

  String? recordMode;

  @override
  bool operator ==(Object other) => identical(this, other) || other is RecordModeDTO &&
    other.recordMode == recordMode;

  @override
  int get hashCode =>
    // ignore: unnecessary_parenthesis
    (recordMode == null ? 0 : recordMode!.hashCode);

  @override
  String toString() => 'RecordModeDTO[recordMode=$recordMode]';

  Map<String, dynamic> toJson() {
    final json = <String, dynamic>{};
    if (this.recordMode != null) {
      json[r'recordMode'] = this.recordMode;
    } else {
      json[r'recordMode'] = null;
    }
    return json;
  }

  /// Returns a new [RecordModeDTO] instance and imports its values from
  /// [value] if it's a [Map], null otherwise.
  // ignore: prefer_constructors_over_static_methods
  static RecordModeDTO? fromJson(dynamic value) {
    if (value is Map) {
      final json = value.cast<String, dynamic>();

      // Ensure that the map contains the required keys.
      // Note 1: the values aren't checked for validity beyond being non-null.
      // Note 2: this code is stripped in release mode!
      assert(() {
        requiredKeys.forEach((key) {
          assert(json.containsKey(key), 'Required key "RecordModeDTO[$key]" is missing from JSON.');
          assert(json[key] != null, 'Required key "RecordModeDTO[$key]" has a null value in JSON.');
        });
        return true;
      }());

      return RecordModeDTO(
        recordMode: mapValueOfType<String>(json, r'recordMode'),
      );
    }
    return null;
  }

  static List<RecordModeDTO> listFromJson(dynamic json, {bool growable = false,}) {
    final result = <RecordModeDTO>[];
    if (json is List && json.isNotEmpty) {
      for (final row in json) {
        final value = RecordModeDTO.fromJson(row);
        if (value != null) {
          result.add(value);
        }
      }
    }
    return result.toList(growable: growable);
  }

  static Map<String, RecordModeDTO> mapFromJson(dynamic json) {
    final map = <String, RecordModeDTO>{};
    if (json is Map && json.isNotEmpty) {
      json = json.cast<String, dynamic>(); // ignore: parameter_assignments
      for (final entry in json.entries) {
        final value = RecordModeDTO.fromJson(entry.value);
        if (value != null) {
          map[entry.key] = value;
        }
      }
    }
    return map;
  }

  // maps a json object with a list of RecordModeDTO-objects as value to a dart map
  static Map<String, List<RecordModeDTO>> mapListFromJson(dynamic json, {bool growable = false,}) {
    final map = <String, List<RecordModeDTO>>{};
    if (json is Map && json.isNotEmpty) {
      // ignore: parameter_assignments
      json = json.cast<String, dynamic>();
      for (final entry in json.entries) {
        map[entry.key] = RecordModeDTO.listFromJson(entry.value, growable: growable,);
      }
    }
    return map;
  }

  /// The list of required keys that must be present in a JSON.
  static const requiredKeys = <String>{
  };
}

