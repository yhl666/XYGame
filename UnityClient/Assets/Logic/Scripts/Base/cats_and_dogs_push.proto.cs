//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: cats_and_dogs_push.proto
// Note: requires additional types generated from: empty_msg.proto
namespace rpc
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"EnterRoomMsg")]
  public partial class EnterRoomMsg : global::ProtoBuf.IExtensible
  {
    public EnterRoomMsg() {}
    
    private string _peer_name = "";
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"peer_name", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string peer_name
    {
      get { return _peer_name; }
      set { _peer_name = value; }
    }
    private uint _OBSOLETE_rand_seed = default(uint);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"OBSOLETE_rand_seed", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint OBSOLETE_rand_seed
    {
      get { return _OBSOLETE_rand_seed; }
      set { _OBSOLETE_rand_seed = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"NewTurnMsg")]
  public partial class NewTurnMsg : global::ProtoBuf.IExtensible
  {
    public NewTurnMsg() {}
    
    private int _wind_strength = default(int);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"wind_strength", DataFormat = global::ProtoBuf.DataFormat.ZigZag)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int wind_strength
    {
      get { return _wind_strength; }
      set { _wind_strength = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"GotAttackMsg")]
  public partial class GotAttackMsg : global::ProtoBuf.IExtensible
  {
    public GotAttackMsg() {}
    
    private uint _strength = default(uint);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"strength", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint strength
    {
      get { return _strength; }
      set { _strength = value; }
    }
    private rpc.GotAttackMsg.AttackResult _result = rpc.GotAttackMsg.AttackResult.UNKNOWN;
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"result", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(rpc.GotAttackMsg.AttackResult.UNKNOWN)]
    public rpc.GotAttackMsg.AttackResult result
    {
      get { return _result; }
      set { _result = value; }
    }
    [global::ProtoBuf.ProtoContract(Name=@"AttackResult")]
    public enum AttackResult
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"UNKNOWN", Value=0)]
      UNKNOWN = 0,
            
      [global::ProtoBuf.ProtoEnum(Name=@"HIT", Value=1)]
      HIT = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"MISS", Value=2)]
      MISS = 2
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"CatsAndGogsResult")]
  public partial class CatsAndGogsResult : global::ProtoBuf.IExtensible
  {
    public CatsAndGogsResult() {}
    
    private bool _win = default(bool);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"win", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(default(bool))]
    public bool win
    {
      get { return _win; }
      set { _win = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
    public interface ICatsAndDogsPush
    {
      rpc.EmptyMsg EnterRoom(rpc.EnterRoomMsg request);
    rpc.EmptyMsg NewTurn(rpc.NewTurnMsg request);
    rpc.EmptyMsg GotAttack(rpc.GotAttackMsg request);
    rpc.EmptyMsg SetResult(rpc.CatsAndGogsResult request);
    
    }
    
    
}