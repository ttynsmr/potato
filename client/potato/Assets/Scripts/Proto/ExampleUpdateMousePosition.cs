// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: generated/example/example_update_mouse_position.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Torikime.Example.UpdateMousePosition {

  /// <summary>Holder for reflection information generated from generated/example/example_update_mouse_position.proto</summary>
  public static partial class ExampleUpdateMousePositionReflection {

    #region Descriptor
    /// <summary>File descriptor for generated/example/example_update_mouse_position.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static ExampleUpdateMousePositionReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "CjVnZW5lcmF0ZWQvZXhhbXBsZS9leGFtcGxlX3VwZGF0ZV9tb3VzZV9wb3Np",
            "dGlvbi5wcm90bxImdG9yaWtpbWUuZXhhbXBsZS51cGRhdGVfbW91c2VfcG9z",
            "aXRpb24aDU1lc3NhZ2UucHJvdG8iLgoHUmVxdWVzdBIjCghwb3NpdGlvbhgB",
            "IAEoCzIRLnRvcmlraW1lLlZlY3RvcjMiFgoIUmVzcG9uc2USCgoCb2sYASAB",
            "KAgiZQoNUmVxdWVzdFBhcmNlbBJACgdyZXF1ZXN0GAEgASgLMi8udG9yaWtp",
            "bWUuZXhhbXBsZS51cGRhdGVfbW91c2VfcG9zaXRpb24uUmVxdWVzdBISCgpy",
            "ZXF1ZXN0X2lkGAIgASgNInkKDlJlc3BvbnNlUGFyY2VsEkIKCHJlc3BvbnNl",
            "GAEgASgLMjAudG9yaWtpbWUuZXhhbXBsZS51cGRhdGVfbW91c2VfcG9zaXRp",
            "b24uUmVzcG9uc2USEgoKcmVxdWVzdF9pZBgCIAEoDRIPCgdzdWNjZXNzGAMg",
            "ASgIIkcKDE5vdGlmaWNhdGlvbhISCgpzZXNzaW9uX2lkGAEgASgFEiMKCHBv",
            "c2l0aW9uGAIgASgLMhEudG9yaWtpbWUuVmVjdG9yMyJ5ChJOb3RpZmljYXRp",
            "b25QYXJjZWwSSgoMbm90aWZpY2F0aW9uGAEgASgLMjQudG9yaWtpbWUuZXhh",
            "bXBsZS51cGRhdGVfbW91c2VfcG9zaXRpb24uTm90aWZpY2F0aW9uEhcKD25v",
            "dGlmaWNhdGlvbl9pZBgCIAEoDWIGcHJvdG8z"));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::Torikime.MessageReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Torikime.Example.UpdateMousePosition.Request), global::Torikime.Example.UpdateMousePosition.Request.Parser, new[]{ "Position" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Torikime.Example.UpdateMousePosition.Response), global::Torikime.Example.UpdateMousePosition.Response.Parser, new[]{ "Ok" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Torikime.Example.UpdateMousePosition.RequestParcel), global::Torikime.Example.UpdateMousePosition.RequestParcel.Parser, new[]{ "Request", "RequestId" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Torikime.Example.UpdateMousePosition.ResponseParcel), global::Torikime.Example.UpdateMousePosition.ResponseParcel.Parser, new[]{ "Response", "RequestId", "Success" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Torikime.Example.UpdateMousePosition.Notification), global::Torikime.Example.UpdateMousePosition.Notification.Parser, new[]{ "SessionId", "Position" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Torikime.Example.UpdateMousePosition.NotificationParcel), global::Torikime.Example.UpdateMousePosition.NotificationParcel.Parser, new[]{ "Notification", "NotificationId" }, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class Request : pb::IMessage<Request> {
    private static readonly pb::MessageParser<Request> _parser = new pb::MessageParser<Request>(() => new Request());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<Request> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Torikime.Example.UpdateMousePosition.ExampleUpdateMousePositionReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Request() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Request(Request other) : this() {
      position_ = other.position_ != null ? other.position_.Clone() : null;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Request Clone() {
      return new Request(this);
    }

    /// <summary>Field number for the "position" field.</summary>
    public const int PositionFieldNumber = 1;
    private global::Torikime.Vector3 position_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Torikime.Vector3 Position {
      get { return position_; }
      set {
        position_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as Request);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(Request other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (!object.Equals(Position, other.Position)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (position_ != null) hash ^= Position.GetHashCode();
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
      if (position_ != null) {
        output.WriteRawTag(10);
        output.WriteMessage(Position);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (position_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Position);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(Request other) {
      if (other == null) {
        return;
      }
      if (other.position_ != null) {
        if (position_ == null) {
          position_ = new global::Torikime.Vector3();
        }
        Position.MergeFrom(other.Position);
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
            if (position_ == null) {
              position_ = new global::Torikime.Vector3();
            }
            input.ReadMessage(position_);
            break;
          }
        }
      }
    }

  }

  public sealed partial class Response : pb::IMessage<Response> {
    private static readonly pb::MessageParser<Response> _parser = new pb::MessageParser<Response>(() => new Response());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<Response> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Torikime.Example.UpdateMousePosition.ExampleUpdateMousePositionReflection.Descriptor.MessageTypes[1]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Response() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Response(Response other) : this() {
      ok_ = other.ok_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Response Clone() {
      return new Response(this);
    }

    /// <summary>Field number for the "ok" field.</summary>
    public const int OkFieldNumber = 1;
    private bool ok_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Ok {
      get { return ok_; }
      set {
        ok_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as Response);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(Response other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Ok != other.Ok) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Ok != false) hash ^= Ok.GetHashCode();
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
      if (Ok != false) {
        output.WriteRawTag(8);
        output.WriteBool(Ok);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Ok != false) {
        size += 1 + 1;
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(Response other) {
      if (other == null) {
        return;
      }
      if (other.Ok != false) {
        Ok = other.Ok;
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
            Ok = input.ReadBool();
            break;
          }
        }
      }
    }

  }

  public sealed partial class RequestParcel : pb::IMessage<RequestParcel> {
    private static readonly pb::MessageParser<RequestParcel> _parser = new pb::MessageParser<RequestParcel>(() => new RequestParcel());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<RequestParcel> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Torikime.Example.UpdateMousePosition.ExampleUpdateMousePositionReflection.Descriptor.MessageTypes[2]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public RequestParcel() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public RequestParcel(RequestParcel other) : this() {
      request_ = other.request_ != null ? other.request_.Clone() : null;
      requestId_ = other.requestId_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public RequestParcel Clone() {
      return new RequestParcel(this);
    }

    /// <summary>Field number for the "request" field.</summary>
    public const int RequestFieldNumber = 1;
    private global::Torikime.Example.UpdateMousePosition.Request request_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Torikime.Example.UpdateMousePosition.Request Request {
      get { return request_; }
      set {
        request_ = value;
      }
    }

    /// <summary>Field number for the "request_id" field.</summary>
    public const int RequestIdFieldNumber = 2;
    private uint requestId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint RequestId {
      get { return requestId_; }
      set {
        requestId_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as RequestParcel);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(RequestParcel other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (!object.Equals(Request, other.Request)) return false;
      if (RequestId != other.RequestId) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (request_ != null) hash ^= Request.GetHashCode();
      if (RequestId != 0) hash ^= RequestId.GetHashCode();
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
      if (request_ != null) {
        output.WriteRawTag(10);
        output.WriteMessage(Request);
      }
      if (RequestId != 0) {
        output.WriteRawTag(16);
        output.WriteUInt32(RequestId);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (request_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Request);
      }
      if (RequestId != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(RequestId);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(RequestParcel other) {
      if (other == null) {
        return;
      }
      if (other.request_ != null) {
        if (request_ == null) {
          request_ = new global::Torikime.Example.UpdateMousePosition.Request();
        }
        Request.MergeFrom(other.Request);
      }
      if (other.RequestId != 0) {
        RequestId = other.RequestId;
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
            if (request_ == null) {
              request_ = new global::Torikime.Example.UpdateMousePosition.Request();
            }
            input.ReadMessage(request_);
            break;
          }
          case 16: {
            RequestId = input.ReadUInt32();
            break;
          }
        }
      }
    }

  }

  public sealed partial class ResponseParcel : pb::IMessage<ResponseParcel> {
    private static readonly pb::MessageParser<ResponseParcel> _parser = new pb::MessageParser<ResponseParcel>(() => new ResponseParcel());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<ResponseParcel> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Torikime.Example.UpdateMousePosition.ExampleUpdateMousePositionReflection.Descriptor.MessageTypes[3]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ResponseParcel() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ResponseParcel(ResponseParcel other) : this() {
      response_ = other.response_ != null ? other.response_.Clone() : null;
      requestId_ = other.requestId_;
      success_ = other.success_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ResponseParcel Clone() {
      return new ResponseParcel(this);
    }

    /// <summary>Field number for the "response" field.</summary>
    public const int ResponseFieldNumber = 1;
    private global::Torikime.Example.UpdateMousePosition.Response response_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Torikime.Example.UpdateMousePosition.Response Response {
      get { return response_; }
      set {
        response_ = value;
      }
    }

    /// <summary>Field number for the "request_id" field.</summary>
    public const int RequestIdFieldNumber = 2;
    private uint requestId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint RequestId {
      get { return requestId_; }
      set {
        requestId_ = value;
      }
    }

    /// <summary>Field number for the "success" field.</summary>
    public const int SuccessFieldNumber = 3;
    private bool success_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Success {
      get { return success_; }
      set {
        success_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as ResponseParcel);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(ResponseParcel other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (!object.Equals(Response, other.Response)) return false;
      if (RequestId != other.RequestId) return false;
      if (Success != other.Success) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (response_ != null) hash ^= Response.GetHashCode();
      if (RequestId != 0) hash ^= RequestId.GetHashCode();
      if (Success != false) hash ^= Success.GetHashCode();
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
      if (response_ != null) {
        output.WriteRawTag(10);
        output.WriteMessage(Response);
      }
      if (RequestId != 0) {
        output.WriteRawTag(16);
        output.WriteUInt32(RequestId);
      }
      if (Success != false) {
        output.WriteRawTag(24);
        output.WriteBool(Success);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (response_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Response);
      }
      if (RequestId != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(RequestId);
      }
      if (Success != false) {
        size += 1 + 1;
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(ResponseParcel other) {
      if (other == null) {
        return;
      }
      if (other.response_ != null) {
        if (response_ == null) {
          response_ = new global::Torikime.Example.UpdateMousePosition.Response();
        }
        Response.MergeFrom(other.Response);
      }
      if (other.RequestId != 0) {
        RequestId = other.RequestId;
      }
      if (other.Success != false) {
        Success = other.Success;
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
            if (response_ == null) {
              response_ = new global::Torikime.Example.UpdateMousePosition.Response();
            }
            input.ReadMessage(response_);
            break;
          }
          case 16: {
            RequestId = input.ReadUInt32();
            break;
          }
          case 24: {
            Success = input.ReadBool();
            break;
          }
        }
      }
    }

  }

  public sealed partial class Notification : pb::IMessage<Notification> {
    private static readonly pb::MessageParser<Notification> _parser = new pb::MessageParser<Notification>(() => new Notification());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<Notification> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Torikime.Example.UpdateMousePosition.ExampleUpdateMousePositionReflection.Descriptor.MessageTypes[4]; }
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
    private global::Torikime.Vector3 position_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Torikime.Vector3 Position {
      get { return position_; }
      set {
        position_ = value;
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
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (SessionId != 0) hash ^= SessionId.GetHashCode();
      if (position_ != null) hash ^= Position.GetHashCode();
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
          position_ = new global::Torikime.Vector3();
        }
        Position.MergeFrom(other.Position);
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
              position_ = new global::Torikime.Vector3();
            }
            input.ReadMessage(position_);
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
      get { return global::Torikime.Example.UpdateMousePosition.ExampleUpdateMousePositionReflection.Descriptor.MessageTypes[5]; }
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
    private global::Torikime.Example.UpdateMousePosition.Notification notification_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Torikime.Example.UpdateMousePosition.Notification Notification {
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
          notification_ = new global::Torikime.Example.UpdateMousePosition.Notification();
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
              notification_ = new global::Torikime.Example.UpdateMousePosition.Notification();
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
