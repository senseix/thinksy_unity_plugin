// Generated by ProtoGen, Version=2.4.1.521, Culture=neutral, PublicKeyToken=55f7125234beb589.  DO NOT EDIT!
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.ProtocolBuffers;
using pbc = global::Google.ProtocolBuffers.Collections;
using pbd = global::Google.ProtocolBuffers.Descriptors;
using scg = global::System.Collections.Generic;
namespace Senseix.Message.PlayerProfile {
  
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
  public static partial class PlayerProfile {
  
    #region Extension registration
    public static void RegisterAllExtensions(pb::ExtensionRegistry registry) {
    }
    #endregion
    #region Static variables
    internal static pbd::MessageDescriptor internal__static_Senseix_Message_PlayerProfile_PlayerProfileData__Descriptor;
    internal static pb::FieldAccess.FieldAccessorTable<global::Senseix.Message.PlayerProfile.PlayerProfileData, global::Senseix.Message.PlayerProfile.PlayerProfileData.Builder> internal__static_Senseix_Message_PlayerProfile_PlayerProfileData__FieldAccessorTable;
    internal static pbd::MessageDescriptor internal__static_Senseix_Message_PlayerProfile_PlayerProfileGetResponse__Descriptor;
    internal static pb::FieldAccess.FieldAccessorTable<global::Senseix.Message.PlayerProfile.PlayerProfileGetResponse, global::Senseix.Message.PlayerProfile.PlayerProfileGetResponse.Builder> internal__static_Senseix_Message_PlayerProfile_PlayerProfileGetResponse__FieldAccessorTable;
    #endregion
    #region Descriptor
    public static pbd::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbd::FileDescriptor descriptor;
    
    static PlayerProfile() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          "ChNQbGF5ZXJQcm9maWxlLnByb3RvEh1TZW5zZWl4Lk1lc3NhZ2UuUGxheWVy" + 
          "UHJvZmlsZSI9ChFQbGF5ZXJQcm9maWxlRGF0YRIKCgJpZBgBIAIoCRIMCgRu" + 
          "YW1lGAIgAigJEg4KBmF2YXRhchgDIAIoDCJkChhQbGF5ZXJQcm9maWxlR2V0" + 
          "UmVzcG9uc2USSAoOcGxheWVyUHJvZmlsZXMYASADKAsyMC5TZW5zZWl4Lk1l" + 
          "c3NhZ2UuUGxheWVyUHJvZmlsZS5QbGF5ZXJQcm9maWxlRGF0YQ==");
      pbd::FileDescriptor.InternalDescriptorAssigner assigner = delegate(pbd::FileDescriptor root) {
        descriptor = root;
        internal__static_Senseix_Message_PlayerProfile_PlayerProfileData__Descriptor = Descriptor.MessageTypes[0];
        internal__static_Senseix_Message_PlayerProfile_PlayerProfileData__FieldAccessorTable = 
            new pb::FieldAccess.FieldAccessorTable<global::Senseix.Message.PlayerProfile.PlayerProfileData, global::Senseix.Message.PlayerProfile.PlayerProfileData.Builder>(internal__static_Senseix_Message_PlayerProfile_PlayerProfileData__Descriptor,
                new string[] { "Id", "Name", "Avatar", });
        internal__static_Senseix_Message_PlayerProfile_PlayerProfileGetResponse__Descriptor = Descriptor.MessageTypes[1];
        internal__static_Senseix_Message_PlayerProfile_PlayerProfileGetResponse__FieldAccessorTable = 
            new pb::FieldAccess.FieldAccessorTable<global::Senseix.Message.PlayerProfile.PlayerProfileGetResponse, global::Senseix.Message.PlayerProfile.PlayerProfileGetResponse.Builder>(internal__static_Senseix_Message_PlayerProfile_PlayerProfileGetResponse__Descriptor,
                new string[] { "PlayerProfiles", });
        return null;
      };
      pbd::FileDescriptor.InternalBuildGeneratedFileFrom(descriptorData,
          new pbd::FileDescriptor[] {
          }, assigner);
    }
    #endregion
    
  }
  #region Messages
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
  public sealed partial class PlayerProfileData : pb::GeneratedMessage<PlayerProfileData, PlayerProfileData.Builder> {
    private PlayerProfileData() { }
    private static readonly PlayerProfileData defaultInstance = new PlayerProfileData().MakeReadOnly();
    private static readonly string[] _playerProfileDataFieldNames = new string[] { "avatar", "id", "name" };
    private static readonly uint[] _playerProfileDataFieldTags = new uint[] { 26, 10, 18 };
    public static PlayerProfileData DefaultInstance {
      get { return defaultInstance; }
    }
    
    public override PlayerProfileData DefaultInstanceForType {
      get { return DefaultInstance; }
    }
    
    protected override PlayerProfileData ThisMessage {
      get { return this; }
    }
    
    public static pbd::MessageDescriptor Descriptor {
      get { return global::Senseix.Message.PlayerProfile.PlayerProfile.internal__static_Senseix_Message_PlayerProfile_PlayerProfileData__Descriptor; }
    }
    
    protected override pb::FieldAccess.FieldAccessorTable<PlayerProfileData, PlayerProfileData.Builder> InternalFieldAccessors {
      get { return global::Senseix.Message.PlayerProfile.PlayerProfile.internal__static_Senseix_Message_PlayerProfile_PlayerProfileData__FieldAccessorTable; }
    }
    
    public const int IdFieldNumber = 1;
    private bool hasId;
    private string id_ = "";
    public bool HasId {
      get { return hasId; }
    }
    public string Id {
      get { return id_; }
    }
    
    public const int NameFieldNumber = 2;
    private bool hasName;
    private string name_ = "";
    public bool HasName {
      get { return hasName; }
    }
    public string Name {
      get { return name_; }
    }
    
    public const int AvatarFieldNumber = 3;
    private bool hasAvatar;
    private pb::ByteString avatar_ = pb::ByteString.Empty;
    public bool HasAvatar {
      get { return hasAvatar; }
    }
    public pb::ByteString Avatar {
      get { return avatar_; }
    }
    
    public override bool IsInitialized {
      get {
        if (!hasId) return false;
        if (!hasName) return false;
        if (!hasAvatar) return false;
        return true;
      }
    }
    
    public override void WriteTo(pb::ICodedOutputStream output) {
      int size = SerializedSize;
      string[] field_names = _playerProfileDataFieldNames;
      if (hasId) {
        output.WriteString(1, field_names[1], Id);
      }
      if (hasName) {
        output.WriteString(2, field_names[2], Name);
      }
      if (hasAvatar) {
        output.WriteBytes(3, field_names[0], Avatar);
      }
      UnknownFields.WriteTo(output);
    }
    
    private int memoizedSerializedSize = -1;
    public override int SerializedSize {
      get {
        int size = memoizedSerializedSize;
        if (size != -1) return size;
        
        size = 0;
        if (hasId) {
          size += pb::CodedOutputStream.ComputeStringSize(1, Id);
        }
        if (hasName) {
          size += pb::CodedOutputStream.ComputeStringSize(2, Name);
        }
        if (hasAvatar) {
          size += pb::CodedOutputStream.ComputeBytesSize(3, Avatar);
        }
        size += UnknownFields.SerializedSize;
        memoizedSerializedSize = size;
        return size;
      }
    }
    
    public static PlayerProfileData ParseFrom(pb::ByteString data) {
      return ((Builder) CreateBuilder().MergeFrom(data)).BuildParsed();
    }
    public static PlayerProfileData ParseFrom(pb::ByteString data, pb::ExtensionRegistry extensionRegistry) {
      return ((Builder) CreateBuilder().MergeFrom(data, extensionRegistry)).BuildParsed();
    }
    public static PlayerProfileData ParseFrom(byte[] data) {
      return ((Builder) CreateBuilder().MergeFrom(data)).BuildParsed();
    }
    public static PlayerProfileData ParseFrom(byte[] data, pb::ExtensionRegistry extensionRegistry) {
      return ((Builder) CreateBuilder().MergeFrom(data, extensionRegistry)).BuildParsed();
    }
    public static PlayerProfileData ParseFrom(global::System.IO.Stream input) {
      return ((Builder) CreateBuilder().MergeFrom(input)).BuildParsed();
    }
    public static PlayerProfileData ParseFrom(global::System.IO.Stream input, pb::ExtensionRegistry extensionRegistry) {
      return ((Builder) CreateBuilder().MergeFrom(input, extensionRegistry)).BuildParsed();
    }
    public static PlayerProfileData ParseDelimitedFrom(global::System.IO.Stream input) {
      return CreateBuilder().MergeDelimitedFrom(input).BuildParsed();
    }
    public static PlayerProfileData ParseDelimitedFrom(global::System.IO.Stream input, pb::ExtensionRegistry extensionRegistry) {
      return CreateBuilder().MergeDelimitedFrom(input, extensionRegistry).BuildParsed();
    }
    public static PlayerProfileData ParseFrom(pb::ICodedInputStream input) {
      return ((Builder) CreateBuilder().MergeFrom(input)).BuildParsed();
    }
    public static PlayerProfileData ParseFrom(pb::ICodedInputStream input, pb::ExtensionRegistry extensionRegistry) {
      return ((Builder) CreateBuilder().MergeFrom(input, extensionRegistry)).BuildParsed();
    }
    private PlayerProfileData MakeReadOnly() {
      return this;
    }
    
    public static Builder CreateBuilder() { return new Builder(); }
    public override Builder ToBuilder() { return CreateBuilder(this); }
    public override Builder CreateBuilderForType() { return new Builder(); }
    public static Builder CreateBuilder(PlayerProfileData prototype) {
      return new Builder(prototype);
    }
    
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    public sealed partial class Builder : pb::GeneratedBuilder<PlayerProfileData, Builder> {
      protected override Builder ThisBuilder {
        get { return this; }
      }
      public Builder() {
        result = DefaultInstance;
        resultIsReadOnly = true;
      }
      internal Builder(PlayerProfileData cloneFrom) {
        result = cloneFrom;
        resultIsReadOnly = true;
      }
      
      private bool resultIsReadOnly;
      private PlayerProfileData result;
      
      private PlayerProfileData PrepareBuilder() {
        if (resultIsReadOnly) {
          PlayerProfileData original = result;
          result = new PlayerProfileData();
          resultIsReadOnly = false;
          MergeFrom(original);
        }
        return result;
      }
      
      public override bool IsInitialized {
        get { return result.IsInitialized; }
      }
      
      protected override PlayerProfileData MessageBeingBuilt {
        get { return PrepareBuilder(); }
      }
      
      public override Builder Clear() {
        result = DefaultInstance;
        resultIsReadOnly = true;
        return this;
      }
      
      public override Builder Clone() {
        if (resultIsReadOnly) {
          return new Builder(result);
        } else {
          return new Builder().MergeFrom(result);
        }
      }
      
      public override pbd::MessageDescriptor DescriptorForType {
        get { return global::Senseix.Message.PlayerProfile.PlayerProfileData.Descriptor; }
      }
      
      public override PlayerProfileData DefaultInstanceForType {
        get { return global::Senseix.Message.PlayerProfile.PlayerProfileData.DefaultInstance; }
      }
      
      public override PlayerProfileData BuildPartial() {
        if (resultIsReadOnly) {
          return result;
        }
        resultIsReadOnly = true;
        return result.MakeReadOnly();
      }
      
      public override Builder MergeFrom(pb::IMessage other) {
        if (other is PlayerProfileData) {
          return MergeFrom((PlayerProfileData) other);
        } else {
          base.MergeFrom(other);
          return this;
        }
      }
      
      public override Builder MergeFrom(PlayerProfileData other) {
        if (other == global::Senseix.Message.PlayerProfile.PlayerProfileData.DefaultInstance) return this;
        PrepareBuilder();
        if (other.HasId) {
          Id = other.Id;
        }
        if (other.HasName) {
          Name = other.Name;
        }
        if (other.HasAvatar) {
          Avatar = other.Avatar;
        }
        this.MergeUnknownFields(other.UnknownFields);
        return this;
      }
      
      public override Builder MergeFrom(pb::ICodedInputStream input) {
        return MergeFrom(input, pb::ExtensionRegistry.Empty);
      }
      
      public override Builder MergeFrom(pb::ICodedInputStream input, pb::ExtensionRegistry extensionRegistry) {
        PrepareBuilder();
        pb::UnknownFieldSet.Builder unknownFields = null;
        uint tag;
        string field_name;
        while (input.ReadTag(out tag, out field_name)) {
          if(tag == 0 && field_name != null) {
            int field_ordinal = global::System.Array.BinarySearch(_playerProfileDataFieldNames, field_name, global::System.StringComparer.Ordinal);
            if(field_ordinal >= 0)
              tag = _playerProfileDataFieldTags[field_ordinal];
            else {
              if (unknownFields == null) {
                unknownFields = pb::UnknownFieldSet.CreateBuilder(this.UnknownFields);
              }
              ParseUnknownField(input, unknownFields, extensionRegistry, tag, field_name);
              continue;
            }
          }
          switch (tag) {
            case 0: {
              throw pb::InvalidProtocolBufferException.InvalidTag();
            }
            default: {
              if (pb::WireFormat.IsEndGroupTag(tag)) {
                if (unknownFields != null) {
                  this.UnknownFields = unknownFields.Build();
                }
                return this;
              }
              if (unknownFields == null) {
                unknownFields = pb::UnknownFieldSet.CreateBuilder(this.UnknownFields);
              }
              ParseUnknownField(input, unknownFields, extensionRegistry, tag, field_name);
              break;
            }
            case 10: {
              result.hasId = input.ReadString(ref result.id_);
              break;
            }
            case 18: {
              result.hasName = input.ReadString(ref result.name_);
              break;
            }
            case 26: {
              result.hasAvatar = input.ReadBytes(ref result.avatar_);
              break;
            }
          }
        }
        
        if (unknownFields != null) {
          this.UnknownFields = unknownFields.Build();
        }
        return this;
      }
      
      
      public bool HasId {
        get { return result.hasId; }
      }
      public string Id {
        get { return result.Id; }
        set { SetId(value); }
      }
      public Builder SetId(string value) {
        pb::ThrowHelper.ThrowIfNull(value, "value");
        PrepareBuilder();
        result.hasId = true;
        result.id_ = value;
        return this;
      }
      public Builder ClearId() {
        PrepareBuilder();
        result.hasId = false;
        result.id_ = "";
        return this;
      }
      
      public bool HasName {
        get { return result.hasName; }
      }
      public string Name {
        get { return result.Name; }
        set { SetName(value); }
      }
      public Builder SetName(string value) {
        pb::ThrowHelper.ThrowIfNull(value, "value");
        PrepareBuilder();
        result.hasName = true;
        result.name_ = value;
        return this;
      }
      public Builder ClearName() {
        PrepareBuilder();
        result.hasName = false;
        result.name_ = "";
        return this;
      }
      
      public bool HasAvatar {
        get { return result.hasAvatar; }
      }
      public pb::ByteString Avatar {
        get { return result.Avatar; }
        set { SetAvatar(value); }
      }
      public Builder SetAvatar(pb::ByteString value) {
        pb::ThrowHelper.ThrowIfNull(value, "value");
        PrepareBuilder();
        result.hasAvatar = true;
        result.avatar_ = value;
        return this;
      }
      public Builder ClearAvatar() {
        PrepareBuilder();
        result.hasAvatar = false;
        result.avatar_ = pb::ByteString.Empty;
        return this;
      }
    }
    static PlayerProfileData() {
      object.ReferenceEquals(global::Senseix.Message.PlayerProfile.PlayerProfile.Descriptor, null);
    }
  }
  
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
  public sealed partial class PlayerProfileGetResponse : pb::GeneratedMessage<PlayerProfileGetResponse, PlayerProfileGetResponse.Builder> {
    private PlayerProfileGetResponse() { }
    private static readonly PlayerProfileGetResponse defaultInstance = new PlayerProfileGetResponse().MakeReadOnly();
    private static readonly string[] _playerProfileGetResponseFieldNames = new string[] { "playerProfiles" };
    private static readonly uint[] _playerProfileGetResponseFieldTags = new uint[] { 10 };
    public static PlayerProfileGetResponse DefaultInstance {
      get { return defaultInstance; }
    }
    
    public override PlayerProfileGetResponse DefaultInstanceForType {
      get { return DefaultInstance; }
    }
    
    protected override PlayerProfileGetResponse ThisMessage {
      get { return this; }
    }
    
    public static pbd::MessageDescriptor Descriptor {
      get { return global::Senseix.Message.PlayerProfile.PlayerProfile.internal__static_Senseix_Message_PlayerProfile_PlayerProfileGetResponse__Descriptor; }
    }
    
    protected override pb::FieldAccess.FieldAccessorTable<PlayerProfileGetResponse, PlayerProfileGetResponse.Builder> InternalFieldAccessors {
      get { return global::Senseix.Message.PlayerProfile.PlayerProfile.internal__static_Senseix_Message_PlayerProfile_PlayerProfileGetResponse__FieldAccessorTable; }
    }
    
    public const int PlayerProfilesFieldNumber = 1;
    private pbc::PopsicleList<global::Senseix.Message.PlayerProfile.PlayerProfileData> playerProfiles_ = new pbc::PopsicleList<global::Senseix.Message.PlayerProfile.PlayerProfileData>();
    public scg::IList<global::Senseix.Message.PlayerProfile.PlayerProfileData> PlayerProfilesList {
      get { return playerProfiles_; }
    }
    public int PlayerProfilesCount {
      get { return playerProfiles_.Count; }
    }
    public global::Senseix.Message.PlayerProfile.PlayerProfileData GetPlayerProfiles(int index) {
      return playerProfiles_[index];
    }
    
    public override bool IsInitialized {
      get {
        foreach (global::Senseix.Message.PlayerProfile.PlayerProfileData element in PlayerProfilesList) {
          if (!element.IsInitialized) return false;
        }
        return true;
      }
    }
    
    public override void WriteTo(pb::ICodedOutputStream output) {
      int size = SerializedSize;
      string[] field_names = _playerProfileGetResponseFieldNames;
      if (playerProfiles_.Count > 0) {
        output.WriteMessageArray(1, field_names[0], playerProfiles_);
      }
      UnknownFields.WriteTo(output);
    }
    
    private int memoizedSerializedSize = -1;
    public override int SerializedSize {
      get {
        int size = memoizedSerializedSize;
        if (size != -1) return size;
        
        size = 0;
        foreach (global::Senseix.Message.PlayerProfile.PlayerProfileData element in PlayerProfilesList) {
          size += pb::CodedOutputStream.ComputeMessageSize(1, element);
        }
        size += UnknownFields.SerializedSize;
        memoizedSerializedSize = size;
        return size;
      }
    }
    
    public static PlayerProfileGetResponse ParseFrom(pb::ByteString data) {
      return ((Builder) CreateBuilder().MergeFrom(data)).BuildParsed();
    }
    public static PlayerProfileGetResponse ParseFrom(pb::ByteString data, pb::ExtensionRegistry extensionRegistry) {
      return ((Builder) CreateBuilder().MergeFrom(data, extensionRegistry)).BuildParsed();
    }
    public static PlayerProfileGetResponse ParseFrom(byte[] data) {
      return ((Builder) CreateBuilder().MergeFrom(data)).BuildParsed();
    }
    public static PlayerProfileGetResponse ParseFrom(byte[] data, pb::ExtensionRegistry extensionRegistry) {
      return ((Builder) CreateBuilder().MergeFrom(data, extensionRegistry)).BuildParsed();
    }
    public static PlayerProfileGetResponse ParseFrom(global::System.IO.Stream input) {
      return ((Builder) CreateBuilder().MergeFrom(input)).BuildParsed();
    }
    public static PlayerProfileGetResponse ParseFrom(global::System.IO.Stream input, pb::ExtensionRegistry extensionRegistry) {
      return ((Builder) CreateBuilder().MergeFrom(input, extensionRegistry)).BuildParsed();
    }
    public static PlayerProfileGetResponse ParseDelimitedFrom(global::System.IO.Stream input) {
      return CreateBuilder().MergeDelimitedFrom(input).BuildParsed();
    }
    public static PlayerProfileGetResponse ParseDelimitedFrom(global::System.IO.Stream input, pb::ExtensionRegistry extensionRegistry) {
      return CreateBuilder().MergeDelimitedFrom(input, extensionRegistry).BuildParsed();
    }
    public static PlayerProfileGetResponse ParseFrom(pb::ICodedInputStream input) {
      return ((Builder) CreateBuilder().MergeFrom(input)).BuildParsed();
    }
    public static PlayerProfileGetResponse ParseFrom(pb::ICodedInputStream input, pb::ExtensionRegistry extensionRegistry) {
      return ((Builder) CreateBuilder().MergeFrom(input, extensionRegistry)).BuildParsed();
    }
    private PlayerProfileGetResponse MakeReadOnly() {
      playerProfiles_.MakeReadOnly();
      return this;
    }
    
    public static Builder CreateBuilder() { return new Builder(); }
    public override Builder ToBuilder() { return CreateBuilder(this); }
    public override Builder CreateBuilderForType() { return new Builder(); }
    public static Builder CreateBuilder(PlayerProfileGetResponse prototype) {
      return new Builder(prototype);
    }
    
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    public sealed partial class Builder : pb::GeneratedBuilder<PlayerProfileGetResponse, Builder> {
      protected override Builder ThisBuilder {
        get { return this; }
      }
      public Builder() {
        result = DefaultInstance;
        resultIsReadOnly = true;
      }
      internal Builder(PlayerProfileGetResponse cloneFrom) {
        result = cloneFrom;
        resultIsReadOnly = true;
      }
      
      private bool resultIsReadOnly;
      private PlayerProfileGetResponse result;
      
      private PlayerProfileGetResponse PrepareBuilder() {
        if (resultIsReadOnly) {
          PlayerProfileGetResponse original = result;
          result = new PlayerProfileGetResponse();
          resultIsReadOnly = false;
          MergeFrom(original);
        }
        return result;
      }
      
      public override bool IsInitialized {
        get { return result.IsInitialized; }
      }
      
      protected override PlayerProfileGetResponse MessageBeingBuilt {
        get { return PrepareBuilder(); }
      }
      
      public override Builder Clear() {
        result = DefaultInstance;
        resultIsReadOnly = true;
        return this;
      }
      
      public override Builder Clone() {
        if (resultIsReadOnly) {
          return new Builder(result);
        } else {
          return new Builder().MergeFrom(result);
        }
      }
      
      public override pbd::MessageDescriptor DescriptorForType {
        get { return global::Senseix.Message.PlayerProfile.PlayerProfileGetResponse.Descriptor; }
      }
      
      public override PlayerProfileGetResponse DefaultInstanceForType {
        get { return global::Senseix.Message.PlayerProfile.PlayerProfileGetResponse.DefaultInstance; }
      }
      
      public override PlayerProfileGetResponse BuildPartial() {
        if (resultIsReadOnly) {
          return result;
        }
        resultIsReadOnly = true;
        return result.MakeReadOnly();
      }
      
      public override Builder MergeFrom(pb::IMessage other) {
        if (other is PlayerProfileGetResponse) {
          return MergeFrom((PlayerProfileGetResponse) other);
        } else {
          base.MergeFrom(other);
          return this;
        }
      }
      
      public override Builder MergeFrom(PlayerProfileGetResponse other) {
        if (other == global::Senseix.Message.PlayerProfile.PlayerProfileGetResponse.DefaultInstance) return this;
        PrepareBuilder();
        if (other.playerProfiles_.Count != 0) {
          result.playerProfiles_.Add(other.playerProfiles_);
        }
        this.MergeUnknownFields(other.UnknownFields);
        return this;
      }
      
      public override Builder MergeFrom(pb::ICodedInputStream input) {
        return MergeFrom(input, pb::ExtensionRegistry.Empty);
      }
      
      public override Builder MergeFrom(pb::ICodedInputStream input, pb::ExtensionRegistry extensionRegistry) {
        PrepareBuilder();
        pb::UnknownFieldSet.Builder unknownFields = null;
        uint tag;
        string field_name;
        while (input.ReadTag(out tag, out field_name)) {
          if(tag == 0 && field_name != null) {
            int field_ordinal = global::System.Array.BinarySearch(_playerProfileGetResponseFieldNames, field_name, global::System.StringComparer.Ordinal);
            if(field_ordinal >= 0)
              tag = _playerProfileGetResponseFieldTags[field_ordinal];
            else {
              if (unknownFields == null) {
                unknownFields = pb::UnknownFieldSet.CreateBuilder(this.UnknownFields);
              }
              ParseUnknownField(input, unknownFields, extensionRegistry, tag, field_name);
              continue;
            }
          }
          switch (tag) {
            case 0: {
              throw pb::InvalidProtocolBufferException.InvalidTag();
            }
            default: {
              if (pb::WireFormat.IsEndGroupTag(tag)) {
                if (unknownFields != null) {
                  this.UnknownFields = unknownFields.Build();
                }
                return this;
              }
              if (unknownFields == null) {
                unknownFields = pb::UnknownFieldSet.CreateBuilder(this.UnknownFields);
              }
              ParseUnknownField(input, unknownFields, extensionRegistry, tag, field_name);
              break;
            }
            case 10: {
              input.ReadMessageArray(tag, field_name, result.playerProfiles_, global::Senseix.Message.PlayerProfile.PlayerProfileData.DefaultInstance, extensionRegistry);
              break;
            }
          }
        }
        
        if (unknownFields != null) {
          this.UnknownFields = unknownFields.Build();
        }
        return this;
      }
      
      
      public pbc::IPopsicleList<global::Senseix.Message.PlayerProfile.PlayerProfileData> PlayerProfilesList {
        get { return PrepareBuilder().playerProfiles_; }
      }
      public int PlayerProfilesCount {
        get { return result.PlayerProfilesCount; }
      }
      public global::Senseix.Message.PlayerProfile.PlayerProfileData GetPlayerProfiles(int index) {
        return result.GetPlayerProfiles(index);
      }
      public Builder SetPlayerProfiles(int index, global::Senseix.Message.PlayerProfile.PlayerProfileData value) {
        pb::ThrowHelper.ThrowIfNull(value, "value");
        PrepareBuilder();
        result.playerProfiles_[index] = value;
        return this;
      }
      public Builder SetPlayerProfiles(int index, global::Senseix.Message.PlayerProfile.PlayerProfileData.Builder builderForValue) {
        pb::ThrowHelper.ThrowIfNull(builderForValue, "builderForValue");
        PrepareBuilder();
        result.playerProfiles_[index] = builderForValue.Build();
        return this;
      }
      public Builder AddPlayerProfiles(global::Senseix.Message.PlayerProfile.PlayerProfileData value) {
        pb::ThrowHelper.ThrowIfNull(value, "value");
        PrepareBuilder();
        result.playerProfiles_.Add(value);
        return this;
      }
      public Builder AddPlayerProfiles(global::Senseix.Message.PlayerProfile.PlayerProfileData.Builder builderForValue) {
        pb::ThrowHelper.ThrowIfNull(builderForValue, "builderForValue");
        PrepareBuilder();
        result.playerProfiles_.Add(builderForValue.Build());
        return this;
      }
      public Builder AddRangePlayerProfiles(scg::IEnumerable<global::Senseix.Message.PlayerProfile.PlayerProfileData> values) {
        PrepareBuilder();
        result.playerProfiles_.Add(values);
        return this;
      }
      public Builder ClearPlayerProfiles() {
        PrepareBuilder();
        result.playerProfiles_.Clear();
        return this;
      }
    }
    static PlayerProfileGetResponse() {
      object.ReferenceEquals(global::Senseix.Message.PlayerProfile.PlayerProfile.Descriptor, null);
    }
  }
  
  #endregion
  
}

#endregion Designer generated code