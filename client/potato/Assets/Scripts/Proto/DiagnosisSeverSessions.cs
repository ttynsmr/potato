// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: generated/diagnosis/diagnosis_sever_sessions.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Torikime.Diagnosis.SeverSessions {

  /// <summary>Holder for reflection information generated from generated/diagnosis/diagnosis_sever_sessions.proto</summary>
  public static partial class DiagnosisSeverSessionsReflection {

    #region Descriptor
    /// <summary>File descriptor for generated/diagnosis/diagnosis_sever_sessions.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static DiagnosisSeverSessionsReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "CjJnZW5lcmF0ZWQvZGlhZ25vc2lzL2RpYWdub3Npc19zZXZlcl9zZXNzaW9u",
            "cy5wcm90bxIhdG9yaWtpbWUuZGlhZ25vc2lzLnNldmVyX3Nlc3Npb25zIh8K",
            "B1JlcXVlc3QSFAoMX2RvX25vdF91c2VfGAEgASgIIiEKCFJlc3BvbnNlEhUK",
            "DXNlc3Npb25fY291bnQYASABKAUiYAoNUmVxdWVzdFBhcmNlbBI7CgdyZXF1",
            "ZXN0GAEgASgLMioudG9yaWtpbWUuZGlhZ25vc2lzLnNldmVyX3Nlc3Npb25z",
            "LlJlcXVlc3QSEgoKcmVxdWVzdF9pZBgCIAEoDSJ0Cg5SZXNwb25zZVBhcmNl",
            "bBI9CghyZXNwb25zZRgBIAEoCzIrLnRvcmlraW1lLmRpYWdub3Npcy5zZXZl",
            "cl9zZXNzaW9ucy5SZXNwb25zZRISCgpyZXF1ZXN0X2lkGAIgASgNEg8KB3N1",
            "Y2Nlc3MYAyABKAhiBnByb3RvMw=="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Torikime.Diagnosis.SeverSessions.Request), global::Torikime.Diagnosis.SeverSessions.Request.Parser, new[]{ "DoNotUse" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Torikime.Diagnosis.SeverSessions.Response), global::Torikime.Diagnosis.SeverSessions.Response.Parser, new[]{ "SessionCount" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Torikime.Diagnosis.SeverSessions.RequestParcel), global::Torikime.Diagnosis.SeverSessions.RequestParcel.Parser, new[]{ "Request", "RequestId" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Torikime.Diagnosis.SeverSessions.ResponseParcel), global::Torikime.Diagnosis.SeverSessions.ResponseParcel.Parser, new[]{ "Response", "RequestId", "Success" }, null, null, null)
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
      get { return global::Torikime.Diagnosis.SeverSessions.DiagnosisSeverSessionsReflection.Descriptor.MessageTypes[0]; }
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
      DoNotUse_ = other.DoNotUse_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Request Clone() {
      return new Request(this);
    }

    /// <summary>Field number for the "_do_not_use_" field.</summary>
    public const int DoNotUseFieldNumber = 1;
    private bool DoNotUse_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool DoNotUse {
      get { return DoNotUse_; }
      set {
        DoNotUse_ = value;
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
      if (DoNotUse != other.DoNotUse) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (DoNotUse != false) hash ^= DoNotUse.GetHashCode();
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
      if (DoNotUse != false) {
        output.WriteRawTag(8);
        output.WriteBool(DoNotUse);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (DoNotUse != false) {
        size += 1 + 1;
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
      if (other.DoNotUse != false) {
        DoNotUse = other.DoNotUse;
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
            DoNotUse = input.ReadBool();
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
      get { return global::Torikime.Diagnosis.SeverSessions.DiagnosisSeverSessionsReflection.Descriptor.MessageTypes[1]; }
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
      sessionCount_ = other.sessionCount_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Response Clone() {
      return new Response(this);
    }

    /// <summary>Field number for the "session_count" field.</summary>
    public const int SessionCountFieldNumber = 1;
    private int sessionCount_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int SessionCount {
      get { return sessionCount_; }
      set {
        sessionCount_ = value;
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
      if (SessionCount != other.SessionCount) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (SessionCount != 0) hash ^= SessionCount.GetHashCode();
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
      if (SessionCount != 0) {
        output.WriteRawTag(8);
        output.WriteInt32(SessionCount);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (SessionCount != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(SessionCount);
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
      if (other.SessionCount != 0) {
        SessionCount = other.SessionCount;
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
            SessionCount = input.ReadInt32();
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
      get { return global::Torikime.Diagnosis.SeverSessions.DiagnosisSeverSessionsReflection.Descriptor.MessageTypes[2]; }
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
    private global::Torikime.Diagnosis.SeverSessions.Request request_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Torikime.Diagnosis.SeverSessions.Request Request {
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
          request_ = new global::Torikime.Diagnosis.SeverSessions.Request();
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
              request_ = new global::Torikime.Diagnosis.SeverSessions.Request();
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
      get { return global::Torikime.Diagnosis.SeverSessions.DiagnosisSeverSessionsReflection.Descriptor.MessageTypes[3]; }
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
    private global::Torikime.Diagnosis.SeverSessions.Response response_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Torikime.Diagnosis.SeverSessions.Response Response {
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
          response_ = new global::Torikime.Diagnosis.SeverSessions.Response();
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
              response_ = new global::Torikime.Diagnosis.SeverSessions.Response();
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

  #endregion

}

#endregion Designer generated code
