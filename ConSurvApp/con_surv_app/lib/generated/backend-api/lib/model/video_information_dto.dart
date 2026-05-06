//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//
// @dart=2.18

// ignore_for_file: unused_element, unused_import
// ignore_for_file: always_put_required_named_parameters_first
// ignore_for_file: constant_identifier_names
// ignore_for_file: lines_longer_than_80_chars

part of openapi.api;

class VideoInformationDTO {
  /// Returns a new [VideoInformationDTO] instance.
  VideoInformationDTO({
    this.streamURL,
    this.supportsPTZViaONVIF,
    this.onvifUrl,
    this.onvifUsername,
    this.onvifPassword,
  });

  String? streamURL;

  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  bool? supportsPTZViaONVIF;

  String? onvifUrl;

  String? onvifUsername;

  String? onvifPassword;

  @override
  bool operator ==(Object other) => identical(this, other) || other is VideoInformationDTO &&
    other.streamURL == streamURL &&
    other.supportsPTZViaONVIF == supportsPTZViaONVIF &&
    other.onvifUrl == onvifUrl &&
    other.onvifUsername == onvifUsername &&
    other.onvifPassword == onvifPassword;

  @override
  int get hashCode =>
    // ignore: unnecessary_parenthesis
    (streamURL == null ? 0 : streamURL!.hashCode) +
    (supportsPTZViaONVIF == null ? 0 : supportsPTZViaONVIF!.hashCode) +
    (onvifUrl == null ? 0 : onvifUrl!.hashCode) +
    (onvifUsername == null ? 0 : onvifUsername!.hashCode) +
    (onvifPassword == null ? 0 : onvifPassword!.hashCode);

  @override
  String toString() => 'VideoInformationDTO[streamURL=$streamURL, supportsPTZViaONVIF=$supportsPTZViaONVIF, onvifUrl=$onvifUrl, onvifUsername=$onvifUsername, onvifPassword=$onvifPassword]';

  Map<String, dynamic> toJson() {
    final json = <String, dynamic>{};
    if (this.streamURL != null) {
      json[r'streamURL'] = this.streamURL;
    } else {
      json[r'streamURL'] = null;
    }
    if (this.supportsPTZViaONVIF != null) {
      json[r'supportsPTZViaONVIF'] = this.supportsPTZViaONVIF;
    } else {
      json[r'supportsPTZViaONVIF'] = null;
    }
    if (this.onvifUrl != null) {
      json[r'onvifUrl'] = this.onvifUrl;
    } else {
      json[r'onvifUrl'] = null;
    }
    if (this.onvifUsername != null) {
      json[r'onvifUsername'] = this.onvifUsername;
    } else {
      json[r'onvifUsername'] = null;
    }
    if (this.onvifPassword != null) {
      json[r'onvifPassword'] = this.onvifPassword;
    } else {
      json[r'onvifPassword'] = null;
    }
    return json;
  }

  /// Returns a new [VideoInformationDTO] instance and imports its values from
  /// [value] if it's a [Map], null otherwise.
  // ignore: prefer_constructors_over_static_methods
  static VideoInformationDTO? fromJson(dynamic value) {
    if (value is Map) {
      final json = value.cast<String, dynamic>();

      // Ensure that the map contains the required keys.
      // Note 1: the values aren't checked for validity beyond being non-null.
      // Note 2: this code is stripped in release mode!
      assert(() {
        requiredKeys.forEach((key) {
          assert(json.containsKey(key), 'Required key "VideoInformationDTO[$key]" is missing from JSON.');
          assert(json[key] != null, 'Required key "VideoInformationDTO[$key]" has a null value in JSON.');
        });
        return true;
      }());

      return VideoInformationDTO(
        streamURL: mapValueOfType<String>(json, r'streamURL'),
        supportsPTZViaONVIF: mapValueOfType<bool>(json, r'supportsPTZViaONVIF'),
        onvifUrl: mapValueOfType<String>(json, r'onvifUrl'),
        onvifUsername: mapValueOfType<String>(json, r'onvifUsername'),
        onvifPassword: mapValueOfType<String>(json, r'onvifPassword'),
      );
    }
    return null;
  }

  static List<VideoInformationDTO> listFromJson(dynamic json, {bool growable = false,}) {
    final result = <VideoInformationDTO>[];
    if (json is List && json.isNotEmpty) {
      for (final row in json) {
        final value = VideoInformationDTO.fromJson(row);
        if (value != null) {
          result.add(value);
        }
      }
    }
    return result.toList(growable: growable);
  }

  static Map<String, VideoInformationDTO> mapFromJson(dynamic json) {
    final map = <String, VideoInformationDTO>{};
    if (json is Map && json.isNotEmpty) {
      json = json.cast<String, dynamic>(); // ignore: parameter_assignments
      for (final entry in json.entries) {
        final value = VideoInformationDTO.fromJson(entry.value);
        if (value != null) {
          map[entry.key] = value;
        }
      }
    }
    return map;
  }

  // maps a json object with a list of VideoInformationDTO-objects as value to a dart map
  static Map<String, List<VideoInformationDTO>> mapListFromJson(dynamic json, {bool growable = false,}) {
    final map = <String, List<VideoInformationDTO>>{};
    if (json is Map && json.isNotEmpty) {
      // ignore: parameter_assignments
      json = json.cast<String, dynamic>();
      for (final entry in json.entries) {
        map[entry.key] = VideoInformationDTO.listFromJson(entry.value, growable: growable,);
      }
    }
    return map;
  }

  /// The list of required keys that must be present in a JSON.
  static const requiredKeys = <String>{
  };
}

