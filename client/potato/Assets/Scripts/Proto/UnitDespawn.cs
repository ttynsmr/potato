// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: generated/unit/unit_despawn.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Torikime.Unit.Despawn {

  /// <summary>Holder for reflection information generated from generated/unit/unit_despawn.proto</summary>
  public static partial class UnitDespawnReflection {

    #region Descriptor
    /// <summary>File descriptor for generated/unit/unit_despawn.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static UnitDespawnReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "CiFnZW5lcmF0ZWQvdW5pdC91bml0X2Rlc3Bhd24ucHJvdG8SFXRvcmlraW1l",
            "LnVuaXQuZGVzcGF3bhoNbWVzc2FnZS5wcm90byJYCgxOb3RpZmljYXRpb24S",
            "EgoKc2Vzc2lvbl9pZBgBIAEoBRIhCghwb3NpdGlvbhgCIAEoCzIPLnBvdGF0",
            "by5WZWN0b3IzEhEKCWRpcmVjdGlvbhgDIAEoAiJoChJOb3RpZmljYXRpb25Q",
            "YXJjZWwSOQoMbm90aWZpY2F0aW9uGAEgASgLMiMudG9yaWtpbWUudW5pdC5k",
            "ZXNwYXduLk5vdGlmaWNhdGlvbhIXCg9ub3RpZmljYXRpb25faWQYAiABKA1i",
            "BnByb3RvMw=="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::Potato.MessageReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Torikime.Unit.Despawn.Notification), global::Torikime.Unit.Despawn.Notification.Parser, new[]{ "SessionId", "Position", "Direction" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Torikime.Unit.Despawn.NotificationParcel), global::Torikime.Unit.Despawn.NotificationParcel.Parser, new[]{ "Notification", "NotificationId" }, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class Notification : pb::IMessage<Notification> {
    private static readonly pb::MessageParser<Notification> _parser = new pb::MessageParser<Notification>(() => new Notification());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<Notification> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Torikime.Unit.Despawn.UnitDespawnReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Notification() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Notification(Notification other) : this() {
      sessionId_ = other.sessionId_;
      position_ = other.position_ != null ? other.position_.Clone() : null;
      direction_ = other.direction_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Notification Clone() {
      return new Notification(this);
    }

    /// <summary>Field number for the "session_id" field.</summary>
    public const int SessionIdFieldNumber = 1;
    private int sessionId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int SessionId {
      get { return sessionId_; }
      set {
        sessionId_ = value;
      }
    }

    /// <summary>Field number for the "position" field.</summary>
    public const int PositionFieldNumber = 2;
    private global::Potato.Vector3 position_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Potato.Vector3 Position {
      get { return position_; }
      set {
        position_ = value;
      }
    }

    /// <summary>Field number for the "direction" field.</summary>
    public const int DirectionFieldNumber = 3;
    private float direction_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public float Direction {
      get { return direction_; }
      set {
        direction_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as Notification);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(Notification other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (SessionId != other.SessionId) return false;
      if (!object.Equals(Position, other.Position)) return false;
      if (!pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.Equals(Direction, other.Direction)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (SessionId != 0) hash ^= SessionId.GetHashCode();
      if (position_ != null) hash ^= Position.GetHashCode();
      if (Direction != 0F) hash ^= pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.GetHashCode(Direction);
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (SessionId != 0) {
        output.WriteRawTag(8);
        output.WriteInt32(SessionId);
      }
      if (position_ != null) {
        output.WriteRawTag(18);
        output.WriteMessage(Position);
      }
      if (Direction != 0F) {
        output.WriteRawTag(29);
        output.WriteFloat(Direction);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (SessionId != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(SessionId);
      }
      if (position_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Position);
      }
      if (Direction != 0F) {
        size += 1 + 4;
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(Notification other) {
      if (other == null) {
        return;
      }
      if (other.SessionId != 0) {
        SessionId = other.SessionId;
      }
      if (other.position_ != null) {
        if (position_ == null) {
          position_ = new global::Potato.Vector3();
        }
        Position.MergeFrom(other.Position);
      }
      if (other.Direction != 0F) {
        Direction = other.Direction;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 8: {
            SessionId = input.ReadInt32();
            break;
          }
          case 18: {
            if (position_ == null) {
              position_ = new global::Potato.Vector3();
            }
            input.ReadMessage(position_);
            break;
          }
          case 29: {
            Direction = input.ReadFloat();
            break;
          }
        }
      }
    }

  }

  public sealed partial class NotificationParcel : pb::IMessage<NotificationParcel> {
    private static readonly pb::MessageParser<NotificationParcel> _parser = new pb::MessageParser<NotificationParcel>(() => new NotificationParcel());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<NotificationParcel> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Torikime.Unit.Despawn.UnitDespawnReflection.Descriptor.MessageTypes[1]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public NotificationParcel() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public NotificationParcel(NotificationParcel other) : this() {
      notification_ = other.notification_ != null ? other.notification_.Clone() : null;
      notificationId_ = other.notificationId_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public NotificationParcel Clone() {
      return new NotificationParcel(this);
    }

    /// <summary>Field number for the "notification" field.</summary>
    public const int NotificationFieldNumber = 1;
    private global::Torikime.Unit.Despawn.Notification notification_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Torikime.Unit.Despawn.Notification Notification {
      get { return notification_; }
      set {
        notification_ = value;
      }
    }

    /// <summary>Field number for the "notification_id" field.</summary>
    public const int NotificationIdFieldNumber = 2;
    private uint notificationId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint NotificationId {
      get { return notificationId_; }
      set {
        notificationId_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as NotificationParcel);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(NotificationParcel other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (!object.Equals(Notification, other.Notification)) return false;
      if (NotificationId != other.NotificationId) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (notification_ != null) hash ^= Notification.GetHashCode();
      if (NotificationId != 0) hash ^= NotificationId.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (notification_ != null) {
        output.WriteRawTag(10);
        output.WriteMessage(Notification);
      }
      if (NotificationId != 0) {
        output.WriteRawTag(16);
        output.WriteUInt32(NotificationId);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (notification_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Notification);
      }
      if (NotificationId != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(NotificationId);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(NotificationParcel other) {
      if (other == null) {
        return;
      }
      if (other.notification_ != null) {
        if (notification_ == null) {
          notification_ = new global::Torikime.Unit.Despawn.Notification();
        }
        Notification.MergeFrom(other.Notification);
      }
      if (other.NotificationId != 0) {
        NotificationId = other.NotificationId;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            if (notification_ == null) {
              notification_ = new global::Torikime.Unit.Despawn.Notification();
            }
            input.ReadMessage(notification_);
            break;
          }
          case 16: {
            NotificationId = input.ReadUInt32();
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
