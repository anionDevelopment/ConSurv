//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//
// @dart=2.18

// ignore_for_file: unused_element, unused_import
// ignore_for_file: always_put_required_named_parameters_first
// ignore_for_file: constant_identifier_names
// ignore_for_file: lines_longer_than_80_chars

part of openapi.api;

class AccessToken {
  /// Returns a new [AccessToken] instance.
  AccessToken({
    this.ownerUserId,
    this.value,
    this.expiredMoment,
  });

  String? ownerUserId;

  String? value;

  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  DateTime? expiredMoment;

  @override
  bool operator ==(Object other) => identical(this, other) || other is AccessToken &&
    other.ownerUserId == ownerUserId &&
    other.value == value &&
    other.expiredMoment == expiredMoment;

  @override
  int get hashCode =>
    // ignore: unnecessary_parenthesis
    (ownerUserId == null ? 0 : ownerUserId!.hashCode) +
    (value == null ? 0 : value!.hashCode) +
    (expiredMoment == null ? 0 : expiredMoment!.hashCode);

  @override
  String toString() => 'AccessToken[ownerUserId=$ownerUserId, value=$value, expiredMoment=$expiredMoment]';

  Map<String, dynamic> toJson() {
    final json = <String, dynamic>{};
    if (this.ownerUserId != null) {
      json[r'ownerUserId'] = this.ownerUserId;
    } else {
      json[r'ownerUserId'] = null;
    }
    if (this.value != null) {
      json[r'value'] = this.value;
    } else {
      json[r'value'] = null;
    }
    if (this.expiredMoment != null) {
      json[r'expiredMoment'] = this.expiredMoment!.toUtc().toIso8601String();
    } else {
      json[r'expiredMoment'] = null;
    }
    return json;
  }

  /// Returns a new [AccessToken] instance and imports its values from
  /// [value] if it's a [Map], null otherwise.
  // ignore: prefer_constructors_over_static_methods
  static AccessToken? fromJson(dynamic value) {
    if (value is Map) {
      final json = value.cast<String, dynamic>();

      // Ensure that the map contains the required keys.
      // Note 1: the values aren't checked for validity beyond being non-null.
      // Note 2: this code is stripped in release mode!
      assert(() {
        requiredKeys.forEach((key) {
          assert(json.containsKey(key), 'Required key "AccessToken[$key]" is missing from JSON.');
          assert(json[key] != null, 'Required key "AccessToken[$key]" has a null value in JSON.');
        });
        return true;
      }());

      return AccessToken(
        ownerUserId: mapValueOfType<String>(json, r'ownerUserId'),
        value: mapValueOfType<String>(json, r'value'),
        expiredMoment: mapDateTime(json, r'expiredMoment', r''),
      );
    }
    return null;
  }

  static List<AccessToken> listFromJson(dynamic json, {bool growable = false,}) {
    final result = <AccessToken>[];
    if (json is List && json.isNotEmpty) {
      for (final row in json) {
        final value = AccessToken.fromJson(row);
        if (value != null) {
          result.add(value);
        }
      }
    }
    return result.toList(growable: growable);
  }

  static Map<String, AccessToken> mapFromJson(dynamic json) {
    final map = <String, AccessToken>{};
    if (json is Map && json.isNotEmpty) {
      json = json.cast<String, dynamic>(); // ignore: parameter_assignments
      for (final entry in json.entries) {
        final value = AccessToken.fromJson(entry.value);
        if (value != null) {
          map[entry.key] = value;
        }
      }
    }
    return map;
  }

  // maps a json object with a list of AccessToken-objects as value to a dart map
  static Map<String, List<AccessToken>> mapListFromJson(dynamic json, {bool growable = false,}) {
    final map = <String, List<AccessToken>>{};
    if (json is Map && json.isNotEmpty) {
      // ignore: parameter_assignments
      json = json.cast<String, dynamic>();
      for (final entry in json.entries) {
        map[entry.key] = AccessToken.listFromJson(entry.value, growable: growable,);
      }
    }
    return map;
  }

  /// The list of required keys that must be present in a JSON.
  static const requiredKeys = <String>{
  };
}

