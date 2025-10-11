//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//
// @dart=2.18

// ignore_for_file: unused_element, unused_import
// ignore_for_file: always_put_required_named_parameters_first
// ignore_for_file: constant_identifier_names
// ignore_for_file: lines_longer_than_80_chars

part of openapi.api;

class RecordStateDTO {
  /// Returns a new [RecordStateDTO] instance.
  RecordStateDTO({
    this.recordState,
  });

  String? recordState;

  @override
  bool operator ==(Object other) => identical(this, other) || other is RecordStateDTO &&
    other.recordState == recordState;

  @override
  int get hashCode =>
    // ignore: unnecessary_parenthesis
    (recordState == null ? 0 : recordState!.hashCode);

  @override
  String toString() => 'RecordStateDTO[recordState=$recordState]';

  Map<String, dynamic> toJson() {
    final json = <String, dynamic>{};
    if (this.recordState != null) {
      json[r'recordState'] = this.recordState;
    } else {
      json[r'recordState'] = null;
    }
    return json;
  }

  /// Returns a new [RecordStateDTO] instance and imports its values from
  /// [value] if it's a [Map], null otherwise.
  // ignore: prefer_constructors_over_static_methods
  static RecordStateDTO? fromJson(dynamic value) {
    if (value is Map) {
      final json = value.cast<String, dynamic>();

      // Ensure that the map contains the required keys.
      // Note 1: the values aren't checked for validity beyond being non-null.
      // Note 2: this code is stripped in release mode!
      assert(() {
        requiredKeys.forEach((key) {
          assert(json.containsKey(key), 'Required key "RecordStateDTO[$key]" is missing from JSON.');
          assert(json[key] != null, 'Required key "RecordStateDTO[$key]" has a null value in JSON.');
        });
        return true;
      }());

      return RecordStateDTO(
        recordState: mapValueOfType<String>(json, r'recordState'),
      );
    }
    return null;
  }

  static List<RecordStateDTO> listFromJson(dynamic json, {bool growable = false,}) {
    final result = <RecordStateDTO>[];
    if (json is List && json.isNotEmpty) {
      for (final row in json) {
        final value = RecordStateDTO.fromJson(row);
        if (value != null) {
          result.add(value);
        }
      }
    }
    return result.toList(growable: growable);
  }

  static Map<String, RecordStateDTO> mapFromJson(dynamic json) {
    final map = <String, RecordStateDTO>{};
    if (json is Map && json.isNotEmpty) {
      json = json.cast<String, dynamic>(); // ignore: parameter_assignments
      for (final entry in json.entries) {
        final value = RecordStateDTO.fromJson(entry.value);
        if (value != null) {
          map[entry.key] = value;
        }
      }
    }
    return map;
  }

  // maps a json object with a list of RecordStateDTO-objects as value to a dart map
  static Map<String, List<RecordStateDTO>> mapListFromJson(dynamic json, {bool growable = false,}) {
    final map = <String, List<RecordStateDTO>>{};
    if (json is Map && json.isNotEmpty) {
      // ignore: parameter_assignments
      json = json.cast<String, dynamic>();
      for (final entry in json.entries) {
        map[entry.key] = RecordStateDTO.listFromJson(entry.value, growable: growable,);
      }
    }
    return map;
  }

  /// The list of required keys that must be present in a JSON.
  static const requiredKeys = <String>{
  };
}

