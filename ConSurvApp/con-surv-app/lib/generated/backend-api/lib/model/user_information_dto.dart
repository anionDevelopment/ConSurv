//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//
// @dart=2.18

// ignore_for_file: unused_element, unused_import
// ignore_for_file: always_put_required_named_parameters_first
// ignore_for_file: constant_identifier_names
// ignore_for_file: lines_longer_than_80_chars

part of openapi.api;

class UserInformationDTO {
  /// Returns a new [UserInformationDTO] instance.
  UserInformationDTO({
    this.id,
    this.name,
    this.isAdmin,
    this.isModerator,
  });

  String? id;

  String? name;

  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  bool? isAdmin;

  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  bool? isModerator;

  @override
  bool operator ==(Object other) => identical(this, other) || other is UserInformationDTO &&
    other.id == id &&
    other.name == name &&
    other.isAdmin == isAdmin &&
    other.isModerator == isModerator;

  @override
  int get hashCode =>
    // ignore: unnecessary_parenthesis
    (id == null ? 0 : id!.hashCode) +
    (name == null ? 0 : name!.hashCode) +
    (isAdmin == null ? 0 : isAdmin!.hashCode) +
    (isModerator == null ? 0 : isModerator!.hashCode);

  @override
  String toString() => 'UserInformationDTO[id=$id, name=$name, isAdmin=$isAdmin, isModerator=$isModerator]';

  Map<String, dynamic> toJson() {
    final json = <String, dynamic>{};
    if (this.id != null) {
      json[r'id'] = this.id;
    } else {
      json[r'id'] = null;
    }
    if (this.name != null) {
      json[r'name'] = this.name;
    } else {
      json[r'name'] = null;
    }
    if (this.isAdmin != null) {
      json[r'isAdmin'] = this.isAdmin;
    } else {
      json[r'isAdmin'] = null;
    }
    if (this.isModerator != null) {
      json[r'isModerator'] = this.isModerator;
    } else {
      json[r'isModerator'] = null;
    }
    return json;
  }

  /// Returns a new [UserInformationDTO] instance and imports its values from
  /// [value] if it's a [Map], null otherwise.
  // ignore: prefer_constructors_over_static_methods
  static UserInformationDTO? fromJson(dynamic value) {
    if (value is Map) {
      final json = value.cast<String, dynamic>();

      // Ensure that the map contains the required keys.
      // Note 1: the values aren't checked for validity beyond being non-null.
      // Note 2: this code is stripped in release mode!
      assert(() {
        requiredKeys.forEach((key) {
          assert(json.containsKey(key), 'Required key "UserInformationDTO[$key]" is missing from JSON.');
          assert(json[key] != null, 'Required key "UserInformationDTO[$key]" has a null value in JSON.');
        });
        return true;
      }());

      return UserInformationDTO(
        id: mapValueOfType<String>(json, r'id'),
        name: mapValueOfType<String>(json, r'name'),
        isAdmin: mapValueOfType<bool>(json, r'isAdmin'),
        isModerator: mapValueOfType<bool>(json, r'isModerator'),
      );
    }
    return null;
  }

  static List<UserInformationDTO> listFromJson(dynamic json, {bool growable = false,}) {
    final result = <UserInformationDTO>[];
    if (json is List && json.isNotEmpty) {
      for (final row in json) {
        final value = UserInformationDTO.fromJson(row);
        if (value != null) {
          result.add(value);
        }
      }
    }
    return result.toList(growable: growable);
  }

  static Map<String, UserInformationDTO> mapFromJson(dynamic json) {
    final map = <String, UserInformationDTO>{};
    if (json is Map && json.isNotEmpty) {
      json = json.cast<String, dynamic>(); // ignore: parameter_assignments
      for (final entry in json.entries) {
        final value = UserInformationDTO.fromJson(entry.value);
        if (value != null) {
          map[entry.key] = value;
        }
      }
    }
    return map;
  }

  // maps a json object with a list of UserInformationDTO-objects as value to a dart map
  static Map<String, List<UserInformationDTO>> mapListFromJson(dynamic json, {bool growable = false,}) {
    final map = <String, List<UserInformationDTO>>{};
    if (json is Map && json.isNotEmpty) {
      // ignore: parameter_assignments
      json = json.cast<String, dynamic>();
      for (final entry in json.entries) {
        map[entry.key] = UserInformationDTO.listFromJson(entry.value, growable: growable,);
      }
    }
    return map;
  }

  /// The list of required keys that must be present in a JSON.
  static const requiredKeys = <String>{
  };
}

